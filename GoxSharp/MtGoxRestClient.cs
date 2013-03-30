
using System;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GoxSharp
{
    public class MtGoxRestClient
    {
        private const string MTGOX_API_URL_v1 = "http://data.mtgox.com";
        private const string MTGOX_API_URL_v2 = "https://data.mtgox.com";
        private const string MTGOX_API_RESOURCE_v1 = "/api/1/";
        private const string MTGOX_API_RESOURCE_v2 = "/api/2/";
        private readonly RestClient rc1;
        private readonly RestClient rc2;
        private readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        private static readonly Encoding encoding = Encoding.UTF8;
        private string _endpoint { get; set; }
        private Method _method { get; set; }
        private NameValueCollection _parameters { get; set; }
        private const string GOXSHARP_MODEL_NAMESPACE = "GoxSharp.Models.";

        public MtGoxRestClient()
        {
            rc1 = new RestClient(MTGOX_API_URL_v1 + MTGOX_API_RESOURCE_v1);
            rc2 = new RestClient(MTGOX_API_URL_v2 + MTGOX_API_RESOURCE_v2);
        }


        public dynamic makeRequestV2()
        {
            return makeRequestV2(_endpoint, _method, _parameters);
        }

        public dynamic makeRequestV2(string endpoint, Method method, NameValueCollection parameters)
        {
            RestRequest req = signRequestV2(new RestRequest(endpoint, method), parameters);
            return rc2.Execute<dynamic>(req);
        }

        public dynamic makeRequestV2(string endpoint, Method method)
        {
            return makeRequestV2(endpoint, method, null);
        }

        private RestRequest signRequestV2(RestRequest req, NameValueCollection parameters)
        {
            String apiKey = ConfigurationManager.AppSettings["MtGoxAPIKey"];
            String apiSecret = ConfigurationManager.AppSettings["MtGoxAPISecret"];

            if (apiKey.Equals("NEEDTOCONFIG") || apiSecret.Equals("NEEDTOCONFIG"))
            {
                throw new MissingFieldException("You must configure your API Keys");
            }
            Int64 nonce = (Int64)DateTime.Now.Ticks;

            string endpoint = req.Resource.ToString();
            string post = "nonce=" + nonce;
            if (parameters != null) { post += ToQueryString(parameters); }
            string prefix = endpoint;

            string sign = getHash(Convert.FromBase64String(apiSecret), prefix + Convert.ToChar(0) + post);
            req.AddHeader("Rest-Key", apiKey);
            req.AddHeader("Rest-Sign", sign);
            req.AddParameter("nonce", nonce);
            if (parameters != null)
            {
                foreach (string s in parameters.Keys)
                {
                    req.AddParameter(s, parameters[s]);
                }
            }
            return req;
        }

        private string getHash(byte[] keyByte, String message)
        {
            var hmacsha512 = new HMACSHA512(keyByte);
            var messageBytes = encoding.GetBytes(message);
            return Convert.ToBase64String(hmacsha512.ComputeHash(messageBytes));
        }

        public Object GetResponse(string type, string endpoint, Method method, NameValueCollection parameters)
        {
            _endpoint = endpoint;
            _method = method;
            _parameters = parameters;

            dynamic jsonObj = getObject(makeRequestV2());
            Type t = Type.GetType(GOXSHARP_MODEL_NAMESPACE + type);


            if (checkStatus(jsonObj))
            {
                return Activator.CreateInstance(t, new object[] { jsonObj });
            }

            return null;
        }

        public JObject getObject(dynamic response)
        {
            return JsonConvert.DeserializeObject(response.Content);
        }

        public bool checkStatus(dynamic jsonObj)
        {
            JToken token = null;
            if (jsonObj.TryGetValue("result", out token))
            {
                if (token.Value<String>().Equals("error"))
                {
                    JToken message = null;
                    JToken errortoken = null;
                    jsonObj.TryGetValue("error", out message);
                    jsonObj.TryGetValue("token", out errortoken);

                    string token_string = errortoken.Value<String>();
                    string error_msg = message.Value<String>();

                    if (token_string.Equals("access_denied"))
                    {
                        throw new UnauthorizedAccessException(token_string + ": " + error_msg);
                    }
                    if (token_string.Equals("unknown_error"))
                    {
                        throw new Exception(token_string + ": " + error_msg);
                    }
                    throw new Exception("Unhandled exception occured while parsing the response from the server.");
                }
                return true;
            }
            throw new MissingFieldException("MtGox did not return a valid response");
        }
        private string ToQueryString(NameValueCollection nvc)
        {
            if (nvc != null)
                return "&" + string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));

            return "";
        }
    }
}
