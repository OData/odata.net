//---------------------------------------------------------------------
// <copyright file="InMemoryStreamProvider`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.OptionalProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// Implementation of an in-memory stream provider which supports named streams
    /// </summary>
    /// <typeparam name="TComparer">The equality comparer type to use for comparing entities</typeparam>
    public class InMemoryStreamProvider<TComparer> : IDataServiceStreamProvider2, IDataServiceStreamProvider, IDisposable
        where TComparer : IEqualityComparer<object>, new()
    {
        protected const int DefaultBufferSize = 1 << 15;
        private const string EntityTypeHintHeaderName = "MediaLinkEntry-TypeNameHint";
        private static IDictionary<object, HashSet<StreamWrapper>> permanentBlobStorage = new Dictionary<object, HashSet<StreamWrapper>>(CreateEqualityComparer());
        private IDictionary<object, HashSet<StreamWrapper>> temporaryBlobStorage = new Dictionary<object, HashSet<StreamWrapper>>(ReferenceEqualityComparer.Default);
        private Func<DataServiceOperationContext, string> getContentTypeForNewStreamFunc = GetContentTypeFromHeaders;

        /// <summary>
        /// Gets the size of Stream Buffer
        /// </summary>
        public int StreamBufferSize
        {
            get
            {
                return DefaultBufferSize;
            }
        }

        /// <summary>
        /// Gets or sets the delegate for use when trying to get the content type of a newly created stream
        /// </summary>
        public Func<DataServiceOperationContext, string> GetContentTypeForNewStreamFunc
        {
            get
            {
                return this.getContentTypeForNewStreamFunc;
            }

            set
            {
                ExceptionUtilities.CheckArgumentNotNull(value, "value");
                this.getContentTypeForNewStreamFunc = value;
            }
        }

        /// <summary>
        /// Gets the long-living static blobs storage dictionary
        /// </summary>
        internal static IDictionary<object, HashSet<StreamWrapper>> BlobsStorage
        {
            get { return permanentBlobStorage; }
        }

        private bool AllowNullOperationContext
        {
            get { return !ProviderImplementationSettings.Current.StrictInputVerification; }
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity.    This is the interface called by V2 blob MR/MLE.
        /// There should always be a read stream for a blob.
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        public virtual Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            return this.GetReadStreamInternal(entity, null, etag, checkETagForEquality, operationContext);
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        public virtual Stream GetReadStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(streamProperty, "streamProperty");
            this.CheckOperationContext(operationContext);

            return this.GetReadStreamInternal(entity, streamProperty, etag, checkETagForEquality, operationContext);
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        public virtual Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            return this.GetWriteStreamInternal(entity, null, etag, checkETagForEquality, operationContext, null);
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702: Compound words should be cased correctly", Justification = "Implemented interface requires function naming with compound words")]
        public virtual Stream GetWriteStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(streamProperty, "streamProperty");
            this.CheckOperationContext(operationContext);

            return this.GetWriteStreamInternal(entity, streamProperty.Name, etag, checkETagForEquality, operationContext, null);
        }

        /// <summary>
        /// Initializes the entity media stream, used to add data during initialization of the service.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="etag">The etag.</param>
        /// <param name="checkForETagEquality">The check for ETag equality.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="streamData">The stream data.</param>
        public void InitializeEntityMediaStream(object entity, string etag, bool? checkForETagEquality, string contentType, byte[] streamData)
        {
            var stream = this.GetWriteStreamInternal(entity, null, etag, checkForETagEquality, null, contentType);
            stream.Write(streamData, 0, streamData.Length);
        }

        /// <summary>
        /// Initializes the entity named stream, used to add data during initialization of the service.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="property">The property.</param>
        /// <param name="etag">The etag.</param>
        /// <param name="checkForETagEquality">The check for E tag equality.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="streamData">The stream data.</param>
        public void InitializeEntityNamedStream(object entity, string property, string etag, bool? checkForETagEquality, string contentType, byte[] streamData)
        {
            var stream = this.GetWriteStreamInternal(entity, property, etag, checkForETagEquality, null, contentType);
            stream.Write(streamData, 0, streamData.Length);
        }

        /// <summary>
        /// Returns the resource type of the media link entry associated with the the media resource being created
        /// </summary>
        /// <param name="entitySetName">The name of the entity set</param>
        /// <param name="operationContext">The data service operation context</param>
        /// <returns>Returns the type of the media link entry</returns>
        public virtual string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetName, "entitySetName");
            this.CheckOperationContext(operationContext);
            var entityTypeHint = operationContext.RequestHeaders[EntityTypeHintHeaderName];

            return entityTypeHint != null ? Uri.UnescapeDataString(entityTypeHint) : null;
        }

        /// <summary>
        /// Deletes a streams associated with an entity
        /// </summary>
        /// <param name="entity">Entity to delete streams from</param>
        /// <param name="operationContext">Gives the operation context</param>
        public virtual void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            // this will remove named streams as well
            BlobsStorage.Remove(entity);
        }
        
        /// <summary>
        /// Returns the content type of the stream
        /// </summary>
        /// <param name="entity">Entity containing a stream</param>
        /// <param name="operationContext">Goves the Operation Context</param>
        /// <returns>Returns the content type</returns>
        public virtual string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            string contentType = this.GetStreamContentTypeInternal(entity, null);
            if (contentType == null)
            {
                return "*/*";   // MLE needs a content type, this is just to unblock for now
            }

            return contentType;
        }

        /// <summary>
        /// Returns the content type of the stream
        /// </summary>
        /// <param name="entity">Entity containing a stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="operationContext">The the operation context</param>
        /// <returns>Returns the content type</returns>
        public virtual string GetStreamContentType(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(streamProperty, "streamProperty");
            this.CheckOperationContext(operationContext);

            return this.GetStreamContentTypeInternal(entity, streamProperty);
        }

        /// <summary>
        /// Returns the URI associated with the stream
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the URI</returns>
        public virtual Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            return this.GetReadStreamUriInternal(entity, null);
        }

        /// <summary>
        /// Returns the URI associated with the stream
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the URI</returns>
        public virtual Uri GetReadStreamUri(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(streamProperty, "streamProperty");
            this.CheckOperationContext(operationContext);

            return this.GetReadStreamUriInternal(entity, streamProperty);
        }

        /// <summary>
        /// Returns the ETag of the stream
        /// </summary>
        /// <param name="entity">Entity with the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the Stream ETag</returns>
        public virtual string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            this.CheckOperationContext(operationContext);

            return this.GetStreamETagInternal(entity, null);
        }

        /// <summary>
        /// Returns the ETag of the stream
        /// </summary>
        /// <param name="entity">Entity with the stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the Stream ETag</returns>
        public virtual string GetStreamETag(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(streamProperty, "streamProperty");
            this.CheckOperationContext(operationContext);

            return this.GetStreamETagInternal(entity, streamProperty.Name);
        }

        /// <summary>
        /// Implements the dispose method
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Saves the current set of streams to the permanant static storage
        /// </summary>
        public void SaveStreams()
        {
            // lock both dictionaries
            lock (permanentBlobStorage)
            {
                lock (this.temporaryBlobStorage)
                {
                    // copy the temporary streams over to the permanent store
                    foreach (var temporaryStorage in this.temporaryBlobStorage)
                    {
                        ExceptionUtilities.Assert(
                            !permanentBlobStorage.ContainsKey(temporaryStorage.Key),
                            "Permanent and temporary storage should never both have an entry for the same entity");

                        permanentBlobStorage[temporaryStorage.Key] = temporaryStorage.Value;
                    }

                    // clear the temporary store
                    this.temporaryBlobStorage.Clear();
                }
            }
        }

        /// <summary>
        /// Generates an ETag for the given stream wrapper and sets it
        /// </summary>
        /// <param name="wrapper">The stream wrapper</param>
        internal static void UpdateStreamETag(StreamWrapper wrapper)
        {
            ResetStream(wrapper);

            if (wrapper.Stream == null)
            {
                wrapper.ETag = null;
                return;
            }

            var streamContent = GetStreamContent(wrapper.Stream);
            if (streamContent.Length == 0)
            {
                wrapper.ETag = string.Empty;
                return;
            }

            // decide whether to write an ETag based on whether the last bit of the stream is 0
            if ((streamContent[streamContent.Length - 1] & 1) == 0)
            {
                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    var hash = cryptoProvider.ComputeHash(streamContent);
                    wrapper.ETag = '"' + Convert.ToBase64String(hash) + '"';
                }
            }
            else
            {
                wrapper.ETag = null;
            }
        }

        internal static string GetContentTypeFromHeaders(DataServiceOperationContext operationContext)
        {
            if (operationContext != null)
            {
                return operationContext.RequestHeaders[HttpRequestHeader.ContentType];
            }

            return null;
        }
       
        /// <summary>
        /// Disposes the provider
        /// </summary>
        /// <param name="disposing">Whether or not to release managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // DO NOT CLEAR THE PERMANENT DICTIONARY
            this.SaveStreams();
        }

        private static void ResetStream(StreamWrapper wrapper)
        {
            if (wrapper.Stream != null && wrapper.Stream.CanSeek)
            {
                wrapper.Stream.Position = 0;
            }
        }

        private static Stream CreateMemoryStream()
        {
            return new ReusableStream();
        }

        private static byte[] GetStreamContent(Stream stream)
        {
            return ((ReusableStream)stream).ToArray();
        }

        private static IEqualityComparer<object> CreateEqualityComparer()
        {
            // doing this instead of 'new TComparer' due to code-coverage issues with that style
            return Activator.CreateInstance<TComparer>();
        }
        
        private void CheckOperationContext(DataServiceOperationContext operationContext)
        {
            if (!this.AllowNullOperationContext)
            {
                ExceptionUtilities.CheckArgumentNotNull(operationContext, "operationContext");
            }
        }

        private Stream GetReadStreamInternal(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            // Check concurrency
            if (etag != null)
            {
                string etagValue;
                if (streamProperty != null)
                {
                    etagValue = this.GetStreamETagInternal(entity, streamProperty.Name);
                }
                else
                {
                      etagValue = this.GetStreamETagInternal(entity, null);                
                }

                // if the Etag is a If-None-Match ex: makeing a GET call for a MLE, Adding in thid Check for A GET call avoids that case
                 if (checkETagForEquality == false && etag == etagValue && operationContext.RequestMethod != "GET")
                {
                    return null;
                }
                
                if (checkETagForEquality == true && etag != etagValue)
                {
                    throw new DataServiceException(412, "Pre condition Failed: stream E-Tag does not match If-Match header value");
                }
            }

            string name = null;
            if (streamProperty != null)
            {
                name = streamProperty.Name;
            }
            
            return this.GetOrCreateStream(entity, name, false, operationContext, null);
        }

        private Stream GetWriteStreamInternal(object entity, string streamPropertyName, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext, string contentType)
        {
            // Check concurrency
            if (etag != null)
            {
                string etagValue = this.GetStreamETagInternal(entity, streamPropertyName);

                if (checkETagForEquality == false && etag == etagValue)
                {
                    return null;
                }

                if (checkETagForEquality == true && etag != etagValue)
                {
                    throw new DataServiceException(412, "Pre condition Failed: stream E-Tag does not match If-Match header value");
                }
            }

            return this.GetOrCreateStream(entity, streamPropertyName, true, operationContext, null);
        }

        private string GetStreamContentTypeInternal(object entity, ResourceProperty streamProperty)
        {
            StreamWrapper wrapper;
            if (streamProperty == null)
            {
                wrapper = this.GetStreamDescriptor(entity, null);
            }
            else
            {
                wrapper = this.GetStreamDescriptor(entity, streamProperty.Name);
            }

            if (wrapper == null)
            {
                return null;
            }

            return wrapper.ContentType;
        }

        private string GetStreamETagInternal(object entity, string streamPropertyName)
        {
            StreamWrapper wrapper;
            if (streamPropertyName == null)
            {
                wrapper = this.GetStreamDescriptor(entity, null);
            }
            else
            {
                wrapper = this.GetStreamDescriptor(entity, streamPropertyName);
            }

            if (wrapper == null)
            {
                return null;
            }

            UpdateStreamETag(wrapper);
            return wrapper.ETag;
        }

        private Uri GetReadStreamUriInternal(object entity, ResourceProperty streamProperty)
        {
            StreamWrapper wrapper;

            if (streamProperty == null)
            {
                wrapper = this.GetStreamDescriptor(entity, null);
            }
            else
            {
                wrapper = this.GetStreamDescriptor(entity, streamProperty.Name);
            }

            if (wrapper == null)
            {
                return null;
            }

            return wrapper.ReadUri;
        }

        private Stream GetOrCreateStream(object entity, string name, bool forWriting, DataServiceOperationContext operationContext, string contentType)
        {
            lock (this.temporaryBlobStorage)
            {
                var wrappers = this.GetStreamStorage(entity);
                var wrapper = wrappers.SingleOrDefault(w => w.Name == name);
                bool isNewStream = wrapper == null;
                if (isNewStream)
                {
                    wrapper = new StreamWrapper() { Name = name };
                    wrappers.Add(wrapper);
                }

                bool mustCreateStream = forWriting || (name == null && wrapper.Stream == null);
                if (isNewStream || mustCreateStream)
                {
                    if (mustCreateStream)
                    {
                        wrapper.Stream = CreateMemoryStream();
                    }

                    if (operationContext == null)
                    {
                        wrapper.ContentType = contentType;
                    }
                    else
                    {
                        wrapper.ContentType = this.GetContentTypeForNewStreamFunc(operationContext);
                    }

                    UpdateStreamETag(wrapper);
                }

                ResetStream(wrapper);

                return wrapper.Stream;
            }
        }
        
        private StreamWrapper GetStreamDescriptor(object entity, string name)
        {
            lock (this.temporaryBlobStorage)
            {
                var wrappers = this.GetStreamStorage(entity);

                var wrapper = wrappers.SingleOrDefault(w => w.Name == name);

                if (wrapper == null && name == null)
                {
                    // initial set of entities are created without going through the stream provider
                    // so we may need to create media-resources for media-link-entries that don't have them yet
                    wrapper = new StreamWrapper();
                    UpdateStreamETag(wrapper);
                    wrappers.Add(wrapper);
                }

                return wrapper;
            }
        }

        private HashSet<StreamWrapper> GetStreamStorage(object entity)
        {
            lock (this.temporaryBlobStorage)
            {
                HashSet<StreamWrapper> wrappers;
                if (!this.temporaryBlobStorage.TryGetValue(entity, out wrappers))
                {
                    lock (permanentBlobStorage)
                    {
                        if (!permanentBlobStorage.TryGetValue(entity, out wrappers))
                        {
                            this.temporaryBlobStorage[entity] = wrappers = new HashSet<StreamWrapper>(new StreamWrapperNameEqualityComparer());
                        }
                    }
                }

                return wrappers;
            }
        }

        /// <summary>
        /// Equality comparer for stream wrappers that uses the stream name to determine equality
        /// </summary>
        internal class StreamWrapperNameEqualityComparer : EqualityComparer<StreamWrapper>
        {
            /// <summary>
            /// Returns whether the streams have the same name
            /// </summary>
            /// <param name="x">The first stream</param>
            /// <param name="y">The second stream</param>
            /// <returns>Whether the stream names are the same</returns>
            public override bool Equals(StreamWrapper x, StreamWrapper y)
            {
                ExceptionUtilities.CheckArgumentNotNull(x, "x");
                ExceptionUtilities.CheckArgumentNotNull(y, "y");
                return x.Name == y.Name;
            }

            /// <summary>
            /// Returns the hash code of the stream's name
            /// </summary>
            /// <param name="obj">The stream to get the hash code for</param>
            /// <returns>The hash code of the stream's name, or 0 if it is null</returns>
            public override int GetHashCode(StreamWrapper obj)
            {
                ExceptionUtilities.CheckArgumentNotNull(obj, "obj");
                if (obj.Name == null)
                {
                    return 0;
                }

                return obj.Name.GetHashCode();
            }
        }
    }
}