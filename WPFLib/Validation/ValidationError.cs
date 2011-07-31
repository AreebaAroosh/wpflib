using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Errors
{
    public class ValidationErrorImpl : IValidationError
    {
        public ValidationErrorImpl(string uri, object errorContent)
        {
            Uri = uri;
            ErrorContent = errorContent;
        }

        public object ErrorContent
        {
            get;
            private set;
        }

        public string Uri
        {
            get;
            private set;
        }
    }
}
