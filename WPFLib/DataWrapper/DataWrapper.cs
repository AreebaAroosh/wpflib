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
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFLib.DataWrapper
{
    public class FuncValidationRule : ValidationRule
    {
        public Func<ValidationResult> Validator { get; private set; }

        public FuncValidationRule(Func<ValidationResult> validator)
        {
            Validator = validator;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return Validator();
        }
    }

    public class FuncValueConverter : IValueConverter
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
    ///     -- не особо нужный функционал
    /// 
    /// вообще если в дереве враперов какое-то значение изменилось то валидацию надо вызвать на всем дереве
    /// это можно сделать через ValidateWithoutUpdate
    /// 
    /// в случае если врапер в дереве приаттачен к контролу то вызывается ValidateWithoutUpdate
    /// если же нет то он сам вызывает правила валидации беря значение из переданного выражения
    /// 
    /// проблема: кто увидит результаты валидации если врапер не приаттачен никуда
    /// возможно дочерние враперы должны брать к себе правила валидации родителей и вызывать их
    ///     -- сделано
    /// </summary>
    internal class DataWrapperImpl : DependencyObject, IDataWrapper
    {
        ObservableCollection<Func<CancellationToken, ValidationResult>> _asyncRules;

        /// <summary>
        /// Правила валидации которые должны быть выполнены асинхронно
        /// </summary>
        ObservableCollection<Func<CancellationToken, ValidationResult>> AsyncRules
        {
            get
            {
                if (_asyncRules == null)
                {
                    _asyncRules = new ObservableCollection<Func<CancellationToken, ValidationResult>>();
                    // Если это первое обращение инициализируем весь механизм
                    InitAsyncValidation();
                }
                return _asyncRules;
            }
        }

        /// <summary>
        /// Добавить асинхронное правило валидации
        /// </summary>
        /// <param name="validator"></param>
        public void AddAsyncRule(Func<CancellationToken, ValidationResult> validator)
        {
            AsyncRules.Add(validator);
        }

        /// <summary>
        /// Вызывается в начале процесса валидации байндинга
        /// </summary>
        /// <returns></returns>
        ValidationResult OnValidationBegin()
        {
            if (asyncResultSet != null)
            {
                // Происходит цикл установки ошибки от асинхронной валидации
                // вернем ранее полученный результат, далее правила валидации проверяться не будут
                return asyncResultSet;
            }
            // Запрос на валидацию
            ValidationReq.OnNext(new Unit());
            return ValidationResult.ValidResult;
        }

        /// <summary>
        /// Правило валидации, которое отслеживает начало валидации, требуется для ручного создания ValidationError
        /// </summary>
        ValidationRule AsyncValidationRule;

        /// <summary>
        /// Только при наличии асинхронных правил инициализируем механизм
        /// </summary>
        void InitAsyncValidation()
        {
            ValidationReq = new Subject<Unit>();

            AsyncValidationRule = new FuncValidationRule(OnValidationBegin) { ValidationStep = ValidationStep.RawProposedValue, ValidatesOnTargetUpdated = true };
            rules.Add(AsyncValidationRule);
            // Запросы на валидацию буферизуем
            ValidationReq.BufferWithTimeAfterValue(TimeSpan.FromMilliseconds(10))
                .Subscribe(OnValidateAsync);
        }

        CancellationTokenSource currentAsyncToken;
        IDisposable currentAsyncSubscription;

        /// <summary>
        /// Производим асинхронную валидацию, вызывается после буферизации запросов на валидацию
        /// </summary>
        void OnValidateAsync()
        {
            lock (this)
            {
                if (currentAsyncToken != null)
                {
                    currentAsyncSubscription.Dispose();
                    currentAsyncToken.Cancel();
                }
                currentAsyncToken = new CancellationTokenSource();
                var tasks = AsyncRules.Select((r) => Task<ValidationResult>.Factory.StartNew(() => r(currentAsyncToken.Token)).ToObservable());
                var result = tasks.Merge().Where(r => !r.IsValid).Take(1);
                AsyncValidationTask = result.ToTaskLast();
                currentAsyncSubscription = result./*ObserveOn(this.currentTargetObject.Dispatcher).*/Subscribe(OnAsyncError);
            }
        }

        Task<ValidationResult> AsyncValidationTask;

        /// <summary>
        /// Результат асинхронной валидации, если не нал, значит идет процесс установки результата
        /// асинхронной валидации в байндинг
        /// </summary>
        ValidationResult asyncResultSet = null;

        /// <summary>
        /// Вызывается в случае получения ошибки(именно ошибки) от асинхронной валидации
        /// </summary>
        /// <param name="result"></param>
        void OnAsyncError(ValidationResult result)
        {
            if (LastError.Any())
            {
                // Уже есть какая-то ошибка, не будем её менять
                // что бы не смущать пользователя
                return;
            }
            // Асинхронная валидация вернула ошибку
            // нам надо просто установить её

            // сразу сохраним её у нас
            SetAsyncErrorExplicit(result.ErrorContent);

            if (IsAttached)
            {
                // Байндинг ещё работает, установим ошибку в него
                // Запустим новый цикл валидации, но обработаем его по особому
                // Так же UpdateSource() необходимо выполнять только в UI потоке
                currentTargetObject.Dispatcher.BeginInvoke((Action<ValidationResult>)SetAsyncError, DispatcherPriority.Send, new object[] { result });
            }
        }

        void SetAsyncErrorExplicit(object errorContent)
        {
            LastError.Clear();
            LastError.Add(new ValidationError(AsyncValidationRule, new object(), errorContent, null));
        }

        /// <summary>
        /// Установка результата асинхронной валидации в байндинг
        /// </summary>
        /// <param name="result"></param>
        void SetAsyncError(ValidationResult result)
        {
            if (IsAttached)
            {
                lock (this)
                {
                    try
                    {
                        asyncResultSet = result;
                        currentBindingExpression.UpdateSource();
                    }
                    finally
                    {
                        asyncResultSet = null;
                    }
                }
            }
            // Если байндинг отсоединился пока нас вызывали - ничего не делаем, ошибку уже сохранили в OnAsyncError
        }

        /// <summary>
        /// Сигналы о начале валидации
        /// </summary>
        Subject<Unit> ValidationReq;

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
        }

        protected void ValidateDependent()
        {
            foreach (var wr in Dependent)
            {
                wr.Validate();
            }
        }

        List<ValidationError> LastError = new List<ValidationError>();

        void OnError(object sender, RoutedEventArgs args)
        {
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

                foreach (var e in LastError)
                    yield return e;
                if (AsyncValidationTask != null && !AsyncValidationTask.IsCompleted)
                {
                    // Если идет асинхронная валидация
                    // то дожидаемся её синхронно
                    AsyncValidationTask.Wait();
                    var res = AsyncValidationTask.Result;
                    if (res != null && !res.IsValid)
                    {
                        // И сразу возвращаем ошибку
                        // тк мы скорее всего в UI потоке, и ошибку в LastError записать не смогут
                        // пока мы держим управление
                        yield return new ValidationError(AsyncValidationRule, new object(), res.ErrorContent, null);
                    }
                }

                yield break;
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
