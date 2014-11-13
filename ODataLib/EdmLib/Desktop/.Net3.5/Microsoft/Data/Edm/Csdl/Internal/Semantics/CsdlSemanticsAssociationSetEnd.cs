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
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlAssociationSetEnd.
    /// </summary>
    internal class CsdlSemanticsAssociationSetEnd : CsdlSemanticsElement, IEdmAssociationSetEnd, IEdmCheckable
    {
        private readonly CsdlSemanticsAssociationSet context;
        private readonly CsdlAssociationSetEnd end;
        private readonly IEdmAssociationEnd role;

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> entitySet = new Cache<CsdlSemanticsAssociationSetEnd, IEdmEntitySet>();
        private static readonly Func<CsdlSemanticsAssociationSetEnd, IEdmEntitySet> ComputeEntitySetFunc = (me) => me.ComputeEntitySet();

        private readonly Cache<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsAssociationSetEnd, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsAssociationSetEnd(CsdlSemanticsAssociationSet context, CsdlAssociationSetEnd end, IEdmAssociationEnd role)
            : base(end)
        {
            this.context = context;
            this.end = end;
            this.role = role;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return (CsdlElement)this.end; }
        }

        public IEdmAssociationEnd Role
        {
            get
            {
                return this.role;
            }
        }

        public IEdmEntitySet EntitySet
        {
            get
            {
                return this.entitySet.GetValue(this, ComputeEntitySetFunc, null);
            }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private IEdmEntitySet ComputeEntitySet()
        {
            if (this.end != null)
            {
                return this.context.Container.FindEntitySet(this.end.EntitySet) ?? new UnresolvedEntitySet(this.end.EntitySet, this.context.Container, this.Location);
            }
            else
            {
                IEnumerable<EdmError> errors = new EdmError[]
                {
                    new EdmError(
                        this.Location,
                        EdmErrorCode.NoEntitySetsFoundForType,
                        Strings.EdmModel_Validator_Semantic_NoEntitySetsFoundForType(this.context.Container.FullName() + this.context.Name, this.role.EntityType.FullName(), this.Role.Name))
                };
                return this.context.Container.EntitySets().Where(set => set.ElementType == this.role.EntityType).FirstOrDefault() ?? new BadEntitySet("UnresolvedEntitySet", this.context.Container, errors);
            }
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = new List<EdmError>();
            if (this.Role is UnresolvedAssociationEnd)
            {
                errors.AddRange(this.Role.Errors());
            }

            if (this.EntitySet is UnresolvedEntitySet)
            {
                errors.AddRange(this.EntitySet.Errors());
            }

            if (this.end == null)
            {
                if (this.context.Container.EntitySets().Where(set => set.ElementType == this.role.EntityType).Count() > 1)
                {
                    errors.Add(new EdmError(
                       this.Location,
                       EdmErrorCode.CannotInferEntitySetWithMultipleSetsPerType,
                       Strings.EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType(this.context.Container.FullName() + this.context.Name, this.role.EntityType.FullName(), this.Role.Name)));
                }
            }

            return errors;
        }
    }
}
