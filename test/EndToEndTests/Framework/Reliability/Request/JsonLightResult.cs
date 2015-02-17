//---------------------------------------------------------------------
// <copyright file="JsonLightResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;

    /// <summary>
    /// The json light result
    /// </summary>
    public class JsonLightResult : Result
    {
        /// <summary>
        /// Initializes a new instance of the JsonLightResult class
        /// </summary>
        /// <param name="response">The response</param>
        public JsonLightResult(Response response)
            : base(response)
        {
            this.Parse();
        }

        private void Parse()
        {
            var deserializer = new JavaScriptSerializer { MaxJsonLength = Response.ResponseStr.Length };
            var d = (Dictionary<string, object>)deserializer.Deserialize(Response.ResponseStr, typeof(object));
            if (d.ContainsKey("value"))
            {
                if (d.ContainsKey("odata.count"))
                {
                    InLineCount = long.Parse(d["odata.count"].ToString());
                }

                if (d.ContainsKey("value"))
                {
                    var values = (object[])d["value"];
                    Entries.AddRange(values.OfType<Dictionary<string, object>>().Select(this.ParseEntry));
                }
            }
            else
            {
                Entries.Add(this.ParseEntry(d));
            }
        }

        private Entry ParseEntry(Dictionary<string, object> entry)
        {
            var ret = new Entry();
            foreach (var property in entry)
            {
                if (property.Key == "odata.metadata")
                {
                    continue;
                }

                if (property.Value != null && property.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret.Links.Add(this.ParseLink(property));
                }
                else
                {
                    ret.Add(this.ParseProperty(property));
                }
            }

            return ret;
        }

        private Property ParseProperty(KeyValuePair<string, object> property)
        {
            var value = property.Value as Dictionary<string, object>;
            if (value != null)
            {
                return new Property
                    {
                        Name = property.Key,
                        Value = this.ParseComplexType(value)
                    };
            }

            var valueArray = property.Value as object[];
            if (valueArray != null && valueArray.Length > 0 &&
                valueArray[0].GetType() == typeof(Dictionary<string, object>))
            {
                return new Property
                    {
                        Name = property.Key,
                        Value = valueArray.OfType<Dictionary<string, object>>().Select(this.ParseComplexType).ToList()
                    };
            }

            return new Property
                {
                    Name = property.Key,
                    Value = property.Value,
                };
        }

        private Link ParseLink(KeyValuePair<string, object> property)
        {
            var ret = new Link
                {
                    Name = property.Key
                };
            var dic = (Dictionary<string, object>)property.Value;
            if (dic.ContainsKey("__deferred"))
            {
                ret.Uri = ((Dictionary<string, object>)dic["__deferred"])["uri"].ToString();
            }

            return ret;
        }

        private Entry ParseComplexType(Dictionary<string, object> complexValue)
        {
            return new Entry { complexValue.Where(kvp => kvp.Key != "__metadata").Select(this.ParseProperty) };
        }
    }
}
