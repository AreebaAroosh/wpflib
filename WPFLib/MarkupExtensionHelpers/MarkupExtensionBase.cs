using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;

namespace WPFLib.MarkupExtensionHelpers
{
    public abstract class MarkupExtensionBase : System.Windows.Markup.MarkupExtension
    {
        private bool _inProvideValue = false;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			var targetElement = service.TargetObject as FrameworkElement;
			if (targetElement == null)
			{
				if (service.TargetObject is DependencyObject)
				{
					throw new Exception(this.GetType().Name + " allowed only on FrameworkElement");
				}
				else
				{
					// Нас используют в шаблоне или дата тимплэйте
					return this;
				}
			}

			var targetProperty = service.TargetProperty as DependencyProperty;

            if (_inProvideValue)
            {
                // В некоторых случаях может быть зацикливание
                // например ClearBinding вызывает переустановку свойства в значение по умолчанию
                // возвращаем значение по умолчанию, предыдущий вызов вернет реальное значение, которое
                // возможно и будет установлено
                var metaData = targetProperty.GetMetadata(targetElement);
                return metaData.DefaultValue;
            }
            try
            {
                _inProvideValue = true;
                return ProvideValue(serviceProvider, targetElement, targetProperty);
            }
            finally
            {
                _inProvideValue = false;
            }
        }

        protected abstract object ProvideValue(IServiceProvider serviceProvider, FrameworkElement targetElement, DependencyProperty targetProperty);
    }
}
