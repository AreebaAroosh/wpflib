using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Errors
{
    public interface IValidationError
    {
        object ErrorContent { get; }
        string Uri { get; }
    }
}
