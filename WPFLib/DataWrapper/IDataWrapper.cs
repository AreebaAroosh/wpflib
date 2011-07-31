using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using WPFLib.AccessUnit;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WPFLib.DataWrapper
{
    public interface IValidationErrorContainer
    {
        IEnumerable<ValidationError> Errors { get; }
        string Uri { get; set; }
    }

    /// <summary>
    /// Что-то вроде IDataWrapper, но цепляется к контролу через аттачед свойство
    /// и по Trigger проводит валидацию
    /// ошибки цепляет на контрол
    /// </summary>
    public interface IValidationWrapper : INotifyPropertyChanged, IValidationErrorContainer
    {
        bool IsAttached { get; }
        string Id { get; }
        IObservable<Unit> Trigger { get; set; }
        void AddRule(Func<ValidationResult> validator);

        void OnBeforeAttach(Binding binding, DependencyObject target, DependencyProperty property);
        void OnAfterAttach(BindingExpressionBase bindingExpression);

        IEnumerable<ValidationError> Validate();
    }

    public interface IDataWrapper : IAccessUnit, INotifyPropertyChanged, IValidationErrorContainer
    {
        string Id { get; }
        bool IsAttached { get; }
        ObservableCollection<IDataWrapper> Dependent { get; }
        ObservableCollection<IDataWrapper> DependsOn { get; }

        ReadOnlyObservableCollection<ValidationRule> Rules { get; }
        void AddRule(Func<ValidationResult> validator);
        Func<object, object> Convert { get; set; }
        Func<object, object> ConvertBack { get; set; }

        bool IsValid { get; }
        IEnumerable<ValidationError> Validate();
        void UpdateSource();

        /// <summary>
        /// Родительский враппер
        /// Для враппера Index.Measure.Name, родителем будет Index.Measure
        /// родительские врапперы создаются автоматически в случае если их ещё нет
        /// </summary>
        IDataWrapper Parent { get; set; }

        /// <summary>
        /// Все дочерние врапперы на один уровень
        /// </summary>
        ObservableCollection<IDataWrapper> Children { get; }

        /// <summary>
        /// Подготовить байндинг
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="target"></param>
        /// <param name="property"></param>
        void OnBeforeAttach(Binding binding, DependencyObject target, DependencyProperty property);
        void OnAfterAttach(BindingExpressionBase bindingExpression);
    }
}
