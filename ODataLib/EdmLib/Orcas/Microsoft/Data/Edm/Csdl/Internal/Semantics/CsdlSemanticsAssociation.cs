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
