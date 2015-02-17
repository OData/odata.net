//---------------------------------------------------------------------
// <copyright file="IAstoriaServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for describing an astoria service
    /// </summary>
    public interface IAstoriaServiceDescriptor
    {
        /// <summary>
        /// Gets the service root uri
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// Gets the model of the service
        /// </summary>
        EntityModelSchema ConceptualModel { get; }
    }
}
