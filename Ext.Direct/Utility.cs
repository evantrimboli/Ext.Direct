using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Ext.Direct
{
    internal static class Utility
    {
        /// <summary>
        /// Checks whether a type has a particular attribute.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>True if the type contains the attribute.</returns>
        internal static bool HasAttribute(Type t, Type attribute)
        {
            return t.GetCustomAttributes(attribute, true).Length > 0;
        }

        /// <summary>
        /// Checks whether a method has a particular attribute.
        /// </summary>
        /// <param name="t">The method.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>True if the method contains the attribute.</returns>
        internal static bool HasAttribute(MethodInfo mi, Type attribute)
        {
            return mi.GetCustomAttributes(attribute, true).Length > 0;
        }

        /// <summary>
        /// Gets the name of a type, factoring in a custom alias.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>The name of the type.</returns>
        internal static string GetName(Type t)
        {
            return Utility.GetName(t.GetCustomAttributes(typeof(DirectAliasAttribute), true), t.Name);
        }

        /// <summary>
        /// Gets the name of a method, factoring in a custom alias.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>The name of the method.</returns>
        internal static string GetName(MethodInfo mi)
        {
            return Utility.GetName(mi.GetCustomAttributes(typeof(DirectAliasAttribute), true), mi.Name);
            
        }

        private static string GetName(object[] attrs, string defaultValue)
        {
            if (attrs.Length > 0)
            {
                DirectAliasAttribute attr = attrs[0] as DirectAliasAttribute;
                if (attr != null)
                {
                    return string.IsNullOrEmpty(attr.Alias) ? defaultValue : attr.Alias;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Shortcut method to write a property value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="jw">The JSON writer.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        internal static void WriteProperty<T>(JsonTextWriter jw, string name, T value)
        {
            jw.WritePropertyName(name);
            jw.WriteValue(value);
        }
    }
}
