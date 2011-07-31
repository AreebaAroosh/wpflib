using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Controls;
using System.Windows;

namespace WPFLib.Misc
{
	public static class MenuHelper
	{
		public static bool CheckItem(ItemsControl item)
		{
			CheckItems(item.Items);
			var anyChild = CheckItems(item.Items.OfType<UIElement>().Reverse());
			if (!anyChild)
				item.Visibility = Visibility.Collapsed;
			return anyChild;
		}

		public static bool CheckItems(IEnumerable items)
		{
			bool anyVisible = false;
			bool hasVisible = false;
			foreach (UIElement item in items)
			{
				if (item is ItemsControl && ((ItemsControl)item).HasItems)
				{
					CheckItem(item as ItemsControl);
				}
				if (item is Separator)
				{
					if (!hasVisible)
						item.Visibility = Visibility.Collapsed;
					else if (item.Visibility == Visibility.Visible)
						hasVisible = false;
				}
				else if (item.Visibility == Visibility.Visible)
				{
					hasVisible = true;
					anyVisible = true;
				}
			}
			return anyVisible;
		}
	}
}
