//---------------------------------------------------------------------
// <copyright file="EntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Entity Type
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("EntityType: {this.NamespaceName}.{this.Name} BaseType={this.BaseTypeName} Properties={this.Properties.Count} NavProps={this.NavigationProperties.Count}")]
    public class EntityType : NamedStructuralType, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the EntityType class.
        /// </summary>
        public EntityType()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityType class with given name and null namespace.
        /// </summary>
        /// <param name="name">EntityType name</param>
        public EntityType(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityType class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">EntityType namespace</param>
        /// <param name="name">EntityType name</param>
        public EntityType(string namespaceName, string name)
            : base(namespaceName, name)
        {
            this.NavigationProperties = new List<NavigationProperty>();
            this.EdmUniqueConstraints = new List<EdmUniqueConstraint>();
        }

        /// <summary>
        /// Gets or sets the EntityType that this entity type derives from.
        /// </summary>
        public EntityType BaseType { get; set; }

        /// <summary>
        /// Gets navigation properties of the entity (self-defined, not including the ones defined in BaseType).
        /// </summary>
        public IList<NavigationProperty> NavigationProperties { get; private set; }

        /// <summary>
        /// Gets EDM Unique Constraints of the entity (self-defined, not including the ones defined in BaseType).
        /// </summary>
        public IList<EdmUniqueConstraint> EdmUniqueConstraints { get; private set; }

        /// <summary>
        /// Gets all NavigationProperties, including the ones defined in all its BaseTypes.
        /// </summary>
        public IEnumerable<NavigationProperty> AllNavigationProperties
        {
            get
            {
                if (this.BaseType == null)
                {
                    return this.NavigationProperties;
                }
                else
                {
                    return this.BaseType.AllNavigationProperties.Concat(this.NavigationProperties);
                }
            }
        }

        /// <summary>
        /// Gets all EDM Unique Constraints , including the ones defined in all its BaseTypes.
        /// </summary>
        public IEnumerable<EdmUniqueConstraint> AllEdmUniqueConstraints
        {
            get
            {
                if (this.BaseType == null)
                {
                    return this.EdmUniqueConstraints;
                }
                else
                {
                    return this.BaseType.AllEdmUniqueConstraints.Concat(this.EdmUniqueConstraints);
                }
            }
        }

        /// <summary>
        /// Gets all Key Properties. If it is derived, then it will get the key from the base type.
        /// </summary>
        public IEnumerable<MemberProperty> AllKeyProperties
        {
            get
            {
                if (this.BaseType == null)
                {
                    return this.Properties.Where(p => p.IsPrimaryKey == true);
                }
                else
                {
                    return this.BaseType.AllKeyProperties;
                }
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.EntityType"/>.
        /// </summary>
        /// <param name="entityTypeName">Name of the entity type.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator EntityType(string entityTypeName)
        {
            return new EntityTypeReference(entityTypeName);
        }

        /// <summary>
        /// Adds given navigation property to <see cref="NavigationProperties"/> collection.
        /// </summary>
        /// <param name="prop">Navigation property to add.</param>
        public void Add(NavigationProperty prop)
        {
            this.NavigationProperties.Add(prop);
        }

        /// <summary>
        /// Adds given Edm Unique Constraint to <see cref="EdmUniqueConstraints"/> collection.
        /// </summary>
        /// <param name="edmUniqueConstraint">Edm Unique Constraint to add.</param>
        public void Add(EdmUniqueConstraint edmUniqueConstraint)
        {
            this.EdmUniqueConstraints.Add(edmUniqueConstraint);
        }

        /// <summary>
        /// Determines whether the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="INamedItem"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(INamedItem other)
        {
            var otherEntity = other as EntityType;
            if (otherEntity == null)
            {
                return false;
            }

            return (this.Name == otherEntity.Name) && (this.NamespaceName == otherEntity.NamespaceName);
        }

        /// <summary>
        /// Determines whether the entity type is or derives from the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// A value of <c>true</c> if this entity type is or derives from the specified entity type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKindOf(EntityType entityType)
        {
            for (EntityType thisType = this; thisType != null; thisType = thisType.BaseType)
            {
                if (thisType == entityType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the root <see cref="EntityType"/> in the type hierarchy.
        /// </summary>
        /// <returns>The root <see cref="EntityType"/>.</returns>
        public EntityType GetRootType()
        {
            EntityType root = this;
            while (root.BaseType != null)
            {
                root = root.BaseType;
            }

            return root;
        }

        /// <summary>
        /// Gets the base type from which this type derives from.
        /// </summary>
        /// <returns>The base type of this type.</returns>
        protected override NamedStructuralType GetBaseTypeInternal()
        {
            return this.BaseType;
        }
    }
}