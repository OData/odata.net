//---------------------------------------------------------------------
// <copyright file="LoadPropertyResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>wrapper around loading a property from a response</summary>
    internal class LoadPropertyResult : QueryResult
    {
        #region Private fields

        /// <summary>entity whose property is being loaded</summary>
        private readonly object entity;

        /// <summary>Projection plan for loading results; possibly null.</summary>
        private readonly ProjectionPlan plan;

        /// <summary>name of the property on the entity that is being loaded</summary>
        private readonly string propertyName;

        #endregion Private fields

        /// <summary>constructor</summary>
        /// <param name="entity">entity</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="context">Originating context</param>
        /// <param name="request">Originating WebRequest</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state</param>
        /// <param name="dataServiceRequest">request object.</param>
        /// <param name="plan">Projection plan for materialization; possibly null.</param>
        /// <param name="isContinuation">Whether this request is a continuation request.</param>
        internal LoadPropertyResult(object entity, string propertyName, DataServiceContext context, ODataRequestMessageWrapper request, AsyncCallback callback, object state, DataServiceRequest dataServiceRequest, ProjectionPlan plan, bool isContinuation)
            : base(context, Util.LoadPropertyMethodName, dataServiceRequest, request, new RequestInfo(context, isContinuation), callback, state)
        {
            this.entity = entity;
            this.propertyName = propertyName;
            this.plan = plan;
        }

        /// <summary>
        /// loading a property from a response
        /// </summary>
        /// <returns>QueryOperationResponse instance containing information about the response.</returns>
        internal QueryOperationResponse LoadProperty()
        {
            MaterializeAtom results = null;

            DataServiceContext context = (DataServiceContext)this.Source;

            ClientEdmModel model = context.Model;
            ClientTypeAnnotation type = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(this.entity.GetType()));
            Debug.Assert(type.IsEntityType, "must be entity type to be contained");

            EntityDescriptor box = context.GetEntityDescriptor(this.entity);

            if (EntityStates.Added == box.State)
            {
                throw Error.InvalidOperation(Strings.Context_NoLoadWithInsertEnd);
            }

            ClientPropertyAnnotation property = type.GetProperty(this.propertyName, UndeclaredPropertyBehavior.ThrowException);
            Type elementType = property.EntityCollectionItemType ?? property.NullablePropertyType;
            try
            {
                if (type.MediaDataMember == property)
                {
                    results = this.ReadPropertyFromRawData(property);
                }
                else
                {
                    results = this.ReadPropertyFromAtom(property);
                }

                return this.GetResponseWithType(results, elementType);
            }
            catch (InvalidOperationException ex)
            {
                QueryOperationResponse response = this.GetResponseWithType(results, elementType);
                if (response != null)
                {
                    response.Error = ex;
                    throw new DataServiceQueryException(Strings.DataServiceException_GeneralError, ex, response);
                }

                throw;
            }
        }

        /// <summary>
        /// Creates the ResponseInfo object.
        /// </summary>
        /// <returns>ResponseInfo object.</returns>
        protected override ResponseInfo CreateResponseInfo()
        {
            DataServiceContext context = (DataServiceContext)this.Source;

            ClientEdmModel model = context.Model;

            ClientTypeAnnotation type = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(this.entity.GetType()));

            Debug.Assert(type.IsEntityType, "Must be entity type to be contained.");

            return this.RequestInfo.GetDeserializationInfoForLoadProperty(
                null,
                context.GetEntityDescriptor(this.entity),
                type.GetProperty(this.propertyName, UndeclaredPropertyBehavior.ThrowException));
        }

        /// <summary>
        /// Reads the data from the response stream into a buffer using the content length.
        /// </summary>
        /// <param name="responseStream">Response stream.</param>
        /// <param name="totalLength">Length of data to read.</param>
        /// <returns>byte array containing read data.</returns>
        private static byte[] ReadByteArrayWithContentLength(Stream responseStream, int totalLength)
        {
            byte[] buffer = new byte[totalLength];
            int read = 0;
            while (read < totalLength)
            {
                int r = responseStream.Read(buffer, read, totalLength - read);
                if (r <= 0)
                {
                    throw Error.InvalidOperation(Strings.Context_UnexpectedZeroRawRead);
                }

                read += r;
            }

            return buffer;
        }

        /// <summary>Reads the data from the response stream in chunks.</summary>
        /// <param name="responseStream">Response stream.</param>
        /// <returns>byte array containing read data.</returns>
        private static byte[] ReadByteArrayChunked(Stream responseStream)
        {
            byte[] completeBuffer = null;
            using (MemoryStream m = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int numRead = 0;
                int totalRead = 0;
                while (true)
                {
                    numRead = responseStream.Read(buffer, 0, buffer.Length);
                    if (numRead <= 0)
                    {
                        break;
                    }

                    m.Write(buffer, 0, numRead);
                    totalRead += numRead;
                }

                completeBuffer = new byte[totalRead];
                m.Position = 0;
                numRead = m.Read(completeBuffer, 0, completeBuffer.Length);
            }

            return completeBuffer;
        }

        /// <summary>
        /// Load property data from an ATOM response
        /// </summary>
        /// <param name="property">The property being loaded</param>
        /// <returns>property values as IEnumerable.</returns>
        private MaterializeAtom ReadPropertyFromAtom(ClientPropertyAnnotation property)
        {
            DataServiceContext context = (DataServiceContext)this.Source;
            bool merging = context.ApplyingChanges;

            try
            {
                context.ApplyingChanges = true;

                // store the results so that they can be there in the response body.
                Type elementType = property.IsEntityCollection ? property.EntityCollectionItemType : property.NullablePropertyType;
                IList results = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                DataServiceQueryContinuation continuation = null;

                // elementType.ElementType has Nullable stripped away, use nestedType for materializer
                using (MaterializeAtom materializer = this.GetMaterializer(this.plan))
                {
                    Debug.Assert(materializer != null, "materializer != null -- otherwise GetMaterializer() returned null rather than empty");

                    // when SetLink to null, we cannot get materializer because have no-content response.
                    if (materializer.IsNoContentResponse()
                        && property.GetValue(entity) != null
                        && context.MergeOption != MergeOption.AppendOnly
                        && context.MergeOption != MergeOption.NoTracking)
                    {
                        property.SetValue(this.entity, null, propertyName, false);
                    }
                    else
                    {
                        foreach (object child in materializer)
                        {
                            if (property.IsEntityCollection)
                            {
                                results.Add(child);
                            }
                            else if (property.IsPrimitiveOrEnumOrComplexCollection)
                            {
                                Debug.Assert(property.PropertyType.IsAssignableFrom(child.GetType()), "Created instance for storing collection items has to be compatible with the actual one.");

                                // Collection materialization rules requires to clear the collection if not null or set the property first and then add the collection items
                                object collectionInstance = property.GetValue(this.entity);
                                if (collectionInstance == null)
                                {
                                    // type of child has been resolved as per rules for collections so it is the correct type to instantiate
                                    collectionInstance = Activator.CreateInstance(child.GetType());

                                    // allowAdd is false - we need to assign instance as the new property value
                                    property.SetValue(this.entity, collectionInstance, this.propertyName, false /* allowAdd? */);
                                }
                                else
                                {
                                    // Clear existing collection
                                    property.ClearBackingICollectionInstance(collectionInstance);
                                }

                                foreach (var collectionItem in (IEnumerable)child)
                                {
                                    Debug.Assert(property.PrimitiveOrComplexCollectionItemType.IsAssignableFrom(collectionItem.GetType()), "Type of materialized collection items have to be compatible with the type of collection items in the actual collection property.");
                                    property.AddValueToBackingICollectionInstance(collectionInstance, collectionItem);
                                }

                                results.Add(collectionInstance);
                            }
                            else
                            {
                                // it is either primitive type, complex type or 1..1 navigation property so we just allow setting the value but not adding.
                                property.SetValue(this.entity, child, this.propertyName, false);
                                results.Add(child);
                            }
                        }
                    }

                    continuation = materializer.GetContinuation(null);
                }

                return MaterializeAtom.CreateWrapper(context, results, continuation);
            }
            finally
            {
                context.ApplyingChanges = merging;
            }
        }

        /// <summary>
        /// Load property data form a raw response
        /// </summary>
        /// <param name="property">The property being loaded</param>
        /// <returns>property values as IEnumerable.</returns>
        private MaterializeAtom ReadPropertyFromRawData(ClientPropertyAnnotation property)
        {
            DataServiceContext context = (DataServiceContext)this.Source;

            bool merging = context.ApplyingChanges;

            try
            {
                context.ApplyingChanges = true;

                // if this is the data property for a media entry, what comes back
                // is the raw value (no markup)
                string mimeType = null;
                Encoding encoding = null;
                Type elementType = property.EntityCollectionItemType ?? property.NullablePropertyType;
                IList results = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                ContentTypeUtil.ReadContentType(this.ContentType, out mimeType, out encoding);

                using (Stream responseStream = this.GetResponseStream())
                {
                    // special case byte[], and for everything else let std conversion kick-in
                    if (property.PropertyType == typeof(byte[]))
                    {
                        int total = checked((int)this.ContentLength);
                        byte[] buffer = null;
                        if (total >= 0)
                        {
                            buffer = LoadPropertyResult.ReadByteArrayWithContentLength(responseStream, total);
                        }
                        else
                        {
                            buffer = LoadPropertyResult.ReadByteArrayChunked(responseStream);
                        }

                        results.Add(buffer);

                        property.SetValue(this.entity, buffer, this.propertyName, false);
                    }
                    else
                    {
                        // responseStream will disposed, StreamReader doesn't need to dispose of it.
                        StreamReader reader = new StreamReader(responseStream, encoding);
                        object convertedValue = property.PropertyType == typeof(string) ?
                                                    reader.ReadToEnd() :
                                                    ClientConvert.ChangeType(reader.ReadToEnd(), property.PropertyType);
                        results.Add(convertedValue);

                        property.SetValue(this.entity, convertedValue, this.propertyName, false);
                    }
                }

                if (property.MimeTypeProperty != null)
                {
                    // an implication of this 3rd-arg-null is that mime type properties cannot be open props
                    property.MimeTypeProperty.SetValue(this.entity, mimeType, null, false);
                }

                return MaterializeAtom.CreateWrapper(context, results);
            }
            finally
            {
                context.ApplyingChanges = merging;
            }
        }
    }
}
