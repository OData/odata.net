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
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlNamedTypeReference.
    /// </summary>
    internal class CsdlSemanticsNamedTypeReference : CsdlSemanticsElement, IEdmTypeReference, IEdmCheckable
    {
        private readonly CsdlSemanticsSchema schema;
        private readonly CsdlNamedTypeReference reference;

        private readonly Cache<CsdlSemanticsNamedTypeReference, IEdmType> definitionCache = new Cache<CsdlSemanticsNamedTypeReference, IEdmType>();
        private readonly static Func<CsdlSemanticsNamedTypeReference, IEdmType> s_computeDefinition = (me) => me.ComputeDefinition();

        public CsdlSemanticsNamedTypeReference(CsdlSemanticsSchema schema, CsdlNamedTypeReference reference)
        {
            this.schema = schema;
            this.reference = reference;
        }

        public IEdmType Definition
        {
            get { return this.definitionCache.GetValue(this, s_computeDefinition, null); }
        }

        private IEdmType ComputeDefinition()
        {
            IEdmType binding = this.schema.FindType(this.reference.FullName);

            return binding ?? new UnresolvedType(this.reference.FullName, this.reference.Location);
        }

        public bool IsNullable
        {
            get { return this.reference.IsNullable; }
        }

        public override string ToString()
        {
            return this.ToTraceString();
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.reference; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.Definition is UnresolvedType)
                {
                    return this.Definition.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
