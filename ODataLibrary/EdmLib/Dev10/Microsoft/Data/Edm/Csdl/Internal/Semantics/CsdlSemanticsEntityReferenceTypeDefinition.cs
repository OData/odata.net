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
    /// Provides semantics for CsdlEntityTypeReference.
    /// </summary>
    internal class CsdlSemanticsEntityReferenceTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEntityReferenceType, IEdmCheckable
    {
        private readonly CsdlSemanticsSchema schema;

        private readonly Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> entityTypeCache = new Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>();
        private readonly static Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> s_computeEntityType = (me) => me.ComputeEntityType();

        private readonly CsdlEntityReferenceType entityTypeReference;

        public CsdlSemanticsEntityReferenceTypeDefinition(CsdlSemanticsSchema schema, CsdlEntityReferenceType entityTypeReference)
        {
            this.schema = schema;
            this.entityTypeReference = entityTypeReference;
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.EntityReference; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.entityTypeCache.GetValue(this, s_computeEntityType, null); }
        }

        private IEdmEntityType ComputeEntityType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.schema, this.entityTypeReference.EntityType);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(type.FullName(), this.Location);
        }

        public override CsdlElement Element
        {
            get { return this.entityTypeReference; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEnumerable<EdmError> Errors
        {
            get 
            {
                return this.EntityType is UnresolvedEntityType 
                    ? this.EntityType.Errors() 
                    : Enumerable.Empty<EdmError>();
            }
        }
    }
}
