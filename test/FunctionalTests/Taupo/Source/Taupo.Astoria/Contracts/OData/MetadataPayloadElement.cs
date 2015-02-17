//---------------------------------------------------------------------
// <copyright file="MetadataPayloadElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// class for metadata payload
    /// </summary>
    public class MetadataPayloadElement : ODataPayloadElement
    {
        /// <summary>
        /// Gets or sets the DataServiceMetadataSchemaNamespace
        /// </summary>
        public string DataServiceMetadataSchemaNamespace { get; set; }

        /// <summary>
        /// Gets or sets the EdmxVersion
        /// </summary>
        public string EdmxVersion { get; set; }

        /// <summary>
        /// Gets or sets the Entity Model Schema for the payload
        /// </summary>
        public EntityModelSchema EntityModelSchema { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return this.ElementType.ToString();
            }
        }

        /// <summary>
        /// Accept call is not implemented
        /// </summary>
        /// <typeparam name="TResult">Result for accept method</typeparam>
        /// <param name="visitor">Visitor for accept method</param>
        /// <returns>A Not Implemented Exception</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            throw new TaupoNotSupportedException("Metadata Payload Element does not visit anything");
        }

        /// <summary>
        /// Accept call is not implemented
        /// </summary>
        /// <param name="visitor">Not applicable</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            throw new TaupoNotSupportedException("Metadata Payload Element does not visit anything");
        }
    }
}
