//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
    internal interface IXmlElementAttributes
    {
        IEnumerable<XmlAttributeInfo> Unused
        {
            get;
        }

        XmlAttributeInfo this[string attributeName]
        {
            get;
        }
    }

    internal class XmlElementInfo : IXmlElementAttributes
    {
        private readonly Dictionary<string, XmlAttributeInfo> attributes;
        private List<XmlAnnotationInfo> annotations;

        internal XmlElementInfo(string elementName, CsdlLocation elementLocation, IList<XmlAttributeInfo> attributes, List<XmlAnnotationInfo> annotations)
        {
            this.Name = elementName;
            this.Location = elementLocation;
            if (attributes != null && attributes.Count > 0)
            {
                this.attributes = new Dictionary<string, XmlAttributeInfo>();
                foreach (XmlAttributeInfo newAttr in attributes)
                {
                    Debug.Assert(!this.attributes.ContainsKey(newAttr.Name), "Multiple attributes with the same name are not supported");
                    this.attributes.Add(newAttr.Name, newAttr);
                }
            }

            this.annotations = annotations;
        }

        IEnumerable<XmlAttributeInfo> IXmlElementAttributes.Unused
        {
            get
            {
                if (this.attributes != null)
                {
                    foreach (XmlAttributeInfo attr in this.attributes.Values.Where(attr => !attr.IsUsed))
                    {
                        yield return attr;
                    }
                }
            }
        }

        internal string Name
        {
            get;
            private set;
        }

        internal CsdlLocation Location
        {
            get;
            private set;
        }

        internal IXmlElementAttributes Attributes
        {
            get { return (IXmlElementAttributes)this; }
        }

        internal IList<XmlAnnotationInfo> Annotations
        {
            get { return this.annotations ?? ((IList<XmlAnnotationInfo>)new XmlAnnotationInfo[] { }); }
        }

        XmlAttributeInfo IXmlElementAttributes.this[string attributeName]
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(attributeName), "Ensure attribute name is not null or empty before accessing Attributes");

                XmlAttributeInfo foundAttr;
                if (this.attributes != null && this.attributes.TryGetValue(attributeName, out foundAttr))
                {
                    foundAttr.IsUsed = true;
                    return foundAttr;
                }

                return XmlAttributeInfo.Missing;
            }
        }

        internal void AddAnnotation(XmlAnnotationInfo annotation)
        {
            if (this.annotations == null)
            {
                this.annotations = new List<XmlAnnotationInfo>();
            }

            this.annotations.Add(annotation);
        }
    }

    internal class XmlAnnotationInfo
    {
        internal XmlAnnotationInfo(CsdlLocation location, string namespaceName, string name, string value, bool isAttribute)
        {
            this.Location = location;
            this.NamespaceName = namespaceName;
            this.Name = name;
            this.Value = value;
            this.IsAttribute = isAttribute;
        }

        internal string NamespaceName
        {
            get;
            private set;
        }

        internal string Name
        {
            get; 
            private set;
        }

        internal CsdlLocation Location
        {
            get; 
            private set;
        }

        internal string Value
        {
            get; 
            private set;
        }

        internal bool IsAttribute
        {
            get; 
            private set;
        }
    }

    internal class XmlAttributeInfo
    {
        internal static readonly XmlAttributeInfo Missing = new XmlAttributeInfo();
        private readonly string name;
        private readonly string attributeValue;
        private readonly CsdlLocation location;

        internal XmlAttributeInfo(string attrName, string attrValue, CsdlLocation attrLocation)
        {
            this.name = attrName;
            this.attributeValue = attrValue;
            this.location = attrLocation;
        }

        private XmlAttributeInfo()
        {
        }

        internal bool IsMissing
        {
            get { return object.ReferenceEquals(XmlAttributeInfo.Missing, this); }
        }

        internal bool IsUsed
        {
            get; 
            set;
        }

        internal CsdlLocation Location
        {
            get { return this.location; }
        }

        internal string Name
        {
            get { return this.name; }
        }

        internal string Value
        {
            get { return this.attributeValue; }
        }
    }
}
