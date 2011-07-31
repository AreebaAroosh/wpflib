using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WPFLib.Controls
{

	public class FloatTabControl : TabControl
	{
		static FloatTabControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatTabControl), new FrameworkPropertyMetadata(typeof(FloatTabControl)));
		}

		public void HideContent()
		{
			VisualStateManager.GoToState(this, "HiddenContent", true);
			SelectedItem = null;
		}

		public void ShowContent()
		{
			VisualStateManager.GoToState(this, "ShownContent", true);
		}



		public bool IsResizeEnabled
		{
			get { return (bool)GetValue(IsResizeEnabledProperty); }
			set { SetValue(IsResizeEnabledProperty, value); }
		}
		
		public static readonly DependencyProperty IsResizeEnabledProperty =
			DependencyProperty.Register("IsResizeEnabled", typeof(bool), typeof(FloatTabControl));

		

		public override void OnApplyTemplate()
		{
			var ClosePanelButton = this.GetTemplateChild("PART_ClosePanelLeft") as Button;
			var ClosePanelButtonRight = this.GetTemplateChild("PART_ClosePanelRight") as Button;
			var ClosePanelButtonTop = this.GetTemplateChild("PART_ClosePanelTop") as Button;
			var ClosePanelButtonBottom = this.GetTemplateChild("PART_ClosePanelBottom") as Button;
			//if (ClosePanelButton != null)
			{
				var closeButtonClicked = ClosePanelButton.ClickObservable();
				var escPressed = this.PreviewKeyDownObservable().Where(e => e.EventArgs.Key == Key.Escape);
				closeButtonClicked.Merge(escPressed).Merge(ClosePanelButtonRight.ClickObservable()).Merge(ClosePanelButtonTop.ClickObservable())
					.Merge(ClosePanelButtonBottom.ClickObservable()).ObserveOnDispatcher().Subscribe(HideContent);
			}

			//var panelLeft = GetTemplateChild("TabPanelLeft") as TabPanel;
			//var panelRight = GetTemplateChild("TabPanelRight") as TabPanel;
			//var panelTop = GetTemplateChild("TabPanelTop") as TabPanel;
			//var panelBottom = GetTemplateChild("TabPanelBottom") as TabPanel;

			//panelRight.MouseDownObservable().Merge(panelLeft.MouseDownObservable()).Merge(panelTop.MouseDownObservable())
			//    .Merge(panelBottom.MouseDownObservable()).Where(e => e.EventArgs.ChangedButton == MouseButton.Left).ObserveOnDispatcher()
			//    .Subscribe(ShowContent);

			base.OnApplyTemplate();
		}


		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			if (SelectedItem != null)
				ShowContent();
			else
				HideContent();
			base.OnSelectionChanged(e);
		}

		protected Button ClosePanelButton { get; set; }

		protected override void AddChild(object value)
		{
			var obj = value;
			base.AddChild(value);
		}
	}
}
