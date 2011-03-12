using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ext.Direct
{
    [JsonObject]
    internal class DirectResponse
    {

        public const string ResponseExceptionType = "exception";

        public DirectResponse(DirectRequest request)
        {
            this.Type = request.Type;
            this.TransactionId = request.TransactionId;
            this.Action = request.Action;
            this.Method = request.Method;
            this.IsUpload = request.IsUpload;
        }

        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "tid")]
        public int TransactionId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "action")]
        public string Action
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "method")]
        public string Method
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "result")]
        public object Result
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "message")]
        public string ExceptionMessage
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsUpload
        {
            get;
            private set;
        }

    }
}
