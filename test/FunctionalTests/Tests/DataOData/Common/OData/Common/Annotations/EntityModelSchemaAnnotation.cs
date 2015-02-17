//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    #endregion Namespaces

    /// <summary>
    /// An annotation to associate the entity schema model with types created for it.
    /// </summary>
    public sealed class EntityModelSchemaAnnotation : Annotation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model to store on the annotation.</param>
        public EntityModelSchemaAnnotation(EntityModelSchema model)
        {
            this.Model = model;
        }

        /// <summary>
        /// The model to store on the annotation.
        /// </summary>
        public EntityModelSchema Model { get; set; }
    }
}
