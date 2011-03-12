using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ext.Direct
{

    /// <summary>
    /// Thrown when an error occurs during a Ext.Direct call.
    /// </summary>
    public class DirectException : ApplicationException
    {

        public DirectException()
            : base()
        {
        }

        public DirectException(string msg)
            : base(msg)
        {
        }

    }
}
