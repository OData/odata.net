//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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

        private IEnumerable<T> GetAnnotations<T>() where T : class
        {
            return this.annotations != null ? this.annotations.OfType<T>() : Enumerable.Empty<T>();
        }

        public IEnumerable<CsdlImmediateValueAnnotation> ImmediateValueAnnotations
        {
            get
            {
                return GetAnnotations<CsdlImmediateValueAnnotation>();
            }
        }

        public IEnumerable<CsdlVocabularyAnnotationBase> VocabularyAnnotations
        {
            get
            {
                return GetAnnotations<CsdlVocabularyAnnotationBase>();
            }
        }

        private void AddUntypedAnnotation(object annotation)
        {
            if (this.annotations == null)
            {
                this.annotations = new List<object>();
            }

            this.annotations.Add(annotation);
        }

        public void AddAnnotation(CsdlImmediateValueAnnotation annotation)
        {
            this.AddUntypedAnnotation(annotation);
        }

        public void AddAnnotation(CsdlVocabularyAnnotationBase annotation)
        {
            this.AddUntypedAnnotation(annotation);
        }

        public EdmLocation Location
        {
            get { return this.location; }
        }
    }
}
