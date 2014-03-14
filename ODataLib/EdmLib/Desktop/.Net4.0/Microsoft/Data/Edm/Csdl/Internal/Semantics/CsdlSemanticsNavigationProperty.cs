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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Internal;
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
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmAssociation> associationCache = new Cache<CsdlSemanticsNavigationProperty, IEdmAssociation>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmAssociation> ComputeAssociationFunc = (me) => me.ComputeAssociation();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> toCache = new Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> ComputeToFunc = (me) => me.ComputeTo();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> fromCache = new Cache<CsdlSemanticsNavigationProperty, IEdmAssociationEnd>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmAssociationEnd> ComputeFromFunc = (me) => me.ComputeFrom();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> partnerCache = new Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> ComputePartnerFunc = (me) => me.ComputePartner();

        public CsdlSemanticsNavigationProperty(CsdlSemanticsEntityTypeDefinition declaringType, CsdlNavigationProperty navigationProperty)
            : base(navigationProperty)
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

        public bool IsPrincipal
        {
            get
            {
                CsdlSemanticsReferentialConstraint referentialConstraint = this.Association.ReferentialConstraint;
                return referentialConstraint != null && referentialConstraint.PrincipalEnd != this.To;
            }
        }

        public EdmOnDeleteAction OnDelete
        {
            get
            {
                IEdmAssociationEnd from = this.From;
                return from != null ? from.OnDelete : EdmOnDeleteAction.None;
            }
        }

        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmAssociationEnd To
        {
            get
            {
                return this.toCache.GetValue(this, ComputeToFunc, null);
            }
        }

        public bool ContainsTarget
        {
            get { return this.navigationProperty.ContainsTarget; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        public IEdmNavigationProperty Partner
        {
            get { return this.partnerCache.GetValue(this, ComputePartnerFunc, null); }
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

                if (this.From is BadCsdlSemanticsNavigationPropertyToEnd)
                {
                    errors.AddRange(this.From.Errors());
                }

                if (this.To is BadCsdlSemanticsNavigationPropertyToEnd)
                {
                    errors.AddRange(this.To.Errors());
                }

                return errors;
            }
        }

        public IEdmAssociation Association
        {
            get
            {
                return this.associationCache.GetValue(this, ComputeAssociationFunc, null);
            }
        }

        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get
            {
                CsdlSemanticsReferentialConstraint referentialConstraint = this.Association.ReferentialConstraint;
                return (referentialConstraint != null && referentialConstraint.PrincipalEnd == this.To) ? referentialConstraint.DependentProperties : null;
            }
        }

        private IEdmAssociationEnd From
        {
            get
            {
                return this.fromCache.GetValue(this, ComputeFromFunc, null);
            }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringType.Context);
        }

        private IEdmAssociation ComputeAssociation()
        {
            IEdmAssociation association = this.declaringType.Context.FindAssociation(this.navigationProperty.Relationship) ?? new UnresolvedAssociation(this.navigationProperty.Relationship, this.Location);

            this.Model.SetAssociationNamespace(this, association.Namespace);
            this.Model.SetAssociationName(this, association.Name);
            CsdlSemanticsAssociation csdlAssociation = association as CsdlSemanticsAssociation;
            CsdlSemanticsAssociationEnd end1 = association.End1 as CsdlSemanticsAssociationEnd;
            CsdlSemanticsAssociationEnd end2 = association.End2 as CsdlSemanticsAssociationEnd;
            if (csdlAssociation != null && end1 != null && end2 != null)
            {
                this.Model.SetAssociationAnnotations(
                    this,
                    csdlAssociation.DirectValueAnnotations,
                    (this.navigationProperty.FromRole == end1.Name ? end1 : end2).DirectValueAnnotations,
                    (this.navigationProperty.FromRole == end1.Name ? end2 : end1).DirectValueAnnotations,
                    association.ReferentialConstraint != null ? association.ReferentialConstraint.DirectValueAnnotations : Enumerable.Empty<IEdmDirectValueAnnotation>());
            }

            return association;
        }

        private IEdmAssociationEnd ComputeFrom()
        {
            IEdmAssociation association = this.Association;
            string fromRole = this.navigationProperty.FromRole;
            if (association.End1.Name == fromRole)
            {
                return association.End1;
            }
            else if (association.End2.Name == fromRole)
            {
                return association.End2;
            }
            else
            {
                return new BadCsdlSemanticsNavigationPropertyToEnd(
                    this.Association,
                    fromRole,
                    new EdmError[]
                    {
                        new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(this.Name, fromRole, association.Name))
                    });
            }
        }

        private IEdmAssociationEnd ComputeTo()
        {
            string toRole = this.navigationProperty.ToRole;
            string fromRole = this.navigationProperty.FromRole;
            this.Model.SetAssociationEndName(this, fromRole);

            IEdmAssociation association = this.Association;

            if (toRole == fromRole)
            {
                return new BadCsdlSemanticsNavigationPropertyToEnd(association, toRole, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame(this.Name)) });
            }

            if (association.End1.Name == toRole)
            {
                return association.End1;
            }
            else if (association.End2.Name == toRole)
            {
                return association.End2;
            }
            else
            {
                return new BadCsdlSemanticsNavigationPropertyToEnd(
                    this.Association,
                    toRole,
                    new EdmError[]
                    {
                        new EdmError(this.Location, EdmErrorCode.BadNavigationProperty, Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(this.Name, toRole, association.Name))
                    });
            }
        }

        private IEdmNavigationProperty ComputePartner()
        {
            // If multiple navigation properties share an association, pair the first with the first, second with second and so forth.
            int propertyIndex = 0;
            foreach (IEdmNavigationProperty navigationProperty in this.declaringType.NavigationProperties())
            {
                if (navigationProperty == this)
                {
                    break;
                }

                CsdlSemanticsNavigationProperty csdlNavigationProperty = navigationProperty as CsdlSemanticsNavigationProperty;
                if (csdlNavigationProperty != null)
                {
                    if (csdlNavigationProperty.Association == this.Association && csdlNavigationProperty.To == this.To)
                    {
                        propertyIndex++;
                    }
                }
            }

            foreach (IEdmNavigationProperty candidate in this.To.EntityType.NavigationProperties())
            {
                CsdlSemanticsNavigationProperty csdlCandidate = candidate as CsdlSemanticsNavigationProperty;
                if (csdlCandidate != null)
                {
                    if (csdlCandidate.Association == this.Association && csdlCandidate.To == this.From)
                    {
                        if (propertyIndex == 0)
                        {
                            return candidate;
                        }
                        else
                        {
                            propertyIndex--;
                        }
                    }
                }
                else if (candidate.Partner == this)
                {
                    return candidate;
                }
            }

            return new SilentPartner(this);
        }

        private IEdmTypeReference ComputeType()
        {
            switch (this.To.Multiplicity)
            {
                case EdmMultiplicity.One:
                    return new EdmEntityTypeReference(this.To.EntityType, false);
                case EdmMultiplicity.ZeroOrOne:
                    return new EdmEntityTypeReference(this.To.EntityType, true);
                case EdmMultiplicity.Many:
                    return new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.To.EntityType, false)), false);
                default:
                    // Error is bad by association because the association has to be invalid or non-existent for the error to be produced.
                    return new BadEntityTypeReference(this.To.EntityType.FullName(), false, new EdmError[] { new EdmError(this.Location, EdmErrorCode.NavigationPropertyTypeInvalidBecauseOfBadAssociation, Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(this.To.EntityType.Name)) });
            }
        }

        private class BadCsdlSemanticsNavigationPropertyToEnd : BadAssociationEnd
        {
            public BadCsdlSemanticsNavigationPropertyToEnd(IEdmAssociation declaringAssociation, string role, IEnumerable<EdmError> errors)
            : base(declaringAssociation, role, errors)
            {
            }
        }

        /// <summary>
        /// Represents a navigation property synthesized for an association end that does not have a corresponding navigation property.
        /// </summary>
        private class SilentPartner : EdmElement, IEdmNavigationProperty
        {
            private readonly CsdlSemanticsNavigationProperty partner;

            private readonly Cache<SilentPartner, IEdmTypeReference> typeCache = new Cache<SilentPartner, IEdmTypeReference>();
            private static readonly Func<SilentPartner, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

            public SilentPartner(CsdlSemanticsNavigationProperty partner)
            {
                this.partner = partner;

                partner.Model.SetAssociationNamespace(this, partner.Association.Namespace);
                partner.Model.SetAssociationName(this, partner.Association.Name);
                partner.Model.SetAssociationEndName(this, partner.To.Name);
            }

            public IEdmNavigationProperty Partner
            {
                get { return this.partner; }
            }

            public EdmOnDeleteAction OnDelete
            {
                get { return this.partner.To.OnDelete; }
            }

            public bool IsPrincipal
            {
                get 
                { 
                    CsdlSemanticsReferentialConstraint constraint = this.partner.Association.ReferentialConstraint;
                    return constraint != null && constraint.PrincipalEnd == this.partner.To;
                }
            }

            public bool ContainsTarget
            {
                get { return false; }
            }

            public IEnumerable<IEdmStructuralProperty> DependentProperties
            {
                get
                {
                    CsdlSemanticsReferentialConstraint constraint = this.partner.Association.ReferentialConstraint;
                    return constraint != null && !this.IsPrincipal ? constraint.DependentProperties : null;
                }
            }

            public EdmPropertyKind PropertyKind
            {
                get { return EdmPropertyKind.Navigation; }
            }

            public IEdmTypeReference Type
            {
                get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
            }

            public IEdmStructuredType DeclaringType
            {
                get { return this.partner.ToEntityType(); }
            }

            public string Name
            {
                get { return this.partner.From.Name; }
            }

            private IEdmTypeReference ComputeType()
            {
                switch (this.partner.From.Multiplicity)
                {
                    case EdmMultiplicity.One:
                        return new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false);
                    case EdmMultiplicity.ZeroOrOne:
                        return new EdmEntityTypeReference(this.partner.DeclaringEntityType(), true);
                    case EdmMultiplicity.Many:
                        return new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false)), false);
                    default:
                        // Error is bad by association because the association has to be invalid or non-existent for the error to be produced.
                        return new BadEntityTypeReference(this.partner.DeclaringEntityType().FullName(), false, new EdmError[] { new EdmError(this.partner.To.Location(), EdmErrorCode.NavigationPropertyTypeInvalidBecauseOfBadAssociation, Edm.Strings.EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(this.partner.DeclaringEntityType().Name)) });
                }
            }
        }
    }
}
