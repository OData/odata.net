//---------------------------------------------------------------------
// <copyright file="ODataQueryEntityModelSchemaToEdmModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Common;

    /// <summary>
    /// Converts an <see cref="EntityModelSchema"/> to its corresponding <see cref="IEdmModel"/>
    /// OData.Query specific implementation preserves Query specific annotations.
    /// </summary>
    public class ODataQueryEntityModelSchemaToEdmModelConverter : EntityModelSchemaToEdmModelConverter
    {
        /// <summary>
        /// Converts the <paramref name="entityModelSchema"/> to the corresponding
        /// <see cref="IEdmModel"/> including the OData metadata annotations.
        /// </summary>
        /// <param name="entityModelSchema">The <see cref="EntityModelSchema"/> to convert.</param>
        /// <param name="loadSerializableAnnotations">true if serializable annotations should be loaded into their user-friendly in-memory representation; otherwise false.</param>
        /// <returns>The <see cref="IEdmModel"/> instance that corresponds to the <paramref name="entityModelSchema"/>.</returns>
        public override IEdmModel Convert(EntityModelSchema entityModelSchema, bool loadSerializableAnnotations = true)
        {
            var model = base.Convert(entityModelSchema, loadSerializableAnnotations);

            // Because we have to go through Xml we cannot easily translate in-memory annotations; fix them now.
            ConvertInMemoryAnnotations(entityModelSchema, model);

            return model;
        }

        /// <summary>
        /// Convert the in-memory annotations from the source schema to the target model.
        /// </summary>
        /// <param name="entityModelSchema">The source schema.</param>
        /// <param name="model">The target model.</param>
        /// <remarks>
        /// We have to do this as a separate step because the base converter goes through
        /// Xml to do the conversion.
        /// </remarks>
        private static void ConvertInMemoryAnnotations(EntityModelSchema entityModelSchema, IEdmModel model)
        {
            Debug.Assert(entityModelSchema != null, "entityModelSchema != null");
            Debug.Assert(model != null, "model != null");

            IEnumerable<EntityContainer> entityContainers = entityModelSchema.EntityContainers;
            if (entityContainers != null)
            {
                foreach (EntityContainer entityContainer in entityContainers)
                {
                    IEdmEntityContainer edmContainer = (IEdmEntityContainer)model.FindEntityContainer(entityContainer.Name);
                    Debug.Assert(edmContainer != null, "edmContainer != null");

                }
            }
        }
    }
}
