using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFLib
{
    public static class DataGridHelp
    {
        static readonly DependencyPropertyKey IsManualCommitPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsManualCommit", typeof(bool), typeof(DataGridHelp), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsManualCommitProperty = IsManualCommitPropertyKey.DependencyProperty;

        private static void SetIsManualCommit(DependencyObject obj, bool value)
        {
            obj.SetValue(IsManualCommitPropertyKey, value);
        }

        public static bool GetIsManualCommit(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsManualCommitProperty);
        }

        public static readonly DependencyProperty CommitCellByCellProperty = DependencyProperty.RegisterAttached("CommitCellByCell", typeof(bool), typeof(DataGridHelp), new FrameworkPropertyMetadata() { PropertyChangedCallback = new PropertyChangedCallback(OnCommitCellByCellChanged), CoerceValueCallback = new CoerceValueCallback(OnCoerceCommitCellByCell) });

        public static void SetCommitCellByCell(DependencyObject obj, bool value)
        {
            obj.SetValue(CommitCellByCellProperty, value);
        }

        public static bool GetCommitCellByCell(DependencyObject obj)
        {
            return (bool)obj.GetValue(CommitCellByCellProperty);
        }

        #region OnCoerceCommitCellByCell
        private static object OnCoerceCommitCellByCell(DependencyObject o, object value)
        {
            return value;
        }
        #endregion

        #region OnCommitCellByCellChanged
        private static void OnCommitCellByCellChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var grid = o as DataGrid;
            if (grid != null)
            {
                if ((bool)e.NewValue)
                {
                    grid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(grid_CellEditEnding);
                }
                else
                {
                    grid.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(grid_CellEditEnding);
                }
            }
        }

        static void grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var grid = sender as DataGrid;
            if (!GetIsManualCommit(grid))
            {
                SetIsManualCommit(grid, true);
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                SetIsManualCommit(grid, false);
            }
        }
        #endregion
    }
}
