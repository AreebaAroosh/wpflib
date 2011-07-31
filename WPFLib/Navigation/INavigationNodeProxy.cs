using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Navigation
{
    /// <summary>
    /// Объект реализующий данный интерфейс не будет восприниматься как узел навигации
    /// (в частности, в процессе построения цепочки узлов навигации, при поиске родительского узла навигации)
    /// </summary>
    public interface INavigationNodeProxy : INavigationNode
    {
        /// <summary>
        /// Это свойство на данный момент предназанчено для реализации прокси для верхнего контрола(закладки)
        /// для того чтобы GetNavigationString знал чью строку навигации действительно надо получить
        /// (менеджер навигации не реализует поиск дочерних узлов навигации)
        /// </summary>
        INavigationNode Child
        {
            get;
        }
    }
}
