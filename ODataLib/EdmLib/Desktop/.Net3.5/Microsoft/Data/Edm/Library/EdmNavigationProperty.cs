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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM navigation property.
    /// </summary>
    public sealed class EdmNavigationProperty : EdmProperty, IEdmNavigationProperty
    {
        private readonly bool containsTarget;
        private readonly EdmOnDeleteAction onDelete;
        private EdmNavigationProperty partner;
        private IEnumerable<IEdmStructuralProperty> dependentProperties;

        private EdmNavigationProperty(
            IEdmEntityType declaringType,
            string name,
            IEdmTypeReference type,
            IEnumerable<IEdmStructuralProperty> dependentProperties,
            bool containsTarget,
            EdmOnDeleteAction onDelete)
            : base(declaringType, name, type)
        {
            this.dependentProperties = dependentProperties;
            this.containsTarget = containsTarget;
            this.onDelete = onDelete;
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
        /// Gets the dependent properties of the association this navigation property expresses.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get { return this.dependentProperties; }
        }

        /// <summary>
        /// Gets a value indicating whether this navigation property is from the principal end of the association.
        /// </summary>
        public bool IsPrincipal
        {
            get
            {
                return this.DependentProperties == null && this.partner != null && this.partner.DependentProperties != null;
            }
        }

        /// <summary>
        /// Gets the entity type that this navigation property belongs to.
        /// </summary>
        public IEdmEntityType DeclaringEntityType
        {
            get { return (IEdmEntityType)this.DeclaringType; }
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
                propertyInfo.ContainsTarget,
                propertyInfo.OnDelete);

            EdmNavigationProperty end2 = new EdmNavigationProperty(
                propertyInfo.Target,
                partnerInfo.Name,
                CreateNavigationPropertyType(partnerInfo.Target, partnerInfo.TargetMultiplicity, "partnerInfo.TargetMultiplicity"),
                partnerInfo.DependentProperties,
                partnerInfo.ContainsTarget,
                partnerInfo.OnDelete);

            end1.partner = end2;
            end2.partner = end1;
            return end1;
        }

        /// <summary>
        /// Creates two navigation properties representing an association between two entity types.
        /// </summary>
        /// <param name="propertyName">Navigation property name.</param>
        /// <param name="propertyType">Type of the navigation property.</param>
        /// <param name="dependentProperties">Dependent properties of the navigation source.</param>
        /// <param name="containsTarget">A value indicating whether the navigation source logically contains the navigation target.</param>
        /// <param name="onDelete">Action to take upon deletion of an instance of the navigation source.</param>
        /// <param name="partnerPropertyName">Navigation partner property name.</param>
        /// <param name="partnerPropertyType">Type of the navigation partner property.</param>
        /// <param name="partnerDependentProperties">Dependent properties of the navigation target.</param>
        /// <param name="partnerContainsTarget">A value indicating whether the navigation target logically contains the navigation source.</param>
        /// <param name="partnerOnDelete">Action to take upon deletion of an instance of the navigation target.</param>
        /// <returns>Navigation property.</returns>
        public static EdmNavigationProperty CreateNavigationPropertyWithPartner(
            string propertyName,
            IEdmTypeReference propertyType,
            IEnumerable<IEdmStructuralProperty> dependentProperties,
            bool containsTarget,
            EdmOnDeleteAction onDelete,
            string partnerPropertyName,
            IEdmTypeReference partnerPropertyType,
            IEnumerable<IEdmStructuralProperty> partnerDependentProperties,
            bool partnerContainsTarget,
            EdmOnDeleteAction partnerOnDelete)
        {
            EdmUtil.CheckArgumentNull(propertyName, "propertyName");
            EdmUtil.CheckArgumentNull(propertyType, "propertyType");
            EdmUtil.CheckArgumentNull(partnerPropertyName, "partnerPropertyName");
            EdmUtil.CheckArgumentNull(partnerPropertyType, "partnerPropertyType");

            IEdmEntityType declaringType = GetEntityType(partnerPropertyType);
            if (declaringType == null)
            {
                throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, "partnerPropertyType");
            }

            IEdmEntityType partnerDeclaringType = GetEntityType(propertyType);
            if (partnerDeclaringType == null)
            {
                throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, "propertyType");
            }

            EdmNavigationProperty end1 = new EdmNavigationProperty(
                declaringType,
                propertyName,
                propertyType,
                dependentProperties,
                containsTarget,
                onDelete);

            EdmNavigationProperty end2 = new EdmNavigationProperty(
                partnerDeclaringType,
                partnerPropertyName,
                partnerPropertyType,
                partnerDependentProperties,
                partnerContainsTarget,
                partnerOnDelete);

            end1.partner = end2;
            end2.partner = end1;
            return end1;
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
