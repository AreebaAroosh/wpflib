using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace WPFLib
{
    public class DisposeHelper
    {
        /// <summary>
        /// Вызывает Dispose у всего визуального дерева
        /// </summary>
        /// <param name="element"></param>
        public static void DisposeVisualTree(DependencyObject element)
        {
            foreach (var child in VisualHelper.FindVisualChildrenOneLevel(element))
            {
                var disposable = child as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                DisposeVisualTree(child);
            }
        }
    }
}
