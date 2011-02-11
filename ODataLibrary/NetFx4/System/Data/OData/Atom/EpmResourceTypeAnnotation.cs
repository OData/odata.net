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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// Annotation stored on a resource type to hold entity property mapping information.
    /// </summary>
    internal sealed class EpmResourceTypeAnnotation
    {
        /// <summary>
        /// Inherited EntityPropertyMapping attributes.
        /// </summary>
        private List<EntityPropertyMappingAttribute> inheritedEpmAttributes;

        /// <summary>
        /// Own EntityPropertyMapping attributes.
        /// </summary>
        private List<EntityPropertyMappingAttribute> ownEpmAttributes;

        /// <summary>
        /// EPM source tree for the resource type this annotation belongs to.
        /// </summary>
        private EpmSourceTree epmSourceTree;

        /// <summary>
        /// EPM target tree for the resource type this annotation belongs to.
        /// </summary>
        private EpmTargetTree epmTargetTree;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal EpmResourceTypeAnnotation()
        {
            DebugUtils.CheckNoExternalCallers();

            // Note that we new up everything here eagerly because we will only create the EPM annotation for types
            // for which we know for sure that they have EPM and thus we will need all of these anyway.
            this.inheritedEpmAttributes = new List<EntityPropertyMappingAttribute>();
            this.ownEpmAttributes = new List<EntityPropertyMappingAttribute>();
            this.epmTargetTree = new EpmTargetTree();
            this.epmSourceTree = new EpmSourceTree(this.epmTargetTree);
        }

        /// <summary>
        /// Inherited EntityPropertyMapping attributes.
        /// </summary>
        internal List<EntityPropertyMappingAttribute> InheritedEpmAttributes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inheritedEpmAttributes;
            }
        }

        /// <summary>
        /// Own EntityPropertyMapping attributes.
        /// </summary>
        internal List<EntityPropertyMappingAttribute> OwnEpmAttributes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.ownEpmAttributes;
            }
        }

        /// <summary>
        /// EPM source tree for the resource type this annotation belongs to.
        /// </summary>
        internal EpmSourceTree EpmSourceTree
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.epmSourceTree;
            }
        }

        /// <summary>
        /// EPM target tree for the resource type this annotation belongs to.
        /// </summary>
        internal EpmTargetTree EpmTargetTree
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.epmTargetTree;
            }
        }

        /// <summary>
        /// All EntityPropertyMapping attributes.
        /// </summary>
        internal IEnumerable<EntityPropertyMappingAttribute> AllEpmAttributes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.OwnEpmAttributes.Concat(this.InheritedEpmAttributes);
            }
        }

        /// <summary>
        /// Initializes the EPM internal information for the specified resource type and all its base types.
        /// </summary>
        /// <param name="resourceType">The resource type to build the EPM information for.</param>
        /// <remarks>If the type in question has no EPM attributes anywhere in its hierarchy this method will do nothing.</remarks>
        internal static void BuildEpm(ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");

            // Walk the base types and determine if there's EPM anywhere in there.
            bool hasEpm = false;
            ResourceType resourceTypeToTest = resourceType;
            while (resourceTypeToTest != null)
            {
                if (resourceTypeToTest.Epm() != null)
                {
                    hasEpm = true;
                    break;
                }

                resourceTypeToTest = resourceTypeToTest.BaseType;
            }

            // If there is build the EPM information for this type and add it as annotation.
            if (hasEpm)
            {
                EpmResourceTypeAnnotation epm = resourceType.Epm();
                if (epm == null)
                {
                    epm = new EpmResourceTypeAnnotation();
                    resourceType.SetAnnotation(epm);
                }

                epm.BuildEpmForType(resourceType, resourceType);
            }
        }

        /// <summary>
        /// Removes all data created internally when building EPM info. This is needed when building EPM 
        /// info fails since the trees may be left in undefined state (i.e. half constructed) and 
        /// if inherited EPM attributes exist duplicates will be added.
        /// </summary>
        internal void Reset()
        {
            DebugUtils.CheckNoExternalCallers();
            this.inheritedEpmAttributes.Clear();
        }

        /// <summary>
        /// Does given property in the attribute exist in the specified resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to inspect.</param>
        /// <param name="epmAttribute">Attribute which has PropertyName.</param>
        /// <returns>true if property exists in the specified type, false otherwise.</returns>
        private static bool PropertyExistsOnType(ResourceType resourceType, EntityPropertyMappingAttribute epmAttribute)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(epmAttribute != null, "epmAttribute != null");

            int indexOfSeparator = epmAttribute.SourcePath.IndexOf('/');
            String propertyToLookFor = indexOfSeparator == -1 ? epmAttribute.SourcePath : epmAttribute.SourcePath.Substring(0, indexOfSeparator);
            return resourceType.PropertiesDeclaredOnThisType.Any(p => p.Name == propertyToLookFor);
        }

        /// <summary>
        /// Initializes the EPM annotation with EPM information from the specified resource type.
        /// </summary>
        /// <param name="definingResourceType">Resource type to use the EPM infromation from.</param>
        /// <param name="affectedResourceType">Resource type for this the EPM information is being built.</param>
        private void BuildEpmForType(ResourceType definingResourceType, ResourceType affectedResourceType)
        {
            Debug.Assert(definingResourceType != null, "definingResourceType != null");
            Debug.Assert(affectedResourceType != null, "affectedResourceType != null");

            if (definingResourceType.BaseType != null)
            {
                this.BuildEpmForType(definingResourceType.BaseType, affectedResourceType);
            }

            EpmResourceTypeAnnotation epm = definingResourceType.Epm();
            if (epm != null)
            {
                foreach (EntityPropertyMappingAttribute epmAttribute in epm.AllEpmAttributes.ToList())
                {
                    this.epmSourceTree.Add(new EntityPropertyMappingInfo(epmAttribute, definingResourceType, affectedResourceType));

                    if (definingResourceType == affectedResourceType)
                    {
                        if (!PropertyExistsOnType(affectedResourceType, epmAttribute))
                        {
                            this.InheritedEpmAttributes.Add(epmAttribute);
                            this.OwnEpmAttributes.Remove(epmAttribute);
                        }
                        else
                        {
                            Debug.Assert(
                                this.OwnEpmAttributes.SingleOrDefault(attr => Object.ReferenceEquals(epmAttribute, attr)) != null,
                                "Own epmInfo should already have the given instance");
                        }
                    }
                }
            }
        }
    }
}
