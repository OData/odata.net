//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> partnerCache = new Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> ComputePartnerFunc = (me) => me.ComputePartner();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint> referentialConstraintCache = new Cache<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint> ComputeReferentialConstraintFunc = (me) => me.ComputeReferentialConstraint();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEdmEntityType> targetEntityTypeCache = new Cache<CsdlSemanticsNavigationProperty, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEdmEntityType> ComputeTargetEntityTypeFunc = (me) => me.ComputeTargetEntityType();

        private readonly Cache<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

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

        public EdmOnDeleteAction OnDelete
        {
            get
            {
                return (this.navigationProperty.OnDelete != null) ? this.navigationProperty.OnDelete.Action : EdmOnDeleteAction.None;
            }
        }

        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
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
            get { return this.partnerCache.GetValue(this, ComputePartnerFunc, cycle => null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        public IEdmReferentialConstraint ReferentialConstraint
        {
            get { return this.referentialConstraintCache.GetValue(this, ComputeReferentialConstraintFunc, null); }
        }

        private IEdmEntityType TargetEntityType
        {
            get { return this.targetEntityTypeCache.GetValue(this, ComputeTargetEntityTypeFunc, null); }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringType.Context);
        }

        private IEdmEntityType ComputeTargetEntityType()
        {
            IEdmType target = this.Type.Definition;
            if (target.TypeKind == EdmTypeKind.Collection)
            {
                target = ((IEdmCollectionType)target).ElementType.Definition;
            }

            return (IEdmEntityType)target;
        }

        private IEdmNavigationProperty ComputePartner()
        {
            string partnerPropertyName = this.navigationProperty.Partner;

            IEdmEntityType targetEntityType = this.TargetEntityType;
            if (partnerPropertyName != null)
            {
                var partner = targetEntityType.FindProperty(partnerPropertyName) as IEdmNavigationProperty;

                if (partner == null)
                {
                    partner = new UnresolvedNavigationPropertyPath(targetEntityType, partnerPropertyName, this.Location);
                }

                return partner;
            }

            foreach (IEdmNavigationProperty potentialPartner in targetEntityType.NavigationProperties())
            {
                if (potentialPartner == this)
                {
                    continue;
                }

                if (potentialPartner.Partner == this)
                {
                    return potentialPartner;
                }
            }

            return null;
        }

        private IEdmTypeReference ComputeType()
        {
            bool wasCollection;
            string typeName = this.navigationProperty.Type;

            const string CollectionPrefix = CsdlConstants.Value_Collection + "(";
            if (typeName.StartsWith(CollectionPrefix, StringComparison.Ordinal) && typeName.EndsWith(")", StringComparison.Ordinal))
            {
                wasCollection = true;
                typeName = typeName.Substring(CollectionPrefix.Length, (typeName.Length - CollectionPrefix.Length) - 1);
            }
            else
            {
                wasCollection = false;
            }

            IEdmEntityType targetType = this.declaringType.Context.FindType(typeName) as IEdmEntityType;
            if (targetType == null)
            {
                targetType = new UnresolvedEntityType(typeName, this.Location);
            }

            bool nullable = !wasCollection && (this.navigationProperty.Nullable ?? CsdlConstants.Default_Nullable);

            IEdmEntityTypeReference targetTypeReference = new EdmEntityTypeReference(targetType, nullable);
            if (wasCollection)
            {
                return new EdmCollectionTypeReference(new EdmCollectionType(targetTypeReference));
            }

            return targetTypeReference;
        }

        private IEdmReferentialConstraint ComputeReferentialConstraint()
        {
            if (this.navigationProperty.ReferentialConstraints.Any())
            {
                return new EdmReferentialConstraint(this.navigationProperty.ReferentialConstraints.Select(this.ComputeReferentialConstraintPropertyPair));
            }

            return null;
        }

        private EdmReferentialConstraintPropertyPair ComputeReferentialConstraintPropertyPair(CsdlReferentialConstraint csdlConstraint)
        {
            // <EntityType Name="Product">
            //   ...
            //   <Property Name="CategoryID" Type="Edm.String" Nullable="false"/>
            //  <NavigationProperty Name="Category" Type="Self.Category" Nullable="false">
            //     <ReferentialConstraint Property="CategoryID" ReferencedProperty="ID" />
            //   </NavigationProperty>
            // </EntityType>
            // the above CategoryID is DependentProperty, ID is PrincipalProperty.
            IEdmStructuralProperty dependentProperty = this.declaringType.FindProperty(csdlConstraint.PropertyName) as IEdmStructuralProperty ?? new UnresolvedProperty(this.declaringType, csdlConstraint.PropertyName, csdlConstraint.Location);
            IEdmStructuralProperty principalProperty = this.TargetEntityType.FindProperty(csdlConstraint.ReferencedPropertyName) as IEdmStructuralProperty ?? new UnresolvedProperty(this.ToEntityType(), csdlConstraint.ReferencedPropertyName, csdlConstraint.Location);
            return new EdmReferentialConstraintPropertyPair(dependentProperty, principalProperty);
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = null;

            if (this.Type.IsCollection() && this.navigationProperty.Nullable.HasValue)
            {
                // TODO: this should happen at parsing time, address this as part of fixing task ID 1473340, which should remove
                // the code for handling type-ref based collection types and unify this parsing logic
                errors = AllocateAndAdd(errors, new EdmError(this.Location, EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, Strings.CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType));
            }

            var badType = this.TargetEntityType as BadEntityType;
            if (badType != null)
            {
                errors = AllocateAndAdd(errors, badType.Errors);
            }

            return errors ?? Enumerable.Empty<EdmError>();
        }
    }
}
