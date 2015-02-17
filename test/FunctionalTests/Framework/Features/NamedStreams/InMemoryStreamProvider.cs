//---------------------------------------------------------------------
// <copyright file="InMemoryStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using provider = Microsoft.OData.Service.Providers;

    /// <summary>
    /// Implementation of an in-memory stream provider which supports named streams
    /// </summary>
    /// <typeparam name="TComparer">The equality comparer type to use for comparing entities</typeparam>
    public class InMemoryStreamProvider<TComparer> : IDataServiceStreamProvider2, IDataServiceStreamProvider, IDisposable
        where TComparer : IEqualityComparer<object>, new()
    {
        protected const int DefaultBufferSize = 8;

        public const string PreconditionFailedMessage = "Precondition Failed: stream E-Tag does not match If-Match header value.";

        /// <summary>
        /// Initializes static members of the InMemoryStreamProvider class
        /// </summary>
        static InMemoryStreamProvider()
        {
            Blobs = new Dictionary<object, IList<StreamWrapper>>(new TComparer());
        }
        
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
        /// Gets the Blobs dictionary
        /// </summary>
        internal static IDictionary<object, IList<StreamWrapper>> Blobs { get; private set; }

        /// <summary>
        /// Returns the Media Stream associated with an entity.
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose as method returns the stream")]
        public Stream GetReadStream(object entity, provider.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Stream stream = this.GetReadStream(entity, streamProperty, null, etag, checkETagForEquality, operationContext);
            if (stream == null)
            {
                stream = CreateMemoryStream();
            }

            return stream;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose as method returns the stream")]
        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Stream stream = this.GetReadStream(entity, null, null, etag, checkETagForEquality, operationContext);
            if (stream == null)
            {
                stream = CreateMemoryStream();
            }

            return stream;
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="name">The name of the stream</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkEtagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose as method returns the stream")]
        public Stream GetReadStream(object entity, provider.ResourceProperty streamProperty, string name, string etag, bool? checkEtagForEquality, DataServiceOperationContext operationContext)
        {
            // Check concurrency
            if (etag != null)
            {
                string etagValue = this.GetStreamETag(entity, streamProperty, operationContext);

                if (checkEtagForEquality.HasValue && checkEtagForEquality == false && etag == etagValue)
                {
                    return null;
                }

                if (checkEtagForEquality.HasValue && checkEtagForEquality == true && etag != etagValue)
                {
                    throw new DataServiceException(412, PreconditionFailedMessage);
                }
            }

            StreamWrapper wrapper;
            if (streamProperty == null)
            {
                wrapper = GetOrCreateStream(entity, name, operationContext);              // Blobs
            }
            else
            {
                wrapper = GetOrCreateStream(entity, streamProperty.Name, operationContext);   // named streams
            }

            if (wrapper.Stream != null && wrapper.Stream.CanSeek)
            {
                wrapper.Stream.Position = 0;
            }

            return wrapper.Stream;
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose stream as method returns a stream")]
        public Stream GetWriteStream(object entity, provider.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Stream stream = this.GetWriteStream(entity, streamProperty, null, etag, checkETagForEquality, operationContext);
            if (stream == null)
            {
                stream = CreateMemoryStream();
            }

            return stream;
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose stream as method returns a stream")]
        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            Stream stream = this.GetWriteStream(entity, null, null, etag, checkETagForEquality, operationContext);
            if (stream == null)
            {
                stream = CreateMemoryStream();
            }

            return stream;
        }

        /// <summary>
        /// Returns the Media Stream associated with an entity
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="name">The name of the stream</param>
        /// <param name="etag">ETag of the entity</param>
        /// <param name="checkETagForEquality">Boolean to check if eTags match</param>
        /// <param name="operationContext">Gives the Opearation Context</param>
        /// <returns>Returns the stream associated with the entity</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702: Compound words should be cased correctly", Justification = "Implemented interface requires function naming with compound words")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Review unused parameters", Justification = "Implemented interface require parameter")]
        public Stream GetWriteStream(object entity, provider.ResourceProperty streamProperty, string name, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            // Check concurrency
            if (etag != null)
            {
                string etagValue = this.GetStreamETag(entity, streamProperty, operationContext);

                if (checkETagForEquality.HasValue && checkETagForEquality == false && etag == etagValue)
                {
                    return null;
                }

                if (checkETagForEquality.HasValue && checkETagForEquality == true && etag != etagValue)
                {
                    throw new DataServiceException(412, PreconditionFailedMessage);
                }
            }

            StreamWrapper wrapper;
            if (streamProperty == null)
            {
                wrapper = GetOrCreateStream(entity, name, operationContext);
            }
            else
            {
                wrapper = GetOrCreateStream(entity, streamProperty.Name, operationContext);
            }

            GenerateStreamETag(wrapper);

            if (wrapper.Stream != null && wrapper.Stream.CanSeek)
            {
                wrapper.Stream.Position = 0;
            }

            return wrapper.Stream;
        }

        /// <summary>
        /// Returns the resource type of the media link entry associated with the the media resource being created
        /// </summary>
        /// <param name="entitySetName">The name of the entity set</param>
        /// <param name="operationContext">The data service operation context</param>
        /// <returns>Returns the type of the media link entry</returns>
        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            // TODO: read from headers?
            return null;
        } // ResolveType

        /// <summary>
        /// Deletes a streams associated with an entity
        /// </summary>
        /// <param name="entity">Entity to delete streams from</param>
        /// <param name="operationContext">Gives the operation context</param>
        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            // this will remove named streams as well
            Blobs.Remove(entity);
        }

        /// <summary>
        /// Returns the content type of the stream
        /// </summary>
        /// <param name="entity">Entity containing a stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="operationContext">Goves the Operation Context</param>
        /// <returns>Returns the content type</returns>
        public string GetStreamContentType(object entity, provider.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            return this.GetStreamContentType(entity, streamProperty, null, operationContext);
        }

        /// <summary>
        /// Returns the content type of the stream
        /// </summary>
        /// <param name="entity">Entity containing a stream</param>
        /// <param name="operationContext">Goves the Operation Context</param>
        /// <returns>Returns the content type</returns>
        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            string contentType = this.GetStreamContentType(entity, null, null, operationContext);
            if (contentType == null)
            {
                return "text/plain";   // MLE needs a content type, this is just to unblock for now
            }

            return contentType;
        }

        /// <summary>
        /// Returns the content type of the stream
        /// </summary>
        /// <param name="entity">Entity containing a stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="name">The name of the stream</param>        
        /// <param name="operationContext">Goves the Operation Context</param>
        /// <returns>Returns the content type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Review unused parameters", Justification = "Implemented interface require parameter")]
        public string GetStreamContentType(object entity, provider.ResourceProperty streamProperty, string name, DataServiceOperationContext operationContext)
        {
            StreamWrapper wrapper;

            if (streamProperty == null)
            {
                wrapper = GetStream(entity, name);
            }
            else
            {
                wrapper = GetStream(entity, streamProperty.Name);
            }

            if (wrapper == null)
            {
                return null;
            }

            return wrapper.ContentType;
        }

        /// <summary>
        /// Returns the URI associated with the stream
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream information</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the URI</returns>
        public Uri GetReadStreamUri(object entity, provider.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            return this.GetReadStreamUri(entity, streamProperty, null, operationContext);
        }

        /// <summary>
        /// Returns the URI associated with the stream
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the URI</returns>
        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return this.GetReadStreamUri(entity, null, null, operationContext);
        }

        /// <summary>
        /// Returns the URI associated with the stream
        /// </summary>
        /// <param name="entity">Entity that contains the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="name">The name of the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the URI</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Review unused parameters", Justification = "Implemented interface require parameter")]
        public Uri GetReadStreamUri(object entity, provider.ResourceProperty streamProperty, string name, DataServiceOperationContext operationContext)
        {
            StreamWrapper wrapper;

            if (streamProperty == null)
            {
                wrapper = GetStream(entity, name);
            }
            else
            {
                wrapper = GetStream(entity, streamProperty.Name);
            }

            if (wrapper == null)
            {
                return null;
            }

            return wrapper.ReadUri;
        }

        /// <summary>
        /// Returns the ETag of the stream
        /// </summary>
        /// <param name="entity">Entity with the stream</param>
        /// <param name="streamProperty">Stream property</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the Stream ETag</returns>
        public string GetStreamETag(object entity, provider.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            return this.GetStreamETag(entity, streamProperty, null, operationContext);
        }

        /// <summary>
        /// Returns the ETag of the stream
        /// </summary>
        /// <param name="entity">Entity with the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the Stream ETag</returns>
        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return this.GetStreamETag(entity, null, null, operationContext);
        }

        /// <summary>
        /// Returns the ETag of the named stream
        /// </summary>
        /// <param name="entity">Entity with the stream</param>
        /// <param name="stremProperty">Stream property</param>
        /// <param name="name">The name of the stream</param>
        /// <param name="operationContext">Gives the operation context</param>
        /// <returns>Returns the Stream ETag</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702: Compound words should be cased correctly", Justification = "Implemented interface requires function naming with compound words")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Review unused parameters", Justification = "Implemented interface require parameter")]
        public string GetStreamETag(object entity, provider.ResourceProperty stremProperty, string name, DataServiceOperationContext operationContext)
        {
            StreamWrapper wrapper;
            if (stremProperty == null)
            {
                wrapper = GetStream(entity, name);
            }
            else
            {
                wrapper = GetStream(entity, stremProperty.Name);
            }

            if (wrapper == null)
            {
                return null;
            }

            GenerateStreamETag(wrapper);
            return wrapper.ETag;
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
        /// Generates an ETag for the given stream wrapper and sets it
        /// </summary>
        /// <param name="wrapper">The stream wrapper</param>
        internal static void GenerateStreamETag(StreamWrapper wrapper)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                ResetStream(wrapper);

                var hash = cryptoProvider.ComputeHash(wrapper.Stream);
                wrapper.ETag = '"' + Convert.ToBase64String(hash) + '"';

                ResetStream(wrapper);
            }
        }

        /// <summary>
        /// Disposes the provider
        /// </summary>
        /// <param name="disposing">Whether or not to release managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // DO NOT CLEAR THE DICTIONARY
        }

        private static void ResetStream(StreamWrapper wrapper)
        {
            if (wrapper.Stream.CanSeek)
            {
                wrapper.Stream.Position = 0;
            }
        }

        private static StreamWrapper GetOrCreateStream(object entity, string name, DataServiceOperationContext operationContext)
        {
            StreamWrapper wrapper;
            lock (Blobs)
            {
                IList<StreamWrapper> wrappers;
                if (!Blobs.TryGetValue(entity, out wrappers))
                {
                    Blobs[entity] = wrappers = new List<StreamWrapper>();
                }

                wrapper = wrappers.SingleOrDefault(w => w.Name == name);
                if (wrapper == null)
                {
                    wrapper = new StreamWrapper();
                    wrapper.Name = name;
                    if (operationContext != null)
                    {
                        wrapper.ContentType = operationContext.RequestHeaders[HttpRequestHeader.ContentType];
                    }

                    wrapper.Stream = CreateMemoryStream();
                    wrapper.ReadUri = null;

                    GenerateStreamETag(wrapper);

                    wrappers.Add(wrapper);
                }
            }

            return wrapper;
        }

        private static StreamWrapper GetStream(object entity, string name)
        {
            lock (Blobs)
            {
                IList<StreamWrapper> wrappers;
                if (!Blobs.TryGetValue(entity, out wrappers))
                {
                    return null;
                }

                if (name == null)
                {
                    return wrappers.FirstOrDefault();
                }

                return wrappers.SingleOrDefault(w => w.Name == name);
            }
        }

        private static Stream CreateMemoryStream()
        {
            return new ReusableStream();
        }

        internal class StreamWrapper
        {
            public string Name { get; set; }

            public string ETag { get; set; }

            public string ContentType { get; set; }

            public Stream Stream { get; set; }

            public Uri ReadUri { get; set; }
        }

        /// <summary>
        /// Overriding the dispose method so that after the Astoria runtime disposes the stream returned by
        /// the stream provider, we can still reuse the streams for testing purposes.
        /// </summary>
        private class ReusableStream : MemoryStream, IDisposable
        {
            void IDisposable.Dispose()
            {
                // Resets the stream
                this.Position = 0;
            }

            protected override void Dispose(bool disposing)
            {
                this.Position = 0;
            }
        }
    }
}