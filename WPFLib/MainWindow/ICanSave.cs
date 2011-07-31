using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPFLib.Contracts
{
    public interface ICanSave : INotifyPropertyChanged
    {
        bool HasChanges { get; }
        bool SaveChanges();
    }
}
