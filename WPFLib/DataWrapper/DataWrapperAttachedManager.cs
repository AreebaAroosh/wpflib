using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using WPFLib.MarkupExtensionHelpers;
using WPFLib.Collections;

namespace WPFLib.DataWrapper
{
    internal class DataWrapperAttachedManager : MarkupExtensionDataContextTracker
    {
        public DataWrapperAttachedManager(string wrapperName, Binding binding,
                    FrameworkElement _targetElement, DependencyProperty _targetProperty,
                    Func<object, ValidationResult> pyRule, string dependent, string dependsOn)
            : base(_targetElement, _targetProperty)
        {
            Dependent = dependent;
            DependsOn = dependsOn;
            PyRule = pyRule;
            Binding = binding;
            WrapperName = wrapperName;
            InitialDataContextCheck();
        }

        string Dependent;
        string DependsOn;
        Func<object, ValidationResult> PyRule;
        Binding Binding;
        string WrapperName;

        WeakValueDictionary<object, IDataWrapper> initialized = new WeakValueDictionary<object, IDataWrapper>();

        protected override void OnTargetDataContextChanged()
        {
            // Возможны ситуации когда контекст меняется по PropertyChanged, до того
            // как байндинг увидел изменения и провел валидацию
            // Для этого откладываем работу по установке нового дата враппера
            Application.Current.Dispatcher.BeginInvoke(OnTargetDataContextChangedWorker, System.Windows.Threading.DispatcherPriority.Send);
        }

        private void OnTargetDataContextChangedWorker()
        {
            var expr = BindingOperations.GetBindingExpressionBase(TargetElement, TargetProperty);
            if (expr != null)
            {
                // Скорее всего свойство уже прицеплено к врапперу
                // Отсоединяем байндинг
                BindingOperations.ClearBinding(TargetElement, TargetProperty);
            }
            // Каждый раз меняем байндинг при смене контекста
            // новый контекст как минимум может нести другие правила валидации
            // и правила валидации могут быть реализованы в объектах - мы можем их держать зря
            var obj = TargetElement.DataContext as IDataWrapperProvider;
            if (obj != null)
            {
                var wrapper = obj.GetDataWrapper(WrapperName);

                if (expr != null)
                {
                    // Очищаем ошибки при смене контекста
                    // иначе они не обнулятся, тк правила валидации уже могут быть другими
                    Validation.ClearInvalid(expr);
                }

                if (!initialized.Values.Contains(wrapper))
                {
                    if (PyRule != null)
                    {
                        var dc = this.TargetElement.DataContext;
                        wrapper.AddRule(() =>
                        {
                            return PyRule(dc);
                        });
                    }
                    if (!String.IsNullOrEmpty(Dependent))
                    {
                        foreach (var id in Dependent.Split(';').Select(s => s.Trim()))
                        {
                            var wr = obj.GetDataWrapper(id);
                            wrapper.Dependent.Add(wr);
                        }
                    }
                    if (!String.IsNullOrEmpty(DependsOn))
                    {
                        foreach (var id in DependsOn.Split(';').Select(s => s.Trim()))
                        {
                            var wr = obj.GetDataWrapper(id);
                            wrapper.DependsOn.Add(wr);
                        }
                    }
                    initialized[new object()] = wrapper;
                }

                var b = this.Binding.Clone();
                b.Source = TargetElement.DataContext;

                //if (wrapper.DependsOn.Count > 0)
                //{
                //    // Эксперементально
                //    var mb = new MultiBinding();
                //    mb.Mode = BindingMode.TwoWay;
                //    mb.Converter = MBConverter;
                //    mb.Bindings.Add(b);
                //    foreach (var wr in wrapper.DependsOn)
                //    {
                //        var depB = new Binding();
                //        depB.Source = TargetElement.DataContext;
                //        depB.Path = new PropertyPath(wr.Id);
                //        mb.Bindings.Add(depB);
                //    }
                //}
                wrapper.OnBeforeAttach(b, TargetElement, TargetProperty);
                var bindingExpr = BindingOperations.SetBinding(TargetElement, TargetProperty, b);
                wrapper.OnAfterAttach(bindingExpr);
            }
        }
    }
}
