//---------------------------------------------------------------------
// <copyright file="JsonResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Json result
    /// </summary>
    public class JsonResult : Result
    {
        /// <summary>
        /// Initializes a new instance of the JsonResult class
        /// </summary>
        /// <param name="response">the response</param>
        public JsonResult(Response response)
            : base(response)
        {
            this.Parse();
        }

        private void Parse()
        {
            var deserializer = new JavaScriptSerializer { MaxJsonLength = Response.ResponseStr.Length };
            var dictionary =
                (Dictionary<string, object>)deserializer.Deserialize(Response.ResponseStr, typeof(object));
            var itemd = dictionary["d"];
            if (itemd is Dictionary<string, object>)
            {
                var d = itemd as Dictionary<string, object>;
                if (d.ContainsKey("results"))
                {
                    var results = (object[])d["results"];
                    Entries.AddRange(results.OfType<Dictionary<string, object>>().Select(this.ParseEntry));
                    if (d.ContainsKey("__count"))
                    {
                        InLineCount = long.Parse(d["__count"].ToString());
                    }
                }
                else
                {
                    Entries.Add(this.ParseEntry(d));
                }
            }
            else if (itemd is object[])
            {
                var d = itemd as object[];
                Entries.AddRange(d.OfType<Dictionary<string, object>>().Select(this.ParseEntry));
            }
        }

        private Entry ParseEntry(Dictionary<string, object> entry)
        {
            var ret = new Entry();
            foreach (var property in entry)
            {
                if (property.Key == "__metadata")
                {
                    var metadata = (Dictionary<string, object>)property.Value;
                    if (metadata.ContainsKey("id"))
                    {
                        ret.Id = metadata["id"].ToString();
                    }
                }
                else
                {
                    var dic = property.Value as Dictionary<string, object>;
                    if (dic != null && dic.ContainsKey("__deferred"))
                    {
                        ret.Links.Add(this.ParseLink(property));
                    }
                    else
                    {
                        ret.Add(this.ParseProperty(property));
                    }
                }
            }

            return ret;
        }

        private Property ParseProperty(KeyValuePair<string, object> property)
        {
            var dic = property.Value as Dictionary<string, object>;
            if (dic != null)
            {
                if (dic.ContainsKey("results"))
                {
                    var results = dic["results"] as object[];
                    if (results != null && results.Length > 0 &&
                        results[0].GetType() == typeof(Dictionary<string, object>))
                    {
                        return new Property
                            {
                                Name = property.Key,
                                Value = results.OfType<Dictionary<string, object>>().Select(this.ParseComplexType).ToList()
                            };
                    }

                    return new Property
                        {
                            Name = property.Key,
                            Value = results,
                        };
                }

                return new Property
                    {
                        Name = property.Key,
                        Value = this.ParseComplexType(dic)
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