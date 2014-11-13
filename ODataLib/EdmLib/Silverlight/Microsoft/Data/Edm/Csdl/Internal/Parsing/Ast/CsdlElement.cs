//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Common base class for CSDL elements.
    /// </summary>
    internal abstract class CsdlElement
    {
        protected List<object> annotations;
        protected EdmLocation location;

        public CsdlElement(CsdlLocation location)
        {
            this.location = location;
        }

        public virtual bool HasDirectValueAnnotations
        {
            get { return this.HasAnnotations<CsdlDirectValueAnnotation>(); }
        }

        public bool HasVocabularyAnnotations
        {
            get { return this.HasAnnotations<CsdlVocabularyAnnotationBase>(); }
        }

        public IEnumerable<CsdlDirectValueAnnotation> ImmediateValueAnnotations
        {
            get
            {
                return this.GetAnnotations<CsdlDirectValueAnnotation>();
            }
        }

        public IEnumerable<CsdlVocabularyAnnotationBase> VocabularyAnnotations
        {
            get
            {
                return this.GetAnnotations<CsdlVocabularyAnnotationBase>();
            }
        }

        public EdmLocation Location
        {
            get { return this.location; }
        }

        public void AddAnnotation(CsdlDirectValueAnnotation annotation)
        {
            this.AddUntypedAnnotation(annotation);
        }

        public void AddAnnotation(CsdlVocabularyAnnotationBase annotation)
        {
            this.AddUntypedAnnotation(annotation);
        }

        private IEnumerable<T> GetAnnotations<T>() where T : class
        {
            return this.annotations != null ? this.annotations.OfType<T>() : Enumerable.Empty<T>();
        }

        private void AddUntypedAnnotation(object annotation)
        {
            if (this.annotations == null)
            {
                this.annotations = new List<object>();
            }

            this.annotations.Add(annotation);
        }

        private bool HasAnnotations<T>()
        {
            if (this.annotations == null)
            {
                return false;
            }

            foreach (object annotation in this.annotations)
            {
                if (annotation is T)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
