using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFLib.Contracts;
using System.ComponentModel;
using System.Globalization;

namespace WPFLib.Dialogs
{
	public partial class ItemSelectWindow : Window
	{
		public ItemSelectWindow(ISimpleController controller)
        {
            InitializeComponent();
			Controller = controller;
            Controller.PropertyChanged += new PropertyChangedEventHandler(Controller_PropertyChanged);
            this.DataContext = Controller;
        }

        private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e == Controller.SelectedItemArgs)
            {
                btnOk.IsEnabled = Controller.SelectedItem != null;
            }
        }

		public ISimpleController Controller
		{
			get;
			private set;
		}

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OKButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Controller.SelectedItem != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }

	public class DefaultCatalogNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is ITreeItemNameProvider ? ((ITreeItemNameProvider)value).DisplayName : ((ITreeItemObject)value).Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
