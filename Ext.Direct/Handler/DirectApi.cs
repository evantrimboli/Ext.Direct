using System;
using System.Web;
using System.Configuration;

namespace Ext.Direct
{
    /// <summary>
    /// ASP.NET handler for generating the Ext Direct API.
    /// </summary>
    public class DirectApi : IHttpHandler
    {
        private const string NameConfig = "ExtDirect_ApiName";
        private const string RouterConfig = "ExtDirect_RouterUrl";
        private const string NamespaceConfig = "ExtDirect_Namespace";
        private const string AssemblyConfig = "ExtDirect_Assembly";

        public bool IsReusable
        {
            get { return true; }
        }


        public void ProcessRequest(HttpContext context)
        {
            string apiName = ConfigurationManager.AppSettings[NameConfig];
            string routerUrl = ConfigurationManager.AppSettings[RouterConfig];
            string ns = ConfigurationManager.AppSettings[NamespaceConfig] ?? "";
            string assemblyName = ConfigurationManager.AppSettings[AssemblyConfig];

            if (String.IsNullOrEmpty(apiName) || String.IsNullOrEmpty(routerUrl))
            {
                string s = "window.alert('Configuration Error:\\n\\nExtDirect_ApiName or ExtDirect_RouterUrl are not defined.\\nPlease add them to appSettings section in your configuration file.');";
                context.Response.Write(s);
                context.Response.End();
            }

            context.Response.ContentType = "text/javascript";

            DirectProviderCache cache = DirectProviderCache.GetInstance();
            DirectProvider provider;

            cache.Clear();
            if (!cache.ContainsKey(apiName))
            {
                provider = new DirectProvider()
                {
                    Name = apiName,
                    Url = routerUrl
                };
                if (!string.IsNullOrEmpty(ns))
                {
                    provider.Namespace = ns;
                }
                provider.Configure(System.Reflection.Assembly.Load(assemblyName));
                cache.Add(apiName, provider);
            }
            else
            {
                provider = cache[apiName];
            }

            context.Response.Write(provider.ToString());

        }
    }
}
