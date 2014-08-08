//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
        private static readonly Func<CsdlSemanticsAssociationEnd, IEdmEntityType> ComputeTypeFunc = (me) => me.ComputeType();

        private readonly Cache<CsdlSemanticsAssociationEnd, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsAssociationEnd, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsAssociationEnd, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsAssociationEnd(CsdlSemanticsSchema context, CsdlSemanticsAssociation association, CsdlAssociationEnd end)
            : base(end)
        {
            this.end = end;
            this.definingAssociation = association;
            this.context = context;
        }

        public EdmMultiplicity Multiplicity
        {
            get { return this.end.Multiplicity; }
        }

        public EdmOnDeleteAction OnDelete
        {
            get { return (this.end.OnDelete != null) ? this.end.OnDelete.Action : EdmOnDeleteAction.None; }
        }

        public IEdmAssociation DeclaringAssociation
        {
            get { return this.definingAssociation; }
        }

        public IEdmEntityType EntityType
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
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
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.end; }
        }

        private IEdmEntityType ComputeType()
        {
            IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.context, this.end.Type);
            return type.TypeKind() == EdmTypeKind.Entity ? type.AsEntity().EntityDefinition() : new UnresolvedEntityType(type.FullName(), this.Location);
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = null;

            if (this.EntityType is UnresolvedEntityType)
            {
                errors =
                    AllocateAndAdd(
                        errors,
                        this.EntityType.Errors());
            }

            return errors ?? Enumerable.Empty<EdmError>();
        }
    }
}
