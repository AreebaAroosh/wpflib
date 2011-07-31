using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPFLib.Contracts
{
    public interface IObservableNotifyPropertyChanged : INotifyPropertyChanged
    {
        IObservable<IEvent<PropertyChangedEventArgs>> PropertyChangedObservable { get; }
    }
}
