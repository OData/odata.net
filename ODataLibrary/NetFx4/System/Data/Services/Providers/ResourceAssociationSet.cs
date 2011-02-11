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
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Class to describe an association between two resource sets.
    /// </summary>
    [DebuggerDisplay("ResourceAssociationSet: ({End1.ResourceSet.Name}, {End1.ResourceType.Name}, {End1.ResourceProperty.Name}) <-> ({End2.ResourceSet.Name}, {End2.ResourceType.Name}, {End2.ResourceProperty.Name})")]
#if INTERNAL_DROP
    internal sealed class ResourceAssociationSet : ODataAnnotatable
#else
    public sealed class ResourceAssociationSet : ODataAnnotatable
#endif
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
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Constructs a resource association set instance.
        /// </summary>
        /// <param name="name">Name of the association set.</param>
        /// <param name="end1">end1 of the association set.</param>
        /// <param name="end2">end2 of the association set.</param>
        public ResourceAssociationSet(string name, ResourceAssociationSetEnd end1, ResourceAssociationSetEnd end2)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentNotNull(end1, "end1");
            ExceptionUtils.CheckArgumentNotNull(end2, "end2");

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
        /// <summary>
        /// Name of the association set.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return this.name; }
        }

        /// <summary>
        /// Source end of the association set.
        /// </summary>
        public ResourceAssociationSetEnd End1
        {
            [DebuggerStepThrough]
            get { return this.end1; }
        }

        /// <summary>
        /// Target end of the association set.
        /// </summary>
        public ResourceAssociationSetEnd End2
        {
            [DebuggerStepThrough]
            get { return this.end2; }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information.
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

        #region Methods
        /// <summary>
        /// Retrieve the end for the given resource set, type and property.
        /// </summary>
        /// <param name="resourceSet">resource set for the end</param>
        /// <param name="resourceType">resource type for the end</param>
        /// <param name="resourceProperty">resource property for the end</param>
        /// <returns>Resource association set end for the given parameters</returns>
        /// <remarks>This method was copied from the product.</remarks>
        internal ResourceAssociationSetEnd GetResourceAssociationSetEnd(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            DebugUtils.CheckNoExternalCallers();
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
        /// <remarks>This method was copied from the product.</remarks>
        internal ResourceAssociationSetEnd GetRelatedResourceAssociationSetEnd(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            ResourceAssociationSetEnd thisEnd = this.GetResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);

            if (thisEnd != null)
            {
                return thisEnd == this.End1 ? this.End2 : this.End1;
            }

            return null;
        }
        #endregion
    }
}
