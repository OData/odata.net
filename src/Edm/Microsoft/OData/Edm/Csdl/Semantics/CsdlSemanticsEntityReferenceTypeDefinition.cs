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
    /// Provides semantics for CsdlEntityTypeReference.
    /// </summary>
    internal class CsdlSemanticsEntityReferenceTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEntityReferenceType
    {
        private readonly CsdlSemanticsSchema schema;

        private readonly Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> entityTypeCache = new Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> ComputeEntityTypeFunc = (me) => me.ComputeEntityType();

        private readonly CsdlEntityReferenceType entityTypeReference;

        public CsdlSemanticsEntityReferenceTypeDefinition(CsdlSemanticsSchema schema, CsdlEntityReferenceType entityTypeReference)
            : base(entityTypeReference)
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
            get { return this.entityTypeCache.GetValue(this, ComputeEntityTypeFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.entityTypeReference; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        private IEdmEntityType ComputeEntityType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.schema, this.entityTypeReference.EntityType);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(this.schema.UnresolvedName(type.FullName()), this.Location);
        }
    }
}
