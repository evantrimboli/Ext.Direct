using System;

namespace Ext.Direct
{
    /// <summary>
    /// Abstract attribute to encapsulate the aliasing behaviour that is exhibited in subclasses.
    /// </summary>
    public abstract class DirectAliasAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public DirectAliasAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        /// <param name="alias">The alias to use.</param>
        public DirectAliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the alias to use for this attribute.
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }
    }
}