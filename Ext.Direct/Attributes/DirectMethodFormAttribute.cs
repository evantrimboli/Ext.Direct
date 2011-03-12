using System;

namespace Ext.Direct
{
    /// <summary>
    /// This attribute should be added to Ext.direct methods that will be used
    /// to save forms.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DirectMethodFormAttribute : DirectMethodAttribute
    {
        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public DirectMethodFormAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Creates a new instance of the object.
        /// <param name="alias">The alias to use for this method.</param>
        /// </summary>
        public DirectMethodFormAttribute(string alias)
            : base(alias)
        {
        }
    }
}
