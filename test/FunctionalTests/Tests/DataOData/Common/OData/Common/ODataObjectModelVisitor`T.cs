//---------------------------------------------------------------------
// <copyright file="ODataObjectModelVisitor`T.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Spatial;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    #endregion Namespaces

    /// <summary>
    /// Visitor base class to use for visiting OData object models
    /// </summary>
    public abstract class ODataObjectModelVisitor<T>
    {
        /// <summary>
        /// Visits an item in the object model.
        /// </summary>
        /// <param name="objectModelItem">The item to visit.</param>
        public virtual T Visit(object objectModelItem)
        {
            ODataResourceSet feed = objectModelItem as ODataResourceSet;
            if (feed != null)
            {
                return this.VisitFeed(feed);
            }

            ODataResource entry = objectModelItem as ODataResource;
            if (entry != null)
            {
                return this.VisitEntry(entry);
            }

            ODataProperty property = objectModelItem as ODataProperty;
            if (property != null)
            {
                return this.VisitProperty(property);
            }

            ODataNestedResourceInfo navigationLink = objectModelItem as ODataNestedResourceInfo;
            if (navigationLink != null)
            {
                return this.VisitNavigationLink(navigationLink);
            }

            ODataCollectionValue collection = objectModelItem as ODataCollectionValue;
            if (collection != null)
            {
                return this.VisitCollectionValue(collection);
            }

            ODataStreamReferenceValue streamReferenceValue = objectModelItem as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                return this.VisitStreamReferenceValue(streamReferenceValue);
            }

            ODataCollectionStart collectionStart = objectModelItem as ODataCollectionStart;
            if (collectionStart != null)
            {
                return this.VisitCollectionStart(collectionStart);
            }

            ODataServiceDocument serviceDocument = objectModelItem as ODataServiceDocument;
            if (serviceDocument != null)
            {
                return this.VisitWorkspace(serviceDocument);
            }

            ODataEntitySetInfo entitySetInfo = objectModelItem as ODataEntitySetInfo;
            if (entitySetInfo != null)
            {
                return this.VisitResourceCollection(entitySetInfo);
            }

            ODataError error = objectModelItem as ODataError;
            if (error != null)
            {
                return this.VisitError(error);
            }

            ODataInnerError innerError = objectModelItem as ODataInnerError;
            if (innerError != null)
            {
                return this.VisitInnerError(innerError);
            }

            ODataEntityReferenceLinks entityReferenceLinks = objectModelItem as ODataEntityReferenceLinks;
            if (entityReferenceLinks != null)
            {
                return this.VisitEntityReferenceLinks(entityReferenceLinks);
            }

            ODataEntityReferenceLink entityReferenceLink = objectModelItem as ODataEntityReferenceLink;
            if (entityReferenceLink != null)
            {
                return this.VisitEntityReferenceLink(entityReferenceLink);
            }

            ODataAction action = objectModelItem as ODataAction;
            if (action != null)
            {
                return this.VisitODataOperation(action);
            }

            ODataFunction function = objectModelItem as ODataFunction;
            if (function != null)
            {
                return this.VisitODataOperation(function);
            }

            ODataParameters parameters = objectModelItem as ODataParameters;
            if (parameters != null)
            {
                return this.VisitParameters(parameters);
            }

            ODataBatch batch = objectModelItem as ODataBatch;
            if (batch != null)
            {
                return this.VisitBatch(batch);
            }

            if (objectModelItem == null || objectModelItem.GetType().IsValueType || objectModelItem is string ||
                objectModelItem is byte[] || objectModelItem is ISpatial)
            {
                return this.VisitPrimitiveValue(objectModelItem);
            }

            if (objectModelItem is ODataUntypedValue)
            {
                object val = ODataObjectModelVisitor.ParseJsonToPrimitiveValue(
                    (objectModelItem as ODataUntypedValue).RawValue);
                return this.VisitPrimitiveValue(val);
            }

            return this.VisitUnsupportedValue(objectModelItem);
        }

        /// <summary>
        /// Visits a feed item.
        /// </summary>
        /// <param name="feed">The feed to visit.</param>
        protected abstract T VisitFeed(ODataResourceSet resourceCollection);

        /// <summary>
        /// Visits an entry item.
        /// </summary>
        /// <param name="entry">The entry to visit.</param>
        protected abstract T VisitEntry(ODataResource entry);

        /// <summary>
        /// Visits a property item.
        /// </summary>
        /// <param name="property">The property to visit.</param>
        protected abstract T VisitProperty(ODataProperty property);

        /// <summary>
        /// Visits a navigation link item.
        /// </summary>
        /// <param name="navigationLink">The navigation link to visit.</param>
        protected abstract T VisitNavigationLink(ODataNestedResourceInfo navigationLink);

        /// <summary>
        /// Visits a collection item.
        /// </summary>
        /// <param name="collectionValue">The collection to visit.</param>
        protected abstract T VisitCollectionValue(ODataCollectionValue collectionValue);

        /// <summary>
        /// Visits a collection start.
        /// </summary>
        /// <param name="collection">The collection start to visit.</param>
        protected abstract T VisitCollectionStart(ODataCollectionStart collection);

        /// <summary>
        /// Visits an error.
        /// </summary>
        /// <param name="error">The error to visit.</param>
        protected abstract T VisitError(ODataError error);

        /// <summary>
        /// Visits an inner error.
        /// </summary>
        /// <param name="innerError">The inner error to visit.</param>
        protected abstract T VisitInnerError(ODataInnerError innerError);

        /// <summary>
        /// Visits a serviceDocument.
        /// </summary>
        /// <param name="serviceDocument">The serviceDocument to visit.</param>
        protected abstract T VisitWorkspace(ODataServiceDocument serviceDocument);

        /// <summary>
        /// Visits a resource collection.
        /// </summary>
        /// <param name="entitySetInfo">The resource collection to visit.</param>
        protected abstract T VisitResourceCollection(ODataEntitySetInfo entitySetInfo);

        /// <summary>
        /// Visits an entity reference link collection.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference link collection to visit.</param>
        protected abstract T VisitEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks);

        /// <summary>
        /// Visits an entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to visit.</param>
        protected abstract T VisitEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink);

        /// <summary>
        /// Visits a stream reference value (named stream).
        /// </summary>
        /// <param name="streamReferenceValue">The stream reference value to visit.</param>
        protected abstract T VisitStreamReferenceValue(ODataStreamReferenceValue streamReferenceValue);

        /// <summary>
        /// Visits a primitive value.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to visit.</param>
        protected abstract T VisitPrimitiveValue(object primitiveValue);

        /// <summary>
        /// Visits an ODataOperation.
        /// </summary>
        /// <param name="operation">The operation to visit.</param>
        protected abstract T VisitODataOperation(ODataOperation operation);

        /// <summary>
        /// Visits parameters.
        /// </summary>
        /// <param name="parameters">The parameters to visit.</param>
        protected abstract T VisitParameters(ODataParameters parameters);

        /// <summary>
        /// Visits an ODataBatch.
        /// </summary>
        /// <param name="batch">The batch to visit.</param>
        protected abstract T VisitBatch(ODataBatch batch);

        /// <summary>
        /// Visits an unsupported value.
        /// </summary>
        /// <param name="value">the unsupported value to visit.</param>
        /// <returns></returns>
        protected virtual T VisitUnsupportedValue(object value)
        {
            string typename = value == null ? null : value.GetType().FullName;
            throw new TaupoNotSupportedException(string.Format("ODataObjectModel visitor doesn't yet support values of type '{0}'.", typename));
        }
    }
}
