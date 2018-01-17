//---------------------------------------------------------------------
// <copyright file="ODataObjectModelVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Spatial;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    #endregion Namespaces

    /// <summary>
    /// Visitor base class to use for visiting OData object models
    /// </summary>
    public class ODataObjectModelVisitor
    {
        public static object ParseJsonToPrimitiveValue(string rawValue)
        {
            Debug.Assert(rawValue != null && rawValue.Length > 0, "");
            ODataCollectionValue collectionValue = (ODataCollectionValue)
                Microsoft.OData.ODataUriUtils.ConvertFromUriLiteral(string.Format("[{0}]", rawValue), ODataVersion.V4);
            foreach (object item in collectionValue.Items)
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Visits an item in the object model.
        /// </summary>
        /// <param name="objectModelItem">The item to visit.</param>
        public virtual void Visit(object objectModelItem)
        {
            ODataResourceSet resourceCollection = objectModelItem as ODataResourceSet;
            if (resourceCollection != null)
            {
                this.VisitFeed(resourceCollection);
                return;
            }

            ODataResource entry = objectModelItem as ODataResource;
            if (entry != null)
            {
                this.VisitEntry(entry);
                return;
            }

            ODataProperty property = objectModelItem as ODataProperty;
            if (property != null)
            {
                this.VisitProperty(property);
                return;
            }

            ODataNestedResourceInfo navigationLink = objectModelItem as ODataNestedResourceInfo;
            if (navigationLink != null)
            {
                this.VisitNavigationLink(navigationLink);
                return;
            }

            ODataCollectionValue collectionValue = objectModelItem as ODataCollectionValue;
            if (collectionValue != null)
            {
                this.VisitCollectionValue(collectionValue);
                return;
            }

            ODataStreamReferenceValue streamReferenceValue = objectModelItem as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                this.VisitStreamReferenceValue(streamReferenceValue);
                return;
            }

            ODataCollectionStart collectionStart = objectModelItem as ODataCollectionStart;
            if (collectionStart != null)
            {
                this.VisitCollectionStart(collectionStart);
                return;
            }

            ODataServiceDocument serviceDocument = objectModelItem as ODataServiceDocument;
            if (serviceDocument != null)
            {
                this.VisitServiceDocument(serviceDocument);
                return;
            }

            ODataEntitySetInfo entitySetInfo = objectModelItem as ODataEntitySetInfo;
            if (entitySetInfo != null)
            {
                this.VisitEntitySet(entitySetInfo);
                return;
            }

            ODataError error = objectModelItem as ODataError;
            if (error != null)
            {
                this.VisitError(error);
                return;
            }

            ODataInnerError innerError = objectModelItem as ODataInnerError;
            if (innerError != null)
            {
                this.VisitInnerError(innerError);
                return;
            }

            ODataEntityReferenceLinks entityReferenceLinks = objectModelItem as ODataEntityReferenceLinks;
            if (entityReferenceLinks != null)
            {
                this.VisitEntityReferenceLinks(entityReferenceLinks);
                return;
            }

            ODataEntityReferenceLink entityReferenceLink = objectModelItem as ODataEntityReferenceLink;
            if (entityReferenceLink != null)
            {
                this.VisitEntityReferenceLink(entityReferenceLink);
                return;
            }

            ODataAction action = objectModelItem as ODataAction;
            if (action != null)
            {
                this.VisitODataOperation(action);
                return;
            }

            ODataFunction function = objectModelItem as ODataFunction;
            if (function != null)
            {
                this.VisitODataOperation(function);
                return;
            }

            ODataParameters parameters = objectModelItem as ODataParameters;
            if (parameters != null)
            {
                this.VisitParameters(parameters);
                return;
            }

            ODataBatch batch = objectModelItem as ODataBatch;
            if (batch != null)
            {
                this.VisitBatch(batch);
                return;
            }

            ODataBatchChangeset batchChangeset = objectModelItem as ODataBatchChangeset;
            if (batchChangeset != null)
            {
                this.VisitBatchChangeset(batchChangeset);
                return;
            }

            ODataBatchRequestOperation batchRequestOperation = objectModelItem as ODataBatchRequestOperation;
            if (batchRequestOperation != null)
            {
                this.VisitBatchRequestOperation(batchRequestOperation);
                return;
            }

            ODataBatchResponseOperation batchResponseOperation = objectModelItem as ODataBatchResponseOperation;
            if (batchResponseOperation != null)
            {
                this.VisitBatchResponseOperation(batchResponseOperation);
                return;
            }

            if (objectModelItem == null || objectModelItem.GetType().IsValueType || objectModelItem is string ||
                objectModelItem is byte[] || objectModelItem is ISpatial)
            {
                this.VisitPrimitiveValue(objectModelItem);
                return;
            }

            if (objectModelItem is ODataUntypedValue)
            {
                this.VisitPrimitiveValue(ParseJsonToPrimitiveValue((objectModelItem as ODataUntypedValue).RawValue));
                return;
            }

            this.VisitUnsupportedValue(objectModelItem);
        }

        /// <summary>
        /// Visits a feed item.
        /// </summary>
        /// <param name="feed">The feed to visit.</param>
        protected virtual void VisitFeed(ODataResourceSet feed)
        {
            var entries = feed.Entries();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    this.Visit(entry);
                }
            }
        }

        /// <summary>
        /// Visits an entry item.
        /// </summary>
        /// <param name="entry">The entry to visit.</param>
        protected virtual void VisitEntry(ODataResource entry)
        {
            if (entry.MediaResource != null)
            {
                this.Visit(entry.MediaResource);
            }

            if (entry.Actions != null)
            {
                foreach (var action in entry.Actions)
                {
                    this.VisitODataOperation(action);
                }
            }

            if (entry.Functions != null)
            {
                foreach (var function in entry.Functions)
                {
                    this.VisitODataOperation(function);
                }
            }

            entry.ProcessPropertiesPreservingPayloadOrder(
                property => this.VisitProperty(property),
                navigationLink => this.VisitNavigationLink(navigationLink));
        }

        /// <summary>
        /// Visits a property item.
        /// </summary>
        /// <param name="property">The property to visit.</param>
        protected virtual void VisitProperty(ODataProperty property)
        {
            this.Visit(property.Value);
        }

        /// <summary>
        /// Visits a navigation link item.
        /// </summary>
        /// <param name="navigationLink">The navigation link to visit.</param>
        protected virtual void VisitNavigationLink(ODataNestedResourceInfo navigationLink)
        {
            object expandedContent;
            if (navigationLink.TryGetExpandedContent(out expandedContent) && expandedContent != null)
            {
                List<ODataItem> items = expandedContent as List<ODataItem>;
                if (items != null)
                {
                    foreach (ODataItem item in items)
                    {
                        this.Visit(item);
                    }
                }
                else
                {
                    this.Visit(expandedContent);
                }
            }
        }

        /// <summary>
        /// Visits an ODataOperation.
        /// </summary>
        /// <param name="operation">The operation to visit.</param>
        protected virtual void VisitODataOperation(ODataOperation operation)
        {
        }

        /// <summary>
        /// Visits a collection item.
        /// </summary>
        /// <param name="collectionValue">The collection to visit.</param>
        protected virtual void VisitCollectionValue(ODataCollectionValue collectionValue)
        {
            var items = collectionValue.Items;
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Visit(item);
                }
            }
        }

        /// <summary>
        /// Visits a collection start.
        /// </summary>
        /// <param name="collection">The collection start to visit.</param>
        protected virtual void VisitCollectionStart(ODataCollectionStart collectionStart)
        {
            ODataCollectionItemsObjectModelAnnotation items = collectionStart.GetAnnotation<ODataCollectionItemsObjectModelAnnotation>();
            if (items != null)
            {
                foreach (object item in items)
                {
                    this.Visit(item);
                }
            }
        }

        /// <summary>
        /// Visits parameters.
        /// </summary>
        /// <param name="collection">The parameters to visit.</param>
        private void VisitParameters(ODataParameters parameters)
        {
            foreach (var parameter in parameters)
            {
                this.Visit(parameter.Value);
            }
        }

        /// <summary>
        /// Visits an error.
        /// </summary>
        /// <param name="error">The error to visit.</param>
        protected virtual void VisitError(ODataError error)
        {
            ODataInnerError innerError = error.InnerError;
            if (innerError != null)
            {
                this.Visit(innerError);
            }
        }

        /// <summary>
        /// Visits an inner error.
        /// </summary>
        /// <param name="innerError">The inner error to visit.</param>
        protected virtual void VisitInnerError(ODataInnerError innerError)
        {
            ODataInnerError nestedInnerError = innerError.InnerError;
            if (nestedInnerError != null)
            {
                this.Visit(nestedInnerError);
            }
        }

        /// <summary>
        /// Visits a serviceDocument.
        /// </summary>
        /// <param name="serviceDocument">The serviceDocument to visit.</param>
        protected virtual void VisitServiceDocument(ODataServiceDocument serviceDocument)
        {
            IEnumerable<ODataEntitySetInfo> collections = serviceDocument.EntitySets;
            if (collections != null)
            {
                foreach (ODataEntitySetInfo collection in collections)
                {
                    this.Visit(collection);
                }
            }
        }

        /// <summary>
        /// Visits a resource collection.
        /// </summary>
        /// <param name="entitySetInfo">The resource collection to visit.</param>
        protected virtual void VisitEntitySet(ODataEntitySetInfo entitySetInfo)
        {
        }

        /// <summary>
        /// Visits an entity reference link collection.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference link collection to visit.</param>
        protected virtual void VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
        {
            IEnumerable<ODataEntityReferenceLink> links = entityReferenceLinks.Links;
            if (links != null)
            {
                foreach (ODataEntityReferenceLink link in links)
                {
                    this.Visit(link);
                }
            }
        }

        /// <summary>
        /// Visits an entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to visit.</param>
        protected virtual void VisitEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
        }

        /// <summary>
        /// Visits a stream reference value (named stream).
        /// </summary>
        /// <param name="streamReferenceValue">The stream reference value to visit.</param>
        protected virtual void VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue)
        {
        }

        /// <summary>
        /// Visits a primitive value.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to visit.</param>
        protected virtual void VisitPrimitiveValue(object primitiveValue)
        {
        }

        /// <summary>
        /// Visits an ODataBatch.
        /// </summary>
        /// <param name="batch">The batch to visit.</param>
        protected virtual void VisitBatch(ODataBatch batch)
        {
        }

        /// <summary>
        /// Visits an ODataBatchChangeset.
        /// </summary>
        /// <param name="changeset">The changeset to visit.</param>
        protected virtual void VisitBatchChangeset(ODataBatchChangeset changeset)
        {
        }

        /// <summary>
        /// Visits an ODataBatchRequestOperation.
        /// </summary>
        /// <param name="requestOperation">The request operation to visit.</param>
        protected virtual void VisitBatchRequestOperation(ODataBatchRequestOperation requestOperation)
        {
        }

        /// <summary>
        /// Visits an ODataBatchResponseOperation.
        /// </summary>
        /// <param name="responseOperation">The response operation to visit.</param>
        protected virtual void VisitBatchResponseOperation(ODataBatchResponseOperation responseOperation)
        {
        }

        /// <summary>
        /// Visits an unsupported value.
        /// </summary>
        /// <param name="value">the unsupported value to visit.</param>
        /// <returns></returns>
        protected virtual void VisitUnsupportedValue(object value)
        {
            string typename = value == null ? null : value.GetType().FullName;
            throw new TaupoNotSupportedException(string.Format("ODataObjectModel visitor doesn't yet support values of type '{0}'.", typename));
        }
    }
}
