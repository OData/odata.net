//---------------------------------------------------------------------
// <copyright file="EntitySetRightsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Annotation for marking the entity set rights of a set
    /// </summary>
    public class EntitySetRightsAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the EntitySetRights
        /// </summary>
        public EntitySetRights Value { get; set; }
    }
}