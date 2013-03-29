using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GoxSharp.Models
{
    public class IdKey
    {
        public string Key { get; set; }

        public IdKey(dynamic jsonObj)
        {
            JToken token = null;
            if (jsonObj.TryGetValue("data", out token))
            {
                this.Key = token.Value<String>();

            }
        }
    }
}
