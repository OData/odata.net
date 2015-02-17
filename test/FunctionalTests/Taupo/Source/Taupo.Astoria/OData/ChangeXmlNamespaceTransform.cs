//---------------------------------------------------------------------
// <copyright file="ChangeXmlNamespaceTransform.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Payload transform class for changing xml namespace prefixes.
    /// </summary>
    public class ChangeXmlNamespaceTransform : IPayloadTransform<XElement>
    {
        /// <summary>
        /// Transforms the original payload by changing xml namespace prefixes.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the payload is transformed else returns false.</returns>
        public bool TryTransform(XElement originalPayload, out XElement transformedPayload)
        {
            ExceptionUtilities.CheckObjectNotNull(originalPayload, "Payload cannot be null.");

            transformedPayload = new XElement(originalPayload);

            IEnumerable<XNode> nodes = transformedPayload.DescendantNodesAndSelf();
            List<XAttribute> newAttributes = new List<XAttribute>();
            XAttribute modifiedAttribute = null;
            XElement childNode;

            foreach (XNode node in nodes)
            {
                childNode = node as XElement;
                if (childNode != null)
                {
                    newAttributes = new List<XAttribute>();

                    foreach (XAttribute attribute in childNode.Attributes())
                    {
                        if (attribute.IsNamespaceDeclaration && !string.IsNullOrEmpty(attribute.Value) && attribute.Value != transformedPayload.GetDefaultNamespace().NamespaceName)
                        {
                            modifiedAttribute = new XAttribute(this.ChangeNamespacePrefix(attribute.Name), attribute.Value);
                            newAttributes.Add(modifiedAttribute);
                        }
                        else
                        {
                            newAttributes.Add(attribute);
                        }
                    }

                    childNode.ReplaceAttributes(newAttributes);
                }
            }

            if (modifiedAttribute != null)
            {
                return true;
            }

            transformedPayload = null;
            return false;
        }

        /// <summary>
        /// Returns the modified namespace prefix.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Modified namespace prefix.</returns>
        private string ChangeNamespacePrefix(XName name)
        {
            return XNamespace.Xmlns + name.LocalName + "x";
        }
    }
}
