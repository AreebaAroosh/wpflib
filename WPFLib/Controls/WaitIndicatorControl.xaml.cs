using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using WPFLib.Misc;

namespace WPFLib
{
    public partial class WaitIndicatorControl : UserControl
    {
        public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register("StrokeBrush", typeof(Brush), typeof(WaitIndicatorControl), new PropertyMetadata(null, new PropertyChangedCallback(OnStrokeBrushChanged)));
        public Brush StrokeBrush
        {
            get { return (Brush)GetValue(StrokeBrushProperty); }
            set { SetValue(StrokeBrushProperty, value); }
        }

        #region OnStrokeBrushChanged
        private static void OnStrokeBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaitIndicatorControl control = o as WaitIndicatorControl;
            if (control != null)
                control.OnStrokeBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual void OnStrokeBrushChanged(Brush oldValue, Brush newValue)
        {
            RefreshStrokeBrush();
        }
        #endregion

        public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register("FillBrush", typeof(Brush), typeof(WaitIndicatorControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnFillBrushChanged)));
        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        #region OnColorChanged
        private static void OnFillBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaitIndicatorControl control = o as WaitIndicatorControl;
            if (control != null)
                control.OnFillBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual void OnFillBrushChanged(Brush oldValue, Brush newValue)
        {
            RefreshFillBrush();
        }
        #endregion

        private void RefreshStrokeBrush()
        {
            foreach (var el in m_ellipseArray)
            {
                el.Stroke = StrokeBrush;
            }
        }

        private void RefreshFillBrush()
        {
            foreach (var el in m_ellipseArray)
            {
                el.Fill = FillBrush;
            }
        }

        #region Member Variables
        private Ellipse[] m_ellipseArray = null;
        #endregion

        Storyboard IndicatorStoryboard;

        #region Constructor
        public WaitIndicatorControl()
        {
            InitializeComponent();

            UserControl.VisibilityProperty.AddValueChangedWeak(this, VisibilityChanged);

            IndicatorStoryboard = this.FindResource("IndicatorStoryboard") as Storyboard;
            IndicatorStoryboard.Completed += new EventHandler(IndicatorStoryboard_Completed);

            m_ellipseArray = new Ellipse[8];
            // Create a control array that allows use to easy enumerate the ellipses
            // without resorting to reflection
            m_ellipseArray[0] = Ellipse1;
            m_ellipseArray[1] = Ellipse2;
            m_ellipseArray[2] = Ellipse3;
            m_ellipseArray[3] = Ellipse4;
            m_ellipseArray[4] = Ellipse5;
            m_ellipseArray[5] = Ellipse6;
            m_ellipseArray[6] = Ellipse7;
            m_ellipseArray[7] = Ellipse8;

            RefreshFillBrush();
            RefreshStrokeBrush();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                LayoutRoot.Visibility = Visibility.Collapsed;

            this.Start();
        }
        #endregion

        private bool WasActive = false;
        private bool IsVisibilityChanged = false;

        private void VisibilityChanged(object sender, EventArgs args)
        {
            if (IsVisibilityChanged)
                return;
            IsVisibilityChanged = true;
            try
            {
                if (IsStart || IsStop)
                    return;
                if (Visibility == Visibility.Visible)
                {
                    if (WasActive)
                    {
                        this.Start();
                    }
                }
                else
                {
                    WasActive = IsActive;
                    if (IsActive)
                    {
                        this.Stop();
                    }
                }
            }
            finally
            {
                IsVisibilityChanged = false;
            }
        }

        private bool IsActive = false;
        private bool IsStart = false;
        private bool IsStop = false;

        #region Public Functions
        public void Start()
        {
            if (WPFHelper.IsInDesignMode)
                return;
            IsStart = true;
            try
            {
                IsActive = true;
                LayoutRoot.Visibility = Visibility.Visible;
                IndicatorStoryboard.Begin();
            }
            finally
            {
                IsStart = false;
            }
        }

        public void Stop()
        {
            IsStop = true;
            try
            {
                IsActive = false;
                LayoutRoot.Visibility = Visibility.Collapsed;
                IndicatorStoryboard.Stop();
            }
            finally
            {
                IsStop = false;
            }
        }
        #endregion

        #region Event Handlers
        private void IndicatorStoryboard_Completed(object sender, EventArgs e)
        {
            // Loop through each ellipse (backwards) and set its fill colour to that
            // of the previous ellipse to get the effect of the colours changing
            // around the circle of ellipses
            //Brush ellipse8LastBrush = Ellipse8.Fill;
            double ellipse8Opacity = Ellipse8.Opacity;

            for (int index = 7; index > 0; index--)
            {
                //m_ellipseArray[index].Fill = m_ellipseArray[index - 1].Fill;
                m_ellipseArray[index].Opacity = m_ellipseArray[index - 1].Opacity;
            }

            //Ellipse1.Fill = ellipse8LastBrush;
            Ellipse1.Opacity = ellipse8Opacity;

            // We need to restart the storyboard timer, at the completion of
            // which this function / event handler will be called again
            IndicatorStoryboard.Begin();
        }
        #endregion
    }
}
