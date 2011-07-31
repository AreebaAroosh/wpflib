using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WPFLib.Contracts
{
    /// <summary>
    /// Интерфейс который надо реализовать элементам дерева
    /// </summary>
    public interface IAsyncTreeItem
    {
        /// <summary>
        /// Признак населения дочерними элементами, дерево само установит его в True,
        /// после вызова IAsyncTreeController.LoadChildren
        /// </summary>
        bool IsLoaded { get; set; }
    }

    /// <summary>
    /// Основная задача интерфейса дать понять что с деревом умеют работать - дерево будет искать реализации IAsyncTreeItemHandler
    /// для основной работы
    /// 
    /// Контроллер для работы с асинхронным деревом
    /// AsyncTreeView при этом работает как обычное TreeView - надо определить HierarchicalDataTemplate
    /// и ItemsSource соответственно - отличий никаких нет.
    /// Просто можно не населять корневые узлы детьми сразу.
    /// 
    /// PS. Про AsyncTreeView - вместо TreeViewItem, в дереве будут AsyncTreeViewItem, которые содержат дополнительные
    /// поля:
    ///     CanExpand - может ли узел разворачиваться(управляет видимостью экспандера)
    ///     Working - включает индикатор ожидания вместо экспандера(используется деревом для идикации работы LoadChildren)
    /// 
    /// Используем для байндинга на свои объекты через TreeView.ItemContainerStyle
    /// </summary>
    public interface IAsyncTreeController
    {
        Task<bool> CanChangeSelection();
    }

    /// <summary>
    /// Интерфейс который надо реализовать для населения детей по требованию.
    /// Тип будет опреден деревом.
    /// TODO: Не обрабатывается случай унаследованных типов - в этом случае дерево может дернуть
    /// не тот метод который нужен
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncTreeItemHandler<T> where T : IAsyncTreeItem
    {
        /// <summary>
        /// Метод по вызову которого элемент должен быть населен дочерними элементами и подготовлен для работы GetParent.
        /// Вызывается только если IAsyncTreeItem.IsLoaded == False
        /// При этом вызове IAsyncTreeItem.IsLoaded устанавливается в True
        /// Метод должен вернуть Task который делает подгрузку, дерево будет его ожидать.
        /// Можно вернуть null, тогда дерево подумает что загрузка произошла синхронно.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task LoadTreeItem(T entity);

        /// <summary>
        /// Возвращает родительский узел, предпологается что родитель будет загружен в LoadTreeItem
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IAsyncTreeItem GetParent(T entity);
        int IndexOf(T entity, IAsyncTreeItem child);
    }
}
