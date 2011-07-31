﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Threading;
using System.Windows;

namespace WPFLib.Misc
{
    public class DispatcherPropertyChangedHelper : PropertyChangedHelper
    {
        //
        // Summary:
        //     Determines whether the calling thread is the thread associated with this
        //     System.Windows.Threading.Dispatcher.
        //
        // Returns:
        //     true if the calling thread is the thread associated with this System.Windows.Threading.Dispatcher;
        //     otherwise, false.
        protected bool CheckAccess()
        {
            if (Dispatcher != null)
            {
                return Dispatcher.CheckAccess();
            }
            else
            {
                return false;
            }
        }

        protected Dispatcher Dispatcher
        {
            get
            {
                if (Application.Current != null)
                {
                    return Application.Current.Dispatcher;
                }
                return null;
            }
        }

        static DispatcherPropertyChangedHelper()
        {
            _raiseQueue = new Subject<IEvent<PropertyChangedEventArgs>>();
            var buffer = _raiseQueue
                .BufferWithTimeAfterValue(TimeSpan.FromMilliseconds(10))
                .ObserveOn(Application.Current.Dispatcher);
            buffer.Subscribe(RaiseMultiple);
        }

        static Subject<IEvent<PropertyChangedEventArgs>> _raiseQueue;
        static IObserver<IEvent<PropertyChangedEventArgs>> RaiseQueue
        {
            get
            {
                return _raiseQueue;
            }
        }

        static void RaiseMultiple(IList<IEvent<PropertyChangedEventArgs>> args)
        {
            foreach (var group in args.GroupBy(a => a.Sender, a => a.EventArgs))
            {
                var sender = group.Key as DispatcherPropertyChangedHelper;
                var needRaise = sender.NeedRaisePropertyChanged();
                if (needRaise)
                {
                    foreach (var ev in group)
                    {
                        sender.RaisePropertyChanged(ev);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (NeedRaisePropertyChanged())
            {
                if (this.Dispatcher != null && !this.Dispatcher.CheckAccess())
                {
                    //Experimental
                    RaiseQueue.OnNext(Event.Create<PropertyChangedEventArgs>(this, args));
                    //this.Dispatcher.BeginInvoke(new RaisePropertyChangedDelegate(base.OnPropertyChanged), args);
                }
                else
                {
                    base.RaisePropertyChanged(args);
                }
            }
            if (NeedRaiseOnNext())
            {
                base.RaiseOnNext(args);
            }
        }

        delegate void RaisePropertyChangedDelegate(PropertyChangedEventArgs args);
    }
}
