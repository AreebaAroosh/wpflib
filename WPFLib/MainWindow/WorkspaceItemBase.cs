using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLib.Misc;
using WPFLib.Contracts;
using System.Windows.Input;
using WPFLib.Configuration;
using System.Windows;
using WPFLib.ModelView;
using System.ComponentModel;

namespace WPFLib.MainWindow
{
    public abstract class WorkspaceItemBase : ModelViewBase, IWorkspaceItem
    {
        #region CloseCommand
        public static readonly string CloseCommandProperty = "CloseCommand";
        private DelegateCommand _CloseCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new DelegateCommand(OnCloseCommandExecuted, OnCloseCommandCanExecute);
                }
                return _CloseCommand;
            }
        }

        protected virtual bool OnCloseCommandCanExecute()
        {
            return true;
        }

        protected virtual void OnCloseCommandExecuted()
        {
			ServiceLocator.Current.GetInstance<IMainWindowController>().Close(this);
        }
        #endregion

        #region IWorkspaceItem Members

        public static readonly PropertyChangedEventArgs HeaderArgs = PropertyChangedHelper.CreateArgs<WorkspaceItemBase>(c => c.Header);

        public abstract object Header
        {
            get;
        }

        public virtual bool CanClose()
        {
			return OnCloseCommandCanExecute();
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        #endregion
    }
}
