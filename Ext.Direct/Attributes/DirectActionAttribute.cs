using System;

namespace Ext.Direct
{
    /// <summary>
    /// This attribute should be added to Ext.direct classes that to indicate they will participate in routing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DirectActionAttribute : DirectAliasAttribute
    {

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public DirectActionAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Creates a new instance of the object.
        /// <param name="alias">The alias to use for this class.</param>
        /// </summary>
        public DirectActionAttribute(string alias)
            : base(alias)
        {
        }
    }
}
