using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ext.Direct
{

    /// <summary>
    /// Simple cache for Ext.Direct providers
    /// </summary>
    public class DirectProviderCache : Dictionary<string, DirectProvider>
    {

        static readonly DirectProviderCache instance = new DirectProviderCache();
        private Dictionary<string, DirectProviderCache> providers;

        static DirectProviderCache()
        {
        }

        DirectProviderCache()
        {
            this.providers = new Dictionary<string, DirectProviderCache>();
        }

        /// <summary>
        /// Gets the singleton instance of this object.
        /// </summary>
        /// <returns>The DirectProviderCache instance.</returns>
        public static DirectProviderCache GetInstance()
        {
            return instance;
        }

    }
}
