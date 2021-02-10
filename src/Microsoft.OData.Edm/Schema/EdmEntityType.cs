//---------------------------------------------------------------------
// <copyright file="EdmEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM entity type.
    /// </summary>
    public class EdmEntityType : EdmStructuredType, IEdmEntityType, IEdmFullNamedElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string fullName;
        private readonly bool hasStream;
        private List<IEdmStructuralProperty> declaredKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityType"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        public EdmEntityType(string namespaceName, string name)
            : this(namespaceName, name, null, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityType"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        public EdmEntityType(string namespaceName, string name, IEdmEntityType baseType)
            : this(namespaceName, name, baseType, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityType"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        public EdmEntityType(string namespaceName, string name, IEdmEntityType baseType, bool isAbstract, bool isOpen)
            : this(namespaceName, name, baseType, isAbstract, isOpen, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityType"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="hasStream">Denotes if the type is a media type.</param>
        public EdmEntityType(string namespaceName, string name, IEdmEntityType baseType, bool isAbstract, bool isOpen, bool hasStream)
            : base(isAbstract, isOpen, baseType)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.namespaceName = namespaceName;
            this.name = name;
            this.hasStream = hasStream;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
        }

        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        public virtual IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get { return this.declaredKey; }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        /// <summary>
        /// Gets the value indicating whether or not this entity is a media type
        /// This value inherits from the base type.
        /// </summary>
        public bool HasStream
        {
            get { return hasStream || (this.BaseType != null && this.BaseEntityType().HasStream); }
        }

        /// <summary>
        /// Adds the <paramref name="keyProperties"/> to the key of this entity type.
        /// </summary>
        /// <param name="keyProperties">The key properties.</param>
        public void AddKeys(params IEdmStructuralProperty[] keyProperties)
        {
            this.AddKeys((IEnumerable<IEdmStructuralProperty>)keyProperties);
        }

        /// <summary>
        /// Adds the <paramref name="keyProperties"/> to the key of this entity type.
        /// </summary>
        /// <param name="keyProperties">The key properties.</param>
        public void AddKeys(IEnumerable<IEdmStructuralProperty> keyProperties)
        {
            EdmUtil.CheckArgumentNull(keyProperties, "keyProperties");

            foreach (IEdmStructuralProperty property in keyProperties)
            {
                if (this.declaredKey == null)
                {
                    this.declaredKey = new List<IEdmStructuralProperty>();
                }

                this.declaredKey.Add(property);
            }
        }

        /// <summary>
        /// Creates and adds a navigation property to this type and adds its navigation partner to the navigation target type.
        /// </summary>
        /// <param name="propertyInfo">Information to create the navigation property.</param>
        /// <param name="partnerInfo">Information to create the partner navigation property.</param>
        /// <returns>Created navigation property.</returns>
        public EdmNavigationProperty AddBidirectionalNavigation(EdmNavigationPropertyInfo propertyInfo, EdmNavigationPropertyInfo partnerInfo)
        {
            EdmUtil.CheckArgumentNull(propertyInfo, "propertyInfo");
            EdmUtil.CheckArgumentNull(propertyInfo.Target, "propertyInfo.Target");

            EdmEntityType targetType = propertyInfo.Target as EdmEntityType;
            if (targetType == null)
            {
                throw new ArgumentException(Strings.Constructable_TargetMustBeStock(typeof(EdmEntityType).FullName), "propertyInfo.Target");
            }

            EdmNavigationProperty property = EdmNavigationProperty.CreateNavigationPropertyWithPartner(propertyInfo, this.FixUpDefaultPartnerInfo(propertyInfo, partnerInfo));

            this.AddProperty(property);
            targetType.AddProperty(property.Partner);
            return property;
        }

        /// <summary>
        /// Sets partner information of a top-level navigation property.
        /// </summary>
        /// <param name="navigationProperty">Navigation property of the entity type.</param>
        /// <param name="navigationPropertyPath">Path to the navigation property of the entity type.</param>
        /// <param name="partnerNavigationProperty">Partner navigation property.</param>
        /// <param name="partnerNavigationPropertyPath">Path to the partner navigation property
        /// from the related entity type.</param>
        /// <remarks>
        /// If partnerNavigationProperty is declared on an entity type, its partner will be set accordingly; there is no
        /// need to call this method twice, once on each side.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void SetNavigationPropertyPartner(
            EdmNavigationProperty navigationProperty,
            IEdmPathExpression navigationPropertyPath,
            EdmNavigationProperty partnerNavigationProperty,
            IEdmPathExpression partnerNavigationPropertyPath)
        {
            Debug.Assert(
                this.IsOnSameTypeHierarchyLineWith(navigationProperty.DeclaringType),
                "Nav. property is not defined on this or a derived entity type.");
            navigationProperty.SetPartner(partnerNavigationProperty, partnerNavigationPropertyPath);
            if (partnerNavigationProperty.DeclaringType is IEdmEntityType)
            {
                partnerNavigationProperty.SetPartner(navigationProperty, navigationPropertyPath);
            }
        }

        /// <summary>
        /// The purpose of this method is to make sure that some of the <paramref name="partnerInfo"/> fields are set to valid partner defaults.
        /// For example if <paramref name="partnerInfo"/>.Target is null, it will be set to this entity type. If <paramref name="partnerInfo"/>.TargetMultiplicity
        /// is unknown, it will be set to 0..1, etc.
        /// Whenever this method applies new values to <paramref name="partnerInfo"/>, it will return a copy of it (thus won't modify the original).
        /// If <paramref name="partnerInfo"/> is null, a new info object will be produced.
        /// </summary>
        /// <param name="propertyInfo">Primary navigation property info.</param>
        /// <param name="partnerInfo">Partner navigation property info. May be null.</param>
        /// <returns>Partner info.</returns>
        private EdmNavigationPropertyInfo FixUpDefaultPartnerInfo(EdmNavigationPropertyInfo propertyInfo, EdmNavigationPropertyInfo partnerInfo)
        {
            EdmNavigationPropertyInfo partnerInfoOverride = null;

            if (partnerInfo == null)
            {
                partnerInfo = partnerInfoOverride = new EdmNavigationPropertyInfo();
            }

            if (partnerInfo.Name == null)
            {
                if (partnerInfoOverride == null)
                {
                    partnerInfoOverride = partnerInfo.Clone();
                }

                partnerInfoOverride.Name = (propertyInfo.Name ?? String.Empty) + "Partner";
            }

            if (partnerInfo.Target == null)
            {
                if (partnerInfoOverride == null)
                {
                    partnerInfoOverride = partnerInfo.Clone();
                }

                partnerInfoOverride.Target = this;
            }

            if (partnerInfo.TargetMultiplicity == EdmMultiplicity.Unknown)
            {
                if (partnerInfoOverride == null)
                {
                    partnerInfoOverride = partnerInfo.Clone();
                }

                partnerInfoOverride.TargetMultiplicity = EdmMultiplicity.ZeroOrOne;
            }

            return partnerInfoOverride ?? partnerInfo;
        }
    }
}
