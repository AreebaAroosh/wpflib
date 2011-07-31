using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Errors
{
    public interface IValidationErrorProvider
    {
        IEnumerable<IValidationError> Errors { get; }
    }
}
