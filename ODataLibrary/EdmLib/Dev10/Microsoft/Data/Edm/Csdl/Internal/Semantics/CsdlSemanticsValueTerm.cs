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
    /// Provides semantics for a CsdlValueTerm.
    /// </summary>
    internal class CsdlSemanticsValueTerm : CsdlSemanticsElement, IEdmValueTerm
    {
        protected readonly CsdlSemanticsSchema context;
        protected CsdlValueTerm valueTerm;

        private readonly Cache<CsdlSemanticsValueTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsValueTerm, IEdmTypeReference>();
        private readonly static Func<CsdlSemanticsValueTerm, IEdmTypeReference> s_computeType = (me) => me.ComputeType();

        public CsdlSemanticsValueTerm(CsdlSemanticsSchema context, CsdlValueTerm valueTerm)
        {
            this.context = context;
            this.valueTerm = valueTerm;
        }

        public string Name
        {
            get { return this.valueTerm.Name; }
        }

        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        string IEdmTerm.NamespaceUri
        {
            get { return this.context.NamespaceUri; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, s_computeType, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.context, this.valueTerm.Type);
        }

        public override CsdlElement Element
        {
            get { return this.valueTerm; }
        }

        protected override IEnumerable<IEdmAnnotation> ComputeImmutableAnnotations()
        {
            return this.Model.WrapAnnotations(this, this.context);
        }
    }
}
