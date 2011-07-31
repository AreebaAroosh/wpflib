using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using WPFLib.Contracts;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using WPFLib.Misc;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFLib.Controls;

namespace WPFLib
{
    public class AsyncTreeView : TreeView
    {
        static AsyncTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncTreeView), new FrameworkPropertyMetadata(typeof(AsyncTreeView)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AsyncTreeViewItem(this);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AsyncTreeViewItem;
        }

        public AsyncTreeView()
        {
            this.DataContextChanged +=new DependencyPropertyChangedEventHandler((s,e) => OnDataContextChanged(e.OldValue as IAsyncTreeController , e.NewValue as IAsyncTreeController));
        }

        IAsyncTreeController Controller
        {
            get
            {
                return this.DataContext as IAsyncTreeController;
            }
        }

        void OnDataContextChanged(IAsyncTreeController oldValue, IAsyncTreeController newValue)
        {
            loadedItems.Clear();
            if (oldValue != null)
            {
                oldValue.RequestExpandTo -= new AsyncTreeRequestExpandToEventhandler(OnRequestExpandTo);
            }
            if (newValue != null)
            {
                newValue.RequestExpandTo += new AsyncTreeRequestExpandToEventhandler(OnRequestExpandTo);
            }
        }

        void OnRequestExpandTo(IAsyncTreeController sender, List<int> expandTo)
        {
            //ExpandTo(expandTo);
        }

        internal void OnExpanded(AsyncTreeViewItem item)
        {
            if(Controller != null)
            {
                var dataItem = item.DataContext as IAsyncTreeItem;
                if(dataItem != null && !loadedItems.Contains(dataItem))
                {
                    loadedItems.Add(dataItem);
                    var task = this.Controller.ExpandItem(dataItem);
                    if (task != null)
                    {
                        item.SetWorking(true);
                        task.ContinueWith((t) => Dispatcher.BeginInvoke(() => item.SetWorking(false)));
                    }
                }
            }
        }

        void DumpGeneratorContent(IItemContainerGenerator generator, int count)
        {
            Debug.WriteLine("Generator positions:");
            for (int i = 0; i < count; i++)
            {
                GeneratorPosition position = generator.GeneratorPositionFromIndex(i);
                Debug.WriteLine("Item index=" + i + ", Generator position: index=" + position.Index + ", offset=" + position.Offset);
            }
            Debug.WriteLine("");
        }

        //void RunExpandTo(List<int> indexes)
        //{
        //    ExpandTo(indexes);
        //    //Run(ExpandTo(indexes).GetEnumerator(), null);
        //}

        //void Run(IEnumerator<Task> en, Action cont)
        //{
        //    bool res;
        //    try
        //    {
        //        res = en.MoveNext();
        //    }
        //    catch
        //    {
        //        en.Dispose();
        //        throw;
        //    }
        //    if (!res) {
        //        try
        //        {
        //            if (cont != null)
        //            {
        //                cont();
        //            }
        //            return;
        //        }
        //        finally
        //        {
        //            en.Dispose();
        //        }
        //    }
        //    en.Current.ContinueWith
        //        //((t) => Dispatcher.BeginInvoke(() => { Run(en, cont); }, System.Windows.Threading.DispatcherPriority.Send));
        //        ((t) => Run(en, cont), TaskScheduler.FromCurrentSynchronizationContext());
        //}

        HashSet<IAsyncTreeItem> loadedItems = new HashSet<IAsyncTreeItem>();

        private TreeViewItem GetTreeViewItem(ItemsControl container, List<int> path)
        {
            int index = path[0];
            if (container != null)
            {
                // Expand the current container
                if (container is TreeViewItem)
                {
                    var dataItem = container.DataContext as IAsyncTreeItem;
                    if (dataItem != null && this.Controller != null && !loadedItems.Contains(dataItem))
                    {
                        loadedItems.Add(dataItem);
                        var task = this.Controller.ExpandItem(dataItem);
                        if (task != null)
                            task.Wait();
                    }
                    if (!((TreeViewItem)container).IsExpanded)
                    {
                        container.SetValue(TreeViewItem.IsExpandedProperty, true);
                    }
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the 
                // virtualizing case even if the item is marked 
                // expanded we still need to do this step in order to 
                // regenerate the visuals because they may have been virtualized away.

                //container.ApplyTemplate();
                ItemsPresenter itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter, 
                    // so walk the descendents and find the child.
                    itemsPresenter = VisualHelper.FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = VisualHelper.FindVisualChild<ItemsPresenter>(container);
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);


                // Ensure that the generator for this panel has been created.
                UIElementCollection children = itemsHostPanel.Children;

                AsyncTreeVirtualizingStackPanel virtualizingPanel =
                    itemsHostPanel as AsyncTreeVirtualizingStackPanel;

                //for (int i = 0, count = container.Items.Count; i < count; i++)
                //{
                TreeViewItem subContainer;
                if (virtualizingPanel != null)
                {
                    // Bring the item into view so 
                    // that the container will be generated.
                    virtualizingPanel.BringIntoView(index);

                    subContainer =
                        (TreeViewItem)container.ItemContainerGenerator.
                        ContainerFromIndex(index);
                }
                else
                {
                    subContainer =
                        (TreeViewItem)container.ItemContainerGenerator.
                        ContainerFromIndex(index);

                    // Bring the item into view to maintain the 
                    // same behavior as with a virtualizing panel.
                    subContainer.BringIntoView();
                }

                if (subContainer != null)
                {
                    if (path.Count > 1)
                    {
                        var newPath = path.ToList();
                        newPath.RemoveAt(0);
                        // Search the next level for the object.
                        TreeViewItem resultContainer = GetTreeViewItem(subContainer, newPath);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                    }
                    return subContainer;
                }
                //}
            }

            return null;
        }

        void ExpandTo(List<int> indexes)
        {
            var container = GetTreeViewItem(this, indexes);
            container.IsSelected = true;
        }

        void gen_StatusChanged(object sender, EventArgs e)
        {
            var gen = sender as ItemContainerGenerator;
            Debug.WriteLine("Status " + gen.Status);
        }
    }
}
