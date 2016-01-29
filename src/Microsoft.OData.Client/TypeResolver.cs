//---------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    /// <summary>
    /// Class which contains all the logic for resolving the type from the wire name.
    /// </summary>
    internal class TypeResolver
    {
        /// <summary>
        /// Dictionary instance to map the edm type name to the client type annotation.
        /// In V1/V2, we never used to cache this and call this during materialization. Hence 2 different contexts can resolve the same wire type
        /// differently and that would have been fine. Also, someone could have written a logic that might differ based on queries within the same context.
        /// For e.g. before executing a projection query, the user can set a flag so that the resolver behaves differently.
        /// Hence caching this locally for every request to simulate that behavior.</summary>
        private readonly IDictionary<string, ClientTypeAnnotation> edmTypeNameMap = new Dictionary<string, ClientTypeAnnotation>(StringComparer.Ordinal);

        /// <summary>The callback for resolving client CLR types.</summary>
        private readonly Func<string, Type> resolveTypeFromName;

        /// <summary>The callback for resolving server type names.</summary>
        private readonly Func<Type, string> resolveNameFromType;

        /// <summary>The client model.</summary>
        private readonly ClientEdmModel clientEdmModel;

        /// <summary>The service model, or null if one has not been provided.</summary>
        private readonly IEdmModel serviceModel;

        /// <summary>Indicates whether or not to skip the type assignability check.</summary>
        private bool skipTypeAssignabilityCheck;

        /// <summary>
        /// Creates an instance of TypeResolver class.
        /// </summary>
        /// <param name="model">The client model.</param>
        /// <param name="resolveTypeFromName">The callback to resolve client CLR types.</param>
        /// <param name="resolveNameFromType">The callback for resolving server type names.</param>
        /// <param name="serviceModel">The service model.</param>
        internal TypeResolver(ClientEdmModel model, Func<string, Type> resolveTypeFromName, Func<Type, string> resolveNameFromType, IEdmModel serviceModel)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(resolveTypeFromName != null, "resolveTypeFromName != null");
            Debug.Assert(resolveNameFromType != null, "resolveNameFromType != null");
            this.resolveTypeFromName = resolveTypeFromName;
            this.resolveNameFromType = resolveNameFromType;
            this.serviceModel = serviceModel;
            this.clientEdmModel = model;

            if (serviceModel != null && clientEdmModel != null)
            {
                if (clientEdmModel.EdmStructuredSchemaElements == null)
                {
                    clientEdmModel.EdmStructuredSchemaElements = serviceModel.SchemaElements.Where(se => se is IEdmStructuredType).ToList();
                }
                else
                {
                    foreach (var element in serviceModel.SchemaElements.Where(se => se is IEdmStructuredType))
                    {
                        if (!clientEdmModel.EdmStructuredSchemaElements.Contains(element))
                        {
                            clientEdmModel.EdmStructuredSchemaElements.Add(element);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the reader model.
        /// </summary>
        internal IEdmModel ReaderModel
        {
            get
            {
                // DEVNOTE: because the two instances are different types, both internal, do NOT use ternary or 
                // coalesce operators here (?: and ??). 
                // This is a known C# compiler issue which produces unverifiable IL causing a runtime exception
                // in partial trust environments.
                if (this.serviceModel != null)
                {
                    return this.serviceModel;
                }

                return this.clientEdmModel;
            }
        }

        /// <summary>
        /// In V2, in projection path, we do not use to check for assignability between the expected type
        /// and the type returned by the type resolver. This variable is used to track that scenario.
        /// If this is true, the current request is a projection request otherwise not.
        /// </summary>
        internal void IsProjectionRequest()
        {
            this.skipTypeAssignabilityCheck = true;
        }

        /// <summary>
        /// Resolves the client type that should be used for materialization.
        /// </summary>
        /// <param name="expectedType">Expected client clr type based on the API called.</param>
        /// <param name="readerTypeName">
        /// The name surfaced by the ODataLib reader. 
        /// If we have a server model, this will be a server type name that needs to be resolved. 
        /// If not, then this will already be a client type name.</param>
        /// <returns>The resolved annotation for the client type to materialize into.</returns>
        internal ClientTypeAnnotation ResolveTypeForMaterialization(Type expectedType, string readerTypeName)
        {
            // If its a collection, get the collection item name
            string collectionItemTypeName = WebUtil.GetCollectionItemWireTypeName(readerTypeName);
            if (collectionItemTypeName == null)
            {
                // Resolve the primitive type first
                PrimitiveType primitiveType;
                if (PrimitiveType.TryGetPrimitiveType(readerTypeName, out primitiveType))
                {
                    return this.clientEdmModel.GetClientTypeAnnotation(primitiveType.ClrType);
                }

                ClientTypeAnnotation resultType;
                if (this.edmTypeNameMap.TryGetValue(readerTypeName, out resultType))
                {
                    return resultType;
                }

                if (this.serviceModel != null)
                {
                    var resolvedType = this.ResolveTypeFromName(readerTypeName, expectedType);
                    return this.clientEdmModel.GetClientTypeAnnotation(resolvedType);
                }

                // If there was no type name specified in the payload, then the type resolver won't be invoked
                // and hence that edm type name might not be in the resolver cache. Hence look that up in the 
                // ClientEdmModel cache. This lookup is more expensive and is unique across the app domain for the
                // given version.
                return this.clientEdmModel.GetClientTypeAnnotation(readerTypeName);
            }

            Type collectionImplementationType = ClientTypeUtil.GetImplementationType(expectedType, typeof(ICollection<>));
            Type collectionElementType = collectionImplementationType.GetGenericArguments()[0];

            // In case of collection, the expectedType might be collection of nullable types (for e.g. ICollection<int?>). 
            // There is no way to know the nullability from the wireTypeName (For e.g. Collection(Edm.Int32)).
            // Hence in case of collections of primitives, we need to look at the element type of the expected type
            // and use that to create the instance otherwise we will not be able to assign the created ICollection<> 
            // instance to the property on the user's entity (ICollection<int> cannot be assigned to ICollection<int?>).
            // There is also no need to invoke the resolver for primitives, so we just use the element type.
            if (!PrimitiveType.IsKnownType(collectionElementType))
            {
                collectionElementType = this.ResolveTypeForMaterialization(collectionElementType, collectionItemTypeName).ElementType;
            }

            Type clrCollectionType = WebUtil.GetBackingTypeForCollectionProperty(expectedType);
            return this.clientEdmModel.GetClientTypeAnnotation(clrCollectionType);
        }

        /// <summary>
        /// ODL callback for client type resolution
        /// </summary>
        /// <param name="expectedEdmType">The expected type for the given wire name</param>
        /// <param name="wireName">The name of the type from the payload</param>
        /// <returns>An IEdmType</returns>
        internal IEdmType ResolveWireTypeName(IEdmType expectedEdmType, string wireName)
        {
            // ODataLib should never pass an empty or null type name
            Debug.Assert(!String.IsNullOrEmpty(wireName), "!String.IsNullOrEmpty(wireName)");

            // For V3 and above, ODataLib will never call the type resolver if there is a collection
            // type specified in the wire. However, in V1/V2, since there was no collection feature
            // supported, it will call us with a collection wire name, but its okay to return null
            // in that case, since there is no collection supported. If the user writes the type
            // resolver in such a way to handle collections themselver, even then it will fail later
            // in ODataLib stating collection types are not supported in V1/V2 versions.
            if (expectedEdmType != null)
            {
                // In V1/V2, we never used to call the type resolver for primitives types.
                // Instead, we just used to look at the expected property type and try to convert
                // the value from the payload to the expected property type. In other words, we
                // used to ignore the type name on the wire for primitive properties.
                if (expectedEdmType.TypeKind == EdmTypeKind.Primitive)
                {
                    return expectedEdmType;
                }
            }

            Type expectedType;
            if (expectedEdmType != null)
            {
                ClientTypeAnnotation expectedAnnotation = this.clientEdmModel.GetClientTypeAnnotation(expectedEdmType);
                Debug.Assert(expectedAnnotation != null, "expectedAnnotation != null");
                expectedType = expectedAnnotation.ElementType;
            }
            else
            {
                expectedType = typeof(object);
            }

            // Breaking change: we decided to validate against the resolved type if the type are not assignable.
            Type resolvedType = this.ResolveTypeFromName(wireName, expectedType);

            ClientTypeAnnotation resolvedTypeAnnotation = this.clientEdmModel.GetClientTypeAnnotation(this.clientEdmModel.GetOrCreateEdmType(resolvedType));
            Debug.Assert(resolvedTypeAnnotation != null, "result != null -- otherwise ClientType.Create returned null");

            IEdmType clientEdmType = resolvedTypeAnnotation.EdmType;
            EdmTypeKind typeKind = clientEdmType.TypeKind;
            if (typeKind == EdmTypeKind.Entity || typeKind == EdmTypeKind.Complex)
            {
                // If the edm type name is not present in the dictionary, add it to the map
                string edmTypeName = clientEdmType.FullName();
                if (!this.edmTypeNameMap.ContainsKey(edmTypeName))
                {
                    this.edmTypeNameMap.Add(edmTypeName, resolvedTypeAnnotation);
                }
            }

            return clientEdmType;
        }

        /// <summary>
        /// Resolves the expected EDM type full name to give to the ODataLib reader based on a client CLR type.
        /// </summary>
        /// <param name="clientClrType">The client side CLR type.</param>
        /// <returns>The resolved EDM type full name to provide to ODataLib.</returns>
        internal string ResolveServiceEntityTypeFullName(Type clientClrType)
        {
            Debug.Assert(clientClrType != null, "materializerType != null");

            IEdmType edmType = this.ResolveExpectedTypeForReading(clientClrType);

            return edmType != null ? edmType.FullName() : null;
        }

        /// <summary>
        /// Resolves the expected EDM type to give to the ODataLib reader based on a client CLR type.
        /// </summary>
        /// <param name="clientClrType">The client side CLR type.</param>
        /// <returns>The resolved EDM type to provide to ODataLib.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        internal IEdmType ResolveExpectedTypeForReading(Type clientClrType)
        {
            Debug.Assert(clientClrType != null, "materializerType != null");

            ClientTypeAnnotation clientTypeAnnotation = this.clientEdmModel.GetClientTypeAnnotation(clientClrType);

            IEdmType clientType = clientTypeAnnotation.EdmType;
            if (this.serviceModel == null)
            {
                return clientType;
            }

            // for primitive types there is no need for the server's metadata
            if (clientType.TypeKind == EdmTypeKind.Primitive)
            {
                return clientType;
            }

            if (clientType.TypeKind == EdmTypeKind.Collection)
            {
                IEdmTypeReference clientEdmElementType = ((IEdmCollectionType)clientType).ElementType;
                if (clientEdmElementType.IsPrimitive())
                {
                    return clientType;
                }

                var elementType = clientClrType.GetGenericArguments()[0];
                var elementEdmType = this.ResolveExpectedTypeForReading(elementType);

                if (elementEdmType == null)
                {
                    // If we can't determine an expected element type, for whatever reason, then we cannot construct an expected collection type.
                    return null;
                }

                return new EdmCollectionType(elementEdmType.ToEdmTypeReference(clientEdmElementType.IsNullable));
            }

            IEdmStructuredType serverType;
            if (!this.TryToResolveServerType(clientTypeAnnotation, out serverType))
            {
                // if we don't know the server type, but do have a server model, then simply read without an expected type.
                return null;
            }

            return serverType;
        }

        /// <summary>
        /// Determines whether or not the client type should be written for a property that is not defined on the server.
        /// DEVNOTE: If there is no server model, the declaring type is complex, or the server type cannot be
        /// found then the server type will be assumed to match the client type.
        /// This is done this way to prevent getting this wrong if the server property is defined, but we cannot find it for some reason.
        /// So if the types do not match, or we aren't able to align them, we will not write the type name, allowing the server to interpret it as the correct type.
        /// </summary>
        /// <param name="clientProperty">The client-side property.</param>
        /// <param name="serverTypeName">The server type name of the current entity.</param>
        /// <returns>True if the client property type should be written because the property definitely not defined on the server type.</returns>
        internal bool ShouldWriteClientTypeForOpenServerProperty(IEdmProperty clientProperty, string serverTypeName)
        {
            if (serverTypeName == null)
            {
                // if the server side type cannot be found, then assume that its types match the client types.
                return false;
            }

            if (this.serviceModel == null)
            {
                // assume client model matches server model
                return false;
            }

            var serverType = this.serviceModel.FindType(serverTypeName) as IEdmStructuredType;
            if (serverType == null)
            {
                // if the server side type cannot be found, then assume that its types match the client types.
                return false;
            }

            // if the property is not defined, then write the type name
            return serverType.FindProperty(clientProperty.Name) == null;
        }

        /// <summary>
        /// Tries to resolve the name of the base type of the given entity set if a server model has been provided.
        /// </summary>
        /// <param name="entitySetName">The name of the entity set.</param>
        /// <param name="serverTypeName">The server type name if one could be found.</param>
        /// <returns>Whether the type name could be found.</returns>
        internal bool TryResolveEntitySetBaseTypeName(string entitySetName, out string serverTypeName)
        {
            serverTypeName = null;
            if (this.serviceModel == null)
            {
                return false;
            }

            var entitySet = this.serviceModel.FindDeclaredEntitySet(entitySetName);
            if (entitySet != null)
            {
                serverTypeName = ExtensionMethods.FullName(entitySet.EntityType());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to resolve the name of a navigation property's target if a server model has been provided.
        /// </summary>
        /// <param name="serverSourceTypeName">The name of the server side source type.</param>
        /// <param name="navigationPropertyName">The name of the navigation property.</param>
        /// <param name="serverTypeName">The server type name if one could be found.</param>
        /// <returns>Whether the type name could be found.</returns>
        internal bool TryResolveNavigationTargetTypeName(string serverSourceTypeName, string navigationPropertyName, out string serverTypeName)
        {
            serverTypeName = null;
            if (this.serviceModel == null || serverSourceTypeName == null)
            {
                return false;
            }

            IEdmEntityType parentServerType = this.serviceModel.FindType(serverSourceTypeName) as IEdmEntityType;
            if (parentServerType == null)
            {
                return false;
            }

            IEdmNavigationProperty serverNavigation = parentServerType.FindProperty(navigationPropertyName) as IEdmNavigationProperty;
            if (serverNavigation == null)
            {
                return false;
            }

            IEdmTypeReference targetType = serverNavigation.Type;
            if (targetType.IsCollection())
            {
                targetType = targetType.AsCollection().ElementType();
            }

            serverTypeName = targetType.FullName();
            return true;
        }

        /// <summary>
        /// Tries to resolve the server type corresponding to the client type.
        /// </summary>
        /// <param name="clientTypeAnnotation">The client type annotation.</param>
        /// <param name="serverType">The server type, if the server type could be resolved.</param>
        /// <returns>Whether or not the server type could be resolved.</returns>
        private bool TryToResolveServerType(ClientTypeAnnotation clientTypeAnnotation, out IEdmStructuredType serverType)
        {
            Debug.Assert(this.serviceModel != null, "this.serviceModel != null");
            Debug.Assert(clientTypeAnnotation != null, "clientTypeAnnotation != null");

            var serverTypeName = this.resolveNameFromType(clientTypeAnnotation.ElementType);
            if (serverTypeName == null)
            {
                serverType = null;
                return false;
            }

            serverType = this.serviceModel.FindType(serverTypeName) as IEdmStructuredType;
            return serverType != null;
        }

        /// <summary>
        /// User hook to resolve name into a type.
        /// </summary>
        /// <param name="wireName">Name to resolve.</param>
        /// <param name="expectedType">Expected type for the name.</param>
        /// <returns>the type as returned by the resolver. If no resolver is registered or resolver returns null, expected type is returned.</returns>
        /// <exception cref="InvalidOperationException">if ResolveType function returns a type not assignable to the userType</exception>
        private Type ResolveTypeFromName(string wireName, Type expectedType)
        {
            Debug.Assert(!String.IsNullOrEmpty(wireName), "!String.IsNullOrEmpty(wireName)");
            Debug.Assert(expectedType != null, "userType != null");

            // If there is a mismatch between the wireName and expected type (For e.g. wireName is Edm.Int32 and expectedType is a complex type)
            // we will return Edm.Int32 from this method and ODatalib fails stating the type kind do not match.
            Type payloadType;
            if (!ClientConvert.ToNamedType(wireName, out payloadType))
            {
                payloadType = this.resolveTypeFromName(wireName);

                if (payloadType == null)
                {
                    // if the type resolution method returns null or the ResolveType property was not set
#if !PORTABLELIB
                    payloadType = ClientTypeCache.ResolveFromName(wireName, expectedType);
#else
                    payloadType = ClientTypeCache.ResolveFromName(wireName, expectedType, this.GetType());
#endif
                }

                if (!this.skipTypeAssignabilityCheck && (payloadType != null) && (!expectedType.IsAssignableFrom(payloadType)))
                {
                    // throw an exception if the type from the resolver is not assignable to the expected type
                    throw Error.InvalidOperation(Strings.Deserialize_Current(expectedType, payloadType));
                }
            }

            return payloadType ?? expectedType;
        }
    }
}
