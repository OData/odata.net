//---------------------------------------------------------------------
// <copyright file="ODataUriExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// A set of useful extension methods for working with OData URIs
    /// </summary>
    public static class ODataUriExtensions
    {
        /// <summary>
        /// The set of segment types that address entities
        /// </summary>
        private static ODataUriSegmentType[] entityScopeSegmentTypes = new[]
        {
            ODataUriSegmentType.ServiceRoot,
            ODataUriSegmentType.EntitySet,
            ODataUriSegmentType.Function,
            ODataUriSegmentType.Key,
            ODataUriSegmentType.EntityType,
            ODataUriSegmentType.NavigationProperty,                
            ODataUriSegmentType.Unrecognized,
        };

        /// <summary>
        /// Returns whether or not the uri contains a '$ref' segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri contains a '$ref' segment</returns>
        public static bool IsEntityReferenceLink(this ODataUri uri)
        {
            return uri.Segments.Any(s => s.SegmentType == ODataUriSegmentType.EntityReferenceLinks);
        }

        /// <summary>
        /// Returns whether or not the uri ends with a '$value' segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri ends with a '$value' segment</returns>
        public static bool IsPropertyValue(this ODataUri uri)
        {
            return uri.Segments.Count > 1 && uri.LastSegment.SegmentType == ODataUriSegmentType.Value && uri.Segments[uri.Segments.Count - 2].IsProperty();
        }

        /// <summary>
        /// Returns whether or not the uri ends with a '$count' segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri ends with a '$count' segment</returns>
        public static bool IsCount(this ODataUri uri)
        {
            return uri.Segments.Count > 0 && uri.LastSegment.SegmentType == ODataUriSegmentType.Count;
        }

        /// <summary>
        /// Returns whether or not the uri ends with a primitive or complex property segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri ends with a primitive or complex property segment</returns>
        public static bool IsProperty(this ODataUri uri)
        {
            return uri.Segments.Count > 0 && uri.LastSegment.IsProperty();
        }

        /// <summary>
        /// Returns whether or not the segment refers to a member property
        /// </summary>
        /// <param name="segment">The segment to extend</param>
        /// <returns>whether or not the segment refers to a primitive, complex, or multivalue property</returns>
        public static bool IsProperty(this ODataUriSegment segment)
        {
            return segment.SegmentType == ODataUriSegmentType.PrimitiveProperty
                || segment.SegmentType == ODataUriSegmentType.ComplexProperty
                || segment.SegmentType == ODataUriSegmentType.MultiValueProperty;
        }

        /// <summary>
        /// Returns whether or not the uri refers to the root of the service
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to the root of the service</returns>
        public static bool IsServiceDocument(this ODataUri uri)
        {
            return uri.Segments.Count == 1 && uri.LastSegment.SegmentType == ODataUriSegmentType.ServiceRoot;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a WebInvoke service operation call
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a WebInvoke service operation call</returns>
        public static bool IsWebInvokeServiceOperation(this ODataUri uri)
        {
            if (uri.IsServiceOperation())
            {
                ODataUriSegment functionSegment = uri.Segments.First(s => s is FunctionSegment);
                return functionSegment.CheckSegment<FunctionSegment>(f => f.Function.Annotations.OfType<LegacyServiceOperationAnnotation>().Single().Method == Http.HttpVerb.Post);
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a service operation call
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a service operation call</returns>
        public static bool IsServiceOperation(this ODataUri uri)
        {
            ODataUriSegment functionSegment = uri.Segments.LastOrDefault(s => s is FunctionSegment);

            if (functionSegment != null)
            {
                return functionSegment.CheckSegment<FunctionSegment>(f => f.Function.IsServiceOperation());
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a procedure call
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a procedure call</returns>
        public static bool IsAction(this ODataUri uri)
        {
            return uri.CheckLastSegment<FunctionSegment>(f => f.Function.IsAction());
        }

        /// <summary>
        /// Returns whether or not the uri refers to a function call
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a function call</returns>
        public static bool IsFunction(this ODataUri uri)
        {
            return uri.CheckLastSegment<FunctionSegment>(f => f.Function.IsFunction())
                || uri.CheckLastTwoSegments<FunctionSegment, ParametersExpressionSegment>(f => f.Function.IsFunction(), p => true);
        }

        /// <summary>
        /// Returns whether or not the uri ends with a '$metadata' segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri ends with a '$metadata' segment</returns>
        public static bool IsMetadata(this ODataUri uri)
        {
            return uri.Segments.Count > 0 && uri.LastSegment.SegmentType == ODataUriSegmentType.Metadata;
        }

        /// <summary>
        /// Returns whether or not the uri ends with a '$batch' segment
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri ends with a '$batch' segment</returns>
        public static bool IsBatch(this ODataUri uri)
        {
            return uri.Segments.Count > 0 && uri.LastSegment.SegmentType == ODataUriSegmentType.Batch;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a media resource stream
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a media resource stream</returns>
        public static bool IsMediaResource(this ODataUri uri)
        {
            // if the uri is ANY entity-uri followed by $value, then it refers to a media-resource
            // /Customers(1)/$value
            // /Customers(1)/BestFriend/$value
            // /GetFirstCustomer/$value
            if (uri.Segments.Count > 1 && uri.LastSegment.SegmentType == ODataUriSegmentType.Value)
            {
                return new ODataUri(uri.Segments.Take(uri.Segments.Count - 1)).IsEntity();
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a named stream
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a named stream</returns>
        public static bool IsNamedStream(this ODataUri uri)
        {
            return uri.Segments.Count > 0 && uri.LastSegment.SegmentType == ODataUriSegmentType.NamedStream;
        }

        /// <summary>
        /// Returns whether or not the uri refers to a single entity
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to a single entity</returns>
        public static bool IsEntity(this ODataUri uri)
        {
            if (uri.Segments.Count == 0)
            {
                return false;
            }

            if (uri.IsEntityReferenceLink())
            {
                return false;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.Key)
            {
                return true;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.NavigationProperty)
            {
                return ((NavigationSegment)uri.LastSegment).NavigationProperty.ToAssociationEnd.Multiplicity != EndMultiplicity.Many;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.EntityType)
            {
                return new ODataUri(uri.Segments.Take(uri.Segments.Count - 1)).IsEntity();
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.Function)
            {
                return ((FunctionSegment)uri.LastSegment).Function.ReturnType is EntityDataType;
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri refers to an entity set (with or without an EntityType Segment) or collection navigation property
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri refers to an entity set or collection navigation property</returns>
        public static bool IsEntitySet(this ODataUri uri)
        {
            if (uri.Segments.Count == 0)
            {
                return false;
            }

            if (uri.IsEntityReferenceLink())
            {
                return false;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.EntitySet)
            {
                return true;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.NavigationProperty)
            {
                return ((NavigationSegment)uri.LastSegment).NavigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many;
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.EntityType)
            {
                return new ODataUri(uri.Segments.Take(uri.Segments.Count - 1)).IsEntitySet();
            }

            if (uri.LastSegment.SegmentType == ODataUriSegmentType.Function)
            {
                var collectionType = ((FunctionSegment)uri.LastSegment).Function.ReturnType as CollectionDataType;
                if (collectionType != null)
                {
                    return collectionType.ElementDataType is EntityDataType;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri contains one or more open properties
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri contains one or more open properties</returns>
        public static bool HasOpenProperties(this ODataUri uri)
        {
            EntitySet currentSet = null;
            NamedStructuralType currentType = null;
            foreach (var segment in uri.Segments)
            {
                var setSegment = segment as EntitySetSegment;
                if (setSegment != null)
                {
                    currentSet = setSegment.EntitySet;
                    currentType = currentSet.EntityType;
                    continue;
                }

                var serviceOperationSegment = segment as FunctionSegment;
                if (serviceOperationSegment != null)
                {
                    EntitySet svcOpEntitySet;
                    if (serviceOperationSegment.Function.TryGetExpectedEntitySet(currentSet, out svcOpEntitySet))
                    {
                        currentSet = svcOpEntitySet;
                        currentType = currentSet.EntityType;
                        continue;
                    }
                }

                var navigationSegment = segment as NavigationSegment;
                if (navigationSegment != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(currentSet, "Cannot have navigation before a root set or function");
                    currentSet = currentSet.GetRelatedEntitySet(navigationSegment.NavigationProperty);
                    currentType = currentSet.EntityType;
                    continue;
                }

                var typeSegment = segment as EntityTypeSegment;
                if (typeSegment != null)
                {
                    currentType = typeSegment.EntityType;
                    continue;
                }

                var propertySegment = segment as PropertySegment;
                if (propertySegment != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(currentType, "Cannot have property before a root set or function");
                    if (currentType.IsOpen && !propertySegment.Property.IsMetadataDeclaredProperty())
                    {
                        return true;
                    }
                }

                var unrecognizedSegment = segment as UnrecognizedSegment;
                if (unrecognizedSegment != null && currentType != null)
                {
                    ExceptionUtilities.CheckObjectNotNull(currentType, "Cannot have property before a root set or function");
                    if (currentType.IsOpen)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the metadata for the set expected for the given uri. If it would return an entity or single property, it will return the set the entity belongs to
        /// </summary>
        /// <param name="uri">The uri to get the set for</param>
        /// <param name="expectedEntitySet">The expected entity set if appropriate, otherwise null</param>
        /// <returns>Whether an expected entity set could be inferred</returns>
        public static bool TryGetExpectedEntitySet(this ODataUri uri, out EntitySet expectedEntitySet)
        {
            EntityType entityType;
            return uri.TryGetExpectedEntitySetAndType(out expectedEntitySet, out entityType);
        }

        /// <summary>
        /// Gets the metadata for the set and type expected for the given uri. If it would return an entity or single property, it will return the set the entity belongs to
        /// </summary>
        /// <param name="uri">The uri to get the set and type for</param>
        /// <param name="expectedEntitySet">The expected entity set if one could be inferred</param>
        /// <param name="expectedEntityType">The expected entity type if one could be inferred</param>
        /// <returns>Whether the set and type could be inferred</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Needs to return both the set and type")]
        public static bool TryGetExpectedEntitySetAndType(this ODataUri uri, out EntitySet expectedEntitySet, out EntityType expectedEntityType)
        {
            EntitySet expectedSet = null;
            EntityType expectedType = null;
            foreach (var segment in uri.Segments)
            {
                var setSegment = segment as EntitySetSegment;
                if (setSegment != null)
                {
                    expectedSet = setSegment.EntitySet;
                    expectedType = expectedSet.EntityType;
                    continue;
                }

                var navigationSegment = segment as NavigationSegment;
                if (navigationSegment != null)
                {
                    expectedSet = expectedSet.GetRelatedEntitySet(navigationSegment.NavigationProperty);
                    expectedType = navigationSegment.NavigationProperty.ToAssociationEnd.EntityType;
                    continue;
                }

                var functionSegment = segment as FunctionSegment;
                if (functionSegment != null)
                {
                    EntitySet svcOpEntitySet;
                    if (!functionSegment.Function.TryGetExpectedEntitySet(expectedSet, out svcOpEntitySet))
                    {
                        expectedSet = null;
                        continue;
                    }

                    expectedSet = svcOpEntitySet;

                    EntityDataType entityDataType;
                    var returnType = functionSegment.Function.ReturnType;
                    var collectionReturnType = returnType as CollectionDataType;
                    if (collectionReturnType != null)
                    {
                        entityDataType = collectionReturnType.ElementDataType as EntityDataType;
                    }
                    else
                    {
                        entityDataType = returnType as EntityDataType;
                    }

                    if (entityDataType != null)
                    {
                        expectedType = entityDataType.Definition;
                    }
                    else
                    {
                        expectedType = expectedSet.EntityType;
                    }

                    continue;
                }

                var typeSegment = segment as EntityTypeSegment;
                if (typeSegment != null)
                {
                    expectedType = typeSegment.EntityType;
                    continue;
                }
            }

            expectedEntitySet = expectedSet;
            expectedEntityType = expectedType;
            return expectedSet != null;
        }

        /// <summary>
        /// Due to some ambiguity between AddQueryOption vs IncludeTotalCount on the client side, we need
        /// to consider custom query options when looking for $inline count. Ideally, we would fix this,
        /// but it is low priority
        /// </summary>
        /// <param name="uri">OData Uri to look at</param>
        /// <param name="inlineCountValue">The inline count value if one was found</param>
        /// <returns>True or false if there is an inlinecount or not</returns>
        public static bool TryGetInlineCountValue(this ODataUri uri, out string inlineCountValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            if (uri.InlineCount != null)
            {
                inlineCountValue = uri.InlineCount;
                return true;
            }

            return uri.CustomQueryOptions.TryGetValue(QueryOptions.InlineCount, out inlineCountValue);
        }

        /// <summary>
        /// Determines whether ODataUri has an InlineCount or not
        /// </summary>
        /// <param name="uri">OData Uri to look at</param>
        /// <returns>True or false if there is an inlinecount or not</returns>
        public static bool IncludesInlineCountAllPages(this ODataUri uri)
        {
            string value;
            return uri.TryGetInlineCountValue(out value) && value == QueryOptions.InlineCountAllPages;
        }

        /// <summary>
        /// Gets the expected payload type for a request to the given uri
        /// </summary>
        /// <param name="requestUri">The request uri</param>
        /// <returns>The expected payload type</returns>
        public static ODataPayloadElementType GetExpectedPayloadType(this ODataUri requestUri)
        {
            if (requestUri.Segments.OfType<UnrecognizedSegment>().Any())
            {
                return ODataPayloadElementType.Unknown;
            }

            if (requestUri.IsMetadata())
            {
                return ODataPayloadElementType.MetadataPayloadElement;
            }

            if (requestUri.IsBatch())
            {
                return ODataPayloadElementType.BatchResponsePayload;
            }

            if (requestUri.IsCount() || requestUri.IsPropertyValue() || requestUri.IsMediaResource() || requestUri.IsNamedStream())
            {
                return ODataPayloadElementType.PrimitiveValue;
            }

            if (requestUri.IsEntity())
            {
                return ODataPayloadElementType.EntityInstance;
            }

            if (requestUri.IsEntitySet())
            {
                return ODataPayloadElementType.EntitySetInstance;
            }

            if (requestUri.IsEntityReferenceLink())
            {
                return requestUri.GetExpectedLinkPayloadType();
            }

            if (requestUri.IsProperty())
            {
                var property = requestUri.Segments.OfType<PropertySegment>().Last();
                return property.Property.GetExpectedPayloadType();
            }

            if (requestUri.IsServiceOperation() || requestUri.IsAction())
            {
                var serviceOp = requestUri.Segments.OfType<FunctionSegment>().Last();
                return serviceOp.Function.GetExpectedPayloadType();
            }

            return ODataPayloadElementType.Unknown;
        }

        /// <summary>
        /// Gets a new uri scoped to the entity addressed by the given uri. Performs no validation, simply filters segments by segment-type.
        /// Examples: 
        /// Customers(1)/Name/$value => Customers(1)
        /// Customers(1)/Orders(2)/OrderLines/$ref => Customers(1)/Orders(2)
        /// </summary>
        /// <param name="uri">The original uri</param>
        /// <returns>A new uri scoped to a single entity</returns>
        public static ODataUri ScopeToEntity(this ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            var scopedUri = new ODataUri(uri.Segments.TakeWhile(s => entityScopeSegmentTypes.Contains(s.SegmentType)));

            // make sure to copy over any parameters
            scopedUri.CustomQueryOptions.AddRange(uri.CustomQueryOptions);

            return scopedUri;
        }

        /// <summary>
        /// Concatenates the given sequence of segments onto the given string builder
        /// </summary>
        /// <param name="converter">The uri to string converter to extend</param>
        /// <param name="builder">The string builder to append onto</param>
        /// <param name="segments">The segments to append</param>
        public static void ConcatenateSegments(this IODataUriToStringConverter converter, StringBuilder builder, IEnumerable<ODataUriSegment> segments)
        {
            ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckCollectionNotEmpty(segments, "segments");

            bool isFirstSegment = true;
            foreach (var segment in segments)
            {
                if (!isFirstSegment && segment.HasPrecedingSlash)
                {
                    if (builder[builder.Length - 1] != '/')
                    {
                        builder.Append('/');
                    }
                }

                isFirstSegment = false;
                builder.Append(converter.ConvertToString(segment));
            }
        }

        /// <summary>
        /// Concatenates the given sequence of segment sets onto the given string builder
        /// </summary>
        /// <param name="converter">The uri to string converter to extend</param>
        /// <param name="builder">The string builder to append onto</param>
        /// <param name="segmentSets">The segment sets to append</param>
        public static void ConcatenateSegments(this IODataUriToStringConverter converter, StringBuilder builder, ODataUriSegmentPathCollection segmentSets)
        {
            ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckCollectionNotEmpty(segmentSets, "segmentSets");

            bool isFirstSegmentSet = true;
            foreach (var segments in segmentSets)
            {
                if (!isFirstSegmentSet)
                {
                    builder.Append(',');
                }

                isFirstSegmentSet = false;
                converter.ConcatenateSegments(builder, segments);
            }
        }

        /// <summary>
        /// Concatenates the given sequence of segments into a slash-delimited string
        /// </summary>
        /// <param name="converter">The uri to string converter to extend</param>
        /// <param name="segments">the segments to concatenate</param>
        /// <returns>the concatenated segment string</returns>
        public static string ConcatenateSegments(this IODataUriToStringConverter converter, IEnumerable<ODataUriSegment> segments)
        {
            var builder = new StringBuilder();
            converter.ConcatenateSegments(builder, segments);
            return builder.ToString();
        }

        /// <summary>
        /// Concatenates the given sequence of segment sets into a comma and slash delimited string
        /// </summary>
        /// <param name="converter">The uri to string converter to extend</param>
        /// <param name="segmentSets">the segment sets to concatenate</param>
        /// <returns>the concatenated segment string</returns>
        public static string ConcatenateSegments(this IODataUriToStringConverter converter, ODataUriSegmentPathCollection segmentSets)
        {
            var builder = new StringBuilder();
            converter.ConcatenateSegments(builder, segmentSets);
            return builder.ToString();
        }

        /// <summary>
        /// Helper method for converting an OData uri directly to a system Uri
        /// </summary>
        /// <param name="converter">The uri-to-string converter to extend</param>
        /// <param name="uri">The uri to convert</param>
        /// <returns>The system uri for the given OData uri</returns>
        public static Uri ConvertToUri(this IODataUriToStringConverter converter, ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            return new Uri(converter.ConvertToString(uri), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Gets all of the entitySets that are involved for the given uri which includes expands
        /// </summary>
        /// <param name="uri">The uri to get the set for</param>
        /// <returns>The expected entity sets for the given uri</returns>
        internal static IEnumerable<EntitySet> GetAllEntitySetsIncludingExpands(this ODataUri uri)
        {
            List<EntitySet> entitySets = new List<EntitySet>();

            EntitySet startingEntitySet = null;
            if (uri.TryGetExpectedEntitySet(out startingEntitySet))
            {
                entitySets.Add(startingEntitySet);
                entitySets.AddRange(GetIncludingExpandsSets(uri));
            }

            return entitySets;
        }

        /// <summary>
        /// Gets all of the entitySets that are involved for the given uri which includes expands
        /// </summary>
        /// <param name="uri">The uri to get the set for</param>
        /// <returns>The expected entity sets for the given uri</returns>
        internal static IEnumerable<EntitySet> GetIncludingExpandsSets(this ODataUri uri)
        {
            List<EntitySet> entitySets = new List<EntitySet>();

            EntitySet startingEntitySet = null;
            if (uri.TryGetExpectedEntitySet(out startingEntitySet))
            {
                // Review all expanded entity sets, get its version, if higher substitute its version
                foreach (IEnumerable<ODataUriSegment> segmentList in uri.ExpandSegments)
                {
                    EntitySet currentEntitySet = startingEntitySet;
                    foreach (ODataUriSegment segment in segmentList)
                    {
                        var navigationSegment = segment as NavigationSegment;
                        if (navigationSegment != null)
                        {
                            EntitySet relatedEntitySet = currentEntitySet.GetRelatedEntitySet(navigationSegment.NavigationProperty);

                            entitySets.Add(relatedEntitySet);
                            currentEntitySet = relatedEntitySet;
                        }
                    }
                }
            }

            return entitySets;
        }

        internal static ODataPayloadElementType GetExpectedPayloadType(this MemberProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            if (property.IsMultiValue(MultiValueType.Complex))
            {
                return ODataPayloadElementType.ComplexMultiValueProperty;
            }
            else if (property.IsMultiValue(MultiValueType.Primitive))
            {
                return ODataPayloadElementType.PrimitiveMultiValueProperty;
            }
            else if (property.PropertyType is ComplexDataType)
            {
                return ODataPayloadElementType.ComplexProperty;
            }
            else
            {
                ExceptionUtilities.Assert(property.PropertyType is PrimitiveDataType, "Type of property '{0}' was not multivalue, complex, primitive", property.PropertyType);
                return ODataPayloadElementType.PrimitiveProperty;
            }
        }

        internal static ODataPayloadElementType GetExpectedLinkPayloadType(this ODataUri linkUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkUri, "linkUri");

            NavigationSegment lastNavigation = null;
            bool keyAfterLastNavigation = false;
            bool hasLinks = false;
            foreach (var segment in linkUri.Segments)
            {
                if (segment.SegmentType == ODataUriSegmentType.NavigationProperty)
                {
                    lastNavigation = (NavigationSegment)segment;
                    keyAfterLastNavigation = false;
                }
                else if (segment.SegmentType == ODataUriSegmentType.Key)
                {
                    keyAfterLastNavigation = true;
                }
                else if (segment.SegmentType == ODataUriSegmentType.EntityReferenceLinks)
                {
                    hasLinks = true;
                }
            }

            ExceptionUtilities.Assert(hasLinks, "Uri did not contain the '$ref' segment");
            ExceptionUtilities.CheckObjectNotNull(lastNavigation, "Uri did not contain any navigation properties");

            if (lastNavigation.NavigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many && !keyAfterLastNavigation)
            {
                return ODataPayloadElementType.LinkCollection;
            }
            else
            {
                return ODataPayloadElementType.DeferredLink;
            }
        }

        internal static ODataPayloadElementType GetExpectedPayloadType(this Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");

            var type = function.ReturnType;
            var collectionType = type as CollectionDataType;
            if (collectionType != null)
            {
                var elementType = collectionType.ElementDataType;
                if (elementType is EntityDataType)
                {
                    return ODataPayloadElementType.EntitySetInstance;
                }
                else if (elementType is ComplexDataType)
                {
                    return ODataPayloadElementType.ComplexInstanceCollection;
                }
                else
                {
                    return ODataPayloadElementType.PrimitiveCollection;
                }
            }
            else
            {
                if (type is EntityDataType)
                {
                    return ODataPayloadElementType.EntityInstance;
                }
                else if (type is ComplexDataType)
                {
                    return ODataPayloadElementType.ComplexProperty;
                }
                else
                {
                    return ODataPayloadElementType.PrimitiveProperty;
                }
            }
        }

        private static bool CheckLastSegment<TSegment>(this ODataUri uri, Func<TSegment, bool> check) where TSegment : ODataUriSegment
        {
            return uri.LastSegment.CheckSegment(check);
        }

        private static bool CheckLastTwoSegments<TSegment1, TSegment2>(this ODataUri uri, Func<TSegment1, bool> check1, Func<TSegment2, bool> check2)
            where TSegment1 : ODataUriSegment
            where TSegment2 : ODataUriSegment
        {
            if (uri.Segments.Count < 2)
            {
                return false;
            }

            if (!uri.CheckLastSegment(check2))
            {
                return false;
            }

            return uri.Segments[uri.Segments.Count - 2].CheckSegment(check1);
        }

        private static bool CheckSegment<TSegment>(this ODataUriSegment segment, Func<TSegment, bool> check) where TSegment : ODataUriSegment
        {
            var afterCast = segment as TSegment;
            if (afterCast == null)
            {
                return false;
            }

            return check(afterCast);
        }

        private static bool TryGetExpectedEntitySet(this Function function, EntitySet bindingEntitySet, out EntitySet svcOpEntitySet)
        {
            var serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (serviceOperationAnnotation == null)
            {
                return function.TryGetExpectedServiceOperationEntitySet(out svcOpEntitySet);
            }
            else
            {
                return function.TryGetExpectedActionEntitySet(bindingEntitySet, out svcOpEntitySet);
            }
        }
    }
}
