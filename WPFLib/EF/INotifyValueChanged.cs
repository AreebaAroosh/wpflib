using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPFLib.Context
{
    /// <summary>
    /// Нотификация об изменении значения сохраняемого в БД
    /// </summary>
    public interface INotifyValueChanged
    {
        event PropertyChangedEventHandler ValueChanged;
    }
}
