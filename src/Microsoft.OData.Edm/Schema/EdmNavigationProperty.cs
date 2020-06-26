//---------------------------------------------------------------------
// <copyright file="EdmNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM navigation property.
    /// </summary>
    public sealed class EdmNavigationProperty : EdmProperty, IEdmNavigationProperty
    {
        private readonly IEdmReferentialConstraint referentialConstraint;
        private readonly bool containsTarget;
        private readonly EdmOnDeleteAction onDelete;
        private IEdmNavigationProperty partner;

        private EdmNavigationProperty(
            IEdmStructuredType declaringType,
            string name,
            IEdmTypeReference type,
            IEnumerable<IEdmStructuralProperty> dependentProperties,
            IEnumerable<IEdmStructuralProperty> principalProperties,
            bool containsTarget,
            EdmOnDeleteAction onDelete)
            : base(declaringType, name, type)
        {
            this.containsTarget = containsTarget;
            this.onDelete = onDelete;

            if (dependentProperties != null)
            {
                this.referentialConstraint = EdmReferentialConstraint.Create(dependentProperties, principalProperties);
            }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public override EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget
        {
            get { return this.containsTarget; }
        }

        /// <summary>
        /// Gets the referential constraint for this navigation, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        public IEdmReferentialConstraint ReferentialConstraint
        {
            get { return this.referentialConstraint; }
        }

        /// <summary>
        /// Gets the action to take when an instance of the declaring type is deleted.
        /// </summary>
        public EdmOnDeleteAction OnDelete
        {
            get { return this.onDelete; }
        }

        /// <summary>
        /// Gets the partner of this navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get { return this.partner; }
        }

        /// <summary>
        /// Gets the path to the partner in the related entity type.
        /// </summary>
        /// <remarks>
        /// Is null if the containing type is a complex type.
        /// </remarks>
        internal IEdmPathExpression PartnerPath { get; private set; }

        /// <summary>
        /// Creates a navigation property from the given information.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="propertyInfo">Information to create the navigation property.</param>
        /// <returns>Created navigation property.</returns>
        public static EdmNavigationProperty CreateNavigationProperty(IEdmStructuredType declaringType, EdmNavigationPropertyInfo propertyInfo)
        {
            EdmUtil.CheckArgumentNull(propertyInfo, "propertyInfo");
            EdmUtil.CheckArgumentNull(propertyInfo.Name, "propertyInfo.Name");
            EdmUtil.CheckArgumentNull(propertyInfo.Target, "propertyInfo.Target");

            return new EdmNavigationProperty(
                declaringType,
                propertyInfo.Name,
                CreateNavigationPropertyType(propertyInfo.Target, propertyInfo.TargetMultiplicity, "propertyInfo.TargetMultiplicity"),
                propertyInfo.DependentProperties,
                propertyInfo.PrincipalProperties,
                propertyInfo.ContainsTarget,
                propertyInfo.OnDelete);
        }

        /// <summary>
        /// Creates two navigation properties representing an association between two entity types.
        /// </summary>
        /// <param name="propertyInfo">Information to create the navigation property.</param>
        /// <param name="partnerInfo">Information to create the partner navigation property.</param>
        /// <returns>Created navigation property.</returns>
        public static EdmNavigationProperty CreateNavigationPropertyWithPartner(EdmNavigationPropertyInfo propertyInfo, EdmNavigationPropertyInfo partnerInfo)
        {
            EdmUtil.CheckArgumentNull(propertyInfo, "propertyInfo");
            EdmUtil.CheckArgumentNull(propertyInfo.Name, "propertyInfo.Name");
            EdmUtil.CheckArgumentNull(propertyInfo.Target, "propertyInfo.Target");
            EdmUtil.CheckArgumentNull(partnerInfo, "partnerInfo");
            EdmUtil.CheckArgumentNull(partnerInfo.Name, "partnerInfo.Name");
            EdmUtil.CheckArgumentNull(partnerInfo.Target, "partnerInfo.Target");

            EdmNavigationProperty end1 = new EdmNavigationProperty(
                partnerInfo.Target,
                propertyInfo.Name,
                CreateNavigationPropertyType(propertyInfo.Target, propertyInfo.TargetMultiplicity, "propertyInfo.TargetMultiplicity"),
                propertyInfo.DependentProperties,
                propertyInfo.PrincipalProperties,
                propertyInfo.ContainsTarget,
                propertyInfo.OnDelete);

            EdmNavigationProperty end2 = new EdmNavigationProperty(
                propertyInfo.Target,
                partnerInfo.Name,
                CreateNavigationPropertyType(partnerInfo.Target, partnerInfo.TargetMultiplicity, "partnerInfo.TargetMultiplicity"),
                partnerInfo.DependentProperties,
                partnerInfo.PrincipalProperties,
                partnerInfo.ContainsTarget,
                partnerInfo.OnDelete);

            end1.SetPartner(end2, new EdmPathExpression(end2.Name));
            end2.SetPartner(end1, new EdmPathExpression(end1.Name));
            return end1;
        }

        /// <summary>
        /// Creates two navigation properties representing an association between two entity types.
        /// </summary>
        /// <param name="propertyName">Navigation property name.</param>
        /// <param name="propertyType">Type of the navigation property.</param>
        /// <param name="dependentProperties">Dependent properties of the navigation source.</param>
        /// <param name="principalProperties">Principal properties of the navigation source.</param>
        /// <param name="containsTarget">A value indicating whether the navigation source logically contains the navigation target.</param>
        /// <param name="onDelete">Action to take upon deletion of an instance of the navigation source.</param>
        /// <param name="partnerPropertyName">Navigation partner property name.</param>
        /// <param name="partnerPropertyType">Type of the navigation partner property.</param>
        /// <param name="partnerDependentProperties">Dependent properties of the navigation target.</param>
        /// <param name="partnerPrincipalProperties">Principal properties of the navigation target.</param>
        /// <param name="partnerContainsTarget">A value indicating whether the navigation target logically contains the navigation source.</param>
        /// <param name="partnerOnDelete">Action to take upon deletion of an instance of the navigation target.</param>
        /// <returns>Navigation property.</returns>
        public static EdmNavigationProperty CreateNavigationPropertyWithPartner(
            string propertyName,
            IEdmTypeReference propertyType,
            IEnumerable<IEdmStructuralProperty> dependentProperties,
            IEnumerable<IEdmStructuralProperty> principalProperties,
            bool containsTarget,
            EdmOnDeleteAction onDelete,
            string partnerPropertyName,
            IEdmTypeReference partnerPropertyType,
            IEnumerable<IEdmStructuralProperty> partnerDependentProperties,
            IEnumerable<IEdmStructuralProperty> partnerPrincipalProperties,
            bool partnerContainsTarget,
            EdmOnDeleteAction partnerOnDelete)
        {
            EdmUtil.CheckArgumentNull(propertyName, "propertyName");
            EdmUtil.CheckArgumentNull(propertyType, "propertyType");
            EdmUtil.CheckArgumentNull(partnerPropertyName, "partnerPropertyName");
            EdmUtil.CheckArgumentNull(partnerPropertyType, "partnerPropertyType");
            IEdmStructuredType declaringType = null;
            if (partnerPropertyType.Definition.TypeKind == EdmTypeKind.Entity)
            {
                declaringType = GetEntityType(partnerPropertyType) as IEdmEntityType;
                if (declaringType == null)
                {
                    throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof(partnerPropertyType));
                }
            }
            else if (partnerPropertyType.Definition.TypeKind == EdmTypeKind.Complex)
            {
                declaringType = GetComplexType(partnerPropertyType) as IEdmComplexType;
                if (declaringType == null)
                {
                    throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof(partnerPropertyType));
                }
            }
            else
            {
                throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof(partnerPropertyType));
            }

            IEdmEntityType partnerDeclaringType = GetEntityType(propertyType);
            if (partnerDeclaringType == null)
            {
                throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof(propertyType));
            }

            EdmNavigationProperty end1 = new EdmNavigationProperty(
                declaringType,
                propertyName,
                propertyType,
                dependentProperties,
                principalProperties,
                containsTarget,
                onDelete);

            EdmNavigationProperty end2 = new EdmNavigationProperty(
                partnerDeclaringType,
                partnerPropertyName,
                partnerPropertyType,
                partnerDependentProperties,
                partnerPrincipalProperties,
                partnerContainsTarget,
                partnerOnDelete);

            end1.SetPartner(end2, new EdmPathExpression(end2.Name));
            end2.SetPartner(end1, new EdmPathExpression(end1.Name));
            return end1;
        }

        /// <summary>
        /// Sets the partner information.
        /// </summary>
        /// <param name="navigationProperty">The partner navigation property.</param>
        /// <param name="navigationPropertyPath">Path to the partner navigation property in the related entity type.</param>
        internal void SetPartner(IEdmNavigationProperty navigationProperty, IEdmPathExpression navigationPropertyPath)
        {
            Debug.Assert(
                DeclaringType is IEdmEntityType || DeclaringType is IEdmComplexType,
                "Partner info cannot be set for nav. property on a non-entity or non-complex type.");
            partner = navigationProperty;
            PartnerPath = navigationPropertyPath;
        }

        private static IEdmEntityType GetEntityType(IEdmTypeReference type)
        {
            IEdmEntityType entityType = null;
            if (type.IsEntity())
            {
                entityType = (IEdmEntityType)type.Definition;
            }
            else if (type.IsCollection())
            {
                type = ((IEdmCollectionType)type.Definition).ElementType;
                if (type.IsEntity())
                {
                    entityType = (IEdmEntityType)type.Definition;
                }
            }

            return entityType;
        }

        private static IEdmComplexType GetComplexType(IEdmTypeReference type)
        {
            if (type.IsComplex())
            {
                return (IEdmComplexType)type.Definition;
            }
            else if (type.IsCollection())
            {
                type = ((IEdmCollectionType)type.Definition).ElementType;
                if (type.IsComplex())
                {
                    return (IEdmComplexType)type.Definition;
                }
            }

            return null;
        }

        private static IEdmTypeReference CreateNavigationPropertyType(IEdmEntityType entityType, EdmMultiplicity multiplicity, string multiplicityParameterName)
        {
            switch (multiplicity)
            {
                case EdmMultiplicity.ZeroOrOne:
                    return new EdmEntityTypeReference(entityType, true);

                case EdmMultiplicity.One:
                    return new EdmEntityTypeReference(entityType, false);

                case EdmMultiplicity.Many:
                    return EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false));

                default:
                    throw new ArgumentOutOfRangeException(multiplicityParameterName, Strings.UnknownEnumVal_Multiplicity(multiplicity));
            }
        }
    }
}
