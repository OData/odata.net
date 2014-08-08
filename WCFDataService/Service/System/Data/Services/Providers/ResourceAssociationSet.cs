//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Class to describe an association between two resource sets.
    /// </summary>
    [DebuggerDisplay("ResourceAssociationSet: ({End1.ResourceSet.Name}, {End1.ResourceType.Name}, {End1.ResourceProperty.Name}) <-> ({End2.ResourceSet.Name}, {End2.ResourceType.Name}, {End2.ResourceProperty.Name})")]
    public sealed class ResourceAssociationSet
    {
        #region Private Fields

        /// <summary>
        /// Name of the association set.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// End1 of the association set.
        /// </summary>
        private readonly ResourceAssociationSetEnd end1;

        /// <summary>
        /// End2 of the association set.
        /// </summary>
        private readonly ResourceAssociationSetEnd end2;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        #endregion Private Fields

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.Providers.ResourceAssociationSet" /> class.</summary>
        /// <param name="name">Name of the association set.</param>
        /// <param name="end1"><see cref="T:System.Data.Services.Providers.ResourceAssociationSetEnd" /> that is at the source end of the association set.</param>
        /// <param name="end2"><see cref="T:System.Data.Services.Providers.ResourceAssociationSetEnd" /> that is at the target end of the association set.</param>
        public ResourceAssociationSet(string name, ResourceAssociationSetEnd end1, ResourceAssociationSetEnd end2)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            WebUtil.CheckArgumentNull(end1, "end1");
            WebUtil.CheckArgumentNull(end2, "end2");

            if (end1.ResourceProperty == null && end2.ResourceProperty == null)
            {
                throw new ArgumentException(Strings.ResourceAssociationSet_ResourcePropertyCannotBeBothNull);
            }

            if (end1.ResourceType == end2.ResourceType && end1.ResourceProperty == end2.ResourceProperty)
            {
                throw new ArgumentException(Strings.ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional);
            }

            this.name = name;
            this.end1 = end1;
            this.end2 = end2;
        }

        #endregion Constructor

        #region Properties

        /// <summary>Gets the name of the association set.</summary>
        /// <returns>The name of the association set.</returns>
        public string Name
        {
            [DebuggerStepThrough]
            get { return this.name; }
        }

        /// <summary>Gets the source end of the association set.</summary>
        /// <returns><see cref="T:System.Data.Services.Providers.ResourceAssociationSetEnd" /> that is at the source end of the association set.</returns>
        public ResourceAssociationSetEnd End1
        {
            [DebuggerStepThrough]
            get { return this.end1; }
        }

        /// <summary>Gets the target end of the association set.</summary>
        /// <returns><see cref="T:System.Data.Services.Providers.ResourceAssociationSetEnd" /> that is at the target end of the association set.</returns>
        public ResourceAssociationSetEnd End2
        {
            [DebuggerStepThrough]
            get { return this.end2; }
        }

        /// <summary>Gets or sets the custom state information about the resource association.</summary>
        /// <returns>The custom state information about the resource association.</returns>
        public object CustomState
        {
            get;
            set;
        }

        /// <summary>
        /// Resource association type for the set.
        /// </summary>
        public ResourceAssociationType ResourceAssociationType
        {
            get;
            set;
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        public void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }

        /// <summary>
        /// Retrieve the end for the given resource set, type and property.
        /// </summary>
        /// <param name="resourceSet">resource set for the end</param>
        /// <param name="resourceType">resource type for the end</param>
        /// <param name="resourceProperty">resource property for the end</param>
        /// <returns>Resource association set end for the given parameters</returns>
        internal ResourceAssociationSetEnd GetResourceAssociationSetEnd(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            foreach (ResourceAssociationSetEnd end in new[] { this.end1, this.end2 })
            {
                if (end.ResourceSet.Name == resourceSet.Name && end.ResourceType.IsAssignableFrom(resourceType))
                {
                    if ((end.ResourceProperty == null && resourceProperty == null) ||
                        (end.ResourceProperty != null && resourceProperty != null && end.ResourceProperty.Name == resourceProperty.Name))
                    {
                        return end;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the related end for the given resource set, type and property.
        /// </summary>
        /// <param name="resourceSet">resource set for the source end</param>
        /// <param name="resourceType">resource type for the source end</param>
        /// <param name="resourceProperty">resource property for the source end</param>
        /// <returns>Related resource association set end for the given parameters</returns>
        internal ResourceAssociationSetEnd GetRelatedResourceAssociationSetEnd(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            ResourceAssociationSetEnd thisEnd = this.GetResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);

            if (thisEnd != null)
            {
                return thisEnd == this.End1 ? this.End2 : this.End1;
            }

            return null;
        }

        /// <summary>
        /// Add the association type associated with this set.
        /// </summary>
        /// <param name="association">Association type.</param>
        internal void SetAssociationType(ResourceAssociationType association)
        {
            Debug.Assert(association != null, "association != null");
            Debug.Assert(this.ResourceAssociationType == null, "this.associationType == null");
            this.ResourceAssociationType = association;
        }

        #endregion Methods
    }
}
