//---------------------------------------------------------------------
// <copyright file="AddXmlCommentsPayloadTransform.cs" company="Microsoft">
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
    /// Payload transformation class for adding comments to XML.
    /// </summary>
    public class AddXmlCommentsPayloadTransform : IPayloadTransform<XElement>
    {
        /// <summary>
        /// Transforms the original payload by adding comments to it.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the transformation is successful else returns false.</returns>
        public bool TryTransform(XElement originalPayload, out XElement transformedPayload)
        {
            // TODO: use Taupo's data generation to randomly set the value for the comments.
            ExceptionUtilities.CheckArgumentNotNull(originalPayload, "originalPayload");

            transformedPayload = new XElement(originalPayload);

            // Add comment at the top.
            transformedPayload.Add(new XComment("This is a test comment."));
            
            // Add comment within nodes.       
            foreach (var node in transformedPayload.Nodes())
            {
                node.AddBeforeSelf(new XComment("This is a test comment."));
            }

            return true;
        }
    }
}
