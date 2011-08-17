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
    /// Interaction logic for ValidationWrapperDemoControl.xaml
    /// </summary>
    [Export("ValidationWrapperDemoControl", typeof(UserControl))]
    public partial class ValidationWrapperDemoControl : UserControl
    {
        public ValidationWrapperDemoControl()
        {
            InitializeComponent();
            this.DataContext = new ValidationWrapperController();
        }
    }
}
