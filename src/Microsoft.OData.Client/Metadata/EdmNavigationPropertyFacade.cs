//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyFacade.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client.Metadata
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using ErrorStrings = Microsoft.OData.Service.Client.Strings;

    /// <summary>
    /// A navigation property implementation which combines information from the client and server models.
    /// </summary>
    internal class EdmNavigationPropertyFacade : IEdmNavigationProperty
    {
        /// <summary>The model facade this property belongs to.</summary>
        private readonly EdmModelFacade modelFacade;

        /// <summary>The type facade this property is declared on.</summary>
        private readonly EdmEntityTypeFacade declaringTypeFacade;

        /// <summary>The server navigation property this facade represents, if one exists.</summary>
        private readonly IEdmNavigationProperty serverProperty;

        /// <summary>The client navigation property this facade represents.</summary>
        private readonly IEdmNavigationProperty clientProperty;

        /// <summary>Storage for the combined type of the navigation once it has been computed.</summary>
        private IEdmTypeReference combinedPropertyType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNavigationPropertyFacade"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="modelFacade">The model facade.</param>
        /// <param name="declaringTypeFacade">The type facade.</param>
        /// <param name="serverProperty">The server property if one exists.</param>
        /// <param name="clientProperty">The client property.</param>
        internal EdmNavigationPropertyFacade(string name, EdmModelFacade modelFacade, EdmEntityTypeFacade declaringTypeFacade, IEdmNavigationProperty serverProperty, IEdmNavigationProperty clientProperty)
        {
            Debug.Assert(clientProperty != null, "clientProperty != null");
            Debug.Assert(serverProperty != null, "serverProperty != null");
            Debug.Assert(modelFacade != null, "modelFacade != null");
            Debug.Assert(declaringTypeFacade != null, "declaringTypeFacade != null");

            this.Name = name;
            this.modelFacade = modelFacade;
            this.declaringTypeFacade = declaringTypeFacade;
            this.serverProperty = serverProperty;
            this.clientProperty = clientProperty;
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get
            {
                if (this.combinedPropertyType == null)
                {
                    this.combinedPropertyType = CombinePropertyTypes(this.Name, this.clientProperty, this.serverProperty, this.modelFacade);
                }

                return this.combinedPropertyType;
            }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringTypeFacade; }
        }

        /// <summary>
        /// Gets the partner of this navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("Partner"); }
        }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction OnDelete
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("OnDelete"); }
        }

        /// <summary>
        /// Gets whether this navigation property originates at the principal end of an association.
        /// </summary>
        public bool IsPrincipal
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("IsPrincipal"); }
        }

        /// <summary>
        /// Gets the dependent properties of this navigation property, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("DependentProperties"); }
        }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("ContainsTarget"); }
        }

        /// <summary>
        /// Finds the navigation target of this navigation property given a server entity set.
        /// </summary>
        /// <param name="sourceServerEntitySet">The source server entity set.</param>
        /// <returns>The navigation target or null if one couldn't be found.</returns>
        internal EdmEntitySetFacade FindNavigationTarget(IEdmEntitySet sourceServerEntitySet)
        {
            Debug.Assert(sourceServerEntitySet != null, "sourceServerEntitySet != null");

            // if no property could be found, then there is no way to get the target.
            if (this.serverProperty == null)
            {
                return null;
            }

            // find the target using the server property.
            IEdmEntitySet serverTarget = sourceServerEntitySet.FindNavigationTarget(this.serverProperty);
            if (serverTarget == null)
            {
                return null;
            }

            // if a target was found, wrap it in a new facade and return it.
            return this.modelFacade.GetOrCreateEntityContainerFacade(serverTarget.Container).GetOrCreateEntitySetFacade(serverTarget);
        }

        /// <summary>
        /// Unit test method for determining whether two facades are equivalent (ie: wrap the same server/client properties).
        /// </summary>
        /// <param name="other">The other facade.</param>
        /// <returns>
        ///   <c>true</c> if the two facades wrap the same properties; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEquivalentTo(EdmNavigationPropertyFacade other)
        {
            return other != null
                && ReferenceEquals(other.modelFacade, this.modelFacade) 
                && ReferenceEquals(other.declaringTypeFacade, this.declaringTypeFacade)
                && ReferenceEquals(other.clientProperty, this.clientProperty)
                && ReferenceEquals(other.serverProperty, this.serverProperty);
        }

        /// <summary>
        /// Creates an exception for a intentionally-unsupported part of the API.
        /// This is used to prevent new code from calling previously-unused API, which could be a breaking change
        /// for user implementations of the interface.
        /// </summary>
        /// <param name="methodName">Name of the unsupported method.</param>
        /// <returns>The exception</returns>
        private static Exception CreateExceptionForUnsupportedPublicMethod(string methodName)
        {
            return new NotSupportedException(ErrorStrings.EdmModelFacade_UnsupportedMethod("IEdmNavigationProperty", methodName));
        }

        /// <summary>
        /// Combines the property types and returns a reference to the resulting facade.
        /// </summary>
        /// <param name="propertyName">The name of the navigation properties being combined. Used only for error messages.</param>
        /// <param name="clientProperty">The client property.</param>
        /// <param name="serverProperty">The server property.</param>
        /// <param name="modelFacade">The model facade.</param>
        /// <returns>
        /// A type reference to the combined type.
        /// </returns>
        private static IEdmTypeReference CombinePropertyTypes(string propertyName, IEdmNavigationProperty clientProperty, IEdmNavigationProperty serverProperty, EdmModelFacade modelFacade)
        {
            Debug.Assert(clientProperty != null, "clientProperty != null");
            Debug.Assert(serverProperty != null, "serverProperty != null");
            
            IEdmTypeReference clientPropertyType = clientProperty.Type;
            IEdmTypeReference serverPropertyType = serverProperty.Type;
            
            // ensure that either both sides are a collection or neither is.
            IEdmCollectionTypeReference clientCollectionType = clientPropertyType as IEdmCollectionTypeReference;
            IEdmCollectionTypeReference serverCollectionType = serverPropertyType as IEdmCollectionTypeReference;
            bool isCollection = clientCollectionType != null;
            if (isCollection != (serverCollectionType != null))
            {
                throw new InvalidOperationException(ErrorStrings.EdmNavigationPropertyFacade_InconsistentMultiplicity(propertyName));
            }

            // For collection properties: extract the element types, combine them, then recreate the collection.
            // For reference properties: get the entity types and combine them.
            IEdmType combinedType;
            if (isCollection)
            {
                // get the client element type and ensure it's an entity type.
                IEdmEntityTypeReference clientElementTypeReference = clientCollectionType.ElementType() as IEdmEntityTypeReference;
                if (clientElementTypeReference == null)
                {
                    throw new InvalidOperationException(ErrorStrings.EdmNavigationPropertyFacade_NonEntityType(propertyName, clientProperty.DeclaringType.FullName()));
                }

                // get the server element type and ensure it's an entity type.
                IEdmEntityTypeReference serverElementTypeReference = serverCollectionType.ElementType() as IEdmEntityTypeReference;
                if (serverElementTypeReference == null)
                {
                    throw new InvalidOperationException(ErrorStrings.EdmNavigationPropertyFacade_NonEntityType(propertyName, serverProperty.DeclaringType.FullName())); 
                }

                // combine the element types.
                combinedType = modelFacade.CombineWithServerType(clientElementTypeReference.EntityDefinition(), serverElementTypeReference.EntityDefinition());

                // turn it back into a collection, maintaining nullability of the client's element type.
                combinedType = new EdmCollectionType(combinedType.ToEdmTypeReference(clientElementTypeReference.IsNullable));
            }
            else
            {
                // ensure the server property type is an entity type.
                IEdmEntityTypeReference clientEntityTypeReference = clientPropertyType as IEdmEntityTypeReference;
                if (clientEntityTypeReference == null)
                {
                    throw new InvalidOperationException(ErrorStrings.EdmNavigationPropertyFacade_NonEntityType(propertyName, clientProperty.DeclaringType.FullName()));
                }

                // ensure the server property type is an entity type.
                IEdmEntityTypeReference serverEntityTypeReference = serverPropertyType as IEdmEntityTypeReference;
                if (serverEntityTypeReference == null)
                {
                    throw new InvalidOperationException(ErrorStrings.EdmNavigationPropertyFacade_NonEntityType(propertyName, serverProperty.DeclaringType.FullName()));
                }

                combinedType = modelFacade.CombineWithServerType(clientEntityTypeReference.EntityDefinition(), serverEntityTypeReference.EntityDefinition());
            }

            // return a type reference, maintaining the original nullability from the client property type.
            return combinedType.ToEdmTypeReference(clientPropertyType.IsNullable);
        }
    }
}