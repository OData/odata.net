//---------------------------------------------------------------------
// <copyright file="ResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Structure to keep information about a resource set
    /// </summary>
    /// <remarks>
    /// Custom providers can choose to use it as is or derive from it
    /// in order to flow provider-specific data.
    /// </remarks>
    [DebuggerDisplay("{Name}: {ResourceType}")]
    public class ResourceSet
    {
        #region Fields
        /// <summary> Reference to resource type that this resource set is a collection of</summary>
        private readonly ResourceType elementType;

        /// <summary>Type of the query root for the set.</summary>
        private readonly Type queryRootType;

        /// <summary>Name of the resource set.</summary>
        private readonly string name;

        /// <summary>Name of the entity container to which the set belongs to.</summary>
        private string entityContainerName;

        /// <summary>Is true, if the resource set is fully initialized and validated. No more changes can be made once its set to readonly.</summary>
        private bool isReadOnly;

        /// <summary>Is true, if key properties should be ordered as per declared order when used for constructing OrderBy queries.
        /// Otherwise the default alphabetical order is used.</summary>
        private bool useMetadataKeyOrder;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        #endregion Fields

        #region Constructors

        /// <summary>Creates a new instance of <see cref="T:Microsoft.OData.Service.Providers.ResourceSet" /> class.</summary>
        /// <param name="name">The name of the set of items as string.</param>
        /// <param name="elementType">The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of the items in the set.</param>
        public ResourceSet(string name, ResourceType elementType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckArgumentNull(elementType, "elementType");

            if (elementType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ArgumentException(Strings.ResourceContainer_ContainerMustBeAssociatedWithEntityType);
            }

            this.name = name;
            this.elementType = elementType;
            this.queryRootType = typeof(System.Linq.IQueryable<>).MakeGenericType(elementType.InstanceType);
        }

        #endregion Constructors

        #region Properties

        /// <summary>Gets the name of the collection.</summary>
        /// <returns>The name of the resource set.</returns>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>Gets the type of the collection.</summary>
        /// <returns>The type of the collection.</returns>
        public ResourceType ResourceType
        {
            get { return this.elementType; }
        }

        /// <summary>Gets or sets custom state information that is defined by the developer.</summary>
        /// <returns>The state information defined by the developer.</returns>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary>Gets whether the set is read-only.</summary>
        /// <returns>true if the set is read-only; Otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>Get or sets whether to use the order of key properties as defined in the metadata of an Entity Framework or reflection provider when constructing an implicit OrderBy query.</summary>
        /// <returns>true when the order of key properties is inferred from the provider metadata; Otherwise, false.</returns>
        public bool UseMetadataKeyOrder
        {
            get
            {
                return this.useMetadataKeyOrder;
            }

            set
            {
                this.ThrowIfSealed();
                this.useMetadataKeyOrder = value;
            }
        }

        /// <summary>Type of the query root for the set.</summary>
        internal Type QueryRootType
        {
            get { return this.queryRootType; }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this set.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, object>> CustomAnnotations
        {
            get
            {
                if (this.customAnnotations == null)
                {
                    return WebUtil.EmptyKeyValuePairStringObject;
                }

                return this.customAnnotations;
            }
        }

        /// <summary>
        /// Name of the entity container to which the set belongs to.
        /// </summary>
        internal string EntityContainerName
        {
            get
            {
                return this.entityContainerName;
            }

            set
            {
                Debug.Assert(this.entityContainerName == null, "this.entityContainerName == null");
                this.entityContainerName = value;
            }
        }

        #endregion Properties

        #region Methods
        /// <summary>Gets or sets the read-only status of the collection.</summary>
        public void SetReadOnly()
        {
            // If its already set to readonly, then its a no-op
            if (this.isReadOnly)
            {
                return;
            }

            this.elementType.SetReadOnly();
            this.isReadOnly = true;
        }

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }

        /// <summary>
        /// Checks if the resource set is sealed. If not, it throws an InvalidOperationException.
        /// </summary>
        private void ThrowIfSealed()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Strings.ResourceSet_Sealed(this.Name));
            }
        }
        #endregion Methods
    }
}
