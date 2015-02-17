//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaToEdmModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Edmlib;

    /// <summary>
    /// Converts an <see cref="EntityModelSchema"/> to its corresponding <see cref="IEdmModel"/>
    /// including all OData-specific metadata annotations.
    /// </summary>
    public class EntityModelSchemaToEdmModelConverter
    {
        [InjectDependency(IsRequired = true)]
        public TaupoToEdmModelConverterUsingParser BaseEntityModelSchemaToEdmModelConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public AstoriaProductAnnotationConverter SerializableToTestAnnotationConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IAttributeToPropertyMappingAnnotationConverter TestToSerializableAnnotationConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public AstoriaAttributeToODataLibAnnotationConverter SerializableToODataLibAnnotationConverter { get; set; }

        /// <summary>
        /// Converts the <paramref name="entityModelSchema"/> to the corresponding
        /// <see cref="IEdmModel"/> including the OData metadata annotations.
        /// </summary>
        /// <param name="entityModelSchema">The <see cref="EntityModelSchema"/> to convert.</param>
        /// <param name="loadSerializableAnnotations">true if serializable annotations should be loaded into their user-friendly in-memory representation; otherwise false.</param>
        /// <returns>The <see cref="IEdmModel"/> instance that corresponds to the <paramref name="entityModelSchema"/>.</returns>
        public virtual IEdmModel Convert(EntityModelSchema entityModelSchema, bool loadSerializableAnnotations = true)
        {
            // TODO: This code is way too complex for what it does.
            //      The current implementation for the underlying converter does not directly convert from EntityModelSchema
            //      to IEdmModel; instead it serializes the entity model schema into Xml and then parses the Xml into an IEdmModel.
            //      Direct conversion of annotations does not work because of this. We need to go from test annotations on the
            //      entity model schema to attribute annotations on that entity model schema, then use the underlying converter,
            //      then convert the serializable annotations on the EDM model back into product annotations as needed.
            //
            //      Talk to the EdmLib team whether we can use a direct converter instead!
            //
            //      Also modify the implementation of DataServiceProviderFactory once this is fixed; we do another post-processing
            //      there that could be folded into a visitor pattern.

            if (entityModelSchema == null)
            {
                return null;
            }


            // 1) Convert OData-specific test annotations to attribute annotations (this clones the model if needed)
            entityModelSchema = this.SerializableToTestAnnotationConverter.ConvertToProductAnnotations(entityModelSchema);

            // 2) Use the underlying converter to convert from entity model schema to Edm model
            this.BaseEntityModelSchemaToEdmModelConverter.EdmVersion = entityModelSchema.MinimumVersion().ToEdmVersion();
            IEdmModel model = this.BaseEntityModelSchemaToEdmModelConverter.ConvertToEdmModel(entityModelSchema);
            Debug.Assert(model != null, "Expected non-null model");

            if (loadSerializableAnnotations)
            {
                // 4) Because we have to go through Xml we cannot ensure the same order of the EPM annotations on the entity
                //    types we have to go fix this now.
                FixEPMAnnotationOrder(entityModelSchema, model);
            }

            return model;
        }

        /// <summary>
        /// Fixes the order of the EPM annotations to be the same in the source and the target models.
        /// They are different because we go through Xml to do the conversion in the base converter.
        /// </summary>
        /// <param name="entityModelSchema">The source schema.</param>
        /// <param name="model">The target model.</param>
        private static void FixEPMAnnotationOrder(EntityModelSchema entityModelSchema, IEdmModel model)
        {
            Debug.Assert(entityModelSchema != null, "entityModelSchema != null");
            Debug.Assert(model != null, "model != null");

            IEnumerable<EntityType> entityTypes = entityModelSchema.EntityTypes;
            if (entityTypes != null)
            {
                foreach (EntityType entityType in entityTypes)
                {
                    IEdmEntityType modelEntityType = model.FindType(entityType.FullName) as IEdmEntityType;
                    Debug.Assert(modelEntityType != null, "modelEntityType != null");
                }
            }
        }
    }
}
