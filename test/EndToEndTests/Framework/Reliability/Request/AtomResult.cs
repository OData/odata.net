//---------------------------------------------------------------------
// <copyright file="AtomResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Atom10 Result
    /// </summary>
    public class AtomResult : Result
    {
        /// <summary>
        /// Atom name space
        /// </summary>
        internal static readonly XNamespace AtomNS = "http://www.w3.org/2005/Atom";
        internal static readonly XNamespace MetadataNS =
            "http://docs.oasis-open.org/odata/ns/metadata";

        internal static readonly XNamespace DataServicesNS = "http://docs.oasis-open.org/odata/ns/data";

        private static readonly Dictionary<string, Type> EdmToClr = new Dictionary<string, Type>
            {
                { "String", typeof(string) },
                { "Int16", typeof(short) },
                { "Int32", typeof(int) },
                { "Double", typeof(double) },
                { "Boolean", typeof(bool) },
                { "Single", typeof(float) },
                { "Byte", typeof(byte) },
                { "Binary", typeof(byte[]) },
                { "Int64", typeof(long) },
                { "Decimal", typeof(decimal) },
                { "Guid", typeof(Guid) },
                { "DateTimeOffset", typeof(DateTimeOffset) },
                { "Duration", typeof(TimeSpan) },
                { "SByte", typeof(sbyte) },
            };

        /// <summary>
        /// Initializes a new instance of the AtomResult class.
        /// </summary>
        /// <param name="response">The http response</param>
        public AtomResult(Response response)
            : base(response)
        {
            this.Parse();
        }

        /// <summary>
        /// Initializes a new instance of the AtomResult class.
        /// </summary>
        /// <param name="rootElement">The root element</param>
        public AtomResult(XElement rootElement)
            : base(null)
        {
            Parse(rootElement);
        }

        private static Entry ParseEntry(XElement entry)
        {
            var ret = new Entry();
            IEnumerable<XElement> columns;

            ret.Links.AddRange(entry.Elements(AtomNS + "link").Select(ParseLink));

            if (entry.Element(AtomNS + "content").HasElements)
            {
                columns = entry.Element(AtomNS + "content")
                               .Element(MetadataNS + "properties")
                               .Elements();
            }
            else
            {
                // sometimes, the OData service may not put the properties element into the content element
                columns = entry.Element(MetadataNS + "properties")
                               .Elements();
            }

            ret.Id = entry.Elements(AtomNS + "id").FirstOrDefault().Value;

            foreach (XElement property in columns)
            {
                ret.Add(ParseProperty(property));
            }

            return ret;
        }

        private static Property ParseProperty(XElement property)
        {
            object value;
            XAttribute edmTypeAttr = property.Attribute(MetadataNS + "type");

            if (null == edmTypeAttr)
            {
                value = typeof(string).ConvertToClr(property.Value, false);
            }
            else if (EdmToClr.ContainsKey(edmTypeAttr.Value))
            {
                value = EdmToClr[edmTypeAttr.Value].ConvertToClr(property.Value, false);
            }
            else if (edmTypeAttr.Value.StartsWith("Collection("))
            {
                string type = edmTypeAttr.Value.Substring(11, edmTypeAttr.Value.Length - 12);
                if (EdmToClr.ContainsKey(type))
                {
                    value =
                        property.Elements(DataServicesNS + "element")
                                .Select(e => EdmToClr[type].ConvertToClr(e.Value, false)).ToList();
                }
                else
                {
                    value =
                        property.Elements(DataServicesNS + "element")
                                .Select(ParseComplexType).ToList();
                }
            }
            else
            {
                value = ParseComplexType(property);
            }

            return new Property
                {
                    Value = value,
                    Name = property.Name.LocalName
                };
        }

        private static Entry ParseComplexType(XElement property)
        {
            return new Entry { property.Elements().Select(ParseProperty) };
        }

        private static Link ParseLink(XElement rootElement)
        {
            var ret = new Link { Value = rootElement.Value };
            var attri = rootElement.Attribute("title");
            if (null != attri)
            {
                ret.Name = attri.Value;
            }

            attri = rootElement.Attribute("rel");
            if (null != attri)
            {
                ret.Rel = attri.Value;
            }

            attri = rootElement.Attribute("href");
            if (null != attri)
            {
                ret.Uri = attri.Value;
            }

            attri = rootElement.Attribute("type");
            if (null != attri)
            {
                ret.Type = attri.Value;
            }

            var inline = rootElement.Elements(MetadataNS + "inline").SingleOrDefault();
            if (null != inline && inline.HasElements)
            {
                if (ret.Type.Contains("type=feed"))
                {
                    ret.InlineFeed = new AtomResult(inline.Elements(AtomNS + "feed").SingleOrDefault());
                }
                else if (ret.Type.Contains("type=entry"))
                {
                    ret.InlineEntry = ParseEntry(inline.Elements(AtomNS + "entry").SingleOrDefault());
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized inline type: " + ret.Type);
                }
            }

            return ret;
        }

        private void Parse()
        {
            Parse(XDocument.Parse(Response.ResponseStr).Root);
        }

        private void Parse(XElement rootElement)
        {
            Links.AddRange(rootElement.Elements(AtomNS + "link").Select(ParseLink));

            XElement inlineCount = rootElement.Elements(MetadataNS + "count").SingleOrDefault();
            if (inlineCount != null)
            {
                InLineCount = long.Parse(inlineCount.Value);
            }
            else
            {
                InLineCount = -1;
            }

            if (rootElement.Name.Equals(AtomNS + "entry"))
            {
                Entries.Add(ParseEntry(rootElement));
            }
            else
            {
                Entries.AddRange(rootElement.Elements(AtomNS + "entry").Select(ParseEntry));
            }
        }
    }
}
