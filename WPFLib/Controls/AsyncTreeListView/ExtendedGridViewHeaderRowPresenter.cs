using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WPFLib
{
    public class ExtendedGridViewHeaderRowPresenter : GridViewHeaderRowPresenter
    {
        public event EventHandler Arrange;
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
        {
            if (Arrange != null)
            {
                Arrange(this, EventArgs.Empty);
            }
            return base.ArrangeOverride(arrangeSize);
        }
    }
}
