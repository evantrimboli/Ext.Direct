using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ext.Direct
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectProvider
    {

        public const string RemotingProvider = "remoting";

        private Dictionary<string, DirectAction> actions;
        private string api;

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        public DirectProvider()
        {
            this.actions = new Dictionary<string, DirectAction>();
        }

        /// <summary>
        /// Configure the provider by adding the available API methods.
        /// </summary>
        /// <param name="assembly">The assembly to automatically generate parameters from.</param>
        public void Configure(Assembly assembly)
        {
            this.Configure(assembly, null);
        }

        /// <summary>
        /// Configure the provider by adding the available API methods.
        /// </summary>
        /// <param name="assembly">The assembly to automatically generate parameters from.</param>
        /// <param name="exclusions">A list of classes to exclude.</param>
        public void Configure(Assembly assembly, IEnumerable<object> exclusions)
        {
            if (!this.Configured)
            {
                List<Type> types = new List<Type>();
                foreach (Type type in assembly.GetTypes())
                {
                    if(DirectProvider.CheckType(exclusions, type))
                    {
                        types.Add(type);
                    }
                }
                this.Configure(types);
            }
        }

        /// <summary>
        /// Configure the provider by adding the available API methods.
        /// </summary>
        /// <param name="items">A series of object instances that contain Ext.Direct methods.</param>
        public void Configure(IEnumerable<object> items)
        {
            if (!this.Configured)
            {
                List<Type> types = new List<Type>();
                foreach (object item in items)
                {
                    if (item != null)
                    {
                        types.Add(item.GetType());
                    }
                }
                this.Configure(types);
            }
        }

        //Generic configuration method for a list of types.
        private void Configure(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (DirectAction.IsAction(type))
                {
                    this.actions.Add(Utility.GetName(type), new DirectAction(type));
                }
            }
            this.Configured = true;
            this.ConstructApi();
        }

        private void ConstructApi()
        {
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (JsonTextWriter jw = new JsonTextWriter(sw))
                {
                    jw.WriteStartObject();
                    if (string.IsNullOrEmpty(this.Type))
                    {
                        this.Type = DirectProvider.RemotingProvider;
                    }
                    Utility.WriteProperty<string>(jw, "type", this.Type);
                    Utility.WriteProperty<string>(jw, "url", this.Url);
                    if (!string.IsNullOrEmpty(this.Namespace))
                    {
                        Utility.WriteProperty<string>(jw, "namespace", this.Namespace);
                    }
                    if (!string.IsNullOrEmpty(this.Id))
                    {
                        Utility.WriteProperty<string>(jw, "id", this.Id);
                    }
                    if (this.Timeout.HasValue)
                    {
                        Utility.WriteProperty<int>(jw, "timeout", this.Timeout.Value);
                    }
                    if (this.MaxRetries.HasValue)
                    {
                        Utility.WriteProperty<int>(jw, "maxRetries", this.MaxRetries.Value);
                    }
                    jw.WritePropertyName("actions");
                    jw.WriteStartObject();
                    foreach (DirectAction action in this.actions.Values)
                    {
                        action.Write(jw);
                    }
                    jw.WriteEndObject();
                    jw.WriteEndObject();
                    this.api = string.Empty;
                    if (this.AutoNamespace)
                    {
                        this.api = String.Format("Ext.ns('{0}');", this.Name);
                    }
                    this.api += String.Format("{0} = {1};", this.Name, sw.ToString());
                }
            }
        }

        public bool AutoNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// Clears any previous configuration for this provider.
        /// </summary>
        public void Clear()
        {
            this.Configured = false;
            this.actions.Clear();
        }

        /// <summary>
        /// Indicates whether the provider has been configured or not.
        /// </summary>
        public bool Configured
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets/sets the id of the provider.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the name of the provider.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the url to router requests to for this provider.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the type of the provider.
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the namespace to use for client side actions.
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timeout to use for this provider (in milliseconds).
        /// </summary>
        public int? Timeout
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the maximum number of retries for a failed request.
        /// </summary>
        public int? MaxRetries
        {
            get;
            set;
        }

        public override string ToString()
        {
            return this.api;
        }

        internal object Execute(DirectRequest request)
        {
            DirectAction action = this.actions[request.Action];
            if (action == null)
            {
                throw new DirectException("Unable to find action, " + request.Action);
            }
            DirectMethod method = action.GetMethod(request.Method);
            if (method == null)
            {
                throw new DirectException("Unable to find action, " + request.Method);
            }
            Type type = action.Type;
            if (request.Data == null)
            {
                if (method.Parameters > 0)
                {
                    throw new DirectException("Parameters length does not match");
                }
            }
            else
            {
                if (request.Data.Length > 1 && method.IsForm)
                {
                    throw new DirectException("Form methods can only have a single parameter.");
                }
                else if (request.Data.Length != method.Parameters)
                {
                    throw new DirectException("Parameters length does not match");
                }
            } 
            try
            {
                this.SanitizeDates(method.Method, request);
                object[] param = request.Data;
                if (method.ParseAsJson)
                {
                    param = new object[] { method.GetParseData(request.RequestData) };
                }
                return method.Method.Invoke(type.Assembly.CreateInstance(type.FullName), param);
            }
            catch (Exception ex)
            {
                throw new DirectException("Error occurred while calling Direct method: " + ex.Message);
            }
        }

        private void SanitizeDates(MethodInfo method, DirectRequest request)
        {
            int idx = 0;
            DateTime d;
            foreach (ParameterInfo p in method.GetParameters())
            {
                if (p.ParameterType == typeof(DateTime))
                {
                    object o = request.Data[idx];
                    if (o != null)
                    {
                        if (DateTime.TryParse(o.ToString(), out d))
                        {
                            request.Data[idx] = d;
                        }
                        else
                        {
                            throw new DirectException("Unable to parse date parameter.");
                        }
                    }
                    else
                    {
                        throw new DirectException("Unable to parse date parameter.");
                    }
                }
                ++idx;
            }
        }

        private static bool CheckType(IEnumerable<object> exclusions, Type t)
        {
            if (exclusions != null)
            {
                foreach (object o in exclusions)
                {
                    if (o != null)
                    {
                        if (o.GetType() == t)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
