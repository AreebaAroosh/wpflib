using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Reflection;
using System.Windows.Media;

namespace WPFLib
{
    public static class DispatcherObjectHelper
    {
        static IEnumerable<DependencyObject> FindVisualChildrenOneLevel(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                yield return VisualTreeHelper.GetChild(obj, i);
            }
        }

        /// <summary>
        /// Изменяет диспетчера во всех DispatcherObject из визуального дерева obj
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dispatcher"></param>
        public static void ChangeDispatcher(this DependencyObject obj, Dispatcher dispatcher)
        {
            // Меняем диспетчера в обратном порядке, от низа вверх
            foreach (var child in VisualHelper.GetFullTree(obj).Reverse())
            {
                ChangeDispatcherDirect(child, dispatcher);
            }
        }

        static FieldInfo dispatcherField;

        static void ChangeDispatcherDirect(this DispatcherObject obj, Dispatcher dispatcher)
        {
            if (dispatcherField == null)
            {
                dispatcherField = typeof(DispatcherObject).GetField("_dispatcher", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }
            dispatcherField.SetValue(obj, dispatcher);
        }
    }
}
