//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
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
            get { return this.HasAnnotations<CsdlAnnotation>(); }
        }

        public IEnumerable<CsdlDirectValueAnnotation> ImmediateValueAnnotations
        {
            get
            {
                return this.GetAnnotations<CsdlDirectValueAnnotation>();
            }
        }

        public IEnumerable<CsdlAnnotation> VocabularyAnnotations
        {
            get
            {
                return this.GetAnnotations<CsdlAnnotation>();
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

        public void AddAnnotation(CsdlAnnotation annotation)
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
