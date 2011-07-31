using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFLib
{
	public static class LogicalHelper
	{
		public static DependencyObject GetAncestorByType(DependencyObject element, Type type)
		{
			if (element == null) return null;
			if (element.GetType() == type) return element;
			return GetAncestorByType(LogicalTreeHelper.GetParent(element), type);
		}

		public static T FindAncestor<T>(DependencyObject element) where T : DependencyObject
		{
			if (element == null) return null;
			if (element is T) return (T)element;
			return FindAncestor<T>(LogicalTreeHelper.GetParent(element));
		}

		public static T FindChild<T>(DependencyObject obj) where T : DependencyObject
		{
			
			foreach (object objChild in  LogicalTreeHelper.GetChildren(obj))
			{
				DependencyObject child = objChild as DependencyObject;
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		public static T FindChild<T>(DependencyObject obj, string name) where T : FrameworkElement
		{
			foreach (object objChild in LogicalTreeHelper.GetChildren(obj))
			{
				DependencyObject child = objChild as DependencyObject;
				if (child != null && child is T && ((T)child).Name == name)
					return (T)child;
				else
				{
					T childOfChild = FindChild<T>(child, name);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}
	}
}
