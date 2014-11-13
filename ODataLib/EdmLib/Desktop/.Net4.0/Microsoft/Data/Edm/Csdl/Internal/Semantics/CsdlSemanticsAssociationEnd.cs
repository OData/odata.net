//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
