//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlTerm.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("CsdlSemanticsValueTerm({Name})")]
    internal class CsdlSemanticsValueTerm : CsdlSemanticsElement, IEdmValueTerm
    {
        protected readonly CsdlSemanticsSchema Context;
        protected CsdlTerm valueTerm;

        private readonly Cache<CsdlSemanticsValueTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsValueTerm, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsValueTerm, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsValueTerm(CsdlSemanticsSchema context, CsdlTerm valueTerm)
            : base(valueTerm)
        {
            this.Context = context;
            this.valueTerm = valueTerm;
        }

        public string Name
        {
            get { return this.valueTerm.Name; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
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
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public string AppliesTo
        {
            get { return this.valueTerm.AppliesTo; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.valueTerm; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.valueTerm.Type);
        }
    }
}
