//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlNavigationProperty.
    /// </summary>
    internal class CsdlSemanticsNavigationProperty : CsdlSemanticsElement, IEdmNavigationProperty, IEdmCheckable
    {
        private readonly CsdlNavigationProperty navigationProperty;
        private readonly CsdlSemanticsStructuredTypeDefinition declaringType;

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

        public CsdlSemanticsNavigationProperty(CsdlSemanticsStructuredTypeDefinition declaringType, CsdlNavigationProperty navigationProperty)
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

        internal static IEdmNavigationProperty ResolvePartnerPath(IEdmEntityType type, IEdmPathExpression path, IEdmModel model)
        {
            Debug.Assert(type != null);
            Debug.Assert(path != null);
            Debug.Assert(model != null);

            IEdmStructuredType currentType = type;
            IEdmProperty property = null;
            foreach (var segment in path.PathSegments)
            {
                if (currentType == null)
                {
                    return null;
                }

                if (segment.IndexOf('.') < 0)
                {
                    property = currentType.FindProperty(segment);
                    if (property == null)
                    {
                        return null;
                    }

                    currentType = property.Type.Definition.AsElementType() as IEdmStructuredType;
                }
                else
                {
                    var derivedType = model.FindDeclaredType(segment);
                    if (derivedType == null || !derivedType.IsOrInheritsFrom(currentType))
                    {
                        return null;
                    }

                    currentType = derivedType as IEdmStructuredType;
                    property = null;
                }
            }

            return property != null
                   ? property as IEdmNavigationProperty
                   : null;
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
            var partnerPropertyPath = this.navigationProperty.PartnerPath;
            IEdmEntityType targetEntityType = this.TargetEntityType;

            if (partnerPropertyPath != null)
            {
                return ResolvePartnerPath(targetEntityType, partnerPropertyPath, Model)
                       ?? new UnresolvedNavigationPropertyPath(targetEntityType, partnerPropertyPath.Path, Location);
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
                // TODO: this should happen at parsing time, which should remove
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
