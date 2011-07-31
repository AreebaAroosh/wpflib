using System.Linq;
using System.Collections.Generic;

namespace System
{
	public static class ExtensionMethods
	{
		public static IObservable<IEvent<System.Windows.Data.DataTransferEventArgs>> TargetUpdatedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Data.DataTransferEventArgs>(source,"TargetUpdated" );
		}
		public static IObservable<IEvent<System.Windows.Data.DataTransferEventArgs>> SourceUpdatedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Data.DataTransferEventArgs>(source,"SourceUpdated" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> DataContextChangedObservable(this System.Windows.FrameworkElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.DataContextChanged += h;
                return () => { source.DataContextChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.RequestBringIntoViewEventArgs>> RequestBringIntoViewObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.RequestBringIntoViewEventArgs>(source,"RequestBringIntoView" );
		}
		public static IObservable<IEvent<System.Windows.SizeChangedEventArgs>> SizeChangedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.SizeChangedEventArgs>(source,"SizeChanged" );
		}
		public static IObservable<IEvent<System.EventArgs>> InitializedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Initialized" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> LoadedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Loaded" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnloadedObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unloaded" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ToolTipEventArgs>> ToolTipOpeningObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ToolTipEventArgs>(source,"ToolTipOpening" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ToolTipEventArgs>> ToolTipClosingObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ToolTipEventArgs>(source,"ToolTipClosing" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ContextMenuEventArgs>> ContextMenuOpeningObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ContextMenuEventArgs>(source,"ContextMenuOpening" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ContextMenuEventArgs>> ContextMenuClosingObservable(this System.Windows.FrameworkElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ContextMenuEventArgs>(source,"ContextMenuClosing" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseDoubleClickObservable(this System.Windows.Controls.Control source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseDoubleClick" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseDoubleClickObservable(this System.Windows.Controls.Control source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseDoubleClick" );
		}
		public static IObservable<IEvent<System.EventArgs>> SourceInitializedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"SourceInitialized" );
		}
		public static IObservable<IEvent<System.EventArgs>> ActivatedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Activated" );
		}
		public static IObservable<IEvent<System.EventArgs>> DeactivatedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Deactivated" );
		}
		public static IObservable<IEvent<System.EventArgs>> StateChangedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"StateChanged" );
		}
		public static IObservable<IEvent<System.EventArgs>> LocationChangedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"LocationChanged" );
		}
		public static IObservable<IEvent<System.ComponentModel.CancelEventArgs>> ClosingObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.ComponentModel.CancelEventArgs>(source,"Closing" );
		}
		public static IObservable<IEvent<System.EventArgs>> ClosedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Closed" );
		}
		public static IObservable<IEvent<System.EventArgs>> ContentRenderedObservable(this System.Windows.Window source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"ContentRendered" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigatingCancelEventArgs>> NavigatingObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigatingCancelEventArgs>(source,"Navigating" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationProgressEventArgs>> NavigationProgressObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationProgressEventArgs>(source,"NavigationProgress" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationFailedEventArgs>> NavigationFailedObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationFailedEventArgs>(source,"NavigationFailed" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigatedObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"Navigated" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> LoadCompletedObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"LoadCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigationStoppedObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"NavigationStopped" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.FragmentNavigationEventArgs>> FragmentNavigationObservable(this System.Windows.Navigation.NavigationWindow source)
		{
			return Observable.FromEvent<System.Windows.Navigation.FragmentNavigationEventArgs>(source,"FragmentNavigation" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> OpenedObservable(this System.Windows.Controls.ContextMenu source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Opened" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ClosedObservable(this System.Windows.Controls.ContextMenu source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Closed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ClickObservable(this System.Windows.Controls.MenuItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Click" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CheckedObservable(this System.Windows.Controls.MenuItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Checked" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UncheckedObservable(this System.Windows.Controls.MenuItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unchecked" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SubmenuOpenedObservable(this System.Windows.Controls.MenuItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"SubmenuOpened" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SubmenuClosedObservable(this System.Windows.Controls.MenuItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"SubmenuClosed" );
		}
		public static IObservable<IEvent<System.EventArgs>> PageViewsChangedObservable(this System.Windows.Controls.Primitives.DocumentViewerBase source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"PageViewsChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ClickObservable(this System.Windows.Controls.Primitives.ButtonBase source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Click" );
		}
		public static IObservable<IEvent<System.Windows.Controls.SelectionChangedEventArgs>> SelectedDatesChangedObservable(this System.Windows.Controls.Calendar source)
		{
			return Observable.FromEvent<System.Windows.Controls.SelectionChangedEventArgs>(source,"SelectedDatesChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.CalendarDateChangedEventArgs>> DisplayDateChangedObservable(this System.Windows.Controls.Calendar source)
		{
			return Observable.FromEvent<System.Windows.Controls.CalendarDateChangedEventArgs>(source,"DisplayDateChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.CalendarModeChangedEventArgs>> DisplayModeChangedObservable(this System.Windows.Controls.Calendar source)
		{
			return Observable.FromEvent<System.Windows.Controls.CalendarModeChangedEventArgs>(source,"DisplayModeChanged" );
		}
		public static IObservable<IEvent<System.EventArgs>> SelectionModeChangedObservable(this System.Windows.Controls.Calendar source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"SelectionModeChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CheckedObservable(this System.Windows.Controls.Primitives.ToggleButton source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Checked" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UncheckedObservable(this System.Windows.Controls.Primitives.ToggleButton source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unchecked" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> IndeterminateObservable(this System.Windows.Controls.Primitives.ToggleButton source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Indeterminate" );
		}
		public static IObservable<IEvent<System.Windows.Controls.SelectionChangedEventArgs>> SelectionChangedObservable(this System.Windows.Controls.Primitives.Selector source)
		{
			return Observable.FromEvent<System.Windows.Controls.SelectionChangedEventArgs>(source,"SelectionChanged" );
		}
		public static IObservable<IEvent<System.EventArgs>> DropDownOpenedObservable(this System.Windows.Controls.ComboBox source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"DropDownOpened" );
		}
		public static IObservable<IEvent<System.EventArgs>> DropDownClosedObservable(this System.Windows.Controls.ComboBox source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"DropDownClosed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SelectedObservable(this System.Windows.Controls.ListBoxItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Selected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnselectedObservable(this System.Windows.Controls.ListBoxItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unselected" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridColumnEventArgs>> ColumnDisplayIndexChangedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridColumnEventArgs>(source,"ColumnDisplayIndexChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowEventArgs>> LoadingRowObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowEventArgs>(source,"LoadingRow" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowEventArgs>> UnloadingRowObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowEventArgs>(source,"UnloadingRow" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowEditEndingEventArgs>> RowEditEndingObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowEditEndingEventArgs>(source,"RowEditEnding" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridCellEditEndingEventArgs>> CellEditEndingObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridCellEditEndingEventArgs>(source,"CellEditEnding" );
		}
		public static IObservable<IEvent<System.EventArgs>> CurrentCellChangedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"CurrentCellChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridBeginningEditEventArgs>> BeginningEditObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridBeginningEditEventArgs>(source,"BeginningEdit" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridPreparingCellForEditEventArgs>> PreparingCellForEditObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridPreparingCellForEditEventArgs>(source,"PreparingCellForEdit" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InitializingNewItemEventArgs>> InitializingNewItemObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.InitializingNewItemEventArgs>(source,"InitializingNewItem" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>> LoadingRowDetailsObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>(source,"LoadingRowDetails" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>> UnloadingRowDetailsObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>(source,"UnloadingRowDetails" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>> RowDetailsVisibilityChangedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowDetailsEventArgs>(source,"RowDetailsVisibilityChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.SelectedCellsChangedEventArgs>> SelectedCellsChangedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.SelectedCellsChangedEventArgs>(source,"SelectedCellsChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridSortingEventArgs>> SortingObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridSortingEventArgs>(source,"Sorting" );
		}
		public static IObservable<IEvent<System.EventArgs>> AutoGeneratedColumnsObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"AutoGeneratedColumns" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs>> AutoGeneratingColumnObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs>(source,"AutoGeneratingColumn" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridColumnReorderingEventArgs>> ColumnReorderingObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridColumnReorderingEventArgs>(source,"ColumnReordering" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragStartedEventArgs>> ColumnHeaderDragStartedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragStartedEventArgs>(source,"ColumnHeaderDragStarted" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragDeltaEventArgs>> ColumnHeaderDragDeltaObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragDeltaEventArgs>(source,"ColumnHeaderDragDelta" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragCompletedEventArgs>> ColumnHeaderDragCompletedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragCompletedEventArgs>(source,"ColumnHeaderDragCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridColumnEventArgs>> ColumnReorderedObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridColumnEventArgs>(source,"ColumnReordered" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridRowClipboardEventArgs>> CopyingRowClipboardContentObservable(this System.Windows.Controls.DataGrid source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridRowClipboardEventArgs>(source,"CopyingRowClipboardContent" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridCellClipboardEventArgs>> CopyingCellClipboardContentObservable(this System.Windows.Controls.DataGridColumn source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridCellClipboardEventArgs>(source,"CopyingCellClipboardContent" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DataGridCellClipboardEventArgs>> PastingCellClipboardContentObservable(this System.Windows.Controls.DataGridColumn source)
		{
			return Observable.FromEvent<System.Windows.Controls.DataGridCellClipboardEventArgs>(source,"PastingCellClipboardContent" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SelectedObservable(this System.Windows.Controls.DataGridCell source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Selected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnselectedObservable(this System.Windows.Controls.DataGridCell source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unselected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SelectedObservable(this System.Windows.Controls.DataGridRow source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Selected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnselectedObservable(this System.Windows.Controls.DataGridRow source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unselected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CalendarClosedObservable(this System.Windows.Controls.DatePicker source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"CalendarClosed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CalendarOpenedObservable(this System.Windows.Controls.DatePicker source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"CalendarOpened" );
		}
		public static IObservable<IEvent<System.Windows.Controls.DatePickerDateValidationErrorEventArgs>> DateValidationErrorObservable(this System.Windows.Controls.DatePicker source)
		{
			return Observable.FromEvent<System.Windows.Controls.DatePickerDateValidationErrorEventArgs>(source,"DateValidationError" );
		}
		public static IObservable<IEvent<System.Windows.Controls.SelectionChangedEventArgs>> SelectedDateChangedObservable(this System.Windows.Controls.DatePicker source)
		{
			return Observable.FromEvent<System.Windows.Controls.SelectionChangedEventArgs>(source,"SelectedDateChanged" );
		}
		public static IObservable<IEvent<System.Windows.Data.DataTransferEventArgs>> TargetUpdatedObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Data.DataTransferEventArgs>(source,"TargetUpdated" );
		}
		public static IObservable<IEvent<System.Windows.Data.DataTransferEventArgs>> SourceUpdatedObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Data.DataTransferEventArgs>(source,"SourceUpdated" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> DataContextChangedObservable(this System.Windows.FrameworkContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.DataContextChanged += h;
                return () => { source.DataContextChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.EventArgs>> InitializedObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Initialized" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> LoadedObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Loaded" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnloadedObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unloaded" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ToolTipEventArgs>> ToolTipOpeningObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ToolTipEventArgs>(source,"ToolTipOpening" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ToolTipEventArgs>> ToolTipClosingObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ToolTipEventArgs>(source,"ToolTipClosing" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ContextMenuEventArgs>> ContextMenuOpeningObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ContextMenuEventArgs>(source,"ContextMenuOpening" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ContextMenuEventArgs>> ContextMenuClosingObservable(this System.Windows.FrameworkContentElement source)
		{
			return Observable.FromEvent<System.Windows.Controls.ContextMenuEventArgs>(source,"ContextMenuClosing" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ExpandedObservable(this System.Windows.Controls.Expander source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Expanded" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CollapsedObservable(this System.Windows.Controls.Expander source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Collapsed" );
		}
		public static IObservable<IEvent<System.EventArgs>> ContentRenderedObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"ContentRendered" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigatingCancelEventArgs>> NavigatingObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigatingCancelEventArgs>(source,"Navigating" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationProgressEventArgs>> NavigationProgressObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationProgressEventArgs>(source,"NavigationProgress" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationFailedEventArgs>> NavigationFailedObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationFailedEventArgs>(source,"NavigationFailed" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigatedObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"Navigated" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> LoadCompletedObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"LoadCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigationStoppedObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"NavigationStopped" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.FragmentNavigationEventArgs>> FragmentNavigationObservable(this System.Windows.Controls.Frame source)
		{
			return Observable.FromEvent<System.Windows.Navigation.FragmentNavigationEventArgs>(source,"FragmentNavigation" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragStartedEventArgs>> DragStartedObservable(this System.Windows.Controls.Primitives.Thumb source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragStartedEventArgs>(source,"DragStarted" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragDeltaEventArgs>> DragDeltaObservable(this System.Windows.Controls.Primitives.Thumb source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragDeltaEventArgs>(source,"DragDelta" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.DragCompletedEventArgs>> DragCompletedObservable(this System.Windows.Controls.Primitives.Thumb source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.DragCompletedEventArgs>(source,"DragCompleted" );
		}
		public static IObservable<IEvent<System.Windows.ExceptionRoutedEventArgs>> ImageFailedObservable(this System.Windows.Controls.Image source)
		{
			return Observable.FromEvent<System.Windows.ExceptionRoutedEventArgs>(source,"ImageFailed" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasStrokeCollectedEventArgs>> StrokeCollectedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasStrokeCollectedEventArgs>(source,"StrokeCollected" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasGestureEventArgs>> GestureObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasGestureEventArgs>(source,"Gesture" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasStrokesReplacedEventArgs>> StrokesReplacedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasStrokesReplacedEventArgs>(source,"StrokesReplaced" );
		}
		public static IObservable<IEvent<System.Windows.Ink.DrawingAttributesReplacedEventArgs>> DefaultDrawingAttributesReplacedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Ink.DrawingAttributesReplacedEventArgs>(source,"DefaultDrawingAttributesReplaced" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ActiveEditingModeChangedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"ActiveEditingModeChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> EditingModeChangedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"EditingModeChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> EditingModeInvertedChangedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"EditingModeInvertedChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasSelectionEditingEventArgs>> SelectionMovingObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasSelectionEditingEventArgs>(source,"SelectionMoving" );
		}
		public static IObservable<IEvent<System.EventArgs>> SelectionMovedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"SelectionMoved" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasStrokeErasingEventArgs>> StrokeErasingObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasStrokeErasingEventArgs>(source,"StrokeErasing" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> StrokeErasedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"StrokeErased" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasSelectionEditingEventArgs>> SelectionResizingObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasSelectionEditingEventArgs>(source,"SelectionResizing" );
		}
		public static IObservable<IEvent<System.EventArgs>> SelectionResizedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"SelectionResized" );
		}
		public static IObservable<IEvent<System.Windows.Controls.InkCanvasSelectionChangingEventArgs>> SelectionChangingObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.Windows.Controls.InkCanvasSelectionChangingEventArgs>(source,"SelectionChanging" );
		}
		public static IObservable<IEvent<System.EventArgs>> SelectionChangedObservable(this System.Windows.Controls.InkCanvas source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"SelectionChanged" );
		}
		public static IObservable<IEvent<System.Windows.ExceptionRoutedEventArgs>> MediaFailedObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.ExceptionRoutedEventArgs>(source,"MediaFailed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> MediaOpenedObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"MediaOpened" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> BufferingStartedObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"BufferingStarted" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> BufferingEndedObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"BufferingEnded" );
		}
		public static IObservable<IEvent<System.Windows.MediaScriptCommandRoutedEventArgs>> ScriptCommandObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.MediaScriptCommandRoutedEventArgs>(source,"ScriptCommand" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> MediaEndedObservable(this System.Windows.Controls.MediaElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"MediaEnded" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> PasswordChangedObservable(this System.Windows.Controls.PasswordBox source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"PasswordChanged" );
		}
		public static IObservable<IEvent<System.Windows.Controls.TextChangedEventArgs>> TextChangedObservable(this System.Windows.Controls.Primitives.TextBoxBase source)
		{
			return Observable.FromEvent<System.Windows.Controls.TextChangedEventArgs>(source,"TextChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SelectionChangedObservable(this System.Windows.Controls.Primitives.TextBoxBase source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"SelectionChanged" );
		}
		public static IObservable<IEvent<System.EventArgs>> PageConnectedObservable(this System.Windows.Controls.Primitives.DocumentPageView source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"PageConnected" );
		}
		public static IObservable<IEvent<System.EventArgs>> PageDisconnectedObservable(this System.Windows.Controls.Primitives.DocumentPageView source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"PageDisconnected" );
		}
		public static IObservable<IEvent<System.EventArgs>> OpenedObservable(this System.Windows.Controls.Primitives.Popup source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Opened" );
		}
		public static IObservable<IEvent<System.EventArgs>> ClosedObservable(this System.Windows.Controls.Primitives.Popup source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Closed" );
		}
		public static IObservable<IEvent<System.Windows.Controls.Primitives.ScrollEventArgs>> ScrollObservable(this System.Windows.Controls.Primitives.ScrollBar source)
		{
			return Observable.FromEvent<System.Windows.Controls.Primitives.ScrollEventArgs>(source,"Scroll" );
		}
		public static IObservable<IEvent<System.Windows.Controls.ScrollChangedEventArgs>> ScrollChangedObservable(this System.Windows.Controls.ScrollViewer source)
		{
			return Observable.FromEvent<System.Windows.Controls.ScrollChangedEventArgs>(source,"ScrollChanged" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> OpenedObservable(this System.Windows.Controls.ToolTip source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Opened" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ClosedObservable(this System.Windows.Controls.ToolTip source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Closed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ExpandedObservable(this System.Windows.Controls.TreeViewItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Expanded" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> CollapsedObservable(this System.Windows.Controls.TreeViewItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Collapsed" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> SelectedObservable(this System.Windows.Controls.TreeViewItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Selected" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> UnselectedObservable(this System.Windows.Controls.TreeViewItem source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Unselected" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigatingCancelEventArgs>> NavigatingObservable(this System.Windows.Controls.WebBrowser source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigatingCancelEventArgs>(source,"Navigating" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigatedObservable(this System.Windows.Controls.WebBrowser source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"Navigated" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> LoadCompletedObservable(this System.Windows.Controls.WebBrowser source)
		{
			return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(source,"LoadCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Data.FilterEventArgs>> FilterObservable(this System.Windows.Data.CollectionViewSource source)
		{
			return Observable.FromEvent<System.Windows.Data.FilterEventArgs>(source,"Filter" );
		}
		public static IObservable<IEvent<System.Windows.Navigation.RequestNavigateEventArgs>> RequestNavigateObservable(this System.Windows.Documents.Hyperlink source)
		{
			return Observable.FromEvent<System.Windows.Navigation.RequestNavigateEventArgs>(source,"RequestNavigate" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> ClickObservable(this System.Windows.Documents.Hyperlink source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"Click" );
		}
		public static IObservable<IEvent<System.Windows.Documents.GetPageRootCompletedEventArgs>> GetPageRootCompletedObservable(this System.Windows.Documents.PageContent source)
		{
			return Observable.FromEvent<System.Windows.Documents.GetPageRootCompletedEventArgs>(source,"GetPageRootCompleted" );
		}
		public static IObservable<IEvent<System.EventArgs>> ClickObservable(this System.Windows.Shell.ThumbButtonInfo source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Click" );
		}
		public static IObservable<IEvent<System.Windows.VisualStateChangedEventArgs>> CurrentStateChangedObservable(this System.Windows.VisualStateGroup source)
		{
			return Observable.FromEvent<System.Windows.VisualStateChangedEventArgs>(source,"CurrentStateChanged" );
		}
		public static IObservable<IEvent<System.Windows.VisualStateChangedEventArgs>> CurrentStateChangingObservable(this System.Windows.VisualStateGroup source)
		{
			return Observable.FromEvent<System.Windows.VisualStateChangedEventArgs>(source,"CurrentStateChanging" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> GotFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"GotFocus" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> LostFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"LostFocus" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsEnabledChanged += h;
                return () => { source.IsEnabledChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.FocusableChanged += h;
                return () => { source.FocusableChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> PreviewMouseMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"PreviewMouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> PreviewMouseWheelObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"PreviewMouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> MouseWheelObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"MouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseEnterObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseLeaveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> GotMouseCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"GotMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> LostMouseCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"LostMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.QueryCursorEventArgs>> QueryCursorObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.QueryCursorEventArgs>(source,"QueryCursor" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> PreviewStylusDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"PreviewStylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> StylusDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"StylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInAirMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInAirMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusEnterObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusLeaveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInRangeObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInRangeObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusOutOfRangeObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusOutOfRangeObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"PreviewStylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> StylusSystemGestureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"StylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> GotStylusCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"GotStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> LostStylusCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"LostStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewGotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> GotKeyboardFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"GotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewLostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> LostKeyboardFocusObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"LostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> PreviewTextInputObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"PreviewTextInput" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> TextInputObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"TextInput" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"PreviewQueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> QueryContinueDragObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"QueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"PreviewGiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> GiveFeedbackObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"GiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragEnterObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragEnterObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragOverObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragOverObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragLeaveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragLeaveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDropObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDrop" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DropObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"Drop" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchDownObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchMoveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchUpObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> GotTouchCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"GotTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> LostTouchCaptureObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"LostTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchEnterObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchLeaveObservable(this System.Windows.ContentElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchLeave" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseDirectlyOverChanged += h;
                return () => { source.IsMouseDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusWithinChanged += h;
                return () => { source.IsKeyboardFocusWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCapturedChanged += h;
                return () => { source.IsMouseCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCaptureWithinChanged += h;
                return () => { source.IsMouseCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusDirectlyOverChanged += h;
                return () => { source.IsStylusDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCapturedChanged += h;
                return () => { source.IsStylusCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCaptureWithinChanged += h;
                return () => { source.IsStylusCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this System.Windows.ContentElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusedChanged += h;
                return () => { source.IsKeyboardFocusedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> PreviewMouseMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"PreviewMouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> PreviewMouseWheelObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"PreviewMouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> MouseWheelObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"MouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseEnterObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseLeaveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> GotMouseCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"GotMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> LostMouseCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"LostMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.QueryCursorEventArgs>> QueryCursorObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.QueryCursorEventArgs>(source,"QueryCursor" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> PreviewStylusDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"PreviewStylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> StylusDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"StylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInAirMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInAirMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusEnterObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusLeaveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInRangeObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInRangeObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusOutOfRangeObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusOutOfRangeObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"PreviewStylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> StylusSystemGestureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"StylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> GotStylusCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"GotStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> LostStylusCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"LostStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewGotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> GotKeyboardFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"GotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewLostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> LostKeyboardFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"LostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> PreviewTextInputObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"PreviewTextInput" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> TextInputObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"TextInput" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"PreviewQueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> QueryContinueDragObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"QueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"PreviewGiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> GiveFeedbackObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"GiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragEnterObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragEnterObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragOverObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragOverObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragLeaveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragLeaveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDropObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDrop" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DropObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"Drop" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchDownObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchMoveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchUpObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> GotTouchCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"GotTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> LostTouchCaptureObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"LostTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchEnterObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchLeaveObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchLeave" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseDirectlyOverChanged += h;
                return () => { source.IsMouseDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusWithinChanged += h;
                return () => { source.IsKeyboardFocusWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCapturedChanged += h;
                return () => { source.IsMouseCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCaptureWithinChanged += h;
                return () => { source.IsMouseCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusDirectlyOverChanged += h;
                return () => { source.IsStylusDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCapturedChanged += h;
                return () => { source.IsStylusCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCaptureWithinChanged += h;
                return () => { source.IsStylusCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusedChanged += h;
                return () => { source.IsKeyboardFocusedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.EventArgs>> LayoutUpdatedObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"LayoutUpdated" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> GotFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"GotFocus" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> LostFocusObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"LostFocus" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsEnabledChanged += h;
                return () => { source.IsEnabledChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsHitTestVisibleChanged += h;
                return () => { source.IsHitTestVisibleChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsVisibleChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsVisibleChanged += h;
                return () => { source.IsVisibleChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this System.Windows.UIElement source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.FocusableChanged += h;
                return () => { source.FocusableChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationStartingEventArgs>> ManipulationStartingObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationStartingEventArgs>(source,"ManipulationStarting" );
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationStartedEventArgs>> ManipulationStartedObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationStartedEventArgs>(source,"ManipulationStarted" );
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationDeltaEventArgs>> ManipulationDeltaObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationDeltaEventArgs>(source,"ManipulationDelta" );
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationInertiaStartingEventArgs>> ManipulationInertiaStartingObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationInertiaStartingEventArgs>(source,"ManipulationInertiaStarting" );
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationBoundaryFeedbackEventArgs>> ManipulationBoundaryFeedbackObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationBoundaryFeedbackEventArgs>(source,"ManipulationBoundaryFeedback" );
		}
		public static IObservable<IEvent<System.Windows.Input.ManipulationCompletedEventArgs>> ManipulationCompletedObservable(this System.Windows.UIElement source)
		{
			return Observable.FromEvent<System.Windows.Input.ManipulationCompletedEventArgs>(source,"ManipulationCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseLeftButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseLeftButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseLeftButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> PreviewMouseRightButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"PreviewMouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseButtonEventArgs>> MouseRightButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseButtonEventArgs>(source,"MouseRightButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> PreviewMouseMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"PreviewMouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> PreviewMouseWheelObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"PreviewMouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseWheelEventArgs>> MouseWheelObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseWheelEventArgs>(source,"MouseWheel" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseEnterObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> MouseLeaveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"MouseLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> GotMouseCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"GotMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.MouseEventArgs>> LostMouseCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.MouseEventArgs>(source,"LostMouseCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.QueryCursorEventArgs>> QueryCursorObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.QueryCursorEventArgs>(source,"QueryCursor" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> PreviewStylusDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"PreviewStylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusDownEventArgs>> StylusDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusDownEventArgs>(source,"StylusDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInAirMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInAirMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInAirMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusEnterObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusLeaveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusLeave" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusInRangeObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusInRangeObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusInRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> PreviewStylusOutOfRangeObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"PreviewStylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> StylusOutOfRangeObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"StylusOutOfRange" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"PreviewStylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusSystemGestureEventArgs>> StylusSystemGestureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusSystemGestureEventArgs>(source,"StylusSystemGesture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> GotStylusCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"GotStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusEventArgs>> LostStylusCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusEventArgs>(source,"LostStylusCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> StylusButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"StylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.StylusButtonEventArgs>> PreviewStylusButtonUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.StylusButtonEventArgs>(source,"PreviewStylusButtonUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> PreviewKeyUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"PreviewKeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyEventArgs>> KeyUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyEventArgs>(source,"KeyUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewGotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> GotKeyboardFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"GotKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"PreviewLostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>> LostKeyboardFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.KeyboardFocusChangedEventArgs>(source,"LostKeyboardFocus" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> PreviewTextInputObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"PreviewTextInput" );
		}
		public static IObservable<IEvent<System.Windows.Input.TextCompositionEventArgs>> TextInputObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TextCompositionEventArgs>(source,"TextInput" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"PreviewQueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.QueryContinueDragEventArgs>> QueryContinueDragObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.QueryContinueDragEventArgs>(source,"QueryContinueDrag" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"PreviewGiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.GiveFeedbackEventArgs>> GiveFeedbackObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.GiveFeedbackEventArgs>(source,"GiveFeedback" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragEnterObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragEnterObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragEnter" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragOverObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragOverObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragOver" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDragLeaveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DragLeaveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"DragLeave" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> PreviewDropObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"PreviewDrop" );
		}
		public static IObservable<IEvent<System.Windows.DragEventArgs>> DropObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.DragEventArgs>(source,"Drop" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchDownObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchDown" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchMoveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchMove" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> PreviewTouchUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"PreviewTouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchUpObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchUp" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> GotTouchCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"GotTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> LostTouchCaptureObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"LostTouchCapture" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchEnterObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchEnter" );
		}
		public static IObservable<IEvent<System.Windows.Input.TouchEventArgs>> TouchLeaveObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.Input.TouchEventArgs>(source,"TouchLeave" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseDirectlyOverChanged += h;
                return () => { source.IsMouseDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusWithinChanged += h;
                return () => { source.IsKeyboardFocusWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCapturedChanged += h;
                return () => { source.IsMouseCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsMouseCaptureWithinChanged += h;
                return () => { source.IsMouseCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusDirectlyOverChanged += h;
                return () => { source.IsStylusDirectlyOverChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCapturedChanged += h;
                return () => { source.IsStylusCapturedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsStylusCaptureWithinChanged += h;
                return () => { source.IsStylusCaptureWithinChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsKeyboardFocusedChanged += h;
                return () => { source.IsKeyboardFocusedChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> GotFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"GotFocus" );
		}
		public static IObservable<IEvent<System.Windows.RoutedEventArgs>> LostFocusObservable(this System.Windows.UIElement3D source)
		{
			return Observable.FromEvent<System.Windows.RoutedEventArgs>(source,"LostFocus" );
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsEnabledChanged += h;
                return () => { source.IsEnabledChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsHitTestVisibleChanged += h;
                return () => { source.IsHitTestVisibleChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsVisibleChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsVisibleChanged += h;
                return () => { source.IsVisibleChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this System.Windows.UIElement3D source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.FocusableChanged += h;
                return () => { source.FocusableChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.Windows.DependencyPropertyChangedEventArgs>> IsFrontBufferAvailableChangedObservable(this System.Windows.Interop.D3DImage source)
		{
            return Observable.Create<IEvent<System.Windows.DependencyPropertyChangedEventArgs>>((o) =>
            {
                System.Windows.DependencyPropertyChangedEventHandler h = (s,e) => 
                    {
                        o.OnNext(Event.Create(s,e));
                    };
                source.IsFrontBufferAvailableChanged += h;
                return () => { source.IsFrontBufferAvailableChanged -= h; };
            });			
		}
		public static IObservable<IEvent<System.EventArgs>> CurrentStateInvalidatedObservable(this System.Windows.Media.Animation.Timeline source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"CurrentStateInvalidated" );
		}
		public static IObservable<IEvent<System.EventArgs>> CurrentTimeInvalidatedObservable(this System.Windows.Media.Animation.Timeline source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"CurrentTimeInvalidated" );
		}
		public static IObservable<IEvent<System.EventArgs>> CurrentGlobalSpeedInvalidatedObservable(this System.Windows.Media.Animation.Timeline source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"CurrentGlobalSpeedInvalidated" );
		}
		public static IObservable<IEvent<System.EventArgs>> CompletedObservable(this System.Windows.Media.Animation.Timeline source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"Completed" );
		}
		public static IObservable<IEvent<System.EventArgs>> RemoveRequestedObservable(this System.Windows.Media.Animation.Timeline source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"RemoveRequested" );
		}
		public static IObservable<IEvent<System.EventArgs>> DownloadCompletedObservable(this System.Windows.Media.Imaging.BitmapSource source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"DownloadCompleted" );
		}
		public static IObservable<IEvent<System.Windows.Media.Imaging.DownloadProgressEventArgs>> DownloadProgressObservable(this System.Windows.Media.Imaging.BitmapSource source)
		{
			return Observable.FromEvent<System.Windows.Media.Imaging.DownloadProgressEventArgs>(source,"DownloadProgress" );
		}
		public static IObservable<IEvent<System.Windows.Media.ExceptionEventArgs>> DownloadFailedObservable(this System.Windows.Media.Imaging.BitmapSource source)
		{
			return Observable.FromEvent<System.Windows.Media.ExceptionEventArgs>(source,"DownloadFailed" );
		}
		public static IObservable<IEvent<System.Windows.Media.ExceptionEventArgs>> DecodeFailedObservable(this System.Windows.Media.Imaging.BitmapSource source)
		{
			return Observable.FromEvent<System.Windows.Media.ExceptionEventArgs>(source,"DecodeFailed" );
		}
		public static IObservable<IEvent<System.Windows.Media.ExceptionEventArgs>> MediaFailedObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.Windows.Media.ExceptionEventArgs>(source,"MediaFailed" );
		}
		public static IObservable<IEvent<System.EventArgs>> MediaOpenedObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"MediaOpened" );
		}
		public static IObservable<IEvent<System.EventArgs>> MediaEndedObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"MediaEnded" );
		}
		public static IObservable<IEvent<System.EventArgs>> BufferingStartedObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"BufferingStarted" );
		}
		public static IObservable<IEvent<System.EventArgs>> BufferingEndedObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.EventArgs>(source,"BufferingEnded" );
		}
		public static IObservable<IEvent<System.Windows.Media.MediaScriptCommandEventArgs>> ScriptCommandObservable(this System.Windows.Media.MediaPlayer source)
		{
			return Observable.FromEvent<System.Windows.Media.MediaScriptCommandEventArgs>(source,"ScriptCommand" );
		}
	}
}
