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
using WPFLib.Misc;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DataGridDemo
{
    class C : DispatcherPropertyChangedHelper
    {
        #region ValueProperty
        public static readonly PropertyChangedEventArgs ValueArgs = PropertyChangedHelper.CreateArgs<C>(c => c.Value);
        private string _Value;

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                var oldValue = Value;
                _Value = value;
                if (oldValue != value)
                {
                    OnValueChanged(oldValue, value);
                    OnPropertyChanged(ValueArgs);
                }
            }
        }

        protected virtual void OnValueChanged(string oldValue, string newValue)
        {
        }
        #endregion

        #region FakeProperty
        public static readonly PropertyChangedEventArgs FakeArgs = PropertyChangedHelper.CreateArgs<C>(c => c.Fake);
        private string _Fake;

        public string Fake
        {
            get
            {
                return _Fake;
            }
            set
            {
                var oldValue = Fake;
                _Fake = value;
                if (oldValue != value)
                {
                    OnFakeChanged(oldValue, value);
                    OnPropertyChanged(FakeArgs);
                }
            }
        }

        protected virtual void OnFakeChanged(string oldValue, string newValue)
        {
        }
        #endregion

    }
    /// <summary>
    /// Interaction logic for CellByCellCommit.xaml
    /// </summary>
    [Export("CellByCellCommit", typeof(UserControl))]
    public partial class CellByCellCommit : UserControl
    {
        public CellByCellCommit()
        {
            InitializeComponent();
            var c = new C() { Value = "InitialValue" };
            this.DataContext = c;
            var coll = new ObservableCollection<C>() { c };
            this.dgCellByCell.ItemsSource = coll;
            this.dgBase.ItemsSource = coll;
        }
    }
}
