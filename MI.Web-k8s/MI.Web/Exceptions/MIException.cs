using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class MIWebException : Exception
    {
        public MIWebException()
        { }

        public MIWebException(string message)
            : base(message)
        { }

        public MIWebException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
