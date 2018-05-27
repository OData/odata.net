//---------------------------------------------------------------------
// <copyright file="AnnotationDocumentationModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    public static class AnnotationDocumentationModels
    {
        public static void AddAttributeAnnotations(this IEdmModel model)
        {
            foreach (var annotableElement in GetAnnotableElements(model))
            {
                model.SetAnnotationValue(annotableElement, "Bogus.com::Internal", "Foo", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "bar"));
            }
        }

        private static IEnumerable<IEdmElement> GetAnnotableElements(IEdmModel model)
        {
            var entityContainer = model.EntityContainer;
            if (entityContainer != null)
            {
                yield return entityContainer;

                foreach (var entitySet in entityContainer.EntitySets())
                {
                    yield return entitySet;
                }
            }

            var schemaElements = model.SchemaElements.ToArray();
            foreach (var function in schemaElements.OfType<IEdmFunction>())
            {
                yield return function;

                foreach (var parameter in function.Parameters)
                {
                    yield return parameter;
                }
            }

            foreach (var structuredType in schemaElements.OfType<IEdmStructuredType>())
            {
                yield return structuredType;

                foreach (var property in structuredType.Properties())
                {
                    yield return property;
                }
            }

            foreach (var enumType in schemaElements.OfType<IEdmEnumType>())
            {
                yield return enumType;

                foreach (var member in enumType.Members)
                {
                    yield return member;
                }
            }
        }
    }
}
