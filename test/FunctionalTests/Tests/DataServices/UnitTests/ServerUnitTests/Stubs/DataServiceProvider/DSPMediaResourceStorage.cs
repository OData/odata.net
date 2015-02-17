//---------------------------------------------------------------------
// <copyright file="DSPMediaResourceStorage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// In memory storage for media resources. While the DSPStreamProvider gets created and disposed per request, this cache can persist across requests.
    /// </summary>
    public class DSPMediaResourceStorage
    {
        /// <summary>
        /// Storage for the streams
        /// </summary>
        private Dictionary<object, Dictionary<string, DSPMediaResource>> inMemoryStreamStorage;

        /// <summary>
        /// Equality comparer for the object key.
        /// </summary>
        private IEqualityComparer<object> keyComparer;

        /// <summary>
        /// Constructs a media resource storage instance.
        /// </summary>
        public DSPMediaResourceStorage()
        {
        }

        /// <summary>
        /// Constructs a media resource storage instance.
        /// </summary>
        /// <param name="keyComparer">key comparer</param>
        public DSPMediaResourceStorage(IEqualityComparer<object> keyComparer)
        {
            this.keyComparer = keyComparer;
        }

        /// <summary>
        /// Content of the cache
        /// </summary>
        public virtual IEnumerable<KeyValuePair<object, IEnumerable<KeyValuePair<string, DSPMediaResource>>>> Content
        {
            get
            {
                if (this.inMemoryStreamStorage != null)
                {
                    foreach (KeyValuePair<object, Dictionary<string, DSPMediaResource>> mle in this.inMemoryStreamStorage)
                    {
                        yield return new KeyValuePair<object, IEnumerable<KeyValuePair<string, DSPMediaResource>>>(mle.Key, (IEnumerable<KeyValuePair<string, DSPMediaResource>>)mle.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new media resource and adds it to the storage
        /// </summary>
        /// <param name="entity">entity instance.</param>
        /// <param name="streamProperty">stream info.</param>
        /// <returns>Returns the newly created media resource instance.</returns>
        public virtual DSPMediaResource CreateMediaResource(object entity, ResourceProperty streamProperty)
        {
            if (this.inMemoryStreamStorage == null)
            {
                this.inMemoryStreamStorage = this.keyComparer == null ? new Dictionary<object, Dictionary<string, DSPMediaResource>>() : new Dictionary<object, Dictionary<string, DSPMediaResource>>(this.keyComparer);
            }

            Dictionary<string, DSPMediaResource> streams;
            if (!this.inMemoryStreamStorage.TryGetValue(entity, out streams))
            {
                streams = new Dictionary<string, DSPMediaResource>();
                this.inMemoryStreamStorage[entity] = streams;
            }

            string streamName = streamProperty == null ? string.Empty : streamProperty.Name;
            streams[streamName] = new DSPMediaResource();
            return streams[streamName];
        }

        /// <summary>
        /// Gets the specified media resource.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        /// <param name="streamProperty">stream info.</param>
        /// <param name="mediaResource">media resource info.</param>
        /// <returns>true if the specified media resource is found.</returns>
        public virtual bool TryGetMediaResource(object entity, ResourceProperty streamProperty, out DSPMediaResource mediaResource)
        {
            Dictionary<string, DSPMediaResource> streams;
            if (this.inMemoryStreamStorage == null || !this.inMemoryStreamStorage.TryGetValue(entity, out streams))
            {
                mediaResource = null;
                return false;
            }

            if (!streams.TryGetValue(streamProperty == null ? string.Empty : streamProperty.Name, out mediaResource))
            {
                mediaResource = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes all streams associated with <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">entity whose streams are to be deleted.</param>
        public virtual void DeleteMediaResources(object entity)
        {
            if (this.inMemoryStreamStorage != null && this.inMemoryStreamStorage.ContainsKey(entity))
            {
                this.inMemoryStreamStorage.Remove(entity);
            }
        }
    }
}
