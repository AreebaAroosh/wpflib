using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.ModelView;
using WPFLib.DataWrapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WPFLib.Misc;
using System.Windows.Controls;
using WPFLib;

namespace DataWrapperDemo
{
    public class Value : DispatcherPropertyChangedHelper
    {
        #region StringProperty
        public static readonly PropertyChangedEventArgs StringArgs = PropertyChangedHelper.CreateArgs<Value>(c => c.String);
        private string _String = "DefaultValue";

        public string String
        {
            get
            {
                return _String;
            }
            set
            {
                var oldValue = String;
                _String = value;
                if (oldValue != value)
                {
                    OnStringChanged(oldValue, value);
                    OnPropertyChanged(StringArgs);
                }
            }
        }

        protected virtual void OnStringChanged(string oldValue, string newValue)
        {
            ValueChanged.OnNext(new Unit());
        }
        #endregion

        // For testing only, better to use ContinuousLinq for complex triggers
        public static readonly Subject<Unit> ValueChanged = new Subject<Unit>();
    }

    public class ValidationWrapperController : AccessUnitModelViewBase
    {
        public ValidationWrapperController()
        {
            InitializeWrappers();
        }

        void InitializeWrappers()
        {
            var valuesCountChanged = Values.ToObservable(c => c.Count).ToUnit();
            DistinctValidation.AddRule(ValidateDistinct);
            DistinctValidation.Trigger = Value.ValueChanged.Merge(valuesCountChanged);

            ValuesValidation.AddRule(ValidateValues);
            ValuesValidation.Trigger = valuesCountChanged;
        }

        #region ValuesProperty
        public static readonly PropertyChangedEventArgs ValuesArgs = PropertyChangedHelper.CreateArgs<ValidationWrapperController>(c => c.Values);
        private ObservableCollection<Value> _Values = new ObservableCollection<Value>();

        public ObservableCollection<Value> Values
        {
            get
            {
                return _Values;
            }
        }
        #endregion

        public IValidationWrapper DistinctValidation
        {
            get
            {
                return this.GetValidationWrapper("DistinctValidation");
            }
        }

        ValidationResult ValidateDistinct()
        {
            return new ValidationResult(Values.Count == 0 || Values.Count == Values.Select(v => v.String).Distinct().Count(), "Values must be unique");
        }

        public IValidationWrapper ValuesValidation
        {
            get
            {
                return this.GetValidationWrapper("ValuesValidation");
            }
        }

        ValidationResult ValidateValues()
        {
            return new ValidationResult(Values.Count > 0, "Values cannot be empty");
        }

        #region AddCommand
        public DelegateAccessUnitCommand AddCommand
        {
            get
            {
                return GetCommand("AddCommand", OnAddCommandExecuted, OnAddCommandCanExecute);
            }
        }

        protected virtual bool OnAddCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnAddCommandExecuted()
        {
            Values.Add(new Value());
        }
        #endregion

        #region DeleteCommand
        public DelegateAccessUnitCommand DeleteCommand
        {
            get
            {
                return GetCommand("DeleteCommand", OnDeleteCommandExecuted, OnDeleteCommandCanExecute);
            }
        }

        protected virtual bool OnDeleteCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnDeleteCommandExecuted()
        {
            Values.Remove(Values.Last());
        }
        #endregion
    }
}
