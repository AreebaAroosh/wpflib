using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Misc;
using WPFLib.DataWrapper;
using System.ComponentModel;
using WPFLib.ModelView;
using WPFLib;
using System.Windows.Controls;
using System.Threading;
using System.Windows;

namespace DataWrapperDemo
{
    public class DataWrapperController : AccessUnitModelViewBase
    {
        #region SaveCommand
        public DelegateAccessUnitCommand SaveCommand
        {
            get
            {
                return GetCommand("SaveCommand", OnSaveCommandExecuted, OnSaveCommandCanExecute);
            }
        }

        protected virtual bool OnSaveCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnSaveCommandExecuted()
        {
            var errors = Errors.Select(e => e.ErrorContent).ToList();
            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join(Environment.NewLine, errors.ToList()), "Ошибка");
            }
            else
            {
                MessageBox.Show("Успешно сохранено");
            }
        }
        #endregion

        public DataWrapperController()
        {
            UserNameWrapper.AddRule(ValidateUserName);
            UserNameWrapper.AddAsyncRule(ValidateUserNameAsync);
        }

        #region UserNameProperty
        public static readonly PropertyChangedEventArgs UserNameArgs = PropertyChangedHelper.CreateArgs<DataWrapperController>(c => c.UserName);
        private string _UserName;

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                var oldValue = UserName;
                _UserName = value;
                if (oldValue != value)
                {
                    OnUserNameChanged(oldValue, value);
                    OnPropertyChanged(UserNameArgs);
                }
            }
        }

        protected virtual void OnUserNameChanged(string oldValue, string newValue)
        {
        }

        ValidationResult ValidateUserName()
        {
            return new ValidationResult(!String.IsNullOrEmpty(UserName), "Имя пользователя обазятельно для заполнения");
        }

        ValidationResult ValidateUserNameAsync(CancellationToken token)
        {
            Thread.Sleep(1500); // Продолжительный запрос на сервер
            return new ValidationResult(!(UserName.Length >= 4), "Имя пользователя занято");
        }
        #endregion

        public IDataWrapper UserNameWrapper
        {
            get
            {
                return this.GetDataWrapper("UserName");
            }
        }

        #region ClearUserNameCommand
        public DelegateAccessUnitCommand ClearUserNameCommand
        {
            get
            {
                return GetCommand("ClearUserNameCommand", OnClearUserNameCommandExecuted, OnClearUserNameCommandCanExecute, true);
            }
        }

        protected virtual bool OnClearUserNameCommandCanExecute()
        {
            return !String.IsNullOrEmpty(UserName);
        }

        protected virtual void OnClearUserNameCommandExecuted()
        {
            UserName = null;
        }
        #endregion
    }
}
