using System;
using System.Windows;
using System.Windows.Input;

namespace WPFLib.Misc
{
    public class WPFLengthyOperation : IDisposable
    {
        protected FrameworkElement owner;
        private Cursor initialCursor;

        public WPFLengthyOperation()
            : this(null)
        {
        }

        public WPFLengthyOperation(FrameworkElement control)
        {
            owner = control ?? Application.Current.MainWindow;

            initialCursor = owner.Cursor;
            if (initialCursor != Cursors.Wait)
                owner.Cursor = Cursors.Wait;
        }

        public virtual void Dispose()
        {
            if (initialCursor != owner.Cursor)
                owner.Cursor = initialCursor;
        }

    }
}