using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Ext.Direct
{
    internal class ResultConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(JContainer));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue = null, JsonSerializer serializer = null)
        {
            //no custom reading here
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer = null)
        {
            JContainer o = value as JContainer;
            if (o != null)
            {
                writer.WriteRawValue(o.ToString(Formatting.None, new JavaScriptDateTimeConverter()));
            }
        }

    }
}
