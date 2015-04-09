//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Serializers
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Services;
    using System.Data.Services.Parsing;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>Abstract base class for all serializers.</summary>
    internal abstract class Serializer
    {
        #region Private fields

        /// <summary>Maximum recursion limit on serializers.</summary>
        private const int RecursionLimit = 100;

        /// <summary>
        /// These query parameters can be copied for each next page link.
        /// Don't need to copy $skiptoken, $skip and $top because they are calculated every time.
        /// </summary>
        private static readonly String[] NextPageQueryParametersToCopy = 
        { 
            XmlConstants.HttpQueryStringFilter,
            XmlConstants.HttpQueryStringExpand,
            XmlConstants.HttpQueryStringOrderBy,
            XmlConstants.HttpQueryStringInlineCount,
            XmlConstants.HttpQueryStringSelect
        };

        /// <summary>Base URI from which resources should be resolved.</summary>
        private readonly Uri absoluteServiceUri;

        /// <summary>Data provider from which metadata should be gathered.</summary>
        private readonly string httpETagHeaderValue;

        /// <summary>Data provider from which metadata should be gathered.</summary>
        private readonly IDataService service;

        /// <summary>Description for the requested results.</summary>
        private readonly RequestDescription requestDescription;

        /// <summary>The payload metadata parameter interpreter.</summary>
        private readonly PayloadMetadataParameterInterpreter payloadMetadataParameterInterpreter;

        /// <summary>The accessor to use for payload properties which may be left out of the response payload.</summary>
        private readonly PayloadMetadataPropertyManager payloadMetadataPropertyManager;

        /// <summary>Collection of complex types, used for cycle detection.</summary>
        private HashSet<object> complexTypeCollection;

        /// <summary>Depth of recursion.</summary>
        private int recursionDepth;

        /// <summary>information for each segment that is encountered during serialization.</summary>
        private SegmentInfo segmentInfo;

        /// <summary>Current skip token object for custom paging.</summary>
        private object[] currentSkipTokenForCustomPaging;

        #endregion Private fields

        /// <summary>Initializes a new base Serializer, ready to write out a description.</summary>
        /// <param name="requestDescription">Description for the requested results.</param>
        /// <param name="absoluteServiceUri">Base URI from which resources should be resolved.</param>
        /// <param name="service">Service with configuration and provider from which metadata should be gathered.</param>
        /// <param name="httpETagHeaderValue">HTTP ETag header value.</param>
        internal Serializer(RequestDescription requestDescription, Uri absoluteServiceUri, IDataService service, string httpETagHeaderValue)
        {
            Debug.Assert(requestDescription != null, "requestDescription != null");
            Debug.Assert(absoluteServiceUri != null, "absoluteServiceUri != null");
            Debug.Assert(service != null, "service != null");

            this.requestDescription = requestDescription;
            this.absoluteServiceUri = absoluteServiceUri;
            this.service = service;
            this.httpETagHeaderValue = httpETagHeaderValue;

            Debug.Assert(requestDescription.ShouldWriteResponseBody, "Response body should not be written according to the request description");
            Debug.Assert(requestDescription.PayloadMetadataParameterInterpreter != null, "Metadata parameter should already have been interpreted");
            this.payloadMetadataPropertyManager = new PayloadMetadataPropertyManager(requestDescription.PayloadMetadataParameterInterpreter);
            this.payloadMetadataParameterInterpreter = requestDescription.PayloadMetadataParameterInterpreter;
        }

        /// <summary>Container for the resource being serialized (possibly null).</summary>
        protected ResourceSetWrapper CurrentContainer
        {
            get
            {
                if (this.segmentInfo == null || this.segmentInfo.Count == 0)
                {
                    return this.requestDescription.LastSegmentInfo.TargetResourceSet;
                }

                return this.segmentInfo.CurrentResourceSet;
            }
        }

        /// <summary>Is current container the root container.</summary>
        protected bool IsRootContainer
        {
            get
            {
                return (this.segmentInfo == null || this.segmentInfo.Count == 1);
            }
        }

        /// <summary>
        /// Gets the Data provider from which metadata should be gathered.
        /// </summary>
        protected DataServiceProviderWrapper Provider
        {
            [DebuggerStepThrough]
            get { return this.service.Provider; }
        }

        /// <summary>
        /// Gets the Data service from which metadata should be gathered.
        /// </summary>
        protected IDataService Service
        {
            [DebuggerStepThrough]
            get { return this.service; }
        }

        /// <summary>Gets the absolute URI to the service.</summary>
        protected Uri AbsoluteServiceUri
        {
            [DebuggerStepThrough]
            get { return this.absoluteServiceUri; }
        }

        /// <summary>
        /// Gets the RequestDescription for the request that is getting serialized.
        /// </summary>
        protected RequestDescription RequestDescription
        {
            [DebuggerStepThrough]
            get
            {
                return this.requestDescription;
            }
        }

        /// <summary>Are we using custom paging?</summary>
        protected bool IsCustomPaged
        {
            get
            {
                return this.service.PagingProvider.IsCustomPagedForSerialization;
            }
        }

        /// <summary>
        /// The payload metadata parameter interpreter.
        /// </summary>
        protected PayloadMetadataParameterInterpreter PayloadMetadataParameterInterpreter
        {
            get { return this.payloadMetadataParameterInterpreter; }
        }

        /// <summary>
        /// Gets the accessor to use for payload properties which may be left out of the response payload.
        /// </summary>
        protected PayloadMetadataPropertyManager PayloadMetadataPropertyManager
        {
            get { return this.payloadMetadataPropertyManager; }
        }

        /// <summary>
        /// Starts enumeration of a collection property value.
        /// </summary>
        /// <param name="collectionPropertyValue">The value of the collection property. (nulls are handled by this method)</param>
        /// <param name="propertyName">The name of the property being serialized. (for error message purposes).</param>
        /// <returns>IEnumerable to enumerate over to get the items of the collection Property.</returns>
        internal static IEnumerable GetCollectionEnumerable(object collectionPropertyValue, string propertyName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!String.IsNullOrEmpty(propertyName)");

            if (WebUtil.IsNullValue(collectionPropertyValue))
            {
                throw new InvalidOperationException(Services.Strings.Serializer_CollectionCanNotBeNull(propertyName));
            }

            IEnumerable enumerable;
            if (!WebUtil.IsElementIEnumerable(collectionPropertyValue, out enumerable))
            {
                throw new InvalidOperationException(Services.Strings.Serializer_CollectionPropertyValueMustImplementIEnumerable(propertyName));
            }

            return enumerable;
        }

        /// <summary>
        /// Returns the value of the primitive property.
        /// </summary>
        /// <param name="propertyValue">Value of the primitive property.</param>
        /// <returns>Returns the value of the primitive property.</returns>
        internal static object GetPrimitiveValue(object propertyValue)
        {
            // Since ODataLib does not handle DBNull, making sure that DBNull values are converted to CLR null values.
            if (WebUtil.IsNullValue(propertyValue))
            {
                return null;
            }

            // ODataLib does not support XElement and Binary type. Hence we need to convert these types into
            // string and byte[] respectively, before sending it to ODataLib for serialization.
            // For spatial data, ODataLib only understands the types in System.Spatial, not the ones defined
            // by the Entity Framework, so we need do that conversion here as well.
            Type type = propertyValue.GetType();
            if (type == typeof(XElement))
            {
                return ((XElement)propertyValue).ToString();
            }

            if (type == typeof(Binary))
            {
                return ((Binary)propertyValue).ToArray();
            }

#if !INTERNAL_DROP && !EFRTM
            if (ObjectContextSpatialUtil.IsDbGeography(type))
            {
                return ObjectContextSpatialUtil.ConvertDbGeography(propertyValue);
            }
#endif

            return propertyValue;
        }

        /// <summary>
        /// Flushes the content of the underlying writers
        /// </summary>
        internal abstract void Flush();

        /// <summary>
        /// Handles the complete serialization for the specified <see cref="RequestDescription"/>.
        /// </summary>
        /// <param name="queryResults">Query results to enumerate.</param>
        /// <remarks>
        /// <paramref name="queryResults"/> should correspond to the RequestQuery of the 
        /// RequestDescription object passed while constructing this serializer
        /// We allow the results to be passed in
        /// to let the query be executed earlier than at result-writing time, which
        /// helps detect data and query errors where they can be better handled.
        /// </remarks>
        internal void WriteRequest(QueryResultInfo queryResults)
        {
            Debug.Assert(queryResults != null, "queryResults != null");
            IExpandedResult expanded = queryResults.AsIExpandedResult();

            if (this.requestDescription.IsSingleResult)
            {
                Debug.Assert(queryResults.HasMoved, "queryResults.HasMoved == true");
                this.WriteTopLevelElement(expanded, queryResults.Current);
                if (queryResults.MoveNext())
                {
                    throw new InvalidOperationException(Services.Strings.SingleResourceExpected);
                }
            }
            else
            {
                this.WriteTopLevelElements(expanded, queryResults);
            }
        }

        /// <summary>Gets the expanded element for the specified expanded result.</summary>
        /// <param name="expanded">The expanded result to process.</param>
        /// <returns>The expanded element.</returns>
        protected static object GetExpandedElement(IExpandedResult expanded)
        {
            Debug.Assert(expanded != null, "expanded != null");
            return expanded.ExpandedElement;
        }

        /// <summary>Gets the expandable value for the specified object.</summary>
        /// <param name="expanded">Expanded properties for the result, possibly null.</param>
        /// <param name="customObject">Object with value to retrieve.</param>
        /// <param name="property">Property for which value will be retrieved.</param>
        /// <param name="expandedNode">expanded node if present for the current navigation property whose value needs to be returned.</param>
        /// <returns>The property value.</returns>
        protected object GetExpandedProperty(IExpandedResult expanded, object customObject, ResourceProperty property, ExpandedProjectionNode expandedNode)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(
                property.IsOfKind(ResourcePropertyKind.ResourceReference) || property.IsOfKind(ResourcePropertyKind.ResourceSetReference),
                "Only nav properties can be expanded.");

            if (expanded == null)
            {
                return WebUtil.GetPropertyValue(this.Provider, customObject, null, property, null);
            }

            // In ExpandedWrapper's decription property, we only append the type name if there is any ambiguity.
            // If the navigation property already refers to the more derived type, which in turn has a 
            // navigation property, we do not need to append the type name while accessing the latter.
            string propertyName = property.Name;
            if (expandedNode != null && this.GetCurrentExpandedProjectionNode().ResourceType != expandedNode.TargetResourceType)
            {
                propertyName = expandedNode.TargetResourceType.FullName + "/" + property.Name;
            }

            // We may end up projecting null as a value of ResourceSetReference property. This can in theory break
            //   the serializers as they expect a non-null (possibly empty) IEnumerable instead. But note that
            //   if we project null into the expanded property, we also project null into the ExpandedElement property
            //   and thus the serializers should recognize this value as null and don't try to expand its properties.
            Debug.Assert(
                expanded.ExpandedElement != null,
                "We should not be accessing expanded properties on null resource.");
            return expanded.GetExpandedPropertyValue(propertyName);
        }

        /// <summary>Writes a single top-level element.</summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="element">Element to write, possibly null.</param>
        protected abstract void WriteTopLevelElement(IExpandedResult expanded, object element);

        /// <summary>Writes multiple top-level elements, possibly none.</summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="elements">Result elements.</param>
        protected abstract void WriteTopLevelElements(IExpandedResult expanded, QueryResultInfo elements);

        /// <summary>
        /// Adds the given object instance to complex type collection
        /// </summary>
        /// <param name="complexTypeInstance">instance to be added</param>
        /// <returns>true, if it got added successfully</returns>
        protected bool AddToComplexTypeCollection(object complexTypeInstance)
        {
            if (this.complexTypeCollection == null)
            {
                this.complexTypeCollection = new HashSet<object>(System.Data.Services.ReferenceEqualityComparer<object>.Instance);
            }

            return this.complexTypeCollection.Add(complexTypeInstance);
        }

        /// <summary>
        /// Gets the skip token object contained in the expanded result for standard paging.
        /// </summary>
        /// <param name="expanded">Current expanded result.</param>
        /// <returns>Skip token object if any.</returns>
        protected IExpandedResult GetSkipToken(IExpandedResult expanded)
        {
            if (expanded != null && !this.IsCustomPaged && !this.RequestDescription.IsRequestForEnumServiceOperation)
            {
                return expanded.GetExpandedPropertyValue(XmlConstants.HttpQueryStringSkipToken) as IExpandedResult;
            }

            return null;
        }

        /// <summary>
        /// Obtains the URI for the link for next page in string format
        /// </summary>
        /// <param name="lastObject">Last object serialized to be used for generating $skiptoken</param>
        /// <param name="skipTokenExpandedResult">The <see cref="IExpandedResult"/> of the $skiptoken property of object corresponding to last serialized object</param>
        /// <param name="absoluteUri">Absolute response URI</param>
        /// <returns>URI for the link for next page</returns>
        protected Uri GetNextLinkUri(object lastObject, IExpandedResult skipTokenExpandedResult, Uri absoluteUri)
        {
            UriBuilder builder = new UriBuilder(absoluteUri);
            SkipTokenBuilder skipTokenBuilder;

            if (this.IsRootContainer)
            {
                if (!this.IsCustomPaged)
                {
                    if (skipTokenExpandedResult != null)
                    {
                        skipTokenBuilder = new SkipTokenBuilderFromExpandedResult(skipTokenExpandedResult, this.RequestDescription.SkipTokenExpressionCount);
                    }
                    else
                    {
                        Debug.Assert(this.RequestDescription.SkipTokenProperties != null, "Must have skip token properties collection");
                        Debug.Assert(this.RequestDescription.SkipTokenProperties.Count > 0, "Must have some valid ordered properties in the skip token properties collection");
                        skipTokenBuilder = new SkipTokenBuilderFromProperties(lastObject, this.Provider, this.RequestDescription.SkipTokenProperties);
                    }
                }
                else
                {
                    Debug.Assert(this.currentSkipTokenForCustomPaging != null, "Must have obtained the skip token for custom paging.");
                    skipTokenBuilder = new SkipTokenBuilderFromCustomPaging(this.currentSkipTokenForCustomPaging);
                }

                builder.Query = this.GetNextPageQueryParametersForRootContainer().Append(skipTokenBuilder.GetSkipToken()).ToString();
            }
            else
            {
                if (!this.IsCustomPaged)
                {
                    // Internal results
                    skipTokenBuilder = new SkipTokenBuilderFromProperties(lastObject, this.Provider, this.CurrentContainer.ResourceType.KeyProperties);
                }
                else
                {
                    Debug.Assert(this.currentSkipTokenForCustomPaging != null, "Must have obtained the skip token for custom paging.");
                    skipTokenBuilder = new SkipTokenBuilderFromCustomPaging(this.currentSkipTokenForCustomPaging);
                }

                builder.Query = this.GetNextPageQueryParametersForExpandedContainer().Append(skipTokenBuilder.GetSkipToken()).ToString();
            }

            return builder.Uri;
        }

        /// <summary>Is next page link needs to be appended to the feed</summary>
        /// <param name="queryResult">Current result enumerator.</param>
        /// <returns>true if the feed must have a next page link</returns>
        protected bool NeedNextPageLink(QueryResultInfo queryResult)
        {
            // For open types, current container could be null
            if (this.CurrentContainer != null && !this.RequestDescription.IsServiceActionRequest && !this.RequestDescription.IsRequestForEnumServiceOperation)
            {
                if (this.IsCustomPaged)
                {
                    this.currentSkipTokenForCustomPaging = queryResult.GetContinuationTokenFromPagingProvider(this.service.PagingProvider);
                    Debug.Assert(
                            this.RequestDescription.ResponseVersion != VersionUtil.DataServiceDefaultResponseVersion,
                            "If custom paging is enabled, our response should be 2.0 and beyond.");

                    return this.currentSkipTokenForCustomPaging != null && this.currentSkipTokenForCustomPaging.Length > 0;
                }

                int pageSize = this.CurrentContainer.PageSize;
                if (pageSize != 0 && this.RequestDescription.ResponseVersion != VersionUtil.DataServiceDefaultResponseVersion)
                {
                    // For the root segment, if the $top parameter value is less than or equal to page size then we
                    // don't need to send the next page link.
                    if (this.segmentInfo.Count == 1)
                    {
                        int? topQueryParameter = this.GetTopQueryParameter();

                        if (topQueryParameter.HasValue)
                        {
                            Debug.Assert(topQueryParameter.Value >= this.segmentInfo.CurrentResultCount, "$top must be the upper limits of the number of results returned.");
                            if (topQueryParameter.Value <= pageSize)
                            {
                                return false;
                            }
                        }
                    }

                    return this.segmentInfo.CurrentResultCount == pageSize;
                }
            }

            return false;
        }

        /// <summary>Increments the result count for the current segment, throws if exceeds the limit.</summary>
        protected void IncrementSegmentResultCount()
        {
            if (this.segmentInfo != null)
            {
                Debug.Assert(this.segmentInfo.Count > 0, "this.segmentResultCounts.Count > 0 -- otherwise we didn't PushSegmentForRoot");
                int max = this.service.Configuration.MaxResultsPerCollection;

                if (!this.IsCustomPaged)
                {
                    // For Open types, current container could be null, even though MaxResultsPerCollection has been set
                    // set we need to check for container before we try to assume page size, also open types do not have
                    // page sizes so we can safely ignore this check here
                    if (this.CurrentContainer != null && this.CurrentContainer.PageSize != 0)
                    {
                        Debug.Assert(max == Int32.MaxValue, "Either page size or max result count can be set, but not both");
                        max = this.CurrentContainer.PageSize;
                    }
                }

                // We want to keep the result count for custom paging, since we need to subtract the number of rows in the
                // result from the $top query parameter while computing the next page link, if $top parameters is specified
                // in the request uri.
                if (max != Int32.MaxValue || this.IsCustomPaged)
                {
                    int count = this.segmentInfo.CurrentResultCount;
                    checked
                    {
                        count++;
                    }

                    // Throw error if the current count is greater than the maximum page size. The only 
                    // exception to this is service actions, because service actions discard paging and return the entire resultset.
                    if (count > max && !this.RequestDescription.IsServiceActionRequest)
                    {
                        throw DataServiceException.CreateBadRequestError(Services.Strings.Serializer_ResultsExceedMax(max));
                    }

                    this.segmentInfo.CurrentResultCount = count;
                }
            }
        }

        /// <summary>Pushes a segment from the stack of names being written.</summary>
        /// <param name='property'>Property to push.</param>
        /// <param name="currentResourceType">resource type of the current entity which is being serialized.</param>
        /// <param name="expandedProjectionNode">expanded node the given navigation property.</param>
        /// <remarks>Calls to this method should be balanced with calls to PopSegmentName.</remarks>
        /// <returns>true if a segment was pushed, false otherwise</returns>
        protected bool PushSegmentForProperty(ResourceProperty property, ResourceType currentResourceType, ExpandedProjectionNode expandedProjectionNode)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(property.TypeKind == ResourceTypeKind.EntityType, "property.TypeKind == ResourceTypeKind.EntityType");

            ResourceSetWrapper current = this.CurrentContainer;
            if (current != null)
            {
                current = this.service.Provider.GetResourceSet(current, currentResourceType, property);
            }

#if DEBUG
            if (this.segmentInfo != null && this.segmentInfo.CurrentExpandedNode != null && expandedProjectionNode != null)
            {
                Debug.Assert(expandedProjectionNode == this.segmentInfo.CurrentExpandedNode.FindExpandedNode(property, currentResourceType), "the node must match");
            }
#endif

            // expandedProjectionNode can be null in couple of scenarios. If you are using V1 IExpandProvider interface,
            // then providers can modify the ExpandSegments, which means that expandedProjectionNodes might not be present,
            // but since ExpandPaths are present, we will expand that property. Look in ShouldExpandForSegment method to 
            // find out more about how we figure out whether we should expand or not when using IExpandProvider.
            return this.PushSegment(current, expandedProjectionNode);
        }

        /// <summary>Pushes a segment for the root of the tree being written out.</summary>
        /// <remarks>Calls to this method should be balanced with calls to PopSegmentName.</remarks>
        /// <returns>true if the segment was pushed, false otherwise</returns>
        protected bool PushSegmentForRoot()
        {
            return this.PushSegment(this.CurrentContainer, this.RequestDescription.RootProjectionNode);
        }

        /// <summary>Pops a segment name from the stack of names being written.</summary>
        /// <param name="needPop">Is a pop required. Only true if last push was successful</param>
        /// <remarks>Calls to this method should be balanced with previous calls to PushSegmentName.</remarks>
        protected void PopSegmentName(bool needPop)
        {
            if (this.segmentInfo != null && needPop)
            {
                this.segmentInfo.PopSegment();
            }
        }

        /// <summary>Marks the fact that a recursive method was entered, and checks that the depth is allowed.</summary>
        protected void RecurseEnter()
        {
            WebUtil.RecurseEnter(RecursionLimit, ref this.recursionDepth);
        }

        /// <summary>Marks the fact that a recursive method is leaving.</summary>
        protected void RecurseLeave()
        {
            WebUtil.RecurseLeave(ref this.recursionDepth);
        }

        /// <summary>
        /// Remove the given object instance from the complex type collection
        /// </summary>
        /// <param name="complexTypeInstance">instance to be removed</param>
        protected void RemoveFromComplexTypeCollection(object complexTypeInstance)
        {
            Debug.Assert(this.complexTypeCollection != null, "this.complexTypeCollection != null");
            Debug.Assert(this.complexTypeCollection.Contains(complexTypeInstance), "this.complexTypeCollection.Contains(complexTypeInstance)");

            this.complexTypeCollection.Remove(complexTypeInstance);
        }

        /// <summary>Checks whether the property with the specified name should be expanded in-line.</summary>
        /// <param name='property'>Property which needs to be checked for expansion.</param>
        /// <param name="currentResourceType">resource type of the entity which is current getting serialized.</param>
        /// <param name="expandedNode">expandedNode for the given navigation property, if found.</param>
        /// <returns>true if the segment should be expanded; false otherwise.</returns>
        protected bool ShouldExpandSegment(ResourceProperty property, ResourceType currentResourceType, out ExpandedProjectionNode expandedNode)
        {
            Debug.Assert(property != null, "property != null");

            expandedNode = null;
            if (this.segmentInfo == null || this.segmentInfo.CurrentExpandedNode == null)
            {
                return false;
            }

            if (this.requestDescription.RootProjectionNode.UseExpandPathsForSerialization &&
                this.requestDescription.RootProjectionNode.ExpandPaths != null)
            {
                // We need to use the old ExpandPaths to determine which segments to expand
                //   since the IExpandProvider might have modified this collection.
                for (int i = 0; i < this.requestDescription.RootProjectionNode.ExpandPaths.Count; i++)
                {
                    List<ExpandSegment> expandPath = this.requestDescription.RootProjectionNode.ExpandPaths[i];
                    if (expandPath.Count < this.segmentInfo.Count)
                    {
                        continue;
                    }

                    // We start off at '1' for segment names because the first one is the
                    // "this" in the query (/Customers?$expand=Orders doesn't include "Customers").
                    bool matchFound = true;
                    for (int j = 1; j < this.segmentInfo.Count; j++)
                    {
                        if (expandPath[j - 1].Name != this.segmentInfo.GetSegmentName(j))
                        {
                            matchFound = false;
                            break;
                        }
                    }

                    if (matchFound && expandPath[this.segmentInfo.Count - 1].Name == property.Name)
                    {
                        return true;
                    }
                }
            }
            else
            {
                // And then if that node contains a child node of the specified name
                //   and that child is also an expanded node, we should expand it.
                expandedNode = this.segmentInfo.CurrentExpandedNode.FindExpandedNode(property, currentResourceType);
            }

            return expandedNode != null;
        }

        /// <summary>Returns a list of projection segments defined for the current segment.</summary>
        /// <returns>List of <see cref="ProjectionNode"/> describing projections for the current segment.
        /// If this method returns null it means no projections are to be applied and the entire resource
        /// for the current segment should be serialized. If it returns non-null only the properties described
        /// by the returned projection segments should be serialized.</returns>
        protected IEnumerable<ProjectionNode> GetProjections()
        {
            ExpandedProjectionNode expandedProjectionNode = this.GetCurrentExpandedProjectionNode();
            if (expandedProjectionNode == null || expandedProjectionNode.ProjectAllProperties)
            {
                return null;
            }

            return expandedProjectionNode.Nodes;
        }

        /// <summary>
        /// Returns the ETag value from the host response header
        /// </summary>
        /// <param name="resource">resource whose etag value gets to be returned</param>
        /// <param name="resourceType">ResourceType instance containing metadata about <paramref name="resource"/>.</param>
        /// <returns>returns the etag value for the given resource</returns>
        protected string GetETagValue(object resource, ResourceType resourceType)
        {
            // this.httpETagHeaderValue is the etag value which got computed for writing the etag in the response
            // headers. The etag response header only gets written out in certain scenarios, whereas we always 
            // write etag in the response payload, if the type has etag properties. So just checking here is the
            // etag has already been computed, and if yes, returning that, otherwise computing the etag.
            if (!String.IsNullOrEmpty(this.httpETagHeaderValue))
            {
                return this.httpETagHeaderValue;
            }

            Debug.Assert(this.CurrentContainer != null, "this.CurrentContainer != null");
            return WebUtil.GetETagValue(this.service, resource, resourceType, this.CurrentContainer);
        }

        /// <summary>
        /// Returns the instance of ResourcePropertyInfo, which keeps track of whether we need to do the expand for the given navigation property.
        /// </summary>
        /// <param name="expanded">Expanded properties for the result.</param>
        /// <param name="customObject">Resource or complex object with properties to write out.</param>
        /// <param name="currentResourceType">resourceType containing metadata about the current custom object</param>
        /// <param name="property">navigation property in question.</param>
        /// <returns>an instance of ResourcePropertyInfo, which keeps all the information about the given navigation property.</returns>
        protected ResourcePropertyInfo GetNavigationPropertyInfo(IExpandedResult expanded, object customObject, ResourceType currentResourceType, ResourceProperty property)
        {
            Debug.Assert(property.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Nav property must be of entity type");

            ExpandedProjectionNode expandedNode;
            object propertyValue = null;
            bool expand = this.ShouldExpandSegment(property, currentResourceType, out expandedNode);
            if (expand)
            {
                propertyValue = this.GetExpandedProperty(expanded, customObject, property, expandedNode);
            }

            return ResourcePropertyInfo.CreateResourcePropertyInfo(property, propertyValue, expandedNode, expand);
        }

        /// <summary>
        /// Returns the property value in terms of OData object model (ODataPrimitiveValue, ODataNullValue, ODataComplexValue or ODataCollectionValue instance) for the given property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyResourceType">Type of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        /// <param name="openProperty">True if the property is an open property, otherwise false.</param>
        /// <returns>Returns the property value in terms of OData object model (CLR type, ODataComplexValue or ODataCollectionValue instance) for the given property value.</returns>
        protected ODataValue GetPropertyValue(string propertyName, ResourceType propertyResourceType, object propertyValue, bool openProperty)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(propertyResourceType != null, "propertyResourceType != null");

            if (propertyResourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
            {
                return this.GetPrimitiveValueAsODataValue(propertyResourceType, propertyValue);
            }

            if (propertyResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                // no need to handle null here as the property setter on ODataProperty will handle it.
                return this.GetComplexValue(propertyName, propertyValue);
            }

            if (!openProperty)
            {
                Debug.Assert(
                       propertyResourceType.ResourceTypeKind == ResourceTypeKind.Collection,
                       "propertyResourceType.ResourceTypeKind == ResourceTypeKind.Collection");
                return this.GetCollection(propertyName, (CollectionResourceType)propertyResourceType, propertyValue);
            }

            // Open collection properties are not supported on OpenTypes
            WebUtil.CheckResourceNotCollectionForOpenProperty(propertyResourceType, propertyName);

            // Open navigation properties are not supported on OpenTypes
            throw DataServiceException.CreateBadRequestError(Services.Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(propertyName));
        }

        /// <summary>
        /// Returns the ODataComplexValue instance for the given property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        /// <returns>Returns the ODataComplexValue instance for the given property value.</returns>
        protected ODataComplexValue GetComplexValue(string propertyName, object propertyValue)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!String.IsNullOrEmpty(propertyName)");

            if (WebUtil.IsNullValue(propertyValue))
            {
                return null;
            }

            ODataComplexValue complexValue = new ODataComplexValue();
            
            // Resolve the complex type instance - no need to check the resource type for the right type
            // since ODataLib will do this validation.
            ResourceType valueResourceType = WebUtil.GetNonPrimitiveResourceType(this.Provider, propertyValue);
            complexValue.TypeName = valueResourceType.FullName;

            // If this is not a complex type, return early. This validation error will be caught by ODataLib later.
            if (valueResourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
            {
                return complexValue;
            }

            this.PayloadMetadataPropertyManager.SetTypeName(complexValue, valueResourceType);

            complexValue.Properties = this.GetPropertiesOfComplexType(propertyValue, valueResourceType, propertyName);
            
            return complexValue;
        }

        /// <summary>
        /// Returns the ODataCollectionValue instance for the given property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyResourceType">Type of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        /// <returns>Returns the ODataCollectionValue instance for the given property value.</returns>
        protected ODataCollectionValue GetCollection(string propertyName, CollectionResourceType propertyResourceType, object propertyValue)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!String.IsNullOrEmpty(propertyName)");
            Debug.Assert(
                propertyResourceType.ItemType.ResourceTypeKind == ResourceTypeKind.ComplexType || propertyResourceType.ItemType.ResourceTypeKind == ResourceTypeKind.Primitive,
                "Collection items must be either primitive or complex type.");

            this.RecurseEnter();

            IEnumerable enumerable = GetCollectionEnumerable(propertyValue, propertyName);

            ODataCollectionValue collection = new ODataCollectionValue { TypeName = propertyResourceType.FullName };
            this.PayloadMetadataPropertyManager.SetTypeName(collection, propertyResourceType);
            if (propertyResourceType.ItemType.ResourceTypeKind == ResourceTypeKind.Primitive)
            {
                collection.Items = GetEnumerable(
                    enumerable,
                    GetPrimitiveValue);
            }
            else
            {
                collection.Items = GetEnumerable(
                    enumerable,
                    value => this.GetComplexValue(propertyName, value));
            }

            this.RecurseLeave();

            return collection;
        }

        /// <summary>Finds the <see cref="ExpandedProjectionNode"/> node which describes the current segment.</summary>
        /// <returns>The <see cref="ExpandedProjectionNode"/> which describes the current segment, or null
        /// if no such node is available.</returns>
        protected ExpandedProjectionNode GetCurrentExpandedProjectionNode()
        {
            return this.segmentInfo == null ? null : this.segmentInfo.CurrentExpandedNode;
        }

        /// <summary>
        /// Converts the given IEnumerable into IEnumerable<typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="enumerable">IEnumerable which contains the list of the objects that needs to be converted.</param>
        /// <param name="valueConverter">Delegate to use to convert the value.</param>
        /// <returns>An instance of IEnumerable<typeparamref name="T"/> which contains the converted values.</returns>
        private static IEnumerable<T> GetEnumerable<T>(IEnumerable enumerable, Func<object, T> valueConverter)
        {
            List<T> values = new List<T>();

            // Note that foreach will call Dispose on the used IEnumerator in a finally block
            foreach (object value in enumerable)
            {
                values.Add(valueConverter(value));
            }

            return values;
        }

        /// <summary>Helper method to append a path to the $expand or $select path list.</summary>
        /// <param name="pathsBuilder">The <see cref="StringBuilder"/> to which to append the path.</param>
        /// <param name="parentPathSegments">The segments of the path up to the last segment.</param>
        /// <param name="lastPathSegment">The last segment of the path.</param>
        private static void AppendProjectionOrExpansionPath(StringBuilder pathsBuilder, IEnumerable<string> parentPathSegments, string lastPathSegment)
        {
            if (pathsBuilder.Length != 0)
            {
                pathsBuilder.Append(',');
            }

            foreach (string parentPathSegment in parentPathSegments)
            {
                pathsBuilder.Append(parentPathSegment).Append('/');
            }

            pathsBuilder.Append(lastPathSegment);
        }

        /// <summary>
        /// Returns the ODataValue representation of the given primitive value.
        /// </summary>
        /// <param name="propertyResourceType">The type of the property.</param>
        /// <param name="propertyValue">The primitive value.</param>
        /// <returns>An ODataNullValue or ODataPrimitiveValue representing the given value.</returns>
        private ODataValue GetPrimitiveValueAsODataValue(ResourceType propertyResourceType, object propertyValue)
        {
            ODataValue primitiveValue = GetPrimitiveValue(propertyValue).ToODataValue();
            this.PayloadMetadataPropertyManager.SetTypeName(primitiveValue, propertyResourceType);

            return primitiveValue;
        }

        /// <summary>
        /// Returns all the properties of the given resource instance.
        /// </summary>
        /// <param name="resource">Resource instance whose properties needs to be written out.</param>
        /// <param name="resourceType">ResourceType containing metadata about the resource instance.</param>
        /// <param name="propertyName">Name of the parent property for which the properties are returned.</param>
        /// <returns>Returns all the properties of the given resource instance.</returns>
        private IEnumerable<ODataProperty> GetPropertiesOfComplexType(object resource, ResourceType resourceType, string propertyName)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(!resourceType.IsOpenType, "Open complex type are not supported");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType");

            // Make sure to handle loops in complex type values
            // PERF: we can keep a single element around and save the HashSet initialization
            // until we find a second complex type - this saves the allocation on trees
            // with shallow (single-level) complex types.
            this.RecurseEnter();
            if (!this.AddToComplexTypeCollection(resource))
            {
                throw new InvalidOperationException(Services.Strings.Serializer_LoopsNotAllowedInComplexTypes(propertyName));
            }

            List<ODataProperty> propertiesList = new List<ODataProperty>(resourceType.Properties.Count);

            foreach (ResourceProperty resourceProperty in resourceType.Properties)
            {
                object propertyValue = WebUtil.GetPropertyValue(this.Service.Provider, resource, resourceType, resourceProperty, null);
                ODataValue odataValue = this.GetPropertyValue(resourceProperty.Name, resourceProperty.ResourceType, propertyValue, false /*openProperty*/);
                ODataProperty odataProperty = new ODataProperty { Name = resourceProperty.Name, Value = odataValue };
                propertiesList.Add(odataProperty);
            }

            // Once all the properties of the complex type has been enumerated, then remove this resource 
            // from the hashset
            this.RemoveFromComplexTypeCollection(resource);
            this.RecurseLeave();

            return propertiesList;
        }

        /// <summary>
        /// Builds the string corresponding to query parameters for top level results to be put in next page link.
        /// </summary>
        /// <returns>StringBuilder which has the query parameters in the URI query parameter format.</returns>
        private StringBuilder GetNextPageQueryParametersForRootContainer()
        {
            StringBuilder queryParametersBuilder = new StringBuilder();

            // Handle service operation parameters
            if (this.RequestDescription.SegmentInfos[0].TargetSource == RequestTargetSource.ServiceOperation)
            {
                foreach (var parameter in this.RequestDescription.SegmentInfos[0].Operation.Parameters)
                {
                    if (queryParametersBuilder.Length > 0)
                    {
                        queryParametersBuilder.Append('&');
                    }

                    string queryStringItem = this.service.OperationContext.RequestMessage.GetQueryStringItem(parameter.Name);
                    if (!string.IsNullOrEmpty(queryStringItem)) {
                        // Only append parameter when it was provided in the first place
                        queryParametersBuilder.Append(parameter.Name).Append('=');
                        string escapedQueryStringItem = DataStringEscapeBuilder.EscapeDataString(queryStringItem);
                        queryParametersBuilder.Append(escapedQueryStringItem);
                    }
                }
            }

            foreach (String queryParameter in NextPageQueryParametersToCopy)
            {
                String value = this.service.OperationContext.RequestMessage.GetQueryStringItem(queryParameter);
                if (!String.IsNullOrEmpty(value))
                {
                    if (queryParametersBuilder.Length > 0)
                    {
                        queryParametersBuilder.Append('&');
                    }

                    queryParametersBuilder.Append(queryParameter).Append('=').Append(DataStringEscapeBuilder.EscapeDataString(value));
                }
            }

            int? topQueryParameter = this.GetTopQueryParameter();
            if (topQueryParameter.HasValue)
            {
                int remainingResults = topQueryParameter.Value;
                if (!this.IsCustomPaged)
                {
                    remainingResults = topQueryParameter.Value - this.CurrentContainer.PageSize;
                }
                else
                {
                    // For custom paging, we need to substract the original value with the number of entities that has been serialized.
                    remainingResults = topQueryParameter.Value - this.segmentInfo.CurrentResultCount;
                }

                if (remainingResults > 0)
                {
                    if (queryParametersBuilder.Length > 0)
                    {
                        queryParametersBuilder.Append('&');
                    }

                    queryParametersBuilder.Append(XmlConstants.HttpQueryStringTop).Append('=').Append(remainingResults);
                }
            }

            if (queryParametersBuilder.Length > 0)
            {
                queryParametersBuilder.Append('&');
            }

            return queryParametersBuilder;
        }

        /// <summary>Recursive method which builds the $expand and $select paths for the specified node.</summary>
        /// <param name="parentPathSegments">List of path segments which lead up to this node. 
        /// So for example if the specified node is Orders/OrderDetails the list will contains two strings
        /// "Orders" and "OrderDetails".</param>
        /// <param name="projectionPaths">The result to which the projection paths are appended as a comma separated list.</param>
        /// <param name="expansionPaths">The result to which the expansion paths are appended as a comma separated list.</param>
        /// <param name="expandedNode">The node to inspect.</param>
        /// <param name="foundProjections">Out parameter which is set to true if there were some explicit projections on the inspected node.</param>
        /// <param name="foundExpansions">Our parameter which is set to true if there were some expansions on the inspected node.</param>
        private void BuildProjectionAndExpansionPathsForNode(
            List<string> parentPathSegments,
            StringBuilder projectionPaths,
            StringBuilder expansionPaths,
            ExpandedProjectionNode expandedNode,
            out bool foundProjections,
            out bool foundExpansions)
        {
            foundProjections = false;
            foundExpansions = false;

            List<ExpandedProjectionNode> expandedChildrenNeededToBeProjected = new List<ExpandedProjectionNode>();
            foreach (ProjectionNode childNode in expandedNode.Nodes)
            {
                ExpandedProjectionNode expandedChildNode = childNode as ExpandedProjectionNode;
                if (expandedChildNode == null)
                {
                    // Explicitely project the property mentioned in this node
                    AppendProjectionOrExpansionPath(projectionPaths, parentPathSegments, childNode.PropertyName);
                    foundProjections = true;
                }
                else
                {
                    foundExpansions = true;

                    parentPathSegments.Add(expandedChildNode.PropertyName);
                    bool foundExpansionChild;
                    bool foundProjectionChild;
                    this.BuildProjectionAndExpansionPathsForNode(
                        parentPathSegments,
                        projectionPaths,
                        expansionPaths,
                        expandedChildNode,
                        out foundProjectionChild,
                        out foundExpansionChild);
                    parentPathSegments.RemoveAt(parentPathSegments.Count - 1);

                    // Add projection paths for this node if all its properties should be projected
                    if (expandedChildNode.ProjectAllProperties)
                    {
                        if (foundProjectionChild)
                        {
                            // There were some projections in our children, but this node requires all properties -> project *
                            AppendProjectionOrExpansionPath(projectionPaths, parentPathSegments, childNode.PropertyName + "/*");
                        }
                        else
                        {
                            // There were no projections underneath this node, so we need to "project" this node
                            // we just don't know yet if we need to project this one explicitly or if some parent will do it for us implicitly.
                            expandedChildrenNeededToBeProjected.Add(expandedChildNode);
                        }
                    }

                    foundProjections |= foundProjectionChild;

                    if (!foundExpansionChild)
                    {
                        // If there were no expansions in children, we need to add this node to expansion list
                        AppendProjectionOrExpansionPath(expansionPaths, parentPathSegments, childNode.PropertyName);
                    }
                }
            }

            if (!expandedNode.ProjectAllProperties || foundProjections)
            {
                // If we already projected some properties explicitely or this node does not want to project all properties 
                // and we have some expanded children which were not projected yet
                // we need to project those explicitely (as the other projections disable the "include all" for this node 
                // or we don't really want the "include all" anyway)
                foreach (ExpandedProjectionNode childToProject in expandedChildrenNeededToBeProjected)
                {
                    AppendProjectionOrExpansionPath(projectionPaths, parentPathSegments, childToProject.PropertyName);

                    // And since we're adding an explicit projection, mark us as using explicit projections
                    foundProjections = true;
                }
            }
        }

        /// <summary>
        /// Builds the string corresponding to query parameters for top level results to be put in next page link.
        /// </summary>
        /// <returns>StringBuilder which has the query parameters in the URI query parameter format.</returns>
        private StringBuilder GetNextPageQueryParametersForExpandedContainer()
        {
            StringBuilder queryParametersBuilder = new StringBuilder();

            ExpandedProjectionNode expandedProjectionNode = this.GetCurrentExpandedProjectionNode();
            if (expandedProjectionNode != null)
            {
                // Build a string containing all the $expand and $select for the current node and children
                List<string> pathSegments = new List<string>();
                StringBuilder projectionPaths = new StringBuilder();
                StringBuilder expansionPaths = new StringBuilder();
                bool foundExpansions;
                bool foundProjections;
                this.BuildProjectionAndExpansionPathsForNode(
                    pathSegments,
                    projectionPaths,
                    expansionPaths,
                    expandedProjectionNode,
                    out foundProjections,
                    out foundExpansions);

                // In most cases the root level of the query is projected by default
                // The only exception is if there were projections in some children
                // and we need all the properties of the root -> then project *
                if (foundProjections && expandedProjectionNode.ProjectAllProperties)
                {
                    AppendProjectionOrExpansionPath(projectionPaths, pathSegments, "*");
                }

                if (projectionPaths.Length > 0)
                {
                    if (queryParametersBuilder.Length > 0)
                    {
                        queryParametersBuilder.Append('&');
                    }

                    queryParametersBuilder.Append(XmlConstants.HttpQueryStringSelect).Append('=').Append(projectionPaths.ToString());
                }

                if (expansionPaths.Length > 0)
                {
                    if (queryParametersBuilder.Length > 0)
                    {
                        queryParametersBuilder.Append('&');
                    }

                    queryParametersBuilder.Append(XmlConstants.HttpQueryStringExpand).Append('=').Append(expansionPaths.ToString());
                }
            }

            if (queryParametersBuilder.Length > 0)
            {
                queryParametersBuilder.Append('&');
            }

            return queryParametersBuilder;
        }

        /// <summary>Pushes a segment from the stack of names being written.</summary>
        /// <param name="container">Container to push (possibly null).</param>
        /// <param name="expandedNode">ExpandedProjectionNode for the given segment.</param>
        /// <remarks>Calls to this method should be balanced with calls to PopSegmentName.</remarks>
        /// <returns>true if a segment was push, false otherwise</returns>
        private bool PushSegment(ResourceSetWrapper container, ExpandedProjectionNode expandedNode)
        {
            // For custom paging, we need to keep the result count, so that if top query parameter is specified in the request uri,
            // we need to write the top parameter in the next link with the new value (originalvalue - result count)
            if (this.service.Configuration.MaxResultsPerCollection != Int32.MaxValue ||
                (container != null && container.PageSize != 0) || // Complex types have null container , paging should force a push
                (this.requestDescription.RootProjectionNode != null) ||
                this.IsCustomPaged)
            {
                if (this.segmentInfo == null)
                {
                    this.segmentInfo = new SegmentInfo();
                }

                this.segmentInfo.PushSegment(container, expandedNode);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtains the $top query parameter value.
        /// </summary>
        /// <returns>Integer value for $top or null otherwise.</returns>
        private int? GetTopQueryParameter()
        {
            String topQueryItem = this.service.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringTop);
            if (!String.IsNullOrEmpty(topQueryItem))
            {
                return Int32.Parse(topQueryItem, CultureInfo.InvariantCulture);
            }

            return null;
        }

        /// <summary>Stores the resource property, its value and a flag which indicates whether this is a open property or not.</summary>
        internal struct ResourcePropertyInfo
        {
            /// <summary>Returns the resource property.</summary>
            internal ResourceProperty Property { get; private set; }

            /// <summary>Returns the value of the resource property.</summary>
            internal object Value { get; private set; }

            /// <summary>returns true, if the given property needs to be expanded.</summary>
            internal bool Expand { get; private set; }

            /// <summary>Expanded node for the given navigation property.</summary>
            internal ExpandedProjectionNode ExpandedNode { get; private set; }

            /// <summary>
            /// Creates a new instance of ResourcePropertyInfo.
            /// </summary>
            /// <param name="resourceProperty">resource property instance.</param>
            /// <param name="value">value for the resource property.</param>
            /// <param name="expandedNode">expanded node for the given property.</param>
            /// <param name="expand">whether the given property needs to be expanded or not.</param>
            /// <returns>an instance of resourcePropertyInfo, containing all information about serializing the given navigation property.</returns>
            internal static ResourcePropertyInfo CreateResourcePropertyInfo(ResourceProperty resourceProperty, object value, ExpandedProjectionNode expandedNode, bool expand)
            {
                Debug.Assert(resourceProperty != null && resourceProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "resourceProperty != null && resourceProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
                Debug.Assert(expand || value == null && expandedNode == null, "for non-expanded properties, the value and expanded node must be null");

                ResourcePropertyInfo resourcePropertyInfo = new ResourcePropertyInfo
                {
                    Property = resourceProperty,
                    Value = value,
                    Expand = expand,
                    ExpandedNode = expandedNode
                };
                return resourcePropertyInfo;
            }
        }

        /// <summary>
        /// Class to keep track of each segment that is encountered during serialization.
        /// </summary>
        private class SegmentInfo
        {
            /// <summary>Resolved segment containers.</summary>
            private readonly List<ResourceSetWrapper> segmentContainers;

            /// <summary>Result counts for segments.</summary>
            private readonly List<int> segmentResultCounts;

            /// <summary>list of expandedProjectionNodes for segments.</summary>
            private readonly List<ExpandedProjectionNode> projectionNodes;

            /// <summary>
            /// Initializes a new instance of SegmentInfo class
            /// </summary>
            internal SegmentInfo()
            {
                this.segmentContainers = new List<ResourceSetWrapper>();
                this.segmentResultCounts = new List<int>();
                this.projectionNodes = new List<ExpandedProjectionNode>();
            }

            /// <summary>returns the number of segments.</summary>
            internal int Count
            {
                get
                {
#if DEBUG
                    this.Validate();
#endif
                    return this.projectionNodes.Count;
                }
            }

            /// <summary>returns the current resource set i.e. the resource set for the most recent segment encountered.</summary>
            internal ResourceSetWrapper CurrentResourceSet
            {
                get
                {
                    Debug.Assert(this.Count > 0, "this.Count > 0");
                    return this.segmentContainers[this.segmentContainers.Count - 1];
                }
            }

            /// <summary>returns the projection node for the current segment.</summary>
            internal ExpandedProjectionNode CurrentExpandedNode
            {
                get
                {
                    Debug.Assert(this.Count > 0, "this.Count > 0");
                    return this.projectionNodes[this.projectionNodes.Count - 1];
                }
            }

            /// <summary>returns the result count of the current segment i.e. the result set of the most recent segment encountered.</summary>
            internal int CurrentResultCount
            {
                get
                {
                    Debug.Assert(this.Count > 0, "this.Count > 0");
                    return this.segmentResultCounts[this.segmentResultCounts.Count - 1];
                }

                set
                {
                    Debug.Assert(this.Count > 0, "this.Count > 0");
                    this.segmentResultCounts[this.segmentResultCounts.Count - 1] = value;
                }
            }

            /// <summary>
            /// Adds a new segment with the given name and set to the list of segments
            /// </summary>
            /// <param name="set">resource set to which the given segment belongs to.</param>
            /// <param name="projectionNode">ExpandedProjectionNode for the current segment.</param>
            internal void PushSegment(ResourceSetWrapper set, ExpandedProjectionNode projectionNode)
            {
#if DEBUG
                this.Validate();
#endif
                this.segmentContainers.Add(set);
                this.segmentResultCounts.Add(0);
                this.projectionNodes.Add(projectionNode);
#if DEBUG
                this.Validate();
#endif
            }

            /// <summary>
            /// returns the name of the segment at the given index.
            /// </summary>
            /// <param name="index">index of the segment whose name needs to returned.</param>
            /// <returns>name of the segment at the given index.</returns>
            internal string GetSegmentName(int index)
            {
                Debug.Assert(this.Count > 0 && index < this.Count, "this.Count > 0 && index < this.Count");
                return this.projectionNodes[index].PropertyName;
            }

            /// <summary>
            /// Pops the most recent segment
            /// </summary>
            internal void PopSegment()
            {
#if DEBUG
                this.Validate();
#endif
                Debug.Assert(this.Count > 0, "this.Count > 0");
                this.segmentContainers.RemoveAt(this.segmentContainers.Count - 1);
                this.segmentResultCounts.RemoveAt(this.segmentResultCounts.Count - 1);
                this.projectionNodes.RemoveAt(this.projectionNodes.Count - 1);
#if DEBUG
                this.Validate();
#endif
            }

#if DEBUG
            /// <summary>
            /// Validates the internal state of the segment info class.
            /// </summary>
            private void Validate()
            {
                Debug.Assert(
                    this.segmentContainers.Count == this.segmentResultCounts.Count,
                    "this.segmentContainers.Count == this.segmentResultCounts.Count -- should always be one-to-one");
                Debug.Assert(
                    this.segmentContainers.Count == this.projectionNodes.Count,
                    "this.segmentContainers.Count == this.projectionNodes.Count -- should always be one-to-one");
            }
#endif
        }

        /// <summary>
        /// Builds the $skiptoken=[value,value] representation for appending to the next page link URI.
        /// </summary>
        private abstract class SkipTokenBuilder
        {
            /// <summary>Skip token string representation.</summary>
            private readonly StringBuilder skipToken;

            /// <summary>Constructor.</summary>
            protected SkipTokenBuilder()
            {
                this.skipToken = new StringBuilder();
                this.skipToken.Append(XmlConstants.HttpQueryStringSkipToken).Append('=');
            }

            /// <summary>Returns the string representation for $skiptoken query parameter.</summary>
            /// <returns>String representation for $skiptoken query parameter.</returns>
            public StringBuilder GetSkipToken()
            {
                object[] skipTokenProperties = this.GetSkipTokenProperties();

                bool first = true;
                for (int i = 0; i < skipTokenProperties.Length; i++)
                {
                    object value = skipTokenProperties[i];
                    string stringValue;

                    if (value == null)
                    {
                        stringValue = ExpressionConstants.KeywordNull;
                    }
                    else
                    {
                        stringValue = LiteralFormatter.ForSkipToken.Format(value);
                    }

                    if (!first)
                    {
                        this.skipToken.Append(',');
                    }

                    this.skipToken.Append(stringValue);
                    first = false;
                }

                return this.skipToken;
            }

            /// <summary>Derived classes override this to provide the collection of values for skip token.</summary>
            /// <returns>Array of primitive values that comprise the skip token.</returns>
            protected abstract object[] GetSkipTokenProperties();
        }

        /// <summary>Obtains the skip token from IExpandedResult values.</summary>
        private class SkipTokenBuilderFromExpandedResult : SkipTokenBuilder
        {
            /// <summary>IExpandedResult to lookup for skip token values.</summary>
            private readonly IExpandedResult skipTokenExpandedResult;

            /// <summary>Number of values in skip token.</summary>
            private readonly int skipTokenExpressionCount;

            /// <summary>Constructor.</summary>
            /// <param name="skipTokenExpandedResult">IExpandedResult to lookup for skip token values.</param>
            /// <param name="skipTokenExpressionCount">Number of values in skip token.</param>
            public SkipTokenBuilderFromExpandedResult(IExpandedResult skipTokenExpandedResult, int skipTokenExpressionCount)
            {
                this.skipTokenExpandedResult = skipTokenExpandedResult;
                this.skipTokenExpressionCount = skipTokenExpressionCount;
            }

            /// <summary>Obtains skip token values by looking up IExpandedResult.</summary>
            /// <returns>Array of primitive values that comprise the skip token.</returns>
            protected override object[] GetSkipTokenProperties()
            {
                object[] values = new object[this.skipTokenExpressionCount];

                for (int i = 0; i < this.skipTokenExpressionCount; i++)
                {
                    String keyName = XmlConstants.SkipTokenPropertyPrefix + i.ToString(CultureInfo.InvariantCulture);
                    object value = this.skipTokenExpandedResult.GetExpandedPropertyValue(keyName);
                    if (WebUtil.IsNullValue(value))
                    {
                        value = null;
                    }

                    values[i] = value;
                }

                return values;
            }
        }

        /// <summary>Obtains the skip token by reading properties directly from an object.</summary>
        private class SkipTokenBuilderFromProperties : SkipTokenBuilder
        {
            /// <summary>Object to read skip token values from.</summary>
            private readonly object element;

            /// <summary>Collection of properties that comprise the skip token.</summary>
            private readonly ICollection<ResourceProperty> properties;

            /// <summary>Current provider.</summary>
            private readonly DataServiceProviderWrapper provider;

            /// <summary>Constructor.</summary>
            /// <param name="element">Object to read skip token values from.</param>
            /// <param name="provider">Current provider.</param>
            /// <param name="properties">Collection of properties that comprise the skip token.</param>
            public SkipTokenBuilderFromProperties(object element, DataServiceProviderWrapper provider, ICollection<ResourceProperty> properties)
            {
                this.element = element;
                this.provider = provider;
                this.properties = properties;
            }

            /// <summary>Obtains skip token values by reading properties directly from the last object.</summary>
            /// <returns>Array of primitive values that comprise the skip token.</returns>
            protected override object[] GetSkipTokenProperties()
            {
                object[] values = new object[this.properties.Count];

                int propertyIndex = 0;
                foreach (ResourceProperty property in this.properties)
                {
                    object value = WebUtil.GetPropertyValue(this.provider, this.element, null, property, null);
                    if (WebUtil.IsNullValue(value))
                    {
                        value = null;
                    }

                    values[propertyIndex++] = value;
                }

                return values;
            }
        }

        /// <summary>Provides the skip token obtained from the custom paging provider.</summary>
        private class SkipTokenBuilderFromCustomPaging : SkipTokenBuilder
        {
            /// <summary>Skip token obtained from custom paging provider.</summary>
            private readonly object[] lastTokenValue;

            /// <summary>Constructor.</summary>
            /// <param name="lastTokenValue">Skip token obtained from custom paging provider.</param>
            public SkipTokenBuilderFromCustomPaging(object[] lastTokenValue)
            {
                this.lastTokenValue = lastTokenValue;
            }

            /// <summary>Provides the skip token values that were obtained from custom paging provider.</summary>
            /// <returns>Array of primitive values that comprise the skip token.</returns>
            protected override object[] GetSkipTokenProperties()
            {
                return this.lastTokenValue;
            }
        }
    }
}
