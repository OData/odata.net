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

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Common base class for CsdlSemantics classes that have Annotations.
    /// </summary>
    internal abstract class CsdlSemanticsElement : IEdmElement, IEdmLocatable
    {
        private object transientAnnotations;

        private readonly Cache<CsdlSemanticsElement, IEnumerable<IEdmAnnotation>> immutableAnnotations = new Cache<CsdlSemanticsElement, IEnumerable<IEdmAnnotation>>();
        private readonly static Func<CsdlSemanticsElement, IEnumerable<IEdmAnnotation>> s_computeImmutableAnnotations = (me) => me.ComputeImmutableAnnotations();

        public abstract CsdlSemanticsModel Model { get; }
        public abstract CsdlElement Element { get; }

        private IEnumerable<IEdmAnnotation> ImmutableAnnotations
        {
            get { return this.immutableAnnotations.GetValue(this, s_computeImmutableAnnotations, null); }
        }

        protected virtual IEnumerable<IEdmAnnotation> ComputeImmutableAnnotations()
        {
            return this.Model.WrapAnnotations(this, null);
        }

        IEnumerable<IEdmAnnotation> IEdmAnnotatable.Annotations
        {
            get { return this.Model.AnnotationsManager.Annotations(this.ImmutableAnnotations, this.transientAnnotations); }
        }

        object IEdmAnnotatable.GetAnnotation(string namespaceName, string localName)
        {
            return AnnotationsManager.GetAnnotation(this.ImmutableAnnotations, this.transientAnnotations, namespaceName, localName);
        }

        void IEdmAnnotatable.SetAnnotation(string namespaceName, string localName, object value)
        {
            this.Model.AnnotationsManager.SetAnnotation(this.ImmutableAnnotations, ref this.transientAnnotations, namespaceName, localName, value);
        }

        public EdmLocation Location
        {
            get { return this.Element.Location; }
        }
    }
}
