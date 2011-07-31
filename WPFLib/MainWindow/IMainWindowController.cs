using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Contracts
{
    public interface IMainWindowController
    {
        bool Close(IWorkspaceItem item);
        List<string> SaveState();
        void RestoreState(List<string> state);
        int ItemsCount { get; }
        bool CloseAll();
        /// <summary>
        /// Открыть элемент
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Открытый элемент, может не совпадать с item если он уже был открыт</returns>
        T Open<T>(T item) where T : IWorkspaceItem;
        IWorkspaceItem ActiveItem { get; set; }
    }
}
