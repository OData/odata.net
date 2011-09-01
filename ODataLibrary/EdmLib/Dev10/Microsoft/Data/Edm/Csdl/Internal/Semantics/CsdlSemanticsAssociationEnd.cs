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
    /// Provides semantics for a CsdlAssociationEnd.
    /// </summary>
    internal class CsdlSemanticsAssociationEnd : CsdlSemanticsElement, IEdmAssociationEnd, IEdmCheckable
    {
        private readonly CsdlAssociationEnd end;
        private readonly CsdlSemanticsAssociation definingAssociation;
        private readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsAssociationEnd, IEdmEntityType> typeCache = new Cache<CsdlSemanticsAssociationEnd, IEdmEntityType>();
        private readonly static Func<CsdlSemanticsAssociationEnd, IEdmEntityType> s_computeType = (me) => me.ComputeType();

        public CsdlSemanticsAssociationEnd(CsdlSemanticsSchema context, CsdlSemanticsAssociation association, CsdlAssociationEnd end)
        {
            this.end = end;
            this.definingAssociation = association;
            this.context = context;
        }

        public EdmAssociationMultiplicity Multiplicity
        {
            get { return this.end.Multiplicity; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return (this.end.OnDelete != null)? this.end.OnDelete.Action : EdmOnDeleteAction.None; }
        }

        public IEdmAssociation DeclaringAssociation
        {
            get { return this.definingAssociation; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.typeCache.GetValue(this, s_computeType, null); }
        }

        private IEdmEntityType ComputeType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.context, this.end.Type);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(type.FullName(), this.Location);
        }

        public string Name
        {
            get { return this.end.Name ?? string.Empty; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.EntityType is UnresolvedEntityType)
                {
                    return this.EntityType.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        public override CsdlElement Element
        {
            get { return this.end; }
        }
    }
}
