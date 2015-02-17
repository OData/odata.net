//---------------------------------------------------------------------
// <copyright file="ResourceAssociationSetEnd.cs" company="Microsoft">
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
    /// Class to describe an end point of a resource association set.
    /// </summary>
    [DebuggerDisplay("ResourceAssociationSetEnd: {Name}: ({ResourceSet.Name}, {ResourceType.Name}, {ResourceProperty.Name})")]
    public sealed class ResourceAssociationSetEnd
    {
        #region Private Fields

        /// <summary>
        /// Resource set for the association end.
        /// </summary>
        private readonly ResourceSet resourceSet;

        /// <summary>
        /// Resource type for the association end.
        /// </summary>
        private readonly ResourceType resourceType;

        /// <summary>
        /// Resource property for the association end.
        /// </summary>
        private readonly ResourceProperty resourceProperty;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        #endregion Private Fields

        #region Constructor

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" /> class.</summary>
        /// <param name="resourceSet">The resource set to which the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" /> end belongs.</param>
        /// <param name="resourceType">The resource type to which the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" /> end belongs.</param>
        /// <param name="resourceProperty">The resource property that returns the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" /> end.</param>
        public ResourceAssociationSetEnd(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            WebUtil.CheckArgumentNull(resourceSet, "resourceSet");
            WebUtil.CheckArgumentNull(resourceType, "resourceType");

            if (resourceProperty != null && (resourceType.TryResolvePropertyName(resourceProperty.Name) == null || resourceProperty.TypeKind != ResourceTypeKind.EntityType))
            {
                throw new ArgumentException(Strings.ResourceAssociationSetEnd_ResourcePropertyMustBeNavigationPropertyOnResourceType);
            }

            if (!resourceSet.ResourceType.IsAssignableFrom(resourceType) && !resourceType.IsAssignableFrom(resourceSet.ResourceType))
            {
                throw new ArgumentException(Strings.ResourceAssociationSetEnd_ResourceTypeMustBeAssignableToResourceSet);
            }

            this.resourceSet = resourceSet;
            this.resourceType = resourceType;

            // Note that for the TargetEnd, resourceProperty can be null.
            this.resourceProperty = resourceProperty;
        }

        #endregion Constructor

        #region Properties

        /// <summary>Gets the resource set for the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" />.</summary>
        /// <returns>The resource set.</returns>
        public ResourceSet ResourceSet
        {
            [DebuggerStepThrough]
            get { return this.resourceSet; }
        }

        /// <summary>Gets the resource type for the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" />.</summary>
        /// <returns>The resource type.</returns>
        public ResourceType ResourceType
        {
            [DebuggerStepThrough]
            get { return this.resourceType; }
        }

        /// <summary>Gets the resource property that returns the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" />.</summary>
        /// <returns>The resource property.</returns>
        public ResourceProperty ResourceProperty
        {
            [DebuggerStepThrough]
            get { return this.resourceProperty; }
        }

        /// <summary>Gets the custom state for the <see cref="T:Microsoft.OData.Service.Providers.ResourceAssociationSetEnd" />.</summary>
        /// <returns>The custom state.</returns>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this association set end.
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
        /// Set the role name for the resource association set end.
        /// </summary>
        internal string Name
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}
