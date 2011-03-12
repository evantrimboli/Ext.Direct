using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ext.Direct
{
    /// <summary>
    /// Class for handling Ext Direct requests
    /// </summary>
    public abstract class DirectHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            DirectProvider provider = this.GetProvider(context, this.ProviderName);
            string data = string.Empty;
            string type = "text/javascript";

            if (context.Request.TotalBytes == 0 && string.IsNullOrEmpty(context.Request["extAction"]))
            {
                data = provider.ToString();
            }
            else
            {
                DirectExecutionResponse resp = DirectProcessor.Execute(provider, context.Request, this.GetConverters());
                data = resp.Data;
                if (resp.IsUpload)
                {
                    type = "text/html";
                }
            }
            context.Response.ContentType = type;
            context.Response.Write(data);
        }

        private DirectProvider GetProvider(HttpContext context, string name)
        {
            DirectProviderCache cache = DirectProviderCache.GetInstance();
            if (!cache.ContainsKey(name))
            {
                DirectProvider provider = new DirectProvider()
                {
                    Name = name,
                    Url = context.Request.Path,
                    Namespace = this.Namespace,
                    Timeout = this.Timeout,
                    MaxRetries = this.MaxRetries,
                    AutoNamespace = this.AutoNamespace,
                    Id = this.Id
                };
                this.ConfigureProvider(provider);
                cache.Add(name, provider);
            }
            return cache[name];
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }

        }

        /// <summary>
        /// True to ensure that Ext.namespace is called on the provider name to ensure it exists. Defaults to True.
        /// </summary>
        public virtual bool AutoNamespace
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the id of the provider.
        /// </summary>
        public virtual string Id
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves the name of the provider
        /// </summary>
        public abstract string ProviderName
        {
            get;
        }

        /// <summary>
        /// Retrieves the namespace for the provider
        /// </summary>
        public virtual string Namespace
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the timeout to use for this provider (in milliseconds).
        /// </summary>
        public virtual int? Timeout
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the maximum number of retries for failed requests.
        /// </summary>
        public virtual int? MaxRetries
        {
            get
            {
                return null;
            }
        }

        protected IEnumerable<JsonConverter> GetConverters()
        {
            List<JsonConverter> list = new List<JsonConverter> { new ResultConverter(), new JavaScriptDateTimeConverter()};
            this.ConfigureConverters(list);
            return list;
        }

        /// <summary>
        /// Allows the handler to modify the converters used for the Direct operation.
        /// </summary>
        /// <param name="converters">The list of converters to modify</param>
        /// <remarks>Defaults to using the ResultConverter and JavaScriptDateTimeConverter classes.</remarks>
        protected virtual void ConfigureConverters(List<JsonConverter> converters)
        {
            //do nothing
        }

        /// <summary>
        /// Configure the provider with the appropriate set of methods.
        /// </summary>
        /// <param name="provider">The provider to be configured.</param>
        /// <remarks>This method is virtual to allow for custom configurations.</remarks>
        protected virtual void ConfigureProvider(DirectProvider provider)
        {
            provider.Configure(new object[] { this });
        }

        /// <summary>
        /// Configure the provider using reflection by getting the appropriate methods from an assembly.
        /// </summary>
        /// <param name="provider">The provider to configure.</param>
        /// <param name="assembly">The assembly to interrogate.</param>
        protected void Configure(DirectProvider provider, Assembly assembly)
        {
            this.Configure(provider, assembly, null);
        }

        /// <summary>
        /// Configure the provider using reflection by getting the appropriate methods from an assembly.
        /// </summary>
        /// <param name="provider">The provider to configure.</param>
        /// <param name="assembly">The assembly to interrogate.</param>
        /// <param name="exclusions">A list of classes to exclude.</param>
        protected void Configure(DirectProvider provider, Assembly assembly, IEnumerable<object> exclusions)
        {
            provider.Configure(assembly, exclusions);
        }

        /// <summary>
        /// Configure the provider given a list of action classes.
        /// </summary>
        /// <param name="provider">The provider to configure.</param>
        /// <param name="items">The list of classes to interrogate.</param>
        protected void Configure(DirectProvider provider, IEnumerable<object> items)
        {
            provider.Configure(items);
        }

        /// <summary>
        /// Method to reset the state of this provider, so that the API will be re-generated
        /// </summary>
        protected void Reset()
        {
            DirectProviderCache cache = DirectProviderCache.GetInstance();
            if (cache.ContainsKey(this.ProviderName))
            {
                cache[this.ProviderName].Clear();
            }
        }

    }
}
