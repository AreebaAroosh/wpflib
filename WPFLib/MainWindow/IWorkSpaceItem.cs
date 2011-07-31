using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WPFLib.Contracts
{
    public interface IWorkspaceItem
    {
        /// <summary>
        /// Закрыть элемент
        /// </summary>
        ICommand CloseCommand { get; }

        /// <summary>
        /// Содержимое заголовка элемента
        /// </summary>
        object Header { get; }

        bool CanClose();
        void OnClose();
        void OnActivate();
        void OnDeactivate();
    }

	public interface IWorkspaceItemHeader
	{
		bool HasChanges { get; }
        object Content { get; }
	}

	public interface IItemHasId
	{
		object Identificator { get; }
	}
}
