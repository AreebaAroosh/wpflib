using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using WPFLib.AccessUnit;

namespace WPFLib.DataWrapper
{
    //internal class ValidationRuleWrapper : ValidationRule
    //{
    //    internal ValidationRule Rule;
    //    internal DataWrapperImpl DataWrapper;

    //    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    //    {
    //        return Rule.Validate(null, cultureInfo);
    //    }
    //}

    class FuncValidationRule : ValidationRule
    {
        Func<ValidationResult> Validator;

        public FuncValidationRule(Func<ValidationResult> validator)
        {
            Validator = validator;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return Validator();
        }
    }

    class FuncValueConverter : IValueConverter
    {
        public Func<object, object> Convert { get; set; }
        public Func<object, object> ConvertBack { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Convert != null)
            {
                return Convert(value);
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConvertBack != null)
            {
                return ConvertBack(value);
            }
            return value;
        }
    }

    public class DataWrapperChildren : ObservableCollection<IDataWrapper>
    {
        protected override void ClearItems()
        {
            new List<IDataWrapper>(this).ForEach(t => Remove(t));
        }

        protected override void InsertItem(int index, IDataWrapper item)
        {
            if (!this.Contains(item))
            {
                base.InsertItem(index, item);
            }
        }
    }

    /// <summary>
    /// TODO:
    /// Реализовать поддержку правил валидации с передачей им значения к которому идет байндинг, как:
    /// GetDataWrapper() - на вход получает Expression который возвращает значение враппера - тоесть сожержит путь к значению
    /// по выражению получается строка идентификатор
    /// при этом DataWrapper сможет по этому выражению получать значение
    /// 
    /// вообще если в дереве враперов какое-то значение изменилось то валидацию надо вызвать на всем дереве
    /// это можно сделать через ValidateWithoutUpdate
    /// 
    /// в случае если врапер в дереве приаттачен к контролу то вызывается ValidateWithoutUpdate
    /// если же нет то он сам вызывает правила валидации беря значение из переданного выражения
    /// 
    /// проблема: кто увидит результаты валидации если врапер не приаттачен никуда
    /// возможно дочерние враперы должны брать к себе правила валидации родителей и вызывать их
    /// </summary>
    internal class DataWrapperImpl : DependencyObject, IDataWrapper
    {
        private void FixupParent(IDataWrapper previousValue)
        {
            if (previousValue != null && previousValue.Children.Contains(this))
            {
                previousValue.Children.Remove(this);
            }

            if (Parent != null)
            {
                if (!Parent.Children.Contains(this))
                {
                    Parent.Children.Add(this);
                }
            }
        }

        IDataWrapper _parent;
        public IDataWrapper Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (!ReferenceEquals(_parent, value))
                {
                    var previousValue = _parent;
                    _parent = value;

                    FixupParent(previousValue);
                    OnParentChanged(previousValue, value);
                }
            }
        }

        protected virtual void OnParentChanged(IDataWrapper oldValue, IDataWrapper newValue)
        {
        }

        DataWrapperChildren _Dependent;
        public ObservableCollection<IDataWrapper> Dependent
        {
            get
            {
                if (_Dependent == null)
                {
                    _Dependent = new DataWrapperChildren();
                    _Dependent.CollectionChanged += (s, e) =>
                    {
                        if (e.NewItems != null)
                        {
                            foreach (IDataWrapper item in e.NewItems)
                            {
                                item.DependsOn.Add(this);
                            }
                        }

                        if (e.OldItems != null)
                        {
                            foreach (IDataWrapper item in e.OldItems)
                            {
                                item.DependsOn.Remove(this);
                            }
                        }
                    };
                }
                return _Dependent;
            }
        }

        DataWrapperChildren _DependsOn;
        public ObservableCollection<IDataWrapper> DependsOn
        {
            get
            {
                if (_DependsOn == null)
                {
                    _DependsOn = new DataWrapperChildren();
                    _DependsOn.CollectionChanged += (s, e) =>
                    {
                        if (e.NewItems != null)
                        {
                            foreach (IDataWrapper item in e.NewItems)
                            {
                                item.Dependent.Add(this);
                            }
                        }

                        if (e.OldItems != null)
                        {
                            foreach (IDataWrapper item in e.OldItems)
                            {
                                item.Dependent.Remove(this);
                            }
                        }
                    };

                }
                return _DependsOn;
            }
        }

        DataWrapperChildren _children;
        public ObservableCollection<IDataWrapper> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new DataWrapperChildren();
                    _children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_children_CollectionChanged);
                }
                return _children;
            }
        }

        void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IDataWrapper item in e.NewItems)
                {
                    item.Parent = this;
                }
            }

            if (e.OldItems != null)
            {
                foreach (IDataWrapper item in e.OldItems)
                {
                    if (ReferenceEquals(item.Parent, this))
                    {
                        item.Parent = null;
                    }
                }
            }
        }

        IAccessUnitModeProvider accessProvider;

        public string Id { get; private set; }

        public DataWrapperImpl(string id, IAccessUnitModeProvider _accessProvider)
        {
            // Берем на себя ошибки родителей которые не прикреплены в UI
            AddRule(new FuncValidationRule(this.ValidateNotAttachedParent));
            accessProvider = _accessProvider;
            Id = id;
        }

        ReadOnlyObservableCollection<ValidationRule> _ro_rules = null;
        public ReadOnlyObservableCollection<ValidationRule> Rules
        {
            get
            {
                if (_ro_rules == null)
                {
                    _ro_rules = new ReadOnlyObservableCollection<ValidationRule>(rules);
                }
                return _ro_rules;
            }
        }

        ObservableCollection<ValidationRule> rules = new ObservableCollection<ValidationRule>();

        // http://stackoverflow.com/questions/2789504/get-the-property-as-a-string-from-an-expressionfunctmodel-tproperty/2789606#2789606
        public void AddRule(Func<ValidationResult> validator)
        {
            AddRule(new FuncValidationRule(validator));
        }

        public Func<object, object> Convert
        {
            get;
            set;
        }

        public Func<object, object> ConvertBack
        {
            get;
            set;
        }

        private void AddRule(ValidationRule rule)
        {
            rule.ValidatesOnTargetUpdated = true;
            rule.ValidationStep = ValidationStep.UpdatedValue;
            rules.Add(rule);
        }

        public void OnBeforeAttach(Binding b, DependencyObject target, DependencyProperty property)
        {
            if (Convert != null || ConvertBack != null)
            {
                b.Converter = new FuncValueConverter() { Convert = Convert, ConvertBack = ConvertBack };
            }
            b.ValidatesOnExceptions = true;
            b.NotifyOnValidationError = true;
            b.NotifyOnSourceUpdated = true;
            b.NotifyOnTargetUpdated = true;

            foreach (var rule in rules)
            {
                b.ValidationRules.Add(rule);
            }

            UnsubscribeOnError();
            currentTargetObject = target;
            currentTargetProperty = property;
        }

        public void OnAfterAttach(BindingExpressionBase bindingExpression)
        {
            currentBindingExpression = bindingExpression;
            SubscribeOnError();
            OnError(null, null);
        }

        void SubscribeOnError()
        {
            IInputElement inp = currentTargetObject as IInputElement;
            if (inp != null)
            {
                inp.AddHandler(Validation.ErrorEvent, (RoutedEventHandler)OnError);
                inp.AddHandler(Binding.TargetUpdatedEvent, (RoutedEventHandler)OnTargetUpdated);
                inp.AddHandler(Binding.SourceUpdatedEvent, (RoutedEventHandler)OnSourceUpdated);
            }
        }

        void UnsubscribeOnError()
        {
            IInputElement inp = currentTargetObject as IInputElement;
            if (inp != null)
            {
                inp.RemoveHandler(Validation.ErrorEvent, (RoutedEventHandler)OnError);
                inp.RemoveHandler(Binding.TargetUpdatedEvent, (RoutedEventHandler)OnTargetUpdated);
                inp.RemoveHandler(Binding.SourceUpdatedEvent, (RoutedEventHandler)OnSourceUpdated);
            }
        }

        void OnTargetUpdated(object sender, RoutedEventArgs args)
        {
            ValidateDependent();
            //ValidateParent();
        }

        void OnSourceUpdated(object sender, RoutedEventArgs args)
        {
            ValidateDependent();
            //ValidateParent();
        }

        protected void ValidateDependent()
        {
            foreach (var wr in Dependent)
            {
                wr.Validate();
            }
        }

        //protected void ValidateParent()
        //{
        //    var parent = this.Parent;
        //    while (parent != null)
        //    {
        //        parent.Validate();
        //        parent = parent.Parent;
        //    }
        //}

        List<ValidationError> LastError = new List<ValidationError>();

        void OnError(object sender, RoutedEventArgs args)
        {
            // Возможны ситуации, когда по PropertyChanged первым срабатывает код, который
            // изменяет дата контекст и при этом происходит отсоединение нашего байндинга(IsAttached == false)
            // а только после этого обработкой PropertyChanged занимается байндинг
            // и вызывает валидацию, но байндинг уже отсоединен и результатов этой валидации мы не увидим
            // при этом LastError может содержать неправильный результат
            if (!IsAttached)
            {
                // Байндинг отсоединен, отпишемся от событий
                // будем ждать следующего аттача, ошибку сохраняем
                // иначе в байндинге она очищается
                UnsubscribeOnError();
            }
            else
            {
                // Сохраняем ошибку на случай отсоединения байндинга
                // если UI переключился куда-то ещё, то мы все ещё сможем сообщить об ошибке
                LastError.Clear();
                if (currentBindingExpression.ValidationError != null)
                {
                    LastError.Add(currentBindingExpression.ValidationError);
                }
            }
        }

        public bool IsAttached
        {
            get
            {
                return currentBindingExpression != null &&
                    currentBindingExpression.Status != BindingStatus.Inactive &&
                    currentBindingExpression.Status != BindingStatus.Detached;
            }
        }
        BindingExpressionBase currentBindingExpression = null;
        DependencyObject currentTargetObject;
        DependencyProperty currentTargetProperty;

        //public void AddValidator(Func<T, ValidationResult> func)
        //{
        //    this.AddRule(new DelegateValidationRule<T>(func));
        //}

        //public void AddValidatorRawValue(Func<T, ValidationResult> func)
        //{
        //    this.AddRule(new DelegateValidationRule<T>(func) { ValidationStep = ValidationStep.RawProposedValue });
        //}

        public void UpdateSource()
        {
            if (IsAttached)
            {
                currentBindingExpression.UpdateSource();
            }
        }


        /// <summary>
        /// Делает валидацию родителей которые не прицеплены в UI
        /// проходит вверх пока не встретит прицепленного родителя
        /// прицепленный сам прицепит следующих родителей
        /// </summary>
        /// <returns></returns>
        protected ValidationResult ValidateNotAttachedParent()
        {
            var parent = this.Parent;
            while (parent != null)
            {
                if (!parent.IsAttached)
                {
                    var err = parent.Validate().FirstOrDefault();
                    if (err != null)
                        return new ValidationResult(false, err.ErrorContent);
                }
                else
                {
                    return ValidationResult.ValidResult;
                }
                parent = parent.Parent;
            }
            return ValidationResult.ValidResult;
        }

        /// <summary>
        /// Выполним валидацию
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValidationError> Validate()
        {
            if (!IsAttached)
            {
                return ValidateExplicit();
            }
            else
            {
                // Не срабатывает, байндинг умный - и лишний раз валидацию не дергает
                // в случае же зависим
                // а нам надо заставить его, что бы в случае чего ошибка подцепилась в UI
                //currentBindingExpression.ValidateWithoutUpdate();
                // только вот так
                currentBindingExpression.UpdateSource();
                return Errors;
            }
        }

        protected IEnumerable<ValidationError> ValidateExplicit()
        {
            LastError.Clear();

            foreach (var rule in Rules)
            {
                var res = rule.Validate(null, Thread.CurrentThread.CurrentUICulture);
                if (res != null && !res.IsValid)
                {
                    var err = new ValidationError(rule, new object(), res.ErrorContent, null);
                    // Сохраняем ошибки
                    LastError.Add(err);
                }
            }
            return LastError;
        }

        /// <summary>
        /// Ошибки с последней валидации
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get
            {
                // Если делать тут явную валидацию - убьем идею instant валидации
                return LastError;
            }
        }

        public bool IsValid
        {
            get
            {
                return LastError == null;
            }
        }

        #region IAccessUnit Members

        public void __RaiseModeChanged()
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
            }
        }

        public AccessUnitMode Mode
        {
            get
            {
                return accessProvider.GetMode(this);
            }
            set
            {
                accessProvider.SetMode(this, value);
            }
        }

        #endregion

        public string Uri
        {
            get;
            set;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public System.Globalization.CultureInfo CurrentThread { get; set; }
    }
}
