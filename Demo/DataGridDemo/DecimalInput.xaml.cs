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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;

namespace DataGridDemo
{
    /// <summary>
    /// Interaction logic for DecimalInput.xaml
    /// </summary>
    [Export("DecimalInput", typeof(UserControl))]
    public partial class DecimalInput : UserControl
    {
        public DecimalInput()
        {
            InitializeComponent();
            var c = new C() { Value = "" };
            this.DataContext = c;
            var coll = new ObservableCollection<C>() { c };

            this.dg.ItemsSource = coll;
        }
    }
}
