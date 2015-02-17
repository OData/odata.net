//---------------------------------------------------------------------
// <copyright file="AstoriaProductAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Looks at test annotations and converts it to product annotation in a <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationName(typeof(ITestToProductAnnotationConverter), "Default")]
    public class AstoriaProductAnnotationConverter : ITestToProductAnnotationConverter
    {
        // TODO: Rename this converter; it converts from higher-level test annotations to the serializable
        //               annotations in CSDL.

        /// <summary>
        /// Performs the conversion.
        /// </summary>
        /// <param name="model">Model to perform conversion on.</param>
        /// <returns>A clone of the <paramref name="model"/> with the converted annotations.</returns>
        public EntityModelSchema ConvertToProductAnnotations(EntityModelSchema model)
        {
            // we have to clone the model here since we will modify it by adding the converted annotations;
            // we do this in all cases even if there are no annotations to convert; if this becomes a performance problem
            // we can change this going forward (to do the clone on-demand).
            model = model.Clone();

            // TODO: support for MimeTypeAnnotation on service operations and primitive properties
            foreach (var entityType in model.EntityTypes)
            {
                if (entityType.HasStream())
                {
                    var attribute = new XAttribute(ODataConstants.DataServicesMetadataNamespace.GetName(ODataConstants.HasStreamAttributeName), true);
                    AddAnnotationIfDoesntExist(entityType, new AttributeAnnotation(attribute));
                }
            }

            return model;
        }

        private static void AddAnnotationIfDoesntExist(AnnotatedItem payload, AttributeAnnotation annotation)
        {
            var exists = payload.Annotations.OfType<AttributeAnnotation>().Any(a =>
            {
                ExceptionUtilities.CheckObjectNotNull(a.Content, "Content expected for attribute");
                return a.Content.Name.Equals(annotation.Content.Name);
            });

            if (!exists)
            {
                payload.Annotations.Add(annotation);
            }
        }
    }
}
