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
    /// Provides semantics for a CsdlNavigationProperty.
    /// </summary>
    internal class CsdlSemanticsNavigationProperty : CsdlSemanticsElement, IEdmNavigationProperty, IEdmCheckable
    {
        private readonly CsdlNavigationProperty navigationProperty;
        private readonly CsdlSemanticsEntityTypeDefinition declaringType;

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsNavigationProperty, IEdmTypeReference>();
        private readonly static Func<CsdlSemanticsNavigationProperty, IEdmTypeReference> s_computeType = (me) => me.ComputeType();

       private readonly Cache<CsdlSemanticsNavigationProperty, IEdmAssociation> associationTypeCache = new Cache<CsdlSemanticsNavigationProperty, IEdmAssociation>();
        private readonly static Func<CsdlSemanticsNavigationProperty, IEdmAssociation> s_computeAssociationType = (me) => me.ComputeAssociationType();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> toCache = new Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd>();
        private readonly static Func<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> s_computeTo = (me) => me.ComputeTo();

        public CsdlSemanticsNavigationProperty(CsdlSemanticsEntityTypeDefinition declaringType, CsdlNavigationProperty navigationProperty)
        {
            this.declaringType = declaringType;
            this.navigationProperty = navigationProperty;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringType.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.navigationProperty; }
        }

        public string Name
        {
            get { return this.navigationProperty.Name; }
        }

        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        private IEdmAssociation ComputeAssociationType()
        {
            return this.declaringType.Context.FindAssociation(this.navigationProperty.Relationship) as IEdmAssociation ?? new UnresolvedAssociation(this.navigationProperty.Relationship, this.Location);
        }

        private IEdmAssociation Association
        {
            get
            {
                return this.associationTypeCache.GetValue(this, s_computeAssociationType, null);
            }
        }

        private IEdmAssociationEnd ComputeTo()
        {
            if (this.navigationProperty.ToRole == this.navigationProperty.FromRole)
            {
                return new BadCsdlSemanticsNavigationPropertyToEnd(this.Association, this.navigationProperty.ToRole, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame) });
            }

            if (this.navigationProperty.FromRole != this.Association.End1.Name && this.navigationProperty.FromRole != this.Association.End2.Name)
            {
                return new BadCsdlSemanticsNavigationPropertyToEnd(this.Association, this.navigationProperty.ToRole, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(this.navigationProperty.FromRole, this.Association.Name)) });
            }

            IEdmAssociationEnd returnedEnd = this.Association.End1;
            if (returnedEnd.Name != this.navigationProperty.ToRole)
            {
                returnedEnd = this.Association.End2;
            }

            if (returnedEnd.Name != this.navigationProperty.ToRole)
            {
                returnedEnd = new BadCsdlSemanticsNavigationPropertyToEnd(this.Association, this.navigationProperty.ToRole, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(this.navigationProperty.ToRole, this.Association.Name)) });
            }

            return returnedEnd;
        }

        public IEdmAssociationEnd To
        {
            get
            {
                return this.toCache.GetValue(this, s_computeTo, null);
            }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, s_computeType, null); }
        }

        private IEdmTypeReference ComputeType()
        {
            switch (this.To.Multiplicity)
            {
                case EdmAssociationMultiplicity.One:
                    return new EdmEntityTypeReference(this.To.EntityType, false);
                case EdmAssociationMultiplicity.ZeroOrOne:
                    return new EdmEntityTypeReference(this.To.EntityType, true);
                case EdmAssociationMultiplicity.Many:
                    return new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.To.EntityType, false), CsdlConstants.Collection_IsAtomic), false);
                default:
                    // Error is bad by association because the association has to be invalid or non-existent for the error to be produced.
                    return new BadEntityTypeReference(this.To.EntityType.Name, false, new EdmError[]{new EdmError(this.Location, EdmErrorCode.NavigationPropertyTypeInvalidBecauseOfBadAssociation, Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(this.To.EntityType.Name))});
            }
        }

        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                List<EdmError> errors = new List<EdmError>();
                if (this.Association is UnresolvedAssociation)
                {
                    errors.AddRange(this.Association.Errors());
                }

                if (this.To is BadCsdlSemanticsNavigationPropertyToEnd)
                {
                    errors.AddRange(this.To.Errors());
                }

                return errors;
            }
        }

        private class BadCsdlSemanticsNavigationPropertyToEnd : BadAssociationEnd
        {
            public BadCsdlSemanticsNavigationPropertyToEnd(IEdmAssociation declaringAssociation, string role, IEnumerable<EdmError> errors)
            : base(declaringAssociation, role, errors)
            {
            }
        }
    }
}
