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
    /// Provides semantics for a CsdlAssociationSet.
    /// </summary>
    internal class CsdlSemanticsAssociationSet : CsdlSemanticsElement, IEdmAssociationSet, IEdmCheckable
    {
        private readonly CsdlSemanticsEntityContainer context;
        private readonly CsdlAssociationSet associationSet;

        private readonly Cache<CsdlSemanticsAssociationSet, TupleInternal<CsdlSemanticsAssociationSetEnd, CsdlSemanticsAssociationSetEnd>> endsCache = new Cache<CsdlSemanticsAssociationSet, TupleInternal<CsdlSemanticsAssociationSetEnd, CsdlSemanticsAssociationSetEnd>>();
        private static readonly Func<CsdlSemanticsAssociationSet, TupleInternal<CsdlSemanticsAssociationSetEnd, CsdlSemanticsAssociationSetEnd>> ComputeEndsFunc = (me) => me.ComputeEnds();

        private readonly Cache<CsdlSemanticsAssociationSet, IEdmAssociation> elementTypeCache = new Cache<CsdlSemanticsAssociationSet, IEdmAssociation>();
        private static readonly Func<CsdlSemanticsAssociationSet, IEdmAssociation> ComputeElementTypeFunc = (me) => me.ComputeElementType();

        private readonly Cache<CsdlSemanticsAssociationSet, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsAssociationSet, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsAssociationSet, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsAssociationSet(CsdlSemanticsEntityContainer context, CsdlAssociationSet associationSet)
            : base(associationSet)
        {
            this.context = context;
            this.associationSet = associationSet;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public IEdmEntityContainer Container
        {
            get { return this.context; }
        }

        public override CsdlElement Element
        {
            get { return this.associationSet; }
        }

        public string Name
        {
            get { return this.associationSet.Name; }
        }

        public IEdmAssociation Association
        {
            get { return this.elementTypeCache.GetValue(this, ComputeElementTypeFunc, null); }
        }

        public IEdmAssociationSetEnd End1
        {
            get { return this.Ends.Item1; }
        }

        public IEdmAssociationSetEnd End2
        {
            get { return this.Ends.Item2; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private TupleInternal<CsdlSemanticsAssociationSetEnd, CsdlSemanticsAssociationSetEnd> Ends
        {
             get { return this.endsCache.GetValue(this, ComputeEndsFunc, null); }
        }

        private IEdmAssociation ComputeElementType()
        {
            return this.context.Context.FindAssociation(this.associationSet.Association) ?? new UnresolvedAssociation(this.associationSet.Association, this.Location);
        }

        private IEdmAssociationEnd GetRole(CsdlAssociationSetEnd end)
        {
            Func<IEdmAssociationEnd, bool> match = (endCandidate) => endCandidate != null && endCandidate.Name == end.Role;
            if (match(this.Association.End1))
            {
                return this.Association.End1;
            }
            else if (match(this.Association.End2))
            {
                return this.Association.End2;
            }
            else
            {
                return new UnresolvedAssociationEnd(this.Association, end.Role, end.Location);
            }
        }

        private TupleInternal<CsdlSemanticsAssociationSetEnd, CsdlSemanticsAssociationSetEnd> ComputeEnds()
        {
            CsdlAssociationSetEnd setEnd1 = this.associationSet.End1;
            CsdlAssociationSetEnd setEnd2 = this.associationSet.End2;
            IEdmAssociationEnd end1Role = null;
            IEdmAssociationEnd end2Role = null;
            bool end1Bad = false;
            bool end2Bad = false;
            if (setEnd1 != null)
            {
                end1Role = this.GetRole(setEnd1);
                end1Bad = end1Role is IUnresolvedElement;
            }

            if (setEnd2 != null)
            {
                end2Role = this.GetRole(setEnd2);
                end2Bad = end2Role is IUnresolvedElement;
            }

            // This is not an else statement above because the logic here depends on end2 having been calculated.
            if (setEnd1 == null)
            {
                if (end2Bad)
                {
                    end1Role = new UnresolvedAssociationEnd(this.Association, "End1", this.Location);
                    end1Bad = true;
                }
                else
                {
                    if (end2Role != null)
                    {
                        end1Role = (end2Role != this.Association.End1) ? this.Association.End1 : this.Association.End2;
                    }
                    else
                    {
                        end1Role = this.Association.End1;
                    }
                }
            }

            // This is not an else statement above because the logic here depends on end1 having been calculated.
            if (setEnd2 == null)
            {
                if (end1Bad)
                {
                    end2Role = new UnresolvedAssociationEnd(this.Association, "End2", this.Location);
                    end2Bad = true;
                }
                else
                {
                    end2Role = (end1Role != this.Association.End1) ? this.Association.End1 : this.Association.End2;
                }
            }

            return TupleInternal.Create(
                new CsdlSemanticsAssociationSetEnd(this, this.associationSet.End1, end1Role),
                new CsdlSemanticsAssociationSetEnd(this, this.associationSet.End2, end2Role));
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = null;

            if (this.Association is UnresolvedAssociation)
            {
                errors = AllocateAndAdd(errors, this.Association.Errors());
            }

            // Validate that the association set does not have duplicate ends.
            if (this.End1.Role != null && this.End2.Role != null && this.End1.Role.Name == this.End2.Role.Name)
            {
                errors = AllocateAndAdd(
                    errors,
                    new EdmError(
                        this.End2.Location(),
                        EdmErrorCode.InvalidName,
                        Strings.EdmModel_Validator_Semantic_DuplicateEndName(this.End1.Role.Name)));
            }

            return errors ?? Enumerable.Empty<EdmError>();
        }
    }
}
