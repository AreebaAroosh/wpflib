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

namespace DataWrapperDemo
{
    /// <summary>
    /// Interaction logic for DataWrapperDemoControl.xaml
    /// </summary>
    [Export("DataWrapperDemoControl", typeof(UserControl))]
    public partial class DataWrapperDemoControl : UserControl
    {
        public DataWrapperDemoControl()
        {
            InitializeComponent();
            this.DataContext = new DataWrapperController();
        }
    }
}
