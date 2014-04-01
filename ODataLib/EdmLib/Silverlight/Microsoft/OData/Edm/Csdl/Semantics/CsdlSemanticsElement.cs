//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;

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

            List<CsdlDirectValueAnnotation> annotations = this.Element.ImmediateValueAnnotations.ToList();
            CsdlElementWithDocumentation elementWithDocumentation = this.Element as CsdlElementWithDocumentation;
            CsdlDocumentation documentation = (elementWithDocumentation != null) ? elementWithDocumentation.Documentation : null;

            if (documentation != null || annotations.FirstOrDefault() != null)
            {
                List<IEdmDirectValueAnnotation> wrappedAnnotations = new List<IEdmDirectValueAnnotation>();

                foreach (CsdlDirectValueAnnotation annotation in annotations)
                {
                    wrappedAnnotations.Add(new CsdlSemanticsDirectValueAnnotation(annotation, this.Model));
                }

                if (documentation != null)
                {
                    wrappedAnnotations.Add(new CsdlSemanticsDocumentation(documentation, this.Model));
                }

                return wrappedAnnotations;
            }

            return null;
        }
    }
}
