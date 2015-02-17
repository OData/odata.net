//---------------------------------------------------------------------
// <copyright file="EntityModelTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// An annotation to associate an entity model type with their values.
    /// </summary>
    public sealed class EntityModelTypeAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityModelType">The entity model type to store on the annotation.</param>
        public EntityModelTypeAnnotation(DataType entityModelType)
        {
            this.EntityModelType = entityModelType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The edmType to store on the annotation.</param>
        public EntityModelTypeAnnotation(IEdmTypeReference type)
        {
            this.EdmModelType = type;
        }

        /// <summary>
        /// The entity model type to store on the annotation.
        /// </summary>
        public DataType EntityModelType { get; set; }

        /// <summary>
        /// The edm model type to store on the annotation.
        /// </summary>
        public IEdmTypeReference EdmModelType { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            // ToString is actually a very good representation holding most of the interesting information.
            get
            {
                if (this.EntityModelType != null)
                    return "EntityModelTypeAnnotation: { " + this.EntityModelType.ToString() + " }";
                if (this.EdmModelType != null)
                    return "EntityModelTypeAnnotation: { " + this.EdmModelType.ToString() + "}";
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new EntityModelTypeAnnotation(this.EntityModelType)
            {
                EdmModelType = this.EdmModelType
            };
        }
    }
}
