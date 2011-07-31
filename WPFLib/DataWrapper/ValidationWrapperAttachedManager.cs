using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace WPFLib.DataWrapper
{
    internal class ValidationWrapperAttachedManager
    {
        public static readonly DependencyProperty InternalValueProperty = DependencyProperty.RegisterAttached("InternalValue", typeof(object), typeof(ValidationWrapperAttachedManager), new FrameworkPropertyMetadata());

        public static void SetInternalValue(DependencyObject obj, object value)
        {
            obj.SetValue(InternalValueProperty, value);
        }

        public static object GetInternalValue(DependencyObject obj)
        {
            return (object)obj.GetValue(InternalValueProperty);
        }

        FrameworkElement TargetElement;

        public ValidationWrapperAttachedManager(string wrapperName,
                    FrameworkElement _targetElement)
        {
            TargetElement = _targetElement;
            WrapperName = wrapperName;
            InitialDataContextCheck();
            TargetElement.DataContextChanged += new DependencyPropertyChangedEventHandler((s, a) => OnTargetDataContextChanged());
        }

        void InitialDataContextCheck()
        {
            OnTargetDataContextChanged();
        }

        string WrapperName;

        protected void OnTargetDataContextChanged()
        {
            // Каждый раз меняем байндинг при смене контекста
            // новый контекст как минимум может нести другие правила валидации
            // и правила валидации могут быть реализованы в объектах - мы можем их держать зря
            var obj = TargetElement.DataContext as IValidationWrapperProvider;
            if (obj != null)
            {
                var wrapper = obj.GetValidationWrapper(WrapperName);

                var expr = BindingOperations.GetBindingExpressionBase(TargetElement, InternalValueProperty);
                if (expr != null)
                {
                    // Скорее всего свойство уже прицеплено к врапперу
                    // Отсоединяем байндинг
                    BindingOperations.ClearBinding(TargetElement, InternalValueProperty);

                    // Очищаем ошибки при смене контекста
                    // иначе они не обнулятся, тк правила валидации уже могут быть другими
                    Validation.ClearInvalid(expr);
                }

                var b = new Binding();
                b.Source = new object();

                wrapper.OnBeforeAttach(b, TargetElement, InternalValueProperty);
                var bindingExpr = BindingOperations.SetBinding(TargetElement, InternalValueProperty, b);
                wrapper.OnAfterAttach(bindingExpr);
            }
            else
            {
                // Если контекст не то что мы ждем, отсоединяем все что может быть прицеплено
                BindingOperations.ClearBinding(TargetElement, InternalValueProperty);
            }
        }
    }
}
