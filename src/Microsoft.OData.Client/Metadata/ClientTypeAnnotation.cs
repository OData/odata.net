//---------------------------------------------------------------------
// <copyright file="ClientTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Edm;

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
        }

        /// <summary>if it is an IEdmEntityType</summary>
        internal bool IsEntityType
        {
            get { return this.EdmType.TypeKind == EdmTypeKind.Entity; }
        }

        /// <summary>if it is an IEdmStructuredType</summary>
        internal bool IsStructuredType
        {
            get
            {
                return this.EdmType.TypeKind == EdmTypeKind.Entity || this.EdmType.TypeKind == EdmTypeKind.Complex;
            }
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

        /// <summary> Gets the EdmTypeReference for the client Type annotation. </summary>
        internal IEdmTypeReference EdmTypeReference { get; private set; }

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
        /// <param name="undeclaredPropertyBehavior">UndeclaredPropertyBehavior</param>
        /// <returns>property wrapper</returns>
        /// <exception cref="InvalidOperationException">for unknown properties on closed types</exception>
        internal ClientPropertyAnnotation GetProperty(string propertyName, UndeclaredPropertyBehavior undeclaredPropertyBehavior)
        {
            Debug.Assert(propertyName != null, "property name");
            if (this.clientPropertyCache == null)
            {
                this.BuildPropertyCache();
            }

            ClientPropertyAnnotation property;

            if (!this.clientPropertyCache.TryGetValue(propertyName, out property))
            {
                string propertyClientName = ClientTypeUtil.GetClientPropertyName(this.ElementType, propertyName, undeclaredPropertyBehavior);
                if ((string.IsNullOrEmpty(propertyClientName) || !this.clientPropertyCache.TryGetValue(propertyClientName, out property)) && (undeclaredPropertyBehavior == UndeclaredPropertyBehavior.ThrowException))
                {
                    throw Microsoft.OData.Client.Error.InvalidOperation(Microsoft.OData.Client.Strings.ClientType_MissingProperty(this.ElementTypeName, propertyName));
                }
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
                Version clientTypeMetadataVersion = Util.ODataVersion4;
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
            // don't write property if it is tagged with IgnoreClientProperty attribute
            return !property.IsDictionary
                && property != type.MediaDataMember
                && !property.IsStreamLinkProperty
                && (type.MediaDataMember == null || type.MediaDataMember.MimeTypeProperty != property)
                && property.PropertyInfo.GetCustomAttributes(typeof(IgnoreClientPropertyAttribute)).Count() == 0;
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
                    throw Microsoft.OData.Client.Error.InvalidOperation(Microsoft.OData.Client.Strings.ClientType_MissingMediaEntryProperty(
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
        }

        /// <summary>
        /// Computes the metadata version of the property.
        /// </summary>
        /// <param name="propertyCollection">List of properties for which metadata version needs to be computed.</param>
        /// <param name="visitedComplexTypes">List of complex type already visited.</param>
        /// <returns>the metadata version of the property collection.</returns>
        private Version ComputeVersionForPropertyCollection(IEnumerable<IEdmProperty> propertyCollection, HashSet<IEdmType> visitedComplexTypes)
        {
            Version propertyMetadataVersion = Util.ODataVersion4;

            foreach (IEdmProperty property in propertyCollection)
            {
                ClientPropertyAnnotation propertyAnnotation = this.model.GetClientPropertyAnnotation(property);

                if (property.Type.TypeKind() == EdmTypeKind.Complex && !propertyAnnotation.IsDictionary)
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
    }
}
