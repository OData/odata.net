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
    /// Provides semantics for a CsdlAssociation.
    /// </summary>
    internal class CsdlSemanticsAssociation : CsdlSemanticsElement, IEdmAssociation, IEdmCheckable
    {
        private readonly CsdlAssociation association;
        private readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsAssociation, CsdlSemanticsReferentialConstraint> referentialConstraintCache = new Cache<CsdlSemanticsAssociation, CsdlSemanticsReferentialConstraint>();
        private static readonly Func<CsdlSemanticsAssociation, CsdlSemanticsReferentialConstraint> ComputeReferentialConstraintFunc = (me) => me.ComputeReferentialConstraint();

        private readonly Cache<CsdlSemanticsAssociation, TupleInternal<IEdmAssociationEnd, IEdmAssociationEnd>> endsCache = new Cache<CsdlSemanticsAssociation, TupleInternal<IEdmAssociationEnd, IEdmAssociationEnd>>();
        private static readonly Func<CsdlSemanticsAssociation, TupleInternal<IEdmAssociationEnd, IEdmAssociationEnd>> ComputeEndsFunc = (me) => me.ComputeEnds();

        private readonly Cache<CsdlSemanticsAssociation, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsAssociation, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsAssociation, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsAssociation(CsdlSemanticsSchema context, CsdlAssociation association)
            : base(association)
        {
            this.association = association;
            this.context = context;
        }

        public string Namespace 
        { 
            get 
            { 
                return this.context.Namespace; 
            } 
        }

        public string Name
        {
            get { return this.association.Name; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.association; }
        }

        public IEdmAssociationEnd End1
        {
            get { return this.endsCache.GetValue(this, ComputeEndsFunc, null).Item1; }
        }

        public IEdmAssociationEnd End2
        {
            get { return this.endsCache.GetValue(this, ComputeEndsFunc, null).Item2; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        public CsdlSemanticsReferentialConstraint ReferentialConstraint
        {
            get { return this.referentialConstraintCache.GetValue(this, ComputeReferentialConstraintFunc, null); }
        }

        private TupleInternal<IEdmAssociationEnd, IEdmAssociationEnd> ComputeEnds()
        {
            return TupleInternal.Create(
                (this.association.End1 != null) ? new CsdlSemanticsAssociationEnd(this.context, this, this.association.End1) : (IEdmAssociationEnd)new BadAssociationEnd(this, "End1", new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidAssociation, Edm.Strings.CsdlParser_InvalidAssociationIncorrectNumberOfEnds(this.Namespace + "." + this.Name)) }),
                (this.association.End2 != null) ? new CsdlSemanticsAssociationEnd(this.context, this, this.association.End2) : (IEdmAssociationEnd)new BadAssociationEnd(this, "End2", new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidAssociation, Edm.Strings.CsdlParser_InvalidAssociationIncorrectNumberOfEnds(this.Namespace + "." + this.Name)) }));
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = null;

            if (this.association.End1.Name == this.association.End2.Name)
            {
                errors = AllocateAndAdd(
                    errors,
                    new EdmError(
                        this.association.End2.Location ?? this.Location,
                        EdmErrorCode.AlreadyDefined,
                        Strings.EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(this.association.End1.Name)));
            }

            return errors ?? Enumerable.Empty<EdmError>();
        }

        private CsdlSemanticsReferentialConstraint ComputeReferentialConstraint()
        {
            return this.association.Constraint != null ? new CsdlSemanticsReferentialConstraint(this, this.association.Constraint) : null;
        }
    }
}
