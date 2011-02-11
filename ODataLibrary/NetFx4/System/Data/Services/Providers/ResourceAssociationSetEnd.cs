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

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System.Diagnostics;
    using System.Data.OData;
    #endregion Namespaces.

    /// <summary>
    /// Class to describe an end point of a resource association set.
    /// </summary>
    [DebuggerDisplay("ResourceAssociationSetEnd: {Name}: ({ResourceSet.Name}, {ResourceType.Name}, {ResourceProperty.Name})")]
#if INTERNAL_DROP
    internal sealed class ResourceAssociationSetEnd : ODataAnnotatable
#else
    public sealed class ResourceAssociationSetEnd : ODataAnnotatable
#endif
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
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Constructs a ResourceAssociationEnd instance.
        /// </summary>
        /// <param name="resourceSet">Resource set of the association end.</param>
        /// <param name="resourceType">Resource type of the association end.</param>
        /// <param name="resourceProperty">Resource property of the association end.</param>
        public ResourceAssociationSetEnd(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            ExceptionUtils.CheckArgumentNotNull(resourceSet, "resourceSet");
            ExceptionUtils.CheckArgumentNotNull(resourceType, "resourceType");

            if (resourceProperty != null && (resourceType.TryResolvePropertyName(resourceProperty.Name) == null 
                || resourceProperty.ResourceType.ResourceTypeKind != ResourceTypeKind.EntityType))
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
        /// <summary>
        /// Resource set for the association end.
        /// </summary>
        public ResourceSet ResourceSet
        {
            [DebuggerStepThrough]
            get { return this.resourceSet; }
        }

        /// <summary>
        /// Resource type for the association end.
        /// </summary>
        public ResourceType ResourceType
        {
            [DebuggerStepThrough]
            get { return this.resourceType; }
        }

        /// <summary>
        /// Resource property for the association end.
        /// </summary>
        public ResourceProperty ResourceProperty
        {
            [DebuggerStepThrough]
            get { return this.resourceProperty; }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information
        /// </summary>
        public object CustomState
        {
            get
            {
                return this.GetCustomState();
            }

            set
            {
                this.SetCustomState(value);
            }
        }
        #endregion Properties
    }
}
