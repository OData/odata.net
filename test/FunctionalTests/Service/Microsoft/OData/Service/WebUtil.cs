//---------------------------------------------------------------------
// <copyright file="WebUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Internal;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using Microsoft.Spatial;
    #endregion Namespaces

    /// <summary>Utility methods for this project.</summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
    internal static class WebUtil
    {
        /// <summary>Bindings Flags for public instance members.</summary>
        internal const BindingFlags PublicInstanceBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>A zero-length object array.</summary>
        internal static readonly object[] EmptyObjectArray = new object[0];

        /// <summary>A zero-length string array.</summary>
        internal static readonly string[] EmptyStringArray = new string[0];

        /// <summary>A zero-length keyValuePair of string and object.</summary>
        internal static readonly IEnumerable<KeyValuePair<string, object>> EmptyKeyValuePairStringObject = new KeyValuePair<string, object>[0];

        /// <summary>
        /// Collection of ExpandedWrapper types and their corresponding number of parameters
        /// </summary>
        private static readonly ExpandWrapperTypeWithIndex[] GenericExpandedWrapperTypes = new ExpandWrapperTypeWithIndex[]
            {
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,>), Index = 1 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,>), Index = 2 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,>), Index = 3 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,>), Index = 4 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,>), Index = 5 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,>), Index = 6 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,>), Index = 7 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,,>), Index = 8 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,,,>), Index = 9 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,,,,>), Index = 10 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,,,,,>), Index = 11 },
                new ExpandWrapperTypeWithIndex { Type = typeof(ExpandedWrapper<,,,,,,,,,,,,>), Index = 12 }
            };

        /// <summary>
        /// Applies the host specified in a request if available to the given <paramref name="baseUri"/>.
        /// </summary>
        /// <param name="baseUri">URI to update with host (and port) information.</param>
        /// <param name="requestHost">RequestMessage header (possibly null or empty)</param>
        /// <returns>The updated URI.</returns>
        internal static Uri ApplyHostHeader(Uri baseUri, string requestHost)
        {
            Debug.Assert(baseUri != null, "baseUri");
            if (!String.IsNullOrEmpty(requestHost))
            {
                UriBuilder builder = new UriBuilder(baseUri);
                string host;
                int port;
                if (GetHostAndPort(requestHost, baseUri.Scheme, out host, out port))
                {
                    builder.Host = host;
                    builder.Port = port;
                }
                else
                {
                    builder.Host = requestHost;
                }

                baseUri = builder.Uri;
            }

            return baseUri;
        }

        /// <summary>
        /// Checks the argument value for null and throw ArgumentNullException if it is null
        /// </summary>
        /// <typeparam name="T">type of the argument</typeparam>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        internal static T CheckArgumentNull<T>([ValidatedNotNull] T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw Error.ArgumentNull(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Checks the string argument value for empty or null and throw ArgumentNullException if it is null
        /// </summary>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        internal static string CheckStringArgumentNullOrEmpty([ValidatedNotNull] string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName, Strings.WebUtil_ArgumentNullOrEmpty);
            }

            return value;
        }

        /// <summary>
        /// Check whether the given value for ServiceOperationResultKind is valid. If not, throw argument exception.
        /// </summary>
        /// <param name="kind">value for ServiceOperationResultKind</param>
        /// <param name="parameterName">name of the parameter</param>
        /// <exception cref="ArgumentException">if the value is not valid.</exception>
        internal static void CheckServiceOperationResultKind(ServiceOperationResultKind kind, string parameterName)
        {
            if (kind < ServiceOperationResultKind.DirectValue ||
                kind > ServiceOperationResultKind.Void)
            {
                throw new ArgumentException(Strings.InvalidEnumValue(kind.GetType().Name), parameterName);
            }
        }

        /// <summary>
        /// Check whether the given value for ResourceTypeKind is valid. If not, throw argument exception.
        /// </summary>
        /// <param name="kind">value for ResourceTypeKind</param>
        /// <param name="parameterName">name of the parameter</param>
        /// <exception cref="ArgumentException">if the value is not valid.</exception>
        internal static void CheckResourceTypeKind(ResourceTypeKind kind, string parameterName)
        {
            if (kind < ResourceTypeKind.EntityType ||
                kind > ResourceTypeKind.EntityCollection)
            {
                throw new ArgumentException(Strings.InvalidEnumValue(kind.GetType().Name), parameterName);
            }
        }

        /// <summary>
        /// Checks that the given ResourceType's ResourceTypeKind is not Collection or EntityCollection, and throws an error
        /// saying that open properties cannot have collections if needed.
        /// </summary>
        /// <param name="resourceType">ResourceType to check.</param>
        /// <param name="propertyName">Name of the property that was assigned a value of type <paramref name="resourceType"/>.</param>
        internal static void CheckResourceNotCollectionForOpenProperty(ResourceType resourceType, string propertyName)
        {
            if (resourceType.ResourceTypeKind == ResourceTypeKind.Collection || resourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
            {
                throw DataServiceException.CreateSyntaxError(Strings.InvalidUri_OpenPropertiesCannotBeCollection(propertyName));
            }
        }

        /// <summary>Checks that the <paramref name="rights"/> are valid and throws an exception otherwise.</summary>
        /// <param name="rights">Value to check.</param>
        /// <param name="parameterName">Name of parameter for the exception message.</param>
        internal static void CheckResourceContainerRights(EntitySetRights rights, string parameterName)
        {
            if (rights < 0 || rights > EntitySetRights.All)
            {
                throw Error.ArgumentOutOfRange(parameterName);
            }
        }

        /// <summary>Checks that the <paramref name="rights"/> are valid and throws an exception otherwise.</summary>
        /// <param name="rights">Value to check.</param>
        /// <param name="parameterName">Name of parameter for the exception message.</param>
        internal static void CheckServiceOperationRights(ServiceOperationRights rights, string parameterName)
        {
            if (rights < 0 || rights > (ServiceOperationRights.All | ServiceOperationRights.OverrideEntitySetRights))
            {
                throw Error.ArgumentOutOfRange(parameterName);
            }
        }

        /// <summary>Checks that the <paramref name="rights"/> are valid and throws an exception otherwise.</summary>
        /// <param name="rights">Value to check.</param>
        /// <param name="parameterName">Name of parameter for the exception message.</param>
        internal static void CheckServiceActionRights(ServiceActionRights rights, string parameterName)
        {
            if (rights < 0 || rights > ServiceActionRights.Invoke)
            {
                throw Error.ArgumentOutOfRange(parameterName);
            }
        }

        /// <summary>Checks the specifid value for syntax validity.</summary>
        /// <param name="resourceExists">Whether syntax is valid.</param>
        /// <param name="identifier">segment indentifier for which the resource was null.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void CheckResourceExists(bool resourceExists, string identifier)
        {
            if (!resourceExists)
            {
                throw DataServiceException.CreateResourceNotFound(identifier);
            }
        }

        /// <summary>Checks the specific value for syntax validity.</summary>
        /// <param name="valid">Whether syntax is valid.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void CheckSyntaxValid(bool valid)
        {
            if (!valid)
            {
                throw DataServiceException.CreateSyntaxError();
            }
        }

        /// <summary>
        /// Try and resolve the path identifier as resource type
        /// </summary>
        /// <param name="provider">provider instance.</param>
        /// <param name="identifier">identifier as specified in the path.</param>
        /// <param name="previousSegmentResourceType">expected resource type.</param>
        /// <param name="previousSegmentIsTypeSegment">whether the previous path segment was a type identifier or not.</param>
        /// <returns>an instance of resource type with the same name as the identifier. If there is no resource type with the given name, returns null.</returns>
        internal static ResourceType ResolveTypeIdentifier(DataServiceProviderWrapper provider, string identifier, ResourceType previousSegmentResourceType, bool previousSegmentIsTypeSegment)
        {
            ResourceType targetResourceType = provider.TryResolveResourceType(identifier);
            if (targetResourceType != null)
            {
                if (previousSegmentIsTypeSegment)
                {
                    // If 2 identifiers are specified back to back, then we need to check and throw in that scenario
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier(identifier, previousSegmentResourceType.FullName));
                }

                // Currently we do not support upcasts at all. Since assignable checks for equality, its okay to specify
                // redundant casts to the same type.
                if (!previousSegmentResourceType.IsAssignableFrom(targetResourceType))
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_InvalidTypeIdentifier_MustBeASubType(identifier, previousSegmentResourceType.FullName));
                }
            }

            return targetResourceType;
        }

        /// <summary>
        /// Debug.Assert(Enum.IsDefined(typeof(T), value))
        /// </summary>
        /// <typeparam name="T">type of enum</typeparam>
        /// <param name="value">enum value</param>
        [Conditional("DEBUG")]
        internal static void DebugEnumIsDefined<T>(T value)
        {
            Debug.Assert(Enum.IsDefined(typeof(T), value), "enum value is not valid");
        }

        /// <summary>Disposes of <paramref name="o"/> if it implements <see cref="IDisposable"/>.</summary>
        /// <param name="o">Object to dispose, possibly null.</param>
        internal static void Dispose(object o)
        {
            IDisposable disposable = o as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>Adds an empty last segment as necessary to the specified <paramref name="absoluteUri"/>.</summary>
        /// <param name="absoluteUri">An absolute URI.</param>
        /// <returns><paramref name="absoluteUri"/> with an empty last segment (ie, "ending with '/'").</returns>
        internal static Uri EnsureLastSegmentEmpty(Uri absoluteUri)
        {
            Debug.Assert(absoluteUri != null, "absoluteUri != null");
            Debug.Assert(absoluteUri.IsAbsoluteUri, "absoluteUri.IsAbsoluteUri");
            string[] segments = absoluteUri.Segments;
            if (segments.Length > 0)
            {
                string lastBaseSegment = segments[segments.Length - 1];
                if (lastBaseSegment.Length > 0 && lastBaseSegment[lastBaseSegment.Length - 1] != '/')
                {
                    absoluteUri = new Uri(absoluteUri, lastBaseSegment + "/");
                }
            }

            return absoluteUri;
        }

        /// <summary>Gets the public name for the specified <paramref name='type' />.</summary>
        /// <param name='type'>Type to get name for.</param>
        /// <returns>A public name for the specified <paramref name='type' />, empty if it cannot be found.</returns>
        internal static string GetTypeName(Type type)
        {
            Debug.Assert(type != null, "type != null");

            // we should not expose .net System.* types, instead, we should use Edm.* type if one can be found.
            ResourceType primitiveType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(type);
            return primitiveType != null ? primitiveType.FullName : type.FullName;
        }

        /// <summary>Marks the fact that a recursive method was entered, and checks that the depth is allowed.</summary>
        /// <param name="recursionLimit">Maximum recursion limit.</param>
        /// <param name="recursionDepth">Depth of recursion.</param>
        internal static void RecurseEnterQueryParser(int recursionLimit, ref int recursionDepth)
        {
            recursionDepth++;
            Debug.Assert(recursionDepth <= recursionLimit, "recursionDepth <= recursionLimit");
            if (recursionDepth == recursionLimit)
            {
                throw DataServiceException.CreateDeepRecursion_General();
            }
        }

        /// <summary>Marks the fact that a recursive method was entered, and checks that the depth is allowed.</summary>
        /// <param name="recursionLimit">Maximum recursion limit.</param>
        /// <param name="recursionDepth">Depth of recursion.</param>
        internal static void RecurseEnter(int recursionLimit, ref int recursionDepth)
        {
            recursionDepth++;
            Debug.Assert(recursionDepth <= recursionLimit, "recursionDepth <= recursionLimit");
            if (recursionDepth == recursionLimit)
            {
                throw DataServiceException.CreateDeepRecursion(recursionLimit);
            }
        }

        /// <summary>Marks the fact that a recursive method is leaving.</summary>
        /// <param name="recursionDepth">Depth of recursion.</param>
        internal static void RecurseLeave(ref int recursionDepth)
        {
            recursionDepth--;
            Debug.Assert(0 <= recursionDepth, "0 <= recursionDepth");
        }

        /// <summary>Converts comma-separated entries with no quotes into a text array.</summary>
        /// <param name="text">Text to convert.</param>
        /// <returns>A string array that represents the comma-separated values in the text.</returns>
        /// <remarks>This method can be used to provide a simpler API facade instead of identifier arrays.</remarks>
        internal static string[] StringToSimpleArray(string text)
        {
            return String.IsNullOrEmpty(text)
                ? EmptyStringArray
                : text.Split(new char[] { ',' }, StringSplitOptions.None);
        }

        /// <summary>
        /// Test if any of the types in the hierarchy of <paramref name="baseType"/> is a Media Link Entry.
        /// </summary>
        /// <param name="baseType">base type of the hierarchy</param>
        /// <param name="provider">IDataServiceMetadataProvider interface instance</param>
        /// <returns>Returns true if <paramref name="baseType"/> or at least one of its descendants is a Media Link Entry.</returns>
        internal static bool HasMediaLinkEntryInHierarchy(ResourceType baseType, DataServiceProviderWrapper provider)
        {
            return baseType.IsMediaLinkEntry || provider.GetDerivedTypes(baseType).Any(derivedType => derivedType.IsMediaLinkEntry);
        }

        /// <summary>copy from one stream to another</summary>
        /// <param name="input">input stream</param>
        /// <param name="output">output stream</param>
        /// <param name="buffer">reusable buffer</param>
        /// <returns>count of copied bytes</returns>
        internal static long CopyStream(Stream input, Stream output, byte[] buffer)
        {
            Debug.Assert(null != input, "null input stream");
            Debug.Assert(input.CanRead, "input.CanRead");
            Debug.Assert(null != output, "null output stream");
            Debug.Assert(output.CanWrite, "output.CanWrite");
            Debug.Assert(buffer != null, "buffer != null");

            long total = 0;
            int count;
            while (0 < (count = input.Read(buffer, 0, buffer.Length)))
            {
                output.Write(buffer, 0, count);
                total += count;
            }

            return total;
        }

        /// <summary>copy from one stream to another</summary>
        /// <param name="input">input stream</param>
        /// <param name="output">output stream</param>
        /// <param name="bufferSize">size of buffer to use during copying. If 0 is specified, the default of 64K will be used.</param>
        /// <returns>count of copied bytes</returns>
        internal static long CopyStream(Stream input, Stream output, int bufferSize)
        {
            // 64K = 65536 bytes.
            const int DefaultBufferSize = 65536;

            byte[] buffer = new byte[bufferSize <= 0 ? DefaultBufferSize : bufferSize];

            return CopyStream(input, output, buffer);
        }

        /// <summary>
        /// Creates a delegate that when called creates a new instance of the specified <paramref name="type" />.
        /// </summary>
        /// <param name="type">Type of the instance.</param>
        /// <param name="fullName">full name of the given clr type.
        /// If the type name is not specified, it takes the full name from the clr type.</param>
        /// <param name="targetType">Type to return from the delegate.</param>
        /// <returns>A delegate that when called creates a new instance of the specified <paramref name="type" />.</returns>
        internal static Delegate CreateNewInstanceConstructor(Type type, string fullName, Type targetType)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(targetType != null, "targetType != null");

            // Create the new instance of the type
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            if (emptyConstructor == null)
            {
                fullName = fullName ?? type.FullName;
                throw new InvalidOperationException(Strings.NoEmptyConstructorFoundForType(fullName));
            }

            DynamicMethod method = new DynamicMethod("invoke_constructor", targetType, Type.EmptyTypes, false);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, emptyConstructor);
            if (targetType.IsValueType)
            {
                generator.Emit(OpCodes.Box);
            }

            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<>).MakeGenericType(targetType));
        }

        /// <summary>Checks whether the specified type is a known primitive type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is known to be a primitive type; false otherwise.</returns>
        internal static bool IsPrimitiveType(Type type)
        {
            return type != null && PrimitiveResourceTypeMap.TypeMap.IsPrimitive(type);
        }

        /// <summary>
        /// Try to resolve a type by name by first trying primitive types and then provider's types
        /// </summary>
        /// <param name="provider">Provider to resolve non-primitive types against</param>
        /// <param name="typeName">Type name</param>
        /// <returns>ResourceType object for this type, or null if none found</returns>
        internal static ResourceType TryResolveResourceType(DataServiceProviderWrapper provider, string typeName)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(typeName != null, "typeName != null");

            ResourceType resourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(typeName) ??
                                        provider.TryResolveResourceType(typeName);

            return resourceType;
        }

        /// <summary>
        /// Get a primitive or EDM type from an instance
        /// </summary>
        /// <param name="provider">Provider to get EDM types from, in case <paramref name="obj"/> is not a primitive.</param>
        /// <param name="obj">Instance to get the type from</param>
        /// <returns>A ResourceType for this instance or null if it is not a known type</returns>
        internal static ResourceType GetResourceType(DataServiceProviderWrapper provider, object obj)
        {
            Debug.Assert(obj != null, "obj != null");
            Debug.Assert(provider != null, "provider != null");

            // Note that GetNonPrimitiveResourceType() will throw if we fail to get the resource type.
            ResourceType r = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(obj.GetType()) ??
                             GetNonPrimitiveResourceType(provider, obj);

            return r;
        }

        /// <summary>
        /// Get the non primitive type resource and checks that the given instance represents a single resource.
        /// </summary>
        /// <param name="provider">underlying data source.</param>
        /// <param name="obj">instance of the resource.</param>
        /// <returns>returns the resource type representing the given resource instance.</returns>
        internal static ResourceType GetNonPrimitiveResourceType(DataServiceProviderWrapper provider, object obj)
        {
            Debug.Assert(obj != null, "obj != null");

            IProjectedResult projectedResult = obj as IProjectedResult;
            ResourceType resourceType = projectedResult != null
                ? (String.IsNullOrEmpty(projectedResult.ResourceTypeName) ? null : provider.TryResolveResourceType(projectedResult.ResourceTypeName)) :
                provider.GetResourceType(obj);

            if (resourceType == null)
            {
                CollectionPropertyValueEnumerable collectionValueEnumerable = obj as CollectionPropertyValueEnumerable;
                if (collectionValueEnumerable != null)
                {
                    return collectionValueEnumerable.ResourceType;
                }

                throw new DataServiceException(500, Strings.BadProvider_InvalidTypeSpecified(obj.GetType().FullName));
            }

            return resourceType;
        }

        /// <summary>
        /// Returns the ItemType if <paramref name="type"/> is a EntityCollectionResourceType or a CollectionResourceType,
        /// otherwise return <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Resource type in question.</param>
        /// <returns>Returns the ItemType if <paramref name="type"/> is a EntityCollectionResourceType or a CollectionResourceType,
        /// otherwise return <paramref name="type"/>.</returns>
        internal static ResourceType ElementType(this ResourceType type)
        {
            if (type.ResourceTypeKind == ResourceTypeKind.EntityCollection)
            {
                return ((EntityCollectionResourceType)type).ItemType;
            }
            else if (type.ResourceTypeKind == ResourceTypeKind.Collection)
            {
                return ((CollectionResourceType)type).ItemType;
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="mimeType"/>
        /// is a valid MIME type with no parameters.
        /// </summary>
        /// <param name="mimeType">Simple MIME type.</param>
        /// <returns>
        /// true if the specified <paramref name="mimeType"/> is valid; 
        /// false otherwise.
        /// </returns>
        /// <remarks>
        /// See http://tools.ietf.org/html/rfc2045#section-5.1 for futher details.
        /// </remarks>
        internal static bool IsValidMimeType(string mimeType)
        {
            Debug.Assert(mimeType != null, "mimeType != null");
            const string Tspecials = "()<>@,;:\\\"/[]?=";
            bool partFound = false;
            bool slashFound = false;
            bool subTypeFound = false;
            foreach (char c in mimeType)
            {
                Debug.Assert(partFound || !slashFound, "partFound || !slashFound -- slashFound->partFound");
                Debug.Assert(slashFound || !subTypeFound, "slashFound || !subTypeFound -- subTypeFound->slashFound");

                if (c == '/')
                {
                    if (!partFound || slashFound)
                    {
                        return false;
                    }

                    slashFound = true;
                }
                else if (c < '\x20' || c > '\x7F' || c == ' ' || Tspecials.IndexOf(c) >= 0)
                {
                    return false;
                }
                else
                {
                    if (slashFound)
                    {
                        subTypeFound = true;
                    }
                    else
                    {
                        partFound = true;
                    }
                }
            }

            return subTypeFound;
        }

        /// <summary>
        /// Checks whether the specified element is an <see cref="IEnumerable"/>
        /// of other elements.
        /// </summary>
        /// <param name="element">Element to check (possibly null).</param>
        /// <param name="enumerable"><paramref name="element"/>, or null if <see cref="IEnumerable"/> is not supported.</param>
        /// <returns>
        /// true if <paramref name="element"/> supports IEnumerable and is not
        /// a primitive type (strings and byte arrays are also enumerables, but
        /// they shouldn't be iterated over, so they return false).
        /// </returns>
        internal static bool IsElementIEnumerable(object element, out IEnumerable enumerable)
        {
            enumerable = element as IEnumerable;

            if (enumerable == null)
            {
                return false;
            }

            // Primitive types are atomic, not enumerable, even if they implement IEnumerable.
            Type elementType = element.GetType();
            return !PrimitiveResourceTypeMap.TypeMap.IsPrimitive(elementType);
        }

        /// <summary>
        /// Returns false if the given etag value is not valid.
        /// Look in http://www.ietf.org/rfc/rfc2616.txt?number=2616 (Section 14.26) for more information
        /// </summary>
        /// <param name="etag">etag value to be checked.</param>
        /// <param name="allowStrongEtag">true if we allow strong etag values.</param>
        /// <returns>returns true if the etag value is valid, otherwise returns false.</returns>
        internal static bool IsETagValueValid(string etag, bool allowStrongEtag)
        {
            if (String.IsNullOrEmpty(etag) || etag == XmlConstants.HttpAnyETag)
            {
                return true;
            }

            // HTTP RFC 2616, section 3.11:
            //   entity-tag = [ weak ] opaque-tag
            //   weak       = "W/"
            //   opaque-tag = quoted-string
            int etagValueStartIndex = 1;
            if (etag.StartsWith(XmlConstants.HttpWeakETagPrefix, StringComparison.Ordinal) && etag[etag.Length - 1] == '"')
            {
                etagValueStartIndex = 3;
            }
            else if (!allowStrongEtag || etag[0] != '"' || etag[etag.Length - 1] != '"')
            {
                return false;
            }

            for (int i = etagValueStartIndex; i < etag.Length - 1; i++)
            {
                // Format of etag looks something like: W/"etag property values" or "strong etag value"
                // according to HTTP RFC 2616, if someone wants to specify more than 1 etag value,
                // then need to specify something like this: W/"etag values", W/"etag values", ...
                // To make sure only one etag is specified, we need to ensure that if " is part of the
                // key value, it needs to be escaped.
                if (etag[i] == '"')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Checks whether the specified <paramref name='type' /> can be assigned null.</summary>
        /// <param name='type'>Type to check.</param>
        /// <returns>true if type is a reference type or a Nullable type; false otherwise.</returns>
        internal static bool TypeAllowsNull(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return !type.IsValueType || IsNullableType(type);
        }

        /// <summary>Gets a type for <paramref name="type"/> that allows null values.</summary>
        /// <param name="type">Type to base resulting type on.</param>
        /// <returns>
        /// <paramref name="type"/> if it's a reference or Nullable&lt;&gt; type;
        /// Nullable&lt;<paramref name="type"/>&gt; otherwise.
        /// </returns>
        internal static Type GetTypeAllowingNull(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return TypeAllowsNull(type) ? type : typeof(Nullable<>).MakeGenericType(type);
        }

        /// <summary>Checks whether the specified type is a generic nullable type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is nullable; false otherwise.</returns>
        internal static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>Returns the etag for the given resource.</summary>
        /// <param name="resource">Resource for which etag value needs to be returned.</param>
        /// <param name="resourceType">Resource type of the <paramref name="resource"/>.</param>
        /// <param name="etagProperties">list of etag properties for the given resource</param>
        /// <param name="service">Service to which the request was made.</param>
        /// <param name="getMethod">whether the request was a get method or not.</param>
        /// <returns>ETag value for the given resource, with values encoded for use in a URI.</returns>
        internal static string GetETagValue(object resource, ResourceType resourceType, ICollection<ResourceProperty> etagProperties, IDataService service, bool getMethod)
        {
            Debug.Assert(etagProperties.Count != 0, "etagProperties.Count != 0");

            StringBuilder resultBuilder = new StringBuilder();
            bool firstKey = true;
            resultBuilder.Append(XmlConstants.HttpWeakETagPrefix);
            foreach (ResourceProperty property in etagProperties)
            {
                object keyValue;

                // We need to call IUpdatable.GetValue, if we are still trying to get
                // property value as part of the update changes. If the CUD operation
                // is done (i.e. IUpdatable.SaveChanges) have been called, and if we
                // need to compute the etag, we go via the IDSP.GetPropertyValue.
                // This was the V1 behavior and not changing this now.
                // The getMethod variable name is misleading, since this might be true
                // even for CUD operations, but only after SaveChanges is called.
                if (getMethod)
                {
                    keyValue = GetPropertyValue(service.Provider, resource, resourceType, property, null);
                }
                else
                {
                    keyValue = service.Updatable.GetValue(resource, property.Name);
                }

                if (firstKey)
                {
                    firstKey = false;
                }
                else
                {
                    resultBuilder.Append(',');
                }

                string etagValueText;
                if (keyValue == null)
                {
                    etagValueText = XmlConstants.NullLiteralInETag;
                }
                else
                {
                    etagValueText = LiteralFormatter.ForETag.Format(keyValue);
                }

                Debug.Assert(etagValueText != null, "keyValueText != null - otherwise TryKeyPrimitiveToString returned true and null value");
                resultBuilder.Append(etagValueText);
            }

            resultBuilder.Append('"');
            return resultBuilder.ToString();
        }

        /// <summary>Returns the etag for the given resource.</summary>
        /// <param name="service">Data service to which the request was made.</param>
        /// <param name="resource">Resource for which etag value needs to be returned.</param>
        /// <param name="resourceType">ResourceType instance containing metadata about <paramref name="resource"/>.</param>
        /// <param name="container">resource set to which the resource belongs to.</param>
        /// <returns>ETag value for the given resource, with values encoded for use in a URI.</returns>
        internal static string GetETagValue(IDataService service, object resource, ResourceType resourceType, ResourceSetWrapper container)
        {
            ICollection<ResourceProperty> etagProperties = service.Provider.GetETagProperties(container.Name, resourceType);
            if (etagProperties.Count != 0)
            {
                return GetETagValue(resource, resourceType, etagProperties, service, true /*getMethod*/);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerator"/> for the specified <paramref name="enumerable"/>,
        /// mapping well-known exceptions to the appropriate HTTP status code.
        /// </summary>
        /// <param name="enumerable">Request enumerable to get enumerator for.</param>
        /// <returns>An <see cref="IEnumerator"/> for the specified <paramref name="enumerable"/>.</returns>
        internal static IEnumerator GetRequestEnumerator(IEnumerable enumerable)
        {
            Debug.Assert(enumerable != null, "enumerable != null");
            try
            {
                return enumerable.GetEnumerator();
            }
            catch (NotImplementedException e)
            {
                // 501: Not Implemented
                throw new DataServiceException(501, null, Strings.DataService_NotImplementedException, null, e);
            }
            catch (NotSupportedException e)
            {
                // 501: Not Implemented
                throw new DataServiceException(501, null, Strings.DataService_NotImplementedException, null, e);
            }
        }

        /// <summary>
        /// Given the request description, query for the parent entity resource
        /// and compare the etag, if specified in the header
        /// </summary>
        /// <param name="parentEntityResource">entity resource for which etag needs to be checked.</param>
        /// <param name="parentEntityToken">token as returned by the IUpdatable interface methods.</param>
        /// <param name="container">container to which the entity resource belongs to.</param>
        /// <param name="service">Underlying service to which the request was made to.</param>
        /// <param name="writeResponseForGetMethods">out bool which indicates whether response needs to be written for GET operations</param>
        /// <returns>current etag value for the given entity resource.</returns>
        internal static string CompareAndGetETag(
            object parentEntityResource,
            object parentEntityToken,
            ResourceSetWrapper container,
            IDataService service,
            out bool writeResponseForGetMethods)
        {
            Debug.Assert(service.OperationContext.RequestMessage != null, "service.OperationContext.RequestMessage != null");
            AstoriaRequestMessage host = service.OperationContext.RequestMessage;

            // If this method is called for Update, we need to pass the token object as well as the actual instance.
            // The actual instance is used to determine the type that's necessary to find out the etag properties.
            // The token is required to pass back to IUpdatable interface, if we need to get the values for etag properties.
            Debug.Assert(host.HttpVerb.IsQuery(), "this method must be called for read-only operations only");

            writeResponseForGetMethods = true;
            string etag = null;

            // For .e.g when you are querying for /Customers(1)/BestFriend, the value can be null.
            // Hence in this case, if the If-Match header value is specified, we throw.
            if (parentEntityResource == null)
            {
                if (!String.IsNullOrEmpty(host.GetRequestIfMatchHeader()))
                {
                    throw DataServiceException.CreatePreConditionFailedError(Strings.Serializer_ETagValueDoesNotMatch);
                }
            }
            else
            {
                ResourceType resourceType = GetNonPrimitiveResourceType(service.Provider, parentEntityResource);
                ICollection<ResourceProperty> etagProperties = service.Provider.GetETagProperties(container.Name, resourceType);
                if (etagProperties.Count == 0)
                {
                    // Cannot specify etag for types that don't have etag properties.
                    if (!String.IsNullOrEmpty(host.GetRequestIfMatchHeader()))
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.Serializer_NoETagPropertiesForType);
                    }
                }
                else if (String.IsNullOrEmpty(host.GetRequestIfMatchHeader()) && String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()))
                {
                    // no need to check anything if no header is specified.
                }
                else if (host.GetRequestIfMatchHeader() == XmlConstants.HttpAnyETag)
                {
                    // just return - for put, perform the operation and get, return the payload
                }
                else if (host.GetRequestIfNoneMatchHeader() == XmlConstants.HttpAnyETag)
                {
                    // If-None-Match is not allowed for PUT. Hence there is no point checking that
                    // For GET, return Not Modified
                    writeResponseForGetMethods = false;
                }
                else
                {
                    etag = GetETagValue(parentEntityToken, resourceType, etagProperties, service, true /*getMethod*/);
                    if (String.IsNullOrEmpty(host.GetRequestIfMatchHeader()))
                    {
                        Debug.Assert(!String.IsNullOrEmpty(host.GetRequestIfNoneMatchHeader()), "Both can't be null, otherwise it should have entered the first condition");
                        if (host.GetRequestIfNoneMatchHeader() == etag)
                        {
                            writeResponseForGetMethods = false;
                        }
                    }
                    else if (etag != host.GetRequestIfMatchHeader())
                    {
                        throw DataServiceException.CreatePreConditionFailedError(Strings.Serializer_ETagValueDoesNotMatch);
                    }
                }

                if (etag == null && etagProperties.Count != 0)
                {
                    etag = GetETagValue(parentEntityResource, resourceType, etagProperties, service, true /*getMethod*/);
                }
            }

            return etag;
        }

        /// <summary>
        /// Write the etag header value in the response
        /// </summary>
        /// <param name="requestDescription">description about the request made</param>
        /// <param name="etagValue">etag value that needs to be written.</param>
        /// <param name="responseMessage">responseMessage implementation for this data service.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "requestDescription", Justification = "Intended to only be used in debug builds.")]
        internal static void WriteETagValueInResponseHeader(RequestDescription requestDescription, string etagValue, IODataResponseMessage responseMessage)
        {
            if (!String.IsNullOrEmpty(etagValue))
            {
#if DEBUG
                // asserting that etag response header is written only in cases when the etag request headers are allowed.
                Debug.Assert(requestDescription == null || requestDescription.IsETagHeaderAllowed, "etag should not be computed before serialization time if etag response header is not allowed");
                Debug.Assert(requestDescription == null || IsETagValueValid(etagValue, requestDescription.TargetKind == RequestTargetKind.MediaResource), "WebUtil.IsETagValueValid(etagValue)");
                Debug.Assert(String.IsNullOrEmpty(responseMessage.GetHeader(XmlConstants.HttpResponseETag)), "string.IsNullOrEmpty(responseMessage.GetHeader(XmlConstants.HttpResponseETag))");
#endif
                responseMessage.SetHeader(XmlConstants.HttpResponseETag, etagValue);
            }
        }

        /// <summary>
        /// If the specified reader is not on an element, advances to one, skipping document declaration
        /// nodes (typically at the beginning of a document), comments, processing instructions and 
        /// whitespace.
        /// </summary>
        /// <param name="reader">Reader to reposition.</param>
        /// <returns>
        /// true if the reader is left on an element; false otherwise.
        /// </returns>
        internal static bool XmlReaderEnsureElement(XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        return true;
                    case XmlNodeType.Comment:
                    case XmlNodeType.None:
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Text:
                        if (IsWhitespace(reader.Value))
                        {
                            break;
                        }
                        else
                        {
                            return false;
                        }

                    default:
                        return false;
                }
            }
            while (reader.Read());

            return false;
        }

        /// <summary>
        /// Checks whether a given object implements IServiceProvider and if it supports the specified service interface
        /// </summary>
        /// <typeparam name="T">The type representing the requested service</typeparam>
        /// <param name="target">Object that acts as the service provider</param>
        /// <returns>An object implementing the requested service, or null if not available</returns>
        internal static T GetService<T>(object target) where T : class
        {
            IServiceProvider provider = target as IServiceProvider;
            if (provider != null)
            {
                object service = provider.GetService(typeof(T));
                if (service != null)
                {
                    return (T)service;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the type corresponding to the wrapper based in input parameter types
        /// </summary>
        /// <param name="wrapperParameters">Parameter types</param>
        /// <param name="errorGenerator">Delegate that generates the error</param>
        /// <returns>Closed generic type</returns>
        internal static Type GetWrapperType(Type[] wrapperParameters, Func<object, string> errorGenerator)
        {
            Debug.Assert(wrapperParameters.Length > 1, "Must have 1 element besides the ProjectedType");
            if (wrapperParameters.Length - 1 > 12)
            {
                throw DataServiceException.CreateBadRequestError(errorGenerator(wrapperParameters.Length - 1));
            }

            return GenericExpandedWrapperTypes.Single(x => x.Index == wrapperParameters.Length - 1).Type.MakeGenericType(wrapperParameters);
        }

        /// <summary>
        /// Checks if the given type is an ExpandedWrapper type
        /// </summary>
        /// <param name="inputType">Input closed type</param>
        /// <returns>true if the given type is one of ExpandedWrapper types</returns>
        internal static bool IsExpandedWrapperType(Type inputType)
        {
            return inputType.IsGenericType && GenericExpandedWrapperTypes.SingleOrDefault(x => x.Type == inputType.GetGenericTypeDefinition()) != null;
        }

        /// <summary>
        /// Returns an enumeration that picks one element from each enumerable and projects from them.
        /// </summary>
        /// <typeparam name="T1">Type of first enumerable.</typeparam>
        /// <typeparam name="T2">Type of second enumerable.</typeparam>
        /// <typeparam name="TResult">Type of zipped projection.</typeparam>
        /// <param name="left">Left enumerable.</param>
        /// <param name="right">Right enumerable.</param>
        /// <param name="resultSelector">Projecting function.</param>
        /// <returns>An enumeration with the projected results.</returns>
        internal static IEnumerable<TResult> Zip<T1, T2, TResult>(IEnumerable<T1> left, IEnumerable<T2> right, Func<T1, T2, TResult> resultSelector)
        {
            if (null == left || null == right)
            {
                yield break;
            }

            resultSelector = resultSelector ?? ((x, y) => default(TResult));
            using (var l = left.GetEnumerator())
            using (var r = right.GetEnumerator())
            {
                while (l.MoveNext() && r.MoveNext())
                {
                    yield return resultSelector(l.Current, r.Current);
                }
            }
        }

        /// <summary>
        /// get attribute value from specified namespace or empty namespace
        /// </summary>
        /// <param name="reader">reader</param>
        /// <param name="attributeName">attributeName</param>
        /// <param name="namespaceUri">namespaceUri</param>
        /// <returns>attribute value</returns>
        internal static string GetAttributeEx(XmlReader reader, string attributeName, string namespaceUri)
        {
            return reader.GetAttribute(attributeName, namespaceUri) ?? reader.GetAttribute(attributeName);
        }

        /// <summary>
        /// Disposes the stream provider and returns a no-op method for a stream-writing action.
        /// </summary>
        /// <returns>A delegate that can serialize the result.</returns>
        internal static Action<Stream> GetEmptyStreamWriter()
        {
            return stream => { };
        }

        /// <summary>
        /// Get the value of the given property.
        /// </summary>
        /// <param name="provider">underlying data provider.</param>
        /// <param name="resource">instance of the type which contains this property.</param>
        /// <param name="resourceType">resource type instance containing metadata about the declaring type.</param>
        /// <param name="resourceProperty">resource property instance containing metadata about the property.</param>
        /// <param name="propertyName">Name of the property to read if <paramref name="resourceProperty"/> is not given.</param>
        /// <returns>the value of the given property.</returns>
        internal static object GetPropertyValue(DataServiceProviderWrapper provider, object resource, ResourceType resourceType, ResourceProperty resourceProperty, String propertyName)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(
                (resourceProperty == null && propertyName != null) || (resourceProperty != null && propertyName == null),
                "One of resourceProperty or propertyName should be null and other non-null.");

            IProjectedResult projectedResult = resource as IProjectedResult;
            if (projectedResult != null)
            {
                object result = projectedResult.GetProjectedPropertyValue(propertyName ?? resourceProperty.Name);
                if (IsNullValue(result))
                {
                    result = null;
                }

                return result;
            }

            if (resourceProperty != null)
            {
                return provider.GetPropertyValue(resource, resourceProperty, resourceType);
            }

            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(resourceType.IsOpenType, "resourceType must be of open type.");
            return provider.GetOpenPropertyValue(resource, propertyName);
        }

        /// <summary>
        /// Returns true if the specified value should be treated as a null value
        /// </summary>
        /// <param name="propertyValue">The property value to check for being null.</param>
        /// <returns>true if the value should be treated as null, or false otherwise.</returns>
        /// <remarks>This methods checks for being CLR null as well as DBNull.Value.</remarks>
        internal static bool IsNullValue(object propertyValue)
        {
            return propertyValue == null || propertyValue == DBNull.Value;
        }

        /// <summary>
        /// Test if the type is an ISpatial derived type
        /// </summary>
        /// <param name="type">the type to be tested</param>
        /// <returns>true if the type implements the ISpatial interface, false otherwise.</returns>
        internal static bool IsSpatial(this Type type)
        {
            return typeof(ISpatial).IsAssignableFrom(type);
        }

        /// <summary>
        /// Sets the response headers for the given operation message.
        /// </summary>
        /// <param name="operationResponseMessage">ODataBatchOperationResponseMessage instance for the batch operation.</param>
        /// <param name="batchHost">RequestMessage containing all the response headers for the batch operation.</param>
        internal static void SetResponseHeadersForBatchRequests(IODataResponseMessage operationResponseMessage, BatchServiceHost batchHost)
        {
            Debug.Assert(operationResponseMessage != null, "operationResponseMessage != null");
            Debug.Assert(batchHost != null, "batchHost != null");

            IDataServiceHost2 host = (IDataServiceHost2)batchHost;
            operationResponseMessage.StatusCode = host.ResponseStatusCode;

            WebHeaderCollection responseHeaders = host.ResponseHeaders;
            foreach (string header in responseHeaders.AllKeys)
            {
                string headerValue = responseHeaders[header];
                if (!String.IsNullOrEmpty(headerValue))
                {
                    operationResponseMessage.SetHeader(header, headerValue);
                }
            }
        }

        /// <summary>
        /// Checks if the given resource type instance is one of the known binary primitive types.
        /// </summary>
        /// <param name="resourceType">Resource type instance.</param>
        /// <returns>True if the given resource type instance is one of the known binary primitive types, otherwise returns false.</returns>
        internal static bool IsBinaryResourceType(ResourceType resourceType)
        {
            if (resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
            {
                if (resourceType.InstanceType == typeof(byte[]) ||
                    resourceType.InstanceType == typeof(Binary))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validate the given annotation.
        /// </summary>
        /// <param name="customAnnotations">Reference to the dictionary instance where custom annotations are stored.</param>
        /// <param name="namespaceName">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="name">Name of the annotation.</param>
        /// <param name="annotation">Value of the annotation.</param>
        internal static void ValidateAndAddAnnotation(ref Dictionary<string, object> customAnnotations, string namespaceName, string name, object annotation)
        {
            // DEVNOTE (pratikp): When we allow providers to add custom annotations, we can change this assert into an exception.
            // Ideally all this check must be done in EdmLib and hopefully we can call some method in EdmLib to do this validation.
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(annotation != null, "annotation != null");

            if (customAnnotations == null)
            {
                customAnnotations = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            if (!String.IsNullOrEmpty(namespaceName))
            {
                // custom annotations can be only of string or XElement type
                Debug.Assert(annotation.GetType() == typeof(string) || annotation.GetType() == typeof(XElement), "only string and xelement annotations are supported");
            }

            string fullName = CreateFullNameForCustomAnnotation(namespaceName, name);
            customAnnotations.Add(fullName, annotation);
        }

        /// <summary>
        /// Creates the full name for a custom annotation.
        /// </summary>
        /// <param name="namespaceName">Namespace to which the custom annotation belongs to.</param>
        /// <param name="name">Name of the annotation.</param>
        /// <returns>The full name for the annotation</returns>
        internal static string CreateFullNameForCustomAnnotation(string namespaceName, string name)
        {
            if (!String.IsNullOrEmpty(namespaceName))
            {
                return namespaceName + ":" + name;
            }

            return name;
        }

        /// <summary>
        /// Reads from the XML reader skipping insignificant nodes.
        /// </summary>
        /// <param name="reader">The XML reader to read from.</param>
        /// <remarks>Do not use MoveToContent since for backward compatibility reasons we skip over nodes reported as Text which have
        /// whitespace only content (even though the XmlReader should report those as Whitespace).</remarks>
        internal static void SkipInsignificantNodes(this XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        return;
                    case XmlNodeType.Comment:
                    case XmlNodeType.None:
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.Text:
                        if (IsNullOrWhitespace(reader.Value))
                        {
                            break;
                        }

                        return;

                    default:
                        return;
                }
            }
            while (reader.Read());
        }

        /// <summary>
        /// Creates ODataMessageReaderSettings for the specified data service.
        /// </summary>
        /// <param name="dataService">The data service instance to create the settings for.</param>
        /// <param name="enableODataServerBehavior">If true, the new reader settings will use the WcfDataServicesServer behavior;
        /// if false, the new reader settings will use the default behavior.</param>
        /// <returns>New instance of the message reader settings.</returns>
        internal static ODataMessageReaderSettings CreateMessageReaderSettings(IDataService dataService, bool enableODataServerBehavior)
        {
            ODataMessageReaderSettings messageReaderSettings = new ODataMessageReaderSettings();

            // messageReaderSettings.EnableAtomSupport();

            // We do our own URL resolution through custom IODataPayloadUriConverter and follow up processing of URLs
            // as a result it doesn't matter which URI we use here (since we don't actually use its value anywhere).
            // So using the absolute URI of the service is the best we can do.
            messageReaderSettings.BaseUri = dataService.OperationContext.AbsoluteServiceUri;
            messageReaderSettings.EnableMessageStreamDisposal = false;
            messageReaderSettings.EnablePrimitiveTypeConversion = dataService.Configuration.EnableTypeConversion;
            messageReaderSettings.EnableCharactersCheck = false;

            ODataProtocolVersion maxProtocolVersion = dataService.Configuration.DataServiceBehavior.MaxProtocolVersion;
            messageReaderSettings.MaxProtocolVersion = 
                maxProtocolVersion == ODataProtocolVersion.V4 ? ODataVersion.V4
                : maxProtocolVersion == ODataProtocolVersion.V401 ? ODataVersion.V401 
                : CommonUtil.ConvertToODataVersion(
                    dataService.Configuration.DataServiceBehavior.MaxProtocolVersion);

            CommonUtil.SetDefaultMessageQuotas(messageReaderSettings.MessageQuotas);

            if (enableODataServerBehavior)
            {
                messageReaderSettings.Validations &= ~(ValidationKinds.ThrowOnDuplicatePropertyNames | ValidationKinds.ThrowIfTypeConflictsWithMetadata);
                messageReaderSettings.ClientCustomTypeResolver = null;
            }

            return messageReaderSettings;
        }

        /// <summary>
        /// Ensures that if there is a direct reference like "/Customers(4)" or $value then the result object is not null.
        /// </summary>
        /// <param name="result">Query result entity or value.</param>
        /// <param name="segmentInfo">Segment details for the <paramref name="result"/>.</param>
        internal static void CheckNullDirectReference(object result, SegmentInfo segmentInfo)
        {
            if (segmentInfo.IsDirectReference && result == null)
            {
                throw DataServiceException.CreateResourceNotFound(segmentInfo.Identifier);
            }
        }

        /// <summary>
        /// Returns the entity set for the given resource set.
        /// </summary>
        /// <param name="provider">Underlying data provider.</param>
        /// <param name="model">IEdmModel instance containing all the metadata information.</param>
        /// <param name="resourceSet">ResourceSetWrapper instance.</param>
        /// <returns>an IEdmEntitySet instance for the given resource set.</returns>
        internal static IEdmEntitySet GetEntitySet(DataServiceProviderWrapper provider, IEdmModel model, ResourceSetWrapper resourceSet)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            string containerName = resourceSet.EntityContainerName ?? provider.ContainerName;
            return model.FindEntityContainer(containerName).FindEntitySet(MetadataProviderUtils.GetEntitySetName(resourceSet.ResourceSet));
        }

        /// <summary>Gets a non-nullable version of the specified type.</summary>
        /// <param name="type">Type to get non-nullable version for.</param>
        /// <returns>
        /// <paramref name="type"/> if type is a reference type or a 
        /// non-nullable type; otherwise, the underlying value type.
        /// </returns>
        internal static Type GetNonNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>Checks that no query arguments were sent in the request.</summary>
        /// <param name="service">Service to check.</param>
        /// <param name="checkForOnlyV2QueryParameters">true if only V2 query parameters must be checked, otherwise all the query parameters will be checked.</param>
        /// <remarks>
        /// Regular processing checks argument applicability, but for
        /// service operations that return an IEnumerable this is part
        /// of the contract on service operations, rather than a structural
        /// check on the request.
        /// </remarks>
        internal static void CheckEmptyQueryArguments(IDataService service, bool checkForOnlyV2QueryParameters)
        {
            Debug.Assert(service != null, "service != null");

            AstoriaRequestMessage host = service.OperationContext.RequestMessage;
            if ((!checkForOnlyV2QueryParameters &&
                 (!String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringExpand)) ||
                  !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringFilter)) ||
                  !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringOrderBy)) ||
                  !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringSkip)) ||
                  !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringTop)))) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringQueryCount)) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringSelect)) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringSkipToken)))
            {
                // 400: Bad Request
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryNoOptionsApplicable);
            }
        }

        /// <summary>Checks that no set query arguments were sent in the request.</summary>
        /// <param name="service">Service to check.</param>
        internal static void CheckEmptySetQueryArguments(IDataService service)
        {
            Debug.Assert(service != null, "service != null");

            AstoriaRequestMessage host = service.OperationContext.RequestMessage;
            if (!String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringOrderBy)) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringSkip)) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringTop)) ||
                !String.IsNullOrEmpty(host.GetQueryStringItem(XmlConstants.HttpQueryStringQueryCount)))
            {
                // 400: Bad Request
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySetOptionsNotApplicable);
            }
        }

        /// <summary>Checks that no query arguments were sent in the request.</summary>
        /// <param name="service">Service to check.</param>
        /// <remarks>
        /// Regular processing checks argument applicability, but for
        /// service operations that return an IEnumerable this is part
        /// of the contract on service operations, rather than a structural
        /// check on the request.
        /// </remarks>
        internal static void CheckV2EmptyQueryArguments(IDataService service)
        {
            Debug.Assert(service != null, "service != null");
            CheckEmptyQueryArguments(service, service.Provider.HasReflectionOrEFProviderQueryBehavior);
        }

        /// <summary>
        /// Test if the given segment is a cross referenced segment in a batch operation
        /// </summary>
        /// <param name="segmentInfo">Segment in question</param>
        /// <param name="service">service instance</param>
        /// <returns>True if the given segment is a cross referenced segment</returns>
        internal static bool IsCrossReferencedSegment(SegmentInfo segmentInfo, IDataService service)
        {
            if (segmentInfo.Identifier.StartsWith("$", StringComparison.Ordinal) && service.GetSegmentForContentId(segmentInfo.Identifier) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the resource type which the resource property is declared on.
        /// </summary>
        /// <param name="resourceType">resource type to start looking</param>
        /// <param name="resourceProperty">resource property in question</param>
        /// <param name="rootType">root type in the hierarchy at which we need to stop.</param>
        /// <returns>actual resource type that declares the property or the root type if the property is declared in a more base type than the given root type.</returns>
        internal static ResourceType GetDeclaringTypeForProperty(this ResourceType resourceType, ResourceProperty resourceProperty, ResourceType rootType = null)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");

            while (resourceType != rootType)
            {
                if (resourceType.PropertiesDeclaredOnThisType.Contains(resourceProperty))
                {
                    break;
                }

                resourceType = resourceType.BaseType;
            }

            return resourceType;
        }

        /// <summary>
        /// Convert the given DateTime instance to corresponding DateTimeOffset instance. 
        /// The conversion rules for a DateTime kind to DateTimeOffset are:
        /// a) Unspecified -> UTC
        /// b) Local -> Local
        /// c) UTC -> UTC
        /// </summary>
        /// <param name="dt">Given DateTime value.</param>
        /// <returns>DateTimeOffset corresponding to given DateTime value.</returns>
        internal static DateTimeOffset ConvertDateTimeToDateTimeOffset(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified)
            {
                return new DateTimeOffset(new DateTime(dt.Ticks, DateTimeKind.Utc));
            }

            return new DateTimeOffset(dt);
        }

        /// <summary>
        /// Converts the given DateTimeOffset value to it's corresponding DateTime value.
        /// </summary>
        /// <param name="dto">Input DateTimeOffset value.</param>
        /// <returns>DateTime value corresponding to the input DateTimeOffset value.</returns>
        /// <remarks>
        /// We have 2 cases:
        /// a) Offset corresponds to UTC timezone in which case, we return the DateTime value of Utc Kind.
        /// b) Offset corresponds to a non-UTC timezone in which case, we return the local DateTime corresponding to server timezone.
        /// </remarks>
        internal static DateTime ConvertDateTimeOffsetToDateTime(DateTimeOffset dto)
        {
            return dto.Offset == TimeSpan.Zero ? dto.UtcDateTime : dto.LocalDateTime;
        }

        /// <summary>
        /// Checks whether the specifies string is null or blank.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <returns>true if text is null, empty, or all whitespace characters.</returns>
        private static bool IsNullOrWhitespace(string text)
        {
#if ORCAS
            if (text == null)
            {
                return true;
            }
            else
            {
                foreach (char c in text)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        return false;
                    }
                }

                return true;
            }
#else
            return String.IsNullOrWhiteSpace(text);
#endif
        }

        /// <summary>Gets the host and port parts of a RequestMessage header if they are both present.</summary>
        /// <param name="hostHeader">RequestMessage header value (non-null).</param>
        /// <param name="scheme">Scheme for the host and port values.</param>
        /// <param name="host">If the result is true, the host part of the header.</param>
        /// <param name="port">If the result is false, the port part of the header.</param>
        /// <returns>true if the header has a host and port part, false otherwise.</returns>
        private static bool GetHostAndPort(string hostHeader, string scheme, out string host, out int port)
        {
            Debug.Assert(hostHeader != null, "hostHeader != null");

            if (scheme != null && !scheme.EndsWith("://", StringComparison.Ordinal))
            {
                scheme += "://";
            }

            Uri result;
            if (Uri.TryCreate(scheme + hostHeader, UriKind.Absolute, out result))
            {
                host = result.Host;
                port = result.Port;
                return true;
            }

            host = null;
            port = default(int);
            return false;
        }

        /// <summary>Checks whether the specifies string is null or blank.</summary>
        /// <param name="text">Text to check.</param>
        /// <returns>true if text is null, empty, or all whitespace characters.</returns>
        private static bool IsWhitespace(string text)
        {
            return text == null || text.All(Char.IsWhiteSpace);
        }

        /// <summary>
        /// Represents a pair of Expanded wrapper with the index in  the array
        /// The only reason to create this class is to avoid CA908 for KeyValuePairs
        /// </summary>
        private sealed class ExpandWrapperTypeWithIndex
        {
            /// <summary>Type</summary>
            internal Type Type
            {
                get;
                set;
            }

            /// <summary>Index</summary>
            internal int Index
            {
                get;
                set;
            }
        }

        /// <summary>
        /// A workaround to a problem with FxCop which does not recognize the CheckArgumentNotNull method
        /// as the one which validates the argument is not null.
        /// </summary>
        /// <remarks>This has been suggested as a workaround in msdn forums by the VS team. Note that even though this is production code
        /// the attribute has no effect on anything else.</remarks>
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
