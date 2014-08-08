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

namespace System.Data.Services.Client.Metadata
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    using c = System.Data.Services.Client;

    #endregion Namespaces

    /// <summary>
    /// Annotates a type on the client.
    /// </summary>
    [DebuggerDisplay("{ElementTypeName}")]
    internal sealed class ClientTypeAnnotation
    {
        /// <summary>Back reference to the EdmType this annotation is part of.</summary>
        internal readonly IEdmType EdmType;

        /// <summary>what is the clr full name using ToString for generic name expansion</summary>
        internal readonly string ElementTypeName;

        /// <summary>what clr type does this represent</summary>
        internal readonly Type ElementType;

        /// <summary>Storage for the client model.</summary>
        private readonly ClientEdmModel model;

        /// <summary>Set to true if the type is marked as ATOM-style media link entry</summary>
        private bool? isMediaLinkEntry;

        /// <summary>Property that holds data for ATOM-style media link entries</summary>
        private ClientPropertyAnnotation mediaDataMember;

        /// <summary>Whether any property (including properties on descendant types) of this type is a collection of primitive or complex types.</summary>
        private Version metadataVersion;

        /// <summary>object to manage and encapsulate the lazy loading of the EPM data.</summary>
        private EpmLazyLoader epmLazyLoader;

        /// <summary>Cached client properties.</summary>
        private Dictionary<String, ClientPropertyAnnotation> clientPropertyCache;

        /// <summary>Cached Edm properties</summary>
        private IEdmProperty[] edmPropertyCache;

        /// <summary>
        /// discover and prepare properties for usage
        /// </summary>
        /// <param name="edmType">Back reference to the EdmType this annotation is part of.</param>
        /// <param name="type">type being processed</param>
        /// <param name="qualifiedName">the qualified name of the type being processed</param>
        /// <param name="model">The client model.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal ClientTypeAnnotation(IEdmType edmType, Type type, string qualifiedName, ClientEdmModel model)
        {
            Debug.Assert(edmType != null, "edmType != null");
            Debug.Assert(null != type, "null type");
            Debug.Assert(!string.IsNullOrEmpty(qualifiedName), "!string.IsNullOrEmpty(qualifiedName)");

            this.EdmType = edmType;
            this.EdmTypeReference = this.EdmType.ToEdmTypeReference(Util.IsNullableType(type));
            this.ElementTypeName = qualifiedName;
            this.ElementType = Nullable.GetUnderlyingType(type) ?? type;
            this.model = model;
            this.epmLazyLoader = new EpmLazyLoader(this);
        }

        /// <summary>if true then EntityType else if !KnownType then ComplexType else PrimitiveType</summary>
        internal bool IsEntityType
        {
            get { return this.EdmType.TypeKind == EdmTypeKind.Entity; }
        }

        /// <summary>Property that holds data for ATOM-style media link entries</summary>
        internal ClientPropertyAnnotation MediaDataMember
        {
            get
            {
                if (!this.isMediaLinkEntry.HasValue)
                {
                    this.CheckMediaLinkEntry();
                    Debug.Assert(this.isMediaLinkEntry.HasValue, "this.isMediaLinkEntry.HasValue");
                }

                return this.mediaDataMember;
            }
        }

        /// <summary>Returns true if the type is marked as ATOM-style media link entry</summary>
        internal bool IsMediaLinkEntry
        {
            get
            {
                if (!this.isMediaLinkEntry.HasValue)
                {
                    this.CheckMediaLinkEntry();
                    Debug.Assert(this.isMediaLinkEntry.HasValue, "this.isMediaLinkEntry.HasValue");
                }

                return this.isMediaLinkEntry.Value;
            }
        }

        /// <summary>
        /// Target tree for <see cref="EntityPropertyMappingAttribute"/>s on this type
        /// </summary>
        internal System.Data.Services.Client.Serializers.EpmTargetTree EpmTargetTree
        {
            get { return this.epmLazyLoader.EpmTargetTree; }
        }

        /// <summary>Are there any entity property mappings on this type</summary>
        internal bool HasEntityPropertyMappings
        {
            get
            {
                return this.epmLazyLoader.EpmSourceTree.Root.SubProperties.Count > 0;
            }
        }

        /// <summary>The minimum DSVP required for EPM mappings</summary>
        internal DataServiceProtocolVersion EpmMinimumDataServiceProtocolVersion
        {
            get
            {
                if (!this.HasEntityPropertyMappings)
                {
                    return DataServiceProtocolVersion.V1;
                }
                else
                {
                    return this.EpmTargetTree.MinimumDataServiceProtocolVersion;
                }
            }
        }

        /// <summary> Gets the EdmTypeReference for the client Type annotation. </summary>
        internal IEdmTypeReference EdmTypeReference { get; private set; }

        /// <summary> Ensures that EPM is loaded </summary>
        internal void EnsureEPMLoaded()
        {
            this.epmLazyLoader.EnsureEPMLoaded();
        }

        /// <summary>
        /// Returns the list of EdmProperties for this type.
        /// </summary>
        /// <returns>Returns the list of EdmProperties for this type.</returns>
        internal IEnumerable<IEdmProperty> EdmProperties()
        {
            if (this.edmPropertyCache == null)
            {
                this.edmPropertyCache = this.DiscoverEdmProperties().ToArray();
            }

            return this.edmPropertyCache;
        }

        /// <summary>Returns the list of properties from this type.</summary>
        /// <returns>Returns the list of properties from this type.</returns>
        internal IEnumerable<ClientPropertyAnnotation> Properties()
        {
            if (this.clientPropertyCache == null)
            {
                this.BuildPropertyCache();
            }

            return this.clientPropertyCache.Values;
        }

        /// <summary>
        /// Gets the set of properties on this type that should be serialized into insert/update payloads.
        /// </summary>
        /// <returns>The properties to serialize.</returns>
        internal IEnumerable<ClientPropertyAnnotation> PropertiesToSerialize()
        {
            return this.Properties().Where(p => ShouldSerializeProperty(this, p)).OrderBy(p => p.PropertyName);
        }

        /// <summary>
        /// get property wrapper for a property name, might be method around open types for otherwise unknown properties
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="ignoreMissingProperties">are missing properties ignored</param>
        /// <returns>property wrapper</returns>
        /// <exception cref="InvalidOperationException">for unknown properties on closed types</exception>
        internal ClientPropertyAnnotation GetProperty(string propertyName, bool ignoreMissingProperties)
        {
            if (this.clientPropertyCache == null)
            {
                this.BuildPropertyCache();
            }

            ClientPropertyAnnotation property;

            if (!this.clientPropertyCache.TryGetValue(propertyName, out property) && !ignoreMissingProperties)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingProperty(this.ElementTypeName, propertyName));
            }

            return property;
        }

        /// <summary>
        /// Checks if any of the properties (including properties of descendant types) is a collection of primitive or complex types.
        /// </summary>
        /// <returns>true if any if any of the properties (including properties of descendant types) is a collection of primitive or complex types. Otherwise false.</returns>
        internal Version GetMetadataVersion()
        {
            if (this.metadataVersion == null)
            {
                Version clientTypeMetadataVersion = Util.DataServiceVersion1;
                WebUtil.RaiseVersion(ref clientTypeMetadataVersion, this.ComputeVersionForPropertyCollection(this.EdmProperties(), null /*visitedComplexTypes*/));
                this.metadataVersion = clientTypeMetadataVersion;
            }

            return this.metadataVersion;
        }

        /// <summary>
        /// Determines whether a given property should be serialized into an insert or update payload.
        /// </summary>
        /// <param name="type">The declaring type of the property.</param>
        /// <param name="property">The property.</param>
        /// <returns>Whether or not the property should be serialized.</returns>
        private static bool ShouldSerializeProperty(ClientTypeAnnotation type, ClientPropertyAnnotation property)
        {
            // don't write property if it is a dictionary
            // don't write mime data member or the mime type member for it
            // link properties need to be ignored
            return !property.IsDictionary
                && property != type.MediaDataMember
                && !property.IsStreamLinkProperty
                && (type.MediaDataMember == null || type.MediaDataMember.MimeTypeProperty != property);
        }

        /// <summary>
        /// build the clientPropertyCache from EdmProperties
        /// </summary>
        private void BuildPropertyCache()
        {
            // this is function is lazy loading the property cache, should only be called once per type annotation
            lock (this)
            {
                if (this.clientPropertyCache == null)
                {
                    var propertyCache = new Dictionary<string, ClientPropertyAnnotation>(StringComparer.Ordinal);
                    foreach (var p in this.EdmProperties())
                    {
                        propertyCache.Add(p.Name, this.model.GetClientPropertyAnnotation(p));
                    }

                    this.clientPropertyCache = propertyCache;
                }
            }
        }

        /// <summary>
        /// Check if this type represents an ATOM-style media link entry and
        /// if so mark the ClientType as such
        /// </summary>
        private void CheckMediaLinkEntry()
        {
            this.isMediaLinkEntry = false;

            // MediaEntryAttribute does not allow multiples, so there can be at most 1 instance on the type.
            MediaEntryAttribute mediaEntryAttribute = (MediaEntryAttribute)this.ElementType.GetCustomAttributes(typeof(MediaEntryAttribute), true).SingleOrDefault();
            if (mediaEntryAttribute != null)
            {
                this.isMediaLinkEntry = true;

                ClientPropertyAnnotation mediaProperty = this.Properties().SingleOrDefault(p => p.PropertyName == mediaEntryAttribute.MediaMemberName);
                if (mediaProperty == null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingMediaEntryProperty(
                        this.ElementTypeName, mediaEntryAttribute.MediaMemberName));
                }

                this.mediaDataMember = mediaProperty;
            }

            // HasStreamAttribute does not allow multiples, so there can be at most 1 instance on the type.
            bool hasStreamAttribute = this.ElementType.GetCustomAttributes(typeof(HasStreamAttribute), true).Any();
            if (hasStreamAttribute)
            {
                this.isMediaLinkEntry = true;
            }

            if (this.isMediaLinkEntry.HasValue && this.isMediaLinkEntry.Value)
            {
                this.SetMediaLinkEntryAnnotation();
            }
        }

        /// <summary>
        /// Set the HasStream boolean annotation in the EdmType to true.
        /// </summary>
        private void SetMediaLinkEntryAnnotation()
        {
            Debug.Assert(this.isMediaLinkEntry.HasValue && this.isMediaLinkEntry.Value, "This method must be called if the entity is an MLE");
            Debug.Assert(this.EdmType is EdmEntityType, "SetMediaLinkEntryAnnotation must be called for entity types");

            // Why is this not a property on ODataEdmTypeAnnotation ??
            ODataUtils.SetHasDefaultStream(this.model, (IEdmEntityType)this.EdmType, true);
        }

        /// <summary>
        /// Computes the metadata version of the property.
        /// </summary>
        /// <param name="propertyCollection">List of properties for which metadata version needs to be computed.</param>
        /// <param name="visitedComplexTypes">List of complex type already visited.</param>
        /// <returns>the metadata version of the property collection.</returns>
        private Version ComputeVersionForPropertyCollection(IEnumerable<IEdmProperty> propertyCollection, HashSet<IEdmType> visitedComplexTypes)
        {
            Version propertyMetadataVersion = Util.DataServiceVersion1;

            foreach (IEdmProperty property in propertyCollection)
            {
                ClientPropertyAnnotation propertyAnnotation = this.model.GetClientPropertyAnnotation(property);

                // Raise the property version to 3.0 if the property is of type
                // collection, Geography or Geometry type.
                if (propertyAnnotation.IsPrimitiveOrComplexCollection ||
                    propertyAnnotation.IsSpatialType)
                {
                    WebUtil.RaiseVersion(ref propertyMetadataVersion, Util.DataServiceVersion3);
                }
                else if (property.Type.TypeKind() == EdmTypeKind.Complex && !propertyAnnotation.IsDictionary)
                {
                    if (visitedComplexTypes == null)
                    {
                        visitedComplexTypes = new HashSet<IEdmType>(EqualityComparer<IEdmType>.Default);
                    }
                    else if (visitedComplexTypes.Contains(property.Type.Definition))
                    {
                        continue;
                    }

                    visitedComplexTypes.Add(property.Type.Definition);

                    WebUtil.RaiseVersion(ref propertyMetadataVersion, this.ComputeVersionForPropertyCollection(this.model.GetClientTypeAnnotation(property).EdmProperties(), visitedComplexTypes));
                }
            }

            return propertyMetadataVersion;
        }

        /// <summary>
        /// Discovers and returns edm properties for this type.
        /// </summary>
        /// <returns>Edm properties on this type.</returns>
        private IEnumerable<IEdmProperty> DiscoverEdmProperties()
        {
            IEdmStructuredType edmStructuredType = this.EdmType as IEdmStructuredType;
            if (edmStructuredType != null)
            {
                //// NOTE: We need to deal with property hiding here
                ////       If a property in a derived type is hiding a property in a base type
                ////       by using the 'new' keyword we treat this as a new property and
                ////       keep the property in the base type (where it has to be) as well as in
                ////       derived type (where it also has to be since it can even have a different property type).
                ////       When returning the Edm properties we thus have to start at the most derived type
                ////       and not report hidden properties from the base types.
                //// NOTE: EDM does not have a concept of property hiding so this an artifact of our mapping
                ////       from CLR types to EDM types.

                HashSet<string> propertyNames = new HashSet<string>(StringComparer.Ordinal);

                do
                {
                    foreach (var property in edmStructuredType.DeclaredProperties)
                    {
                        string propertyName = property.Name;
                        if (!propertyNames.Contains(propertyName))
                        {
                            propertyNames.Add(propertyName);
                            yield return property;
                        }
                    }

                    edmStructuredType = edmStructuredType.BaseType;
                }
                while (edmStructuredType != null);
            }
        }

        /// <summary>
        /// Class to encpsulate the lazy loading logic for EPM data
        /// </summary>
        private class EpmLazyLoader
        {
            /// <summary>Souce Epm mappings</summary>
            private System.Data.Services.Client.Serializers.EpmSourceTree epmSourceTree;

            /// <summary>Target Epm mappings</summary>
            private System.Data.Services.Client.Serializers.EpmTargetTree epmTargetTree;

            /// <summary>object to lock on when building the epm info</summary>
            private object epmDataLock = new object();
            
            /// <summary>the current client annotation that the mappings are for</summary>
            private ClientTypeAnnotation clientTypeAnnotation;

            /// <summary>
            /// Initializes a new instance of the <see cref="EpmLazyLoader"/> class.
            /// </summary>
            /// <param name="clientTypeAnnotation">The client type annotation.</param>
            internal EpmLazyLoader(ClientTypeAnnotation clientTypeAnnotation)
            {
                this.clientTypeAnnotation = clientTypeAnnotation;
            }

            /// <summary>
            /// Target tree for <see cref="EntityPropertyMappingAttribute"/>s on this type
            /// </summary>
            internal System.Data.Services.Client.Serializers.EpmTargetTree EpmTargetTree
            {
                get
                {
                    if (this.EpmNeedsInitializing)
                    {
                        this.InitializeAndBuildTree();
                    }

                    Debug.Assert(this.epmTargetTree != null, "Must have valid target tree");
                    return this.epmTargetTree;
                }
            }

            /// <summary>
            /// Source tree for <see cref="EntityPropertyMappingAttribute"/>s on this type
            /// </summary>
            internal System.Data.Services.Client.Serializers.EpmSourceTree EpmSourceTree
            {
                get
                {
                    if (this.EpmNeedsInitializing)
                    {
                        this.InitializeAndBuildTree();
                    }

                    return this.epmSourceTree;
                }
            }

            /// <summary>
            /// Determines if the Epm fields need initializing
            /// </summary>
            private bool EpmNeedsInitializing
            {
                get { return this.epmSourceTree == null || this.epmTargetTree == null; }
            }

            /// <summary>
            /// Ensures that the EPM data is loaded.
            /// </summary>
            internal void EnsureEPMLoaded()
            {
                if (this.EpmNeedsInitializing)
                {
                    this.InitializeAndBuildTree();
                }
            }

            /// <summary>
            /// By going over EntityPropertyMappingInfoAttribute(s) defined on the ElementType
            /// builds the corresponding EntityPropertyMappingInfo
            /// </summary>
            /// <param name="clientTypeAnnotation">The ClientTypeAnnotation to refer to</param>
            /// <param name="sourceTree">The source tree to populate.</param>
            /// <remarks>This method should be called after all properties are set on the edm type.</remarks>
            private static void BuildEpmInfo(ClientTypeAnnotation clientTypeAnnotation, System.Data.Services.Client.Serializers.EpmSourceTree sourceTree)
            {
                BuildEpmInfo(clientTypeAnnotation.ElementType, clientTypeAnnotation, sourceTree);
            }

            /// <summary>
            /// By going over EntityPropertyMappingInfoAttribute(s) defined on <paramref name="type"/>
            /// builds the corresponding EntityPropertyMappingInfo
            /// </summary>
            /// <param name="type">Type being looked at</param>
            /// <param name="clientTypeAnnotation">The ClientTypeAnnotation to refer to</param>
            /// <param name="sourceTree">The source tree to populate.</param>
            private static void BuildEpmInfo(Type type, ClientTypeAnnotation clientTypeAnnotation, System.Data.Services.Client.Serializers.EpmSourceTree sourceTree)
            {
                // EPM is only allowed on entity types.  Note that we can't throw now if the EPM attribute is on a complex type since we didn't throw when we ship V2.
                if (clientTypeAnnotation.IsEntityType)
                {
                    Type baseType = c.PlatformHelper.GetBaseType(type);
                    ClientTypeAnnotation baseClientTypeAnnotation = null;
                    ClientEdmModel clientEdmModel = clientTypeAnnotation.model;
                    ODataEntityPropertyMappingCollection mappings = null;

                    if (baseType != null && baseType != typeof(object))
                    {
                        // have CLR base type
                        if (((EdmStructuredType)clientTypeAnnotation.EdmType).BaseType == null)
                        {
                            // CLR base type is not an entity type - append its EPM onto the current type annotation
                            BuildEpmInfo(baseType, clientTypeAnnotation, sourceTree);
                            
                            // we should not create a new mapping in this case
                            mappings = clientEdmModel.GetAnnotationValue<ODataEntityPropertyMappingCollection>(clientTypeAnnotation.EdmType);
                        }
                        else
                        {
                            // CLR base type is an entity type, build EPM onto the base type annotation
                            baseClientTypeAnnotation = clientEdmModel.GetClientTypeAnnotation(baseType);
                            BuildEpmInfo(baseType, baseClientTypeAnnotation, sourceTree);
                        }
                    }

                    foreach (EntityPropertyMappingAttribute epmAttr in type.GetCustomAttributes(typeof(EntityPropertyMappingAttribute), false))
                    {
                        BuildEpmInfo(epmAttr, type, clientTypeAnnotation, sourceTree);

                        // Add these mappings to the ODataEntityPropertyMapping class so that
                        // ODataLib can consume them
                        if (mappings == null)
                        {
                            mappings = new ODataEntityPropertyMappingCollection();
                        }

                        mappings.Add(epmAttr);
                    }

                    if (mappings != null)
                    {
                        ODataEntityPropertyMappingCollection oldMappings = clientEdmModel.GetAnnotationValue<ODataEntityPropertyMappingCollection>(clientTypeAnnotation.EdmType);
                        if (oldMappings != null)
                        {
                            List<EntityPropertyMappingAttribute> exclusiveMappings = oldMappings.Where(oldM => !mappings.Any(newM => oldM.SourcePath == newM.SourcePath)).ToList();
                            foreach (EntityPropertyMappingAttribute epmAttr in exclusiveMappings)
                            {
                                mappings.Add(epmAttr);
                            }
                        }

                        clientEdmModel.SetAnnotationValue(clientTypeAnnotation.EdmType, mappings);
                    }
                }
            }

            /// <summary>
            /// Builds the EntityPropertyMappingInfo corresponding to an EntityPropertyMappingAttribute, also builds the delegate to
            /// be invoked in order to retrieve the property provided in the <paramref name="epmAttr"/>
            /// </summary>
            /// <param name="epmAttr">Source EntityPropertyMappingAttribute</param>
            /// <param name="definingType">ResourceType on which to look for the property</param>
            /// <param name="clientTypeAnnotation">The ClientTypeAnnotation to refer to</param>
            /// <param name="sourceTree">The source tree to populate.</param>
            private static void BuildEpmInfo(EntityPropertyMappingAttribute epmAttr, Type definingType, ClientTypeAnnotation clientTypeAnnotation, System.Data.Services.Client.Serializers.EpmSourceTree sourceTree)
            {
                sourceTree.Add(new System.Data.Services.Client.Serializers.EntityPropertyMappingInfo(epmAttr, definingType, clientTypeAnnotation));
            }

            /// <summary>
            /// Initializes the epm fields and builds the information into the fields
            /// </summary>
            private void InitializeAndBuildTree()
            {
                lock (this.epmDataLock)
                {
                    if (!this.EpmNeedsInitializing)
                    {
                        // already done
                        return;
                    }

                    // work on local objects so we don't set the fields and start handing out 
                    // unbuilt trees
                    System.Data.Services.Client.Serializers.EpmTargetTree localEpmTargetTree = new System.Data.Services.Client.Serializers.EpmTargetTree();
                    System.Data.Services.Client.Serializers.EpmSourceTree localEpmSourceTree = new System.Data.Services.Client.Serializers.EpmSourceTree(localEpmTargetTree);
                    BuildEpmInfo(this.clientTypeAnnotation, localEpmSourceTree);
                    localEpmSourceTree.Validate(this.clientTypeAnnotation);
                    localEpmTargetTree.Validate();

                    // now that we are all built we can set the real fields and hand them out
                    this.epmTargetTree = localEpmTargetTree;
                    this.epmSourceTree = localEpmSourceTree;
                }
            }
        }
    }
}
