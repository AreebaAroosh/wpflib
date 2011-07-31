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
using System.Diagnostics;

namespace WPFLib
{
    /// <summary>
    /// Interaction logic for DateTimePickerWrapper.xaml
    /// </summary>
    public partial class DateTimePickerWrapper : UserControl, IDisposable
    {
        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), typeof(DateTimePickerWrapper), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnCheckedChanged) });
        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        #region OnCheckedChanged
        private static void OnCheckedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimePickerWrapper control = o as DateTimePickerWrapper;
            if (control != null)
                control.OnCheckedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        bool isCheckedChange = false;
        System.Windows.Forms.DateTimePickerFormat? prevFormat = null;

        protected virtual void OnCheckedChanged(bool oldValue, bool newValue)
        {
            try
            {
                isCheckedChange = true;
                if (!newValue)
                {
                    picker.CustomFormat = "Не задано";
                    prevFormat = picker.Format;
                    picker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
                }
                else
                {
                    if (prevFormat.HasValue)
                    {
                        picker.Format = prevFormat.Value;
                    }
                }
                if (isPickerChanged || isCheckedChange || isUndo)
                return;
                picker.Checked = newValue;
            }
            finally
            {
                isCheckedChange = false;
            }
        }
        #endregion 

        public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(DateTimePickerWrapper), new UIPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnShowCheckBoxChanged) });
        public bool ShowCheckBox
        {
            get { return (bool)GetValue(ShowCheckBoxProperty); }
            set { SetValue(ShowCheckBoxProperty, value); }
        }

        #region OnShowCheckBoxChanged
        private static void OnShowCheckBoxChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimePickerWrapper control = o as DateTimePickerWrapper;
            if (control != null)
                control.OnShowCheckBoxChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnShowCheckBoxChanged(bool oldValue, bool newValue)
        {
            picker.ShowCheckBox = newValue;
        }
        #endregion 

        public static readonly DependencyProperty ValueProperty =  DependencyProperty.Register("Value", typeof(DateTime?), typeof(DateTimePickerWrapper), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnValueChanged), BindsTwoWayByDefault = true });
        public DateTime? Value
        {
             get { return (DateTime?)GetValue(ValueProperty); }
             set { SetValue(ValueProperty, value); }
        }

        #region OnValueChanged
        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimePickerWrapper control = o as DateTimePickerWrapper;
            if (control != null)
                control.OnValueChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
        }

        bool isValueChanged = false;
        bool isPickerChanged = false;
        bool isUndo = false;

        protected virtual void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
			try
			{
				if (isPickerChanged || isUndo)
					return;
				try
				{
					isValueChanged = true;
					try
					{
						if (newValue.HasValue)
						{
							picker.Checked = true;
							if (picker.MinDate <= newValue && newValue <= picker.MaxDate)
								picker.Value = newValue.Value;
						}
						else
						{
							picker.Checked = false;
						}
                        Checked = picker.Checked;
                    }
					catch (ArgumentOutOfRangeException e)
					{
						try
						{
							isUndo = true;
							this.Value = oldValue;
							this.Checked = oldValue.HasValue;
						}
						finally
						{
							isUndo = false;
						}
					}
				}
				finally
				{
					isValueChanged = false;
				}
			}
			finally
			{
				if (ValueChanged != null)
					ValueChanged(oldValue, newValue);
			}
        }
        #endregion

        #region OnCoerceValue
        private static object OnCoerceValue(DependencyObject o, object value)
        {
            DateTimePickerWrapper control = o as DateTimePickerWrapper;
            if (control != null)
                return control.OnCoerceValue((DateTime)value);
            else
                return value;
        }
        #endregion

        protected virtual DateTime OnCoerceValue(DateTime value)
        {
            return value;
        }

    	public DateTime MinDate
    	{
    		get
    		{
    			return picker.MinDate;
    		}
			set
			{
				picker.MinDate = value;
			}
    	}

    	public DateTime MaxDate
    	{
    		get
    		{
    			return picker.MaxDate;
    		}
			set
			{
				picker.MaxDate = value;
			}
    	}

        System.Windows.Forms.DateTimePicker picker;

        public DateTimePickerWrapper()
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            this.Unloaded += new RoutedEventHandler(DateTimePickerWrapper_Unloaded);
            picker = new System.Windows.Forms.DateTimePicker();
            InitializeComponent();
            host.Child = picker;
            picker.ValueChanged += new EventHandler(picker_ValueChanged);
            picker.DropDown += new EventHandler(picker_DropDown);
            picker.CloseUp += new EventHandler(picker_CloseUp);

            picker.Value = DateTime.Now;
        }

        void DateTimePickerWrapper_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        private void SetValue(DateTime? date)
        {
            try
            {
                isPickerChanged = true;

                this.Value = date;
                this.Checked = date.HasValue;
            }
            finally
            {
                isPickerChanged = false;
            }
        }

        DateTime? midDate = null;

        void picker_CloseUp(object sender, EventArgs e)
        {
            isCalendarShown = false;
            if (midDate != null)
            {
                this.SetValue(midDate.Value);
                midDate = null;
            }
        }

        bool isCalendarShown = false;

        void picker_DropDown(object sender, EventArgs e)
        {
            isCalendarShown = true;
        }

        void picker_ValueChanged(object sender, EventArgs e)
        {
            if (isCheckedChange)
                return;
            if (isCalendarShown)
            {
                midDate = picker.Value;
            }
            else
            {
                if (isValueChanged)
                    return;

                if (picker.Checked)
                {
                    this.SetValue(picker.Value);
                }
                else
                {
                    this.SetValue(null);
                }
            }
        }

    	public event Action<DateTime?, DateTime?> ValueChanged;

        #region IDisposable Members

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            if (dispose)
            {
                host.Dispose();
            }
        }

        ~DateTimePickerWrapper()
        {
            Dispose(false);
        }
        #endregion
    }
}
