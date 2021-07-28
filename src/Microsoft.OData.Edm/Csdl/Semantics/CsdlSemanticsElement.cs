//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Common base class for CsdlSemantics classes that have Annotations.
    /// </summary>
    internal abstract class CsdlSemanticsElement : IEdmElement, IEdmLocatable
    {
        private readonly Cache<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>> inlineVocabularyAnnotationsCache;
        private static readonly Func<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>> ComputeInlineVocabularyAnnotationsFunc = (me) => me.ComputeInlineVocabularyAnnotations();

        private readonly Cache<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>> directValueAnnotationsCache;
        private static readonly Func<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>> ComputeDirectValueAnnotationsFunc = (me) => me.ComputeDirectValueAnnotations();

        private static readonly IEnumerable<IEdmVocabularyAnnotation> emptyVocabularyAnnotations = Enumerable.Empty<IEdmVocabularyAnnotation>();

        private string annotationFullName = null;

        protected CsdlSemanticsElement(CsdlElement element)
        {
            if (element != null)
            {
                // Many elements have no attached annotations. For these, save the allocation of the cache and the cost of the cache mechanism.
                if (element.HasDirectValueAnnotations)
                {
                    this.directValueAnnotationsCache = new Cache<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>>();
                }

                if (element.HasVocabularyAnnotations)
                {
                    this.inlineVocabularyAnnotationsCache = new Cache<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>>();
                }
            }
        }

        public abstract CsdlSemanticsModel Model { get; }

        public abstract CsdlElement Element { get; }

        public IEnumerable<IEdmVocabularyAnnotation> InlineVocabularyAnnotations
        {
            get
            {
                if (this.inlineVocabularyAnnotationsCache == null)
                {
                    return emptyVocabularyAnnotations;
                }

                return this.inlineVocabularyAnnotationsCache.GetValue(this, ComputeInlineVocabularyAnnotationsFunc, null);
            }
        }

        public EdmLocation Location
        {
            get
            {
                if (this.Element == null || this.Element.Location == null)
                {
                    return new ObjectLocation(this);
                }

                return this.Element.Location;
            }
        }

        public IEnumerable<IEdmDirectValueAnnotation> DirectValueAnnotations
        {
            get
            {
                if (this.directValueAnnotationsCache == null)
                {
                    return null;
                }

                return this.directValueAnnotationsCache.GetValue(this, ComputeDirectValueAnnotationsFunc, null);
            }
        }

        /// <summary>
        /// Gets the cached annotation full qualified name.
        /// </summary>
        /// <param name="element">This element as <see cref="IEdmVocabularyAnnotatable"/>.</param>
        /// <returns>The cached annotation full qualified name.</returns>
        public string GetAnnotationFullQualifiedName(IEdmVocabularyAnnotatable element)
        {
            Debug.Assert(object.ReferenceEquals(this as IEdmVocabularyAnnotatable, element));
            this.annotationFullName = this.annotationFullName ?? EdmUtil.FullyQualifiedName(element);
            return this.annotationFullName;
        }

        /// <summary>
        /// Allocates a new list if needed, and adds the item to the list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="list">List to add the item to.</param>
        /// <param name="item">Item being added.</param>
        /// <returns>List containing then new item.</returns>
        protected static List<T> AllocateAndAdd<T>(List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            list.Add(item);
            return list;
        }

        protected static List<T> AllocateAndAdd<T>(List<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            list.AddRange(items);
            return list;
        }

        protected virtual IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, null);
        }

        protected IEnumerable<IEdmDirectValueAnnotation> ComputeDirectValueAnnotations()
        {
            if (this.Element == null)
            {
                return null;
            }

            List<IEdmDirectValueAnnotation> wrappedAnnotations = null;

            foreach (CsdlDirectValueAnnotation annotation in this.Element.ImmediateValueAnnotations)
            {
                if (wrappedAnnotations == null)
                {
                    wrappedAnnotations = new List<IEdmDirectValueAnnotation>();
                }

                wrappedAnnotations.Add(new CsdlSemanticsDirectValueAnnotation(annotation, this.Model));
            }

            return wrappedAnnotations;
        }
    }
}
