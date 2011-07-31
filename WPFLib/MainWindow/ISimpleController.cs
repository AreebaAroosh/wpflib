using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPFLib.Contracts
{
	public interface ISimpleController : INotifyPropertyChanged
	{
		string DimensionName
		{
			get;
		}

		object SelectedItem
		{
			get;
			set;
		}

		PropertyChangedEventArgs SelectedItemArgs
		{
			get;
		}

		void ExpandTo(object item);
	}

	public interface ITreeItemObject
	{
		int Id
		{
			get;
		}
		string Name
		{
			get;
		}
	}

	public interface ITreeItemNameProvider
	{
		string DisplayName
		{
			get;
		}
	}
}
