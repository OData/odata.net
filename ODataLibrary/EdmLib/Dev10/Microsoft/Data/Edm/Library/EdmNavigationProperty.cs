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
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM navigation property.
    /// </summary>
    public sealed class EdmNavigationProperty : EdmProperty, IEdmNavigationProperty, IEdmAssociationEnd, IFlushCaches
    {
        private EdmNavigationProperty partner;
        private EdmOnDeleteAction onDelete;
        private List<IEdmStructuralProperty> dependentProperties;

        // From cache.
        private readonly Cache<EdmNavigationProperty, IEdmAssociationEnd> from = new Cache<EdmNavigationProperty, IEdmAssociationEnd>();
        private readonly static Func<EdmNavigationProperty, IEdmAssociationEnd> s_computeFrom = (me) => me.ComputeFrom();

        // Association cache.
        private readonly Cache<EdmNavigationProperty, IEdmAssociation> association = new Cache<EdmNavigationProperty, IEdmAssociation>();
         private readonly static Func<EdmNavigationProperty, IEdmAssociation> s_computeAssociation = (me) => me.ComputeAssociation();

        /// <summary>
        /// Initializes a new instance of the EdmNavigationProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this navigation property.</param>
        /// <param name="name">Name of the navigation property.</param>
        /// <param name="type">Type that this navigation property points to.</param>
        /// <param name="onDelete">Action to take upon deleting the declaring type.</param>
        public EdmNavigationProperty(
            IEdmEntityType declaringType,
            string name,
            IEdmTypeReference type,
            EdmOnDeleteAction onDelete)
            : base(declaringType, name, type)
        {
            this.onDelete = onDelete;
            this.dependentProperties = null;
        }

        /// <summary>
        /// Initializes a new instance of the EdmNavigationProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this navigation property.</param>
        public EdmNavigationProperty(IEdmEntityType declaringType)
            : base(declaringType)
        {
            this.onDelete = EdmOnDeleteAction.None;
            this.dependentProperties = null;
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public override EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        IEdmAssociationEnd IEdmNavigationProperty.To
        {
            get { return this; }
        }

        private IEdmAssociationEnd From
        {
            get { return this.from.GetValue(this, s_computeFrom, null); }
        }

        /// <summary>
        /// Gets or sets the navigation property from this properties destination back to the declaring type of this property.
        /// </summary>
        public EdmNavigationProperty Partner
        {
            get
            {
                return this.partner;
            }

            set
            {
                if (this.partner != null)
                {
                    this.partner.SetField(this.partner.DeclaringType as IDependent, ref this.partner.partner, null);
                }

                this.SetField(DeclaringType as IDependent, ref this.partner, value);

                if (value != null)
                {
                    EdmNavigationProperty valuePartner = value.partner;
                    if (valuePartner != null)
                    {
                        valuePartner.SetField(valuePartner.DeclaringType as IDependent, ref valuePartner.partner, null);
                    }

                    value.SetField(value.DeclaringType as IDependent, ref value.partner, this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dependent properties of the association this navigation property expresses.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get
            {
                return this.dependentProperties;
            }

            set
            {
                if (this.partner == null || this.partner.DependentProperties != null)
                {
                    throw new InvalidOperationException(Edm.Strings.EdmNavigation_RequiresPartner);
                }

                this.dependentProperties = new List<IEdmStructuralProperty>(value);
                this.association.Clear();
                this.partner.association.Clear();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this navigation property is from the principal end of the association.
        /// </summary>
        public bool IsFromPrincipal
        {
            get
            {
                return
                    this.partner == null ||
                    this.partner.DependentProperties != null ||
                    (this.DependentProperties == null && ((IEdmAssociationEnd)this).DeclaringAssociation.End1 == this);
            }
        }

        /// <summary>
        /// Gets the entity type that this navigation property belongs to.
        /// </summary>
        public IEdmEntityType DeclaringEntityType
        {
            get { return (IEdmEntityType)this.DeclaringType; }
        }

        IEdmAssociation IEdmAssociationEnd.DeclaringAssociation
        {
            get { return this.association.GetValue(this, s_computeAssociation, null); }
        }

        IEdmEntityType IEdmAssociationEnd.EntityType
        {
            get
            {
                IEdmTypeReference towardsEntity = Type;
                if (towardsEntity.TypeKind() == EdmTypeKind.Collection)
                {
                    towardsEntity = towardsEntity.AsCollection().ElementType();
                }

                return towardsEntity.AsEntity().EntityDefinition();
            }
        }

        EdmAssociationMultiplicity IEdmAssociationEnd.Multiplicity
        {
            get
            {
                if (Type.TypeKind() == EdmTypeKind.Collection)
                {
                    return EdmAssociationMultiplicity.Many;
                }

                return Type.IsNullable ? EdmAssociationMultiplicity.ZeroOrOne : EdmAssociationMultiplicity.One;
            }
        }

        /// <summary>
        /// Gets or sets the action to take when an element of the defining type is deleted.
        /// </summary>
        public EdmOnDeleteAction OnDelete
        {
            get { return this.onDelete; }
            set { this.onDelete = value; }
        }

        private IEdmAssociationEnd ComputeFrom()
        {
            // CLR amd64 bug causes an issue if we use ??
            return this.partner ?? (IEdmAssociationEnd) new SilentPartner(this);
        }

        private IEdmAssociation ComputeAssociation()
        {
            if (this.partner != null && this.partner != this)
            {
                // Only the FromPrincipal end computes the association--the ToPrincipal end uses that of the FromPrincipal end.
                // If neither end is principal the first requested one is computed.
                if (this.dependentProperties != null ||
                    (this.partner.dependentProperties == null && this.partner.association.HasValue))
                {   
                    return ((IEdmAssociationEnd)this.partner).DeclaringAssociation;
                }
            }

            return new Association(this, this.partner);
        }

        void IFlushCaches.FlushCaches()
        {
            this.association.Clear();
            this.from.Clear();
        }

       private class Association : EdmElement, IEdmAssociation, IEdmReferentialConstraint
      {
            private readonly EdmNavigationProperty fromPrincipal;
            private readonly EdmNavigationProperty toPrincipal;

            public Association(EdmNavigationProperty fromPrincipal, EdmNavigationProperty toPrincipal)
            {
                this.fromPrincipal = fromPrincipal;
                this.toPrincipal = toPrincipal;
            }

            public IEdmAssociationEnd End1
            {
                get { return this.fromPrincipal; }
            }

            public IEdmAssociationEnd End2
            {
                get { return this.toPrincipal ?? this.fromPrincipal.From; }
            }

            public IEdmReferentialConstraint ReferentialConstraint
            {
                get { return this.toPrincipal != null && this.toPrincipal.DependentProperties != null ? this : null; }
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.Association; }
            }

            public string Namespace
            {
                get { return ((IEdmEntityType)this.fromPrincipal.DeclaringType).Namespace ?? string.Empty; }
            }

            public string Name
            {
                get 
                {
                    return this.toPrincipal != null ? this.toPrincipal.DeclaringEntityType.Namespace + this.toPrincipal.DeclaringEntityType.Name + this.toPrincipal.Name + this.fromPrincipal.DeclaringEntityType.Namespace + this.fromPrincipal.DeclaringEntityType.Name + this.fromPrincipal.Name : 
                        this.fromPrincipal.DeclaringEntityType.Namespace + this.fromPrincipal.DeclaringEntityType.Name + this.fromPrincipal.Name; 
                }
            }

            IEdmAssociationEnd IEdmReferentialConstraint.PrincipalEnd
            {
                get { return this.toPrincipal; }
            }

            IEnumerable<IEdmStructuralProperty> IEdmReferentialConstraint.DependentProperties
            {
                get { return this.toPrincipal.DependentProperties; }
            }
        }

        private class SilentPartner : EdmElement, IEdmAssociationEnd
        {
            private readonly EdmNavigationProperty partner;

            public SilentPartner(EdmNavigationProperty partner)
            {
                this.partner = partner;
            }

            public IEdmAssociation DeclaringAssociation
            {
                get { return ((IEdmAssociationEnd)this.partner).DeclaringAssociation; }
            }

            public IEdmEntityType EntityType
            {
                get { return (IEdmEntityType)this.partner.DeclaringType; }
            }

            public EdmAssociationMultiplicity Multiplicity
            {
                get { return EdmAssociationMultiplicity.One; }
            }

            public EdmOnDeleteAction OnDelete
            {
                get { return EdmOnDeleteAction.None; }
            }

            public string Name
            {
                get { return this.EntityType.Name; }
            }
        }
    }
}
