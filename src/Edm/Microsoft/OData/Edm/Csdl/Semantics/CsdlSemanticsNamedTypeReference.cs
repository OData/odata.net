//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlNamedTypeReference.
    /// </summary>
    internal class CsdlSemanticsNamedTypeReference : CsdlSemanticsElement, IEdmTypeReference
    {
        private readonly CsdlSemanticsSchema schema;
        private readonly CsdlNamedTypeReference reference;

        private readonly Cache<CsdlSemanticsNamedTypeReference, IEdmType> definitionCache = new Cache<CsdlSemanticsNamedTypeReference, IEdmType>();
        private static readonly Func<CsdlSemanticsNamedTypeReference, IEdmType> ComputeDefinitionFunc = (me) => me.ComputeDefinition();

        public CsdlSemanticsNamedTypeReference(CsdlSemanticsSchema schema, CsdlNamedTypeReference reference)
            : base(reference)
        {
            this.schema = schema;
            this.reference = reference;
        }

        public IEdmType Definition
        {
            get { return this.definitionCache.GetValue(this, ComputeDefinitionFunc, null); }
        }

        public bool IsNullable
        {
            get { return this.reference.IsNullable; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.reference; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }

        private IEdmType ComputeDefinition()
        {
            IEdmType binding = this.schema.FindType(this.reference.FullName);

            return binding ?? new UnresolvedType(this.schema.ReplaceAlias(this.reference.FullName) ?? this.reference.FullName, this.Location);
        }
    }
}
