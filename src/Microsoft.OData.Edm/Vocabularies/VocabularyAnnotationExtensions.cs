//---------------------------------------------------------------------
// <copyright file="VocabularyAnnotationExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Extension methods for the vocabulary annotations.
    /// </summary>
    public static class VocabularyAnnotationExtensions
    {
        /// <summary>
        /// Gets the collection of string for a target annotatable.
        /// </summary>
        /// <param name="model">The model referenced to.</param>
        /// <param name="target">The target annotatable to find annotation.</param>
        /// <param name="term">The target annotatable to find annotation.</param>
        /// <returns>Null or a collection string of qualified type name.</returns>
        public static IEnumerable<string> GetVocabularyStringCollection(this IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(target, "term");

            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
            if (annotation != null)
            {
                IEdmCollectionExpression collectionExpression = annotation.Value as IEdmCollectionExpression;
                if (collectionExpression != null && collectionExpression.Elements != null)
                {
                    return collectionExpression.Elements.OfType<IEdmStringConstantExpression>().Select(e => e.Value);
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}
