using System;

namespace Ext.Direct
{
    /// <summary>
    /// This attribute should be added to methods that will be Ext.direct methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DirectMethodAttribute : DirectAliasAttribute
    {

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public DirectMethodAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Creates a new instance of the object.
        /// <param name="alias">The alias to use for this method.</param>
        /// </summary>
        public DirectMethodAttribute(string alias)
            : base(alias)
        {
        }

    }
}
