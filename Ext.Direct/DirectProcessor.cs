using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Ext.Direct
{
    /// <summary>
    /// Class for processing Ext Direct requests.
    /// </summary>
    internal class DirectProcessor
    {

        /// <summary>
        /// Processes an incoming request.
        /// </summary>
        /// <param name="provider">The provider that triggered the request.</param>
        /// <param name="httpRequest">The http information.</param>
        /// <returns>The result from the client method.</returns>
        internal static DirectExecutionResponse Execute(DirectProvider provider, HttpRequest httpRequest, IEnumerable<JsonConverter> converters)
        {
            List<DirectResponse> responses = new List<DirectResponse>();
            if (!String.IsNullOrEmpty(httpRequest[DirectRequest.RequestFormAction]))
            {
                DirectRequest request = new DirectRequest()
                {
                    Action = httpRequest[DirectRequest.RequestFormAction] ?? string.Empty,
                    Method = httpRequest[DirectRequest.RequestFormMethod] ?? string.Empty,
                    Type = httpRequest[DirectRequest.RequestFormType] ?? string.Empty,
                    IsUpload = Convert.ToBoolean(httpRequest[DirectRequest.RequestFormUpload]),
                    TransactionId = Convert.ToInt32(httpRequest[DirectRequest.RequestFormTransactionId]),
                    Data = new object[] { httpRequest }
                };
                responses.Add(DirectProcessor.ProcessRequest(provider, request));
            }
            else
            {
                UTF8Encoding encoding = new UTF8Encoding();
                string json = encoding.GetString(httpRequest.BinaryRead(httpRequest.TotalBytes));
                /**************************************************************************************
                 skygreen:解决bug:Self referencing loop
                 参考:http://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
                 **************************************************************************************/
                if (substr_count(json,"data")>1)
                {
                    List<DirectRequest> requests = JsonConvert.DeserializeObject<List<DirectRequest>>(json);
                    if (requests.Count > 0)
                    {
                        JArray raw = JArray.Parse(json);
                        int i = 0;
                        foreach (DirectRequest request in requests)
                        {
                            request.RequestData = (JObject)raw[i];
                            responses.Add(DirectProcessor.ProcessRequest(provider, request));
                            ++i;
                        }
                    }
                    else
                    {
                        DirectRequest request = JsonConvert.DeserializeObject<DirectRequest>(json);
                        request.RequestData = JObject.Parse(json);
                        responses.Add(DirectProcessor.ProcessRequest(provider, request));
                    }
                }
                else
                {
                    DirectRequest request = JsonConvert.DeserializeObject<DirectRequest>(json);
                    request.RequestData = JObject.Parse(json);
                    responses.Add(DirectProcessor.ProcessRequest(provider, request));
                }
            }
            DirectExecutionResponse response = new DirectExecutionResponse();

            JsonSerializerSettings outputSettings = new JsonSerializerSettings() { 
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling =ReferenceLoopHandling.Ignore
            };
            foreach (JsonConverter converter in converters)
            {
                outputSettings.Converters.Add(converter);
            }
            if (responses.Count > 1 || !responses[0].IsUpload)
            {
                response.Data = JsonConvert.SerializeObject(responses, Formatting.None, outputSettings);
            }
            else
            {
                response.IsUpload = true;
                string outputJson = JsonConvert.SerializeObject(responses[0], Formatting.None, outputSettings);
                response.Data = String.Format("<html><body><textarea>{0}</textarea></body></html>", outputJson.Replace("&quot;", "\\&quot;"));
            }
            return response;
        }

        /// <summary>
        ///  计算字符串出现的次数
        /// </summary>
        /// <param name="haystack">必需。规定要检查的字符串。</param>
        /// <param name="needle">要搜索的字符串</param>
        /// <param name="type">查找方式，默认0:正则表达式方式,这种方式如果子字符串有特殊符号不推荐用；其他:标准的查找子字符串的方式</param>
        /// <returns></returns>
        public static int substr_count(string haystack, string needle,int type=0)
        {
            
            int count = 0;
            if (type == 0)
            {
                if (haystack != String.Empty && needle != String.Empty)
                {
                    MatchCollection mc = Regex.Matches(haystack, needle);
                    count = mc.Count;
                }
            }
            else
            {
                for (int i = 0; i < haystack.Length; i++)
                {
                    for (int j = 1; j <= (haystack.Length - i); j++)
                    {
                        if (haystack.Substring(i, j) == needle)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private static DirectResponse ProcessRequest(DirectProvider provider, DirectRequest request)
        {
            DirectResponse r = new DirectResponse(request);
            try
            {
                r.Result = provider.Execute(request);
            }
            catch (DirectException ex)
            {
                r.ExceptionMessage = ex.Message;
                r.Type = DirectResponse.ResponseExceptionType;
            }
            return r;
        }

    }
}
