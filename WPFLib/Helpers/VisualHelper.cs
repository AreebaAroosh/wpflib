using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace WPFLib
{
    public static class VisualHelper
    {
        /// <summary>
        /// Все дочерние элементы включая obj
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetFullTree(DependencyObject obj)
        {
            yield return obj;
            foreach (var child in FindVisualChildrenOneLevel(obj))
            {
                foreach (var item in GetFullTree(child))
                    yield return item;
            }
        }

        public static DependencyObject GetAncestorByType(DependencyObject element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            return GetAncestorByType(VisualTreeHelper.GetParent(element), type);
        }

        public static T FindVisualAncestor<T>(DependencyObject element) where T : class
        {
            if (element == null) return null;
            if (element is T) return (T)(object)element;
            return FindVisualAncestor<T>(VisualTreeHelper.GetParent(element));
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && ((T)child).Name == name)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildrenOneLevel<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    yield return (T)child;
            }
            //            yield return null;
        }

        public static IEnumerable<DependencyObject> FindVisualChildrenOneLevel(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                yield return VisualTreeHelper.GetChild(obj, i);
            }
        }
    }
}
