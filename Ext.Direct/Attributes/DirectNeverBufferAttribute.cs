using System;

namespace Ext.Direct
{
    /// <summary>
    /// An attribute that indicates requests of this type should never be buffered
    /// regardless of the settings on the provider, they will always be fired straight away.
    /// This attribute is invalid when used with a form method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DirectNeverBufferAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public DirectNeverBufferAttribute()
            : base()
        {
        }
    }
}
