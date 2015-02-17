//---------------------------------------------------------------------
// <copyright file="WriteFeedIDFirstFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// This class ensures that the test serializer writes the id before the entries.
    /// </summary>
    class WriteFeedIDFirstFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            XmlRewriteElementAnnotation annotation = new XmlRewriteElementAnnotation()
            {
                RewriteFunction = (node) =>
                    {
                        XNode foundNode = null;
                        var currentNode = node.FirstNode;
                        if (node.Nodes().Count() > 1)
                        {
                            while (currentNode != null && foundNode == null)
                            {
                                var element = (XElement)currentNode;
                                if (element.Name.LocalName.Equals("id"))
                                {
                                    foundNode = currentNode;
                                    currentNode.Remove();
                                }
                                currentNode = currentNode.NextNode;
                            }

                            if (foundNode != null)
                            {
                                node.FirstNode.AddBeforeSelf(foundNode);
                            }
                        }

                        return node;
                    }
            };

            payloadElement.Add(annotation);
            base.Visit(payloadElement);
        }
    }
}
