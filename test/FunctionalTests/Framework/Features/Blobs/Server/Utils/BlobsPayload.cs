//---------------------------------------------------------------------
// <copyright file="BlobsPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;
using System.Xml;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Holds and manages request or response MLE payloads.
    //---------------------------------------------------------------------
    public abstract class BlobsPayload
    {
        // Whole payload conversions.
        public abstract override string ToString();
        public abstract string AdjustedForVerify();

        // Individual MLE property access.
        public abstract string GetProperty(string name);
        public abstract void SetProperty(string name, string value);

        //---------------------------------------------------------------------
        public static BlobsPayload Parse(SerializationFormatKind format, AstoriaResponse response)
        {
            switch (format)
            {
                case SerializationFormatKind.Atom: return new BlobsPayload.Atom(response.Payload);
                case SerializationFormatKind.JSON: return new BlobsPayload.JSON(response.Payload);
                default:
                    AstoriaTestLog.FailAndThrow("Unknown serialization format kind: " + format.ToString());
                    return null;
            }
        }

        //---------------------------------------------------------------------
        // Atom payload parser.
        //---------------------------------------------------------------------
        public class Atom : BlobsPayload
        {
            private XmlDocument atomDoc = new XmlDocument();
            private static XmlNamespaceManager atomNS = new XmlNamespaceManager(new NameTable());

            //---------------------------------------------------------------------
            static Atom()
            {
                atomNS.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                atomNS.AddNamespace("d", "http://docs.oasis-open.org/odata/ns/data");
                atomNS.AddNamespace("m", "http://docs.oasis-open.org/odata/ns/metadata");
            }

            //---------------------------------------------------------------------
            public Atom(string payload)
            {
                atomDoc.LoadXml(payload);

                // Remove <atom:link> elements except those whose attributes "rel" have values starting with "edit".
                foreach (XmlNode node in atomDoc.SelectNodes("/atom:entry/atom:link[not(starts-with(@rel, 'edit'))]", atomNS))
                    node.ParentNode.RemoveChild(node);

                // Remove <atom:updated> timestamp as it prevents direct comparison.
                XmlNode atomUpdated = atomDoc.SelectSingleNode("/atom:entry/atom:updated", atomNS);
                if (atomUpdated != null)
                    atomUpdated.ParentNode.RemoveChild(atomUpdated);

                // Save and remove m:etag attribute.
                XmlAttribute etag = atomDoc.SelectSingleNode("/atom:entry/atom:link[@rel='edit-media']/@m:etag", atomNS) as XmlAttribute;
                if (etag != null)
                {
                    BlobsRequest.ETagMRR = etag.Value;
                    etag.OwnerElement.RemoveAttributeNode(etag);
                }

                // Remove all m:null attributes.
                foreach (XmlNode mNull in atomDoc.SelectNodes("//@m:null", atomNS))
                    (mNull as XmlAttribute).OwnerElement.RemoveAttributeNode(mNull as XmlAttribute);

                // Remove all xml:space="preserve" attributes.
                foreach (XmlNode node in atomDoc.SelectNodes("//@xml:space", atomNS))
                    (node as XmlAttribute).OwnerElement.RemoveAttributeNode(node as XmlAttribute);
            }

            //---------------------------------------------------------------------
            // Locates MLE property and returns its value.
            //---------------------------------------------------------------------
            public override string GetProperty(string name)
            {
                XmlElement propertyElement = (XmlElement)atomDoc.SelectSingleNode("/atom:entry/m:properties/d:" + name, atomNS);
                return propertyElement.InnerText;
            }


            //---------------------------------------------------------------------
            // Modifies MLE by assigning value to property.
            //---------------------------------------------------------------------
            public override void SetProperty(string name, string value)
            {
                XmlElement propertyElement = (XmlElement)atomDoc.SelectSingleNode("/atom:entry/m:properties/d:" + name, atomNS);
                propertyElement.InnerText = value;
            }

            //---------------------------------------------------------------------
            public override string ToString()
            {
                return atomDoc.InnerXml;
            }

            //---------------------------------------------------------------------
            // Moves <m:Properties> inside <atom:content> to make MLE look like normal entity (for AstoriaResponse.Verify()).
            //---------------------------------------------------------------------
            public override string AdjustedForVerify()
            {
                // Create a copy of this XML document.
                XmlDocument atomCopy = new XmlDocument();
                atomCopy.LoadXml(atomDoc.OuterXml);

                // Move 'properties' into 'content'.
                XmlElement properties = atomCopy.SelectSingleNode("/atom:entry/m:properties", atomNS) as XmlElement;
                XmlElement content = atomCopy.SelectSingleNode("/atom:entry/atom:content", atomNS) as XmlElement;
                if (properties != null && content != null)
                    content.AppendChild(properties);

                return atomCopy.InnerXml;
            }
        }

        //---------------------------------------------------------------------
        // JSON payload parser.
        //---------------------------------------------------------------------
        public class JSON : BlobsPayload
        {
            private const string jsonPropertyRegex = @"(,?""name"":"")(.*?)""([,}])";
            private string payload;

            //---------------------------------------------------------------------
            public JSON(string payload)
            {
                // Remove outermost "d".
                var prefix = "{\"d\":";
                int metadataPosition = payload.IndexOf(prefix);
                if (metadataPosition != -1)
                {
                    payload = payload.Substring(metadataPosition + prefix.Length);  // {"d":
                    payload = payload.Remove(payload.LastIndexOf("}")); // }
                }

                // Remove "etag".
                string etagRegex = jsonPropertyRegex.Replace("name", "etag");
                Match m = Regex.Match(payload, etagRegex);
                if (m.Success)
                    payload = Regex.Replace(payload, etagRegex, m.Groups[3].Value);

                // Save and remove "media_etag".
                string mediaEtagRegex = jsonPropertyRegex.Replace("name", "media_etag");
                m = Regex.Match(payload, mediaEtagRegex);
                if (m.Success)
                {
                    BlobsRequest.ETagMRR = m.Groups[2].Value.Replace("\\'", "'").Replace("\\\"", "\"");
                    payload = Regex.Replace(payload, mediaEtagRegex, m.Groups[3].Value);
                }

                this.payload = payload;
            }

            //---------------------------------------------------------------------
            // Locates MLE property and returns its value.
            //---------------------------------------------------------------------
            public override string GetProperty(string name)
            {
                Match m = Regex.Match(payload, jsonPropertyRegex.Replace("name", name));
                return m.Groups[2].Value;
            }

            //---------------------------------------------------------------------
            // Modifies MLE by assigning value to property.
            //---------------------------------------------------------------------
            public override void SetProperty(string name, string newValue)
            {
                Match m = Regex.Match(payload, jsonPropertyRegex.Replace("name", name));
                string prefix = m.Groups[1].Value;
                string suffix = m.Groups[3].Value;
                payload = Regex.Replace(payload, jsonPropertyRegex.Replace("name", name), prefix + newValue + "\"" + suffix);
            }

            //---------------------------------------------------------------------
            public override string ToString() { return payload; }

            //---------------------------------------------------------------------
            public override string AdjustedForVerify() { return payload; }
        }
    }
}

