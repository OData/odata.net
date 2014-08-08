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

#if ASTORIA_SERVER
namespace System.Data.Services.Serializers
#else
namespace System.Data.Services.Client.Serializers
#endif
{
    using System.Diagnostics;
    using System.Linq;
    using System.Data.Services.Client.Metadata;
    using System.Data.Services.Common;

#if ASTORIA_CLIENT
    using System.Data.Services.Client;
    using Microsoft.Data.Edm;
    using c = System.Data.Services.Client;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Client.Metadata.ClientTypeAnnotation;
#else
    using System.Collections.Generic;
    using System.Data.Services;
    using System.Data.Services.Providers;
    using c = System.Data.Services;
    using ClientTypeOrResourceType_Alias = System.Data.Services.Providers.ResourceType;
#endif

    /// <summary>
    /// Tree representing the sourceName properties in all the EntityPropertyMappingAttributes for a resource type
    /// </summary>
    internal sealed class EpmSourceTree
    {
        #region Fields

        /// <summary>Root of the tree</summary>
        private readonly EpmSourcePathSegment root;
        
        /// <summary><see cref="EpmTargetTree"/> corresponding to this tree</summary>
        private readonly EpmTargetTree epmTargetTree;

        #endregion

        /// <summary>Default constructor creates a null root</summary>
        /// <param name="epmTargetTree">Target xml tree</param>
        internal EpmSourceTree(EpmTargetTree epmTargetTree)
        {
            this.root = new EpmSourcePathSegment();
            this.epmTargetTree = epmTargetTree;
        }

        #region Properties

        /// <summary>
        /// Root of the tree
        /// </summary>
        internal EpmSourcePathSegment Root
        {
            get
            {
                return this.root;
            }
        }

        #endregion

#if ASTORIA_CLIENT
        /// <summary>
        /// Adds a path to the source and target tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the source path</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
#else
        /// <summary>
        /// Adds a path to the source and target tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the source path</param>
        /// <param name="declaredProperties">the declared properties for the currentType</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void Add(EntityPropertyMappingInfo epmInfo, IEnumerable<ResourceProperty> declaredProperties)
        {
            Dictionary<ResourceType, IEnumerable<ResourceProperty>> declaredPropertiesLookup = new Dictionary<ResourceType, IEnumerable<ResourceProperty>>(EqualityComparer<ResourceType>.Default);
            declaredPropertiesLookup.Add(epmInfo.ActualPropertyType, declaredProperties);
#endif
            EpmSourcePathSegment currentSourceSegment = this.Root;
            EpmSourcePathSegment foundSourceSegment = null;
            ClientTypeOrResourceType_Alias currentType = epmInfo.ActualPropertyType;

            Debug.Assert(epmInfo.PropertyValuePath != null && epmInfo.PropertyValuePath.Length > 0, "Must have been validated during EntityPropertyMappingAttribute construction");
            for (int sourcePropertyIndex = 0; sourcePropertyIndex < epmInfo.PropertyValuePath.Length; sourcePropertyIndex++)
            {
                string propertyName = epmInfo.PropertyValuePath[sourcePropertyIndex];

                if (propertyName.Length == 0)
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_InvalidSourcePath(epmInfo.DefiningType.Name, epmInfo.Attribute.SourcePath));
                }

#if ASTORIA_CLIENT
                currentType = GetPropertyType(currentType, propertyName);
#else
                currentType = GetPropertyType(currentType, propertyName, declaredPropertiesLookup);
#endif

                foundSourceSegment = currentSourceSegment.SubProperties.SingleOrDefault(e => e.PropertyName == propertyName);
                if (foundSourceSegment != null)
                {
                    currentSourceSegment = foundSourceSegment;
                }
                else
                {
                    EpmSourcePathSegment newSourceSegment = new EpmSourcePathSegment(propertyName);
                    currentSourceSegment.SubProperties.Add(newSourceSegment);
                    currentSourceSegment = newSourceSegment;
                }
            }

            // The last segment is the one being mapped from by the user specified attribute.
            // It must be a primitive type - we don't allow mappings of anything else than primitive properties directly.
            // Note that we can only verify this for non-open properties, for open properties we must assume it's a primitive type
            //   and we will make this check later during serialization again when we actually have the value of the property.
            if (currentType != null)
            {
#if ASTORIA_CLIENT
                if (!PrimitiveType.IsKnownNullableType(currentType.ElementType))
#else
                if (currentType.ResourceTypeKind != ResourceTypeKind.Primitive)
#endif
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_EndsWithNonPrimitiveType(currentSourceSegment.PropertyName));
                }
            }

            // Note that once we're here the EpmInfo we have is never the collection property itself, it's always either a non-collection property
            //   or collection item property.
            Debug.Assert(foundSourceSegment == null || foundSourceSegment.EpmInfo != null, "Can't have a leaf node in the tree without EpmInfo.");

            // Two EpmAttributes with same PropertyName in the same ResourceType, this could be a result of inheritance
            if (foundSourceSegment != null)
            {
                Debug.Assert(Object.ReferenceEquals(foundSourceSegment, currentSourceSegment), "currentSourceSegment variable should have been updated already to foundSourceSegment");

                // Check for duplicates on the same entity type
                Debug.Assert(foundSourceSegment.SubProperties.Count == 0, "If non-leaf, it means we allowed complex type to be a leaf node");
                if (foundSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo))
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_DuplicateEpmAttrsWithSameSourceName(epmInfo.Attribute.SourcePath, epmInfo.DefiningType.Name));
                }

                // In case of inheritance, we need to remove the node from target tree which was mapped to base type property
                this.epmTargetTree.Remove(foundSourceSegment.EpmInfo);
            }

            currentSourceSegment.EpmInfo = epmInfo;

            this.epmTargetTree.Add(epmInfo);
        }

#if ASTORIA_CLIENT
        /// <summary>Validates the source tree.</summary>
        /// <param name="resourceType">The resource type for which the validation is performed.</param>
        internal void Validate(ClientTypeOrResourceType_Alias resourceType)
        {
            Validate(this.Root, resourceType);
        }
#else
        /// <summary>Validates the source tree.</summary>
        /// <param name="resourceType">The resource type for which the validation is performed.</param>
        /// <param name="declaredProperties">The declaredProperties of the declaredPropertiesResourceType.</param>
        internal void Validate(ResourceType resourceType, IEnumerable<ResourceProperty> declaredProperties)
        {
            // use this so we don't call PropertiesDeclaredOnThisType recursively
            Dictionary<ResourceType, IEnumerable<ResourceProperty>> declaredPropertyLookup = new Dictionary<ResourceType, IEnumerable<ResourceProperty>>(EqualityComparer<ResourceType>.Default);
            declaredPropertyLookup.Add(resourceType, declaredProperties);
            Validate(this.Root, resourceType, declaredPropertyLookup);
        }
#endif
#if ASTORIA_CLIENT
        /// <summary>Validates the specified segment and all its subsegments.</summary>
        /// <param name="pathSegment">The path segment to validate.</param>
        /// <param name="resourceType">The resource type of the property represented by this segment (null for open properties).</param>
        private static void Validate(EpmSourcePathSegment pathSegment, ClientTypeOrResourceType_Alias resourceType)
#else
        /// <summary>Validates the specified segment and all its subsegments.</summary>
        /// <param name="pathSegment">The path segment to validate.</param>
        /// <param name="resourceType">The resource type of the property represented by this segment (null for open properties).</param>
        /// <param name="declaredPropertiesLookup">The dictionary to lookup and add declaredProperties associated with resourceTypes.</param>
        private static void Validate(EpmSourcePathSegment pathSegment, ClientTypeOrResourceType_Alias resourceType, Dictionary<ResourceType, IEnumerable<ResourceProperty>> declaredPropertiesLookup)
#endif
        {
            Debug.Assert(pathSegment != null, "pathSegment != null");
            foreach (EpmSourcePathSegment subSegment in pathSegment.SubProperties)
            {
#if ASTORIA_CLIENT
                ClientTypeOrResourceType_Alias subSegmentResourceType = GetPropertyType(resourceType, subSegment.PropertyName);

                // sometimes the previous call returns null,  WHY do we even bother
                // to continue on after we can't find a resourceType?
                Validate(subSegment, subSegmentResourceType);
#else
                ClientTypeOrResourceType_Alias subSegmentResourceType = GetPropertyType(resourceType, subSegment.PropertyName, declaredPropertiesLookup);

                // sometimes the previous call returns null,  WHY do we even bother
                // to continue on after we can't find a resourceType?
                Validate(subSegment, subSegmentResourceType, declaredPropertiesLookup);
#endif
            }
        }

#if ASTORIA_CLIENT
        /// <summary>
        /// Returns a client type of the property on the specified resource type.
        /// </summary>
        /// <param name="clientType">The client type to look for the property on.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <returns>The type of the property specified. Note that for collection properties this returns the type of the item of the collection property.</returns>
        private static ClientTypeAnnotation GetPropertyType(ClientTypeAnnotation clientType, string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            ClientPropertyAnnotation clientProperty = clientType.GetProperty(propertyName, true);
            if (clientProperty == null)
            {
                throw c.Error.InvalidOperation(c.Strings.EpmSourceTree_InaccessiblePropertyOnType(propertyName, clientType.ElementTypeName));
            }
            
            if (clientProperty.IsStreamLinkProperty)
            {
                throw c.Error.InvalidOperation(c.Strings.EpmSourceTree_NamedStreamCannotBeMapped(propertyName, clientType.ElementTypeName));
            }
            
            if (clientProperty.IsSpatialType)
            {
                throw c.Error.InvalidOperation(c.Strings.EpmSourceTree_SpatialTypeCannotBeMapped(propertyName, clientType.ElementTypeName));
            }

            if (clientProperty.IsPrimitiveOrComplexCollection)
            {
                throw c.Error.InvalidOperation(c.Strings.EpmSourceTree_CollectionPropertyCannotBeMapped(propertyName, clientType.ElementTypeName));
            }

            ClientEdmModel model = clientProperty.Model;
            IEdmType edmType1 = model.GetOrCreateEdmType(clientProperty.PropertyType);
            var edmType = edmType1;

            return model.GetClientTypeAnnotation(edmType);
        }

#else
        /// <summary>
        /// Looks up the declaredProperties in the dictionary, or gets them and adds to the dictionary
        /// </summary>
        /// <param name="resourceType">The resource type to look for the properties on.</param>
        /// <param name="declaredPropertiesLookup">The dictionary of declaredProperties.</param>
        /// <returns>The declaredProperties to use for the given resourcType.</returns>
        private static IEnumerable<ResourceProperty> GetDeclaredProperties(ResourceType resourceType, Dictionary<ResourceType, IEnumerable<ResourceProperty>> declaredPropertiesLookup)
        {
            Debug.Assert(resourceType != null, "resourceType cannot be null");
            Debug.Assert(declaredPropertiesLookup != null, "declaredPropertiesLookup cannot be null");
            
            IEnumerable<ResourceProperty> declaredProperties;
            if (!declaredPropertiesLookup.TryGetValue(resourceType, out declaredProperties))
            {
                declaredProperties = resourceType.PropertiesDeclaredOnThisType;
                declaredPropertiesLookup.Add(resourceType, declaredProperties);
            }
            
            return declaredProperties;
        }

        /// <summary>
        /// Returns a resource type of the property on the specified resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to look for the property on.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <param name="declaredPropertiesLookup">The dictionary of resourceTypes to declaredProperties.</param>
        /// <returns>The type of the property specified. Note that for collection properties this returns the type of the item of the collection property.</returns>
        private static ResourceType GetPropertyType(ResourceType resourceType, string propertyName, Dictionary<ResourceType, IEnumerable<ResourceProperty>> declaredPropertiesLookup)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            ResourceProperty resourceProperty = null;
            if (resourceType != null)
            {
                // try the passed declared properties first so we don't get endless recursion on the properties lazy load
                // code that we are being called from.
                resourceProperty = GetDeclaredProperties(resourceType, declaredPropertiesLookup).FirstOrDefault(p => p.Name == propertyName);
                if (resourceProperty == null && resourceType.BaseType != null)
                {
                    resourceProperty = resourceType.BaseType.TryResolvePropertyName(propertyName);
                }
            }

            if (resourceProperty != null)
            {
                if (resourceProperty.IsOfKind(ResourcePropertyKind.Collection))
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_CollectionPropertyCannotBeMapped(propertyName, resourceType.FullName));
                }
                
                if (resourceProperty.IsOfKind(ResourcePropertyKind.Stream))
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_NamedStreamCannotBeMapped(propertyName, resourceType.FullName));
                }
                
                if (resourceProperty.IsOfKind(ResourcePropertyKind.Primitive) && resourceProperty.ResourceType.InstanceType.IsSpatial())
                {
                    throw new InvalidOperationException(c.Strings.EpmSourceTree_SpatialTypeCannotBeMapped(propertyName, resourceType.FullName));
                }

                return resourceProperty.ResourceType;
            }
            else if (resourceType != null && !resourceType.IsOpenType)
            {
                throw new InvalidOperationException(c.Strings.EpmSourceTree_InaccessiblePropertyOnType(propertyName, resourceType.FullName));
            }

            return null;
        }
#endif
    }
}
