//---------------------------------------------------------------------
// <copyright file="ClientMediaEntryAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation to represent client properties for MediaEntry and MimeTypeProperty attributes
    /// </summary>
    public class ClientMediaEntryAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets a value indicating the client type property holding V1 stream bytes
        /// </summary>
        public string MediaEntryName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the client type property holding V1 stream byte array name
        /// </summary>
        public string MimeTypePropertyName { get; set; }
    }
}