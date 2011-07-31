using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.DataWrapper;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using WPFLib.AccessUnit;
using WPFLib.Misc;
using WPFLib.Errors;

namespace WPFLib.ModelView
{
    /// <summary>
    /// Базовый класс реализующий DataWrapper и управление доступом к нему
    /// </summary>
    public class AccessUnitModelViewBase : ModelViewBase, IAccessUnitModeProvider, IDataWrapperProvider, IValidationErrorProvider, IValidationWrapperProvider
    {
        public string DefaultUri
        {
            get;
            protected set;
        }

        public IEnumerable<IValidationError> Errors
        {
            get
            {
                return Wrappers.Values.Cast<IValidationErrorContainer>()
                    .Concat(ValidationWrappers.Values.Cast<IValidationErrorContainer>())
                    .SelectMany(w => w.Errors.Select(er => new ValidationErrorImpl((String.IsNullOrEmpty(w.Uri) ? DefaultUri : w.Uri), er.ErrorContent)));
            }
        }

        Dictionary<string, IDataWrapper> _wrappers;
        Dictionary<string, IDataWrapper> Wrappers
        {
            get
            {
                if (_wrappers == null)
                {
                    _wrappers = new Dictionary<string, IDataWrapper>();
                }
                return _wrappers;
            }
        }

        public IDataWrapper GetDataWrapper(string property)
        {
            IDataWrapper wrapper;
            if(Wrappers.TryGetValue(property, out wrapper))
            {
                return wrapper;
            }
            return CreateDataWrapper(property);
        }

        private IDataWrapper CreateDataWrapper(string property)
        {
            var path = new PropertyPath(property);
            var wrapper = new DataWrapperImpl(property, this);
            Wrappers[property] = wrapper;
            return wrapper;
        }

        public IAccessUnit GetUnit(string name)
        {
            return GetDataWrapper(name);
        }

        Dictionary<IAccessUnit, AccessUnitMode> _unitModes;
        private Dictionary<IAccessUnit, AccessUnitMode> UnitModes
        {
            get
            {
                if (_unitModes == null)
                {
                    _unitModes = new Dictionary<IAccessUnit, AccessUnitMode>();
                }
                return _unitModes;
            }
        }

        public static readonly PropertyChangedEventArgs DefaultModeArgs = PropertyChangedHelper.CreateArgs<AccessUnitModelViewBase>(c => c.DefaultMode);
        private AccessUnitMode? _DefaultMode;

        public AccessUnitMode DefaultMode
        {
            get
            {
                if (_DefaultMode == null)
                {
                    // Попробуем так решить проблему передачи признака "только чтение"
                    // Другой вариант это введение признака в контекст
                    if (MVParent == null || !(MVParent is IAccessUnitModeProvider))
                    {
                        return AccessUnitMode.Edit;
                    }
                    else
                    {
                        return ((IAccessUnitModeProvider)MVParent).DefaultMode;
                    }
                }
                return _DefaultMode.Value;
            }
            set
            {
                var oldValue = _DefaultMode;
                _DefaultMode = value;
                if (oldValue != value)
                {
                    OnPropertyChanged(DefaultModeArgs);
                }
            }
        }

        private void RenewAll()
        {
            OnUnitModeChanged(AccessUnitProviderHelper.AllUnits);
        }

        public virtual AccessUnitMode GetMode(IAccessUnit unit)
        {
            AccessUnitMode m;
            if (UnitModes.TryGetValue(unit, out m))
            {
                return m.And(DefaultMode);
            }
            return DefaultMode;
        }

        public virtual void SetMode(IAccessUnit unit, AccessUnitMode mode)
        {
            AccessUnitMode? old = null;

            if (UnitModes.ContainsKey(unit))
            {
                old = UnitModes[unit];
            }

            UnitModes[unit] = mode;

            if (old == null || (old != null && old.Value != mode))
            {
                OnUnitModeChanged(unit);
            }
        }

        protected void OnUnitModeChanged(IAccessUnit unit)
        {
            unit.__RaiseModeChanged();
            if (UnitModeChanged != null)
            {
                UnitModeChanged(this, new AccessUnitModeChangedEventArgs(unit));
            }
        }

        public event AccessUnitModeChangedEventHandler UnitModeChanged;

        Dictionary<string, IValidationWrapper> _validationWrappers;
        Dictionary<string, IValidationWrapper> ValidationWrappers
        {
            get
            {
                if (_validationWrappers == null)
                {
                    _validationWrappers = new Dictionary<string, IValidationWrapper>();
                }
                return _validationWrappers;
            }
        }

        public IValidationWrapper GetValidationWrapper(string id)
        {
            IValidationWrapper wrapper;
            if (ValidationWrappers.TryGetValue(id, out wrapper))
            {
                return wrapper;
            }
            return CreateValidationDataWrapper(id);
        }

        private IValidationWrapper CreateValidationDataWrapper(string id)
        {
            var wr = new ValidationWrapperImpl(id);
            ValidationWrappers[id] = wr;
            return wr;
        }
    }
}
