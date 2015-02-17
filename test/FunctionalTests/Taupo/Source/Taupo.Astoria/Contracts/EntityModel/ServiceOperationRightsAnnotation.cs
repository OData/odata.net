//---------------------------------------------------------------------
// <copyright file="ServiceOperationRightsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation for marking the rights of a service operation
    /// </summary>
    public class ServiceOperationRightsAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the ServiceOperation Rights
        /// </summary>
        public ServiceOperationRights Value { get; set; }
    }
}