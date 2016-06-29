//---------------------------------------------------------------------
// <copyright file="VocabularyAnnotationCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;


    #endregion Namespaces

    /// <summary>
    /// Represents an annotated EDM model.
    /// </summary>
    internal sealed class VocabularyAnnotationCache
    {
        /// <summary>The primary model</summary>
        private readonly IEdmModel primaryModel;

        /// <summary>The unique collection of vocabulary annotatedModels that are either contained in the primary model or that target items in the primary model.</summary>
        private readonly HashSet<IEdmVocabularyAnnotation> uniqueAnnotationsStorage = new HashSet<IEdmVocabularyAnnotation>(new AnnotationComparer());

        /// <summary>
        /// Initializes a new instance of the VocabularyAnnotationCache class.
        /// </summary>
        /// <param name="primaryModel">The primary model the annotatedModels belong to.</param>
        internal VocabularyAnnotationCache(IEdmModel primaryModel)
        {
            Debug.Assert(primaryModel != null, "primaryModel != null");
            this.primaryModel = primaryModel;
        }

        /// <summary>
        /// Gets the collection of vocabulary annotatedModels that are either contained in the primary model or that 
        /// target items in the primary model. In case of duplicates, the first annotation is taken and the rest are 
        /// ignored (i.e. the ordering of the annotation models is significant). Uniqueness of annotatedModels is 
        /// determined based on [target, term, qualifier].
        /// </summary>
        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get
            {
                return this.uniqueAnnotationsStorage;
            }
        }

        /// <summary>
        /// Searches for vocabulary annotatedModels specified by the primary model or a referenced model (including the annotation models) for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotatedModels for the element.</returns>
        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            return this.VocabularyAnnotations.Where(a => a.Target == element);
        }

        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation to add.</param>
        internal void Add(IEdmVocabularyAnnotation annotation)
        {
            Debug.Assert(annotation != null, "annotation != null");

            IEdmVocabularyAnnotatable target = annotation.Target;
            if (target != null && IsModelMember(target, this.primaryModel))
            {
                this.uniqueAnnotationsStorage.Add(annotation);
            }
        }

        /// <summary>
        /// Populates the annotation cache based on the current configuration of the service.
        /// 1) Adds any annotatedModels specific to the current URL convention.
        /// 2) Invokes the user-provided 'AnnotationsBuilder' delegate.
        /// </summary>
        /// <param name="configuration">The service configuration.</param>
        internal void PopulateFromConfiguration(DataServiceConfiguration configuration)
        {
            Debug.Assert(configuration != null, "configuration != null");

            IEnumerable<IEdmModel> annotatedModels = null;
            Func<IEdmModel, IEnumerable<IEdmModel>> annotationsBuilder = configuration.AnnotationsBuilder;
            if (annotationsBuilder != null)
            {
                annotatedModels = annotationsBuilder(this.primaryModel);
            }

            // if the delegate was not provided or returned null, stop now.
            if (annotatedModels == null)
            {
                return;
            }

            // add the annotations form each of the models to the vocab cache
            foreach (IEdmModel annotationModel in annotatedModels)
            {
                if (annotationModel == null)
                {
                    throw new InvalidOperationException(Strings.DataServiceProviderWrapper_AnnotationsBuilderCannotReturnNullModels);
                }

                // to avoid issues with the closure, create a local variable to store the model.
                IEdmModel localReferenceToModel = annotationModel;
                this.AddAnnotations(localReferenceToModel.VocabularyAnnotations);
            }
        }

        /// <summary>
        /// Checks whether an annotatable item is contained within a given model.
        /// </summary>
        /// <param name="item">The annotatable item.</param>
        /// <param name="model">The model to search within.</param>
        /// <returns>A boolean to indicate whether or not the item was found.</returns>
        private static bool IsModelMember(IEdmVocabularyAnnotatable item, IEdmModel model)
        {
            Debug.Assert(item != null, "item != null");
            Debug.Assert(model != null, "model != null");

            IEdmProperty propertyItem;

            if (item is IEdmSchemaElement)
            {
                // note that IEdmEntityContainer is a schema element, so a specific check for it is not needed.
                return model.SchemaElements.Contains(item);
            }

            if ((propertyItem = item as IEdmProperty) != null)
            {
                return model.SchemaElements.Contains((IEdmSchemaElement)propertyItem.DeclaringType);
            }

            if (item is IEdmEntityContainerElement)
            {
                return model.EntityContainer.Elements.Contains(item);
            }

            return false;
        }

        /// <summary>
        /// Adds a set of annotations that can be returned when either FindDeclaredVocabularyAnnotations or VocabulationAnnotations are accessed.
        /// </summary>
        /// <param name="annotations">The enumerable list of annotations.</param>
        private void AddAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                this.Add(annotation);
            }
        }

        /// <summary>Equality comparer for IEdmVocabularyAnnotation.</summary>
        private class AnnotationComparer : IEqualityComparer<IEdmVocabularyAnnotation>
        {
            /// <summary>Determines whether two IEdmVocabularyAnnotations are the same.</summary>
            /// <param name="x">First AnnotationDescriptor to compare.</param>
            /// <param name="y">Second AnnotationDescriptor to compare.</param>
            /// <returns>true if both are the same; false otherwise.</returns>
            public bool Equals(IEdmVocabularyAnnotation x, IEdmVocabularyAnnotation y)
            {
                if (x == null || y == null)
                {
                    return (x == null && y == null);
                }

                return x.Target == y.Target &&
                        x.Term.Namespace == y.Term.Namespace &&
                        x.Term.Name == y.Term.Name &&
                        x.Qualifier == y.Qualifier;
            }

            /// <summary>Computes hashcode for IEdmVocabularyAnnotation.</summary>
            /// <param name="obj">Object to compute hashcode for.</param>
            /// <returns>Computed hashcode.</returns>
            public int GetHashCode(IEdmVocabularyAnnotation obj)
            {
                if (obj != null)
                {
                    int hashCode = obj.Target.GetHashCode() ^ obj.Term.Namespace.GetHashCode() ^ obj.Term.Name.GetHashCode();

                    return (obj.Qualifier != null)
                        ? hashCode ^ obj.Qualifier.GetHashCode()
                        : hashCode;
                }

                return 0;
            }
        }
    }
}
