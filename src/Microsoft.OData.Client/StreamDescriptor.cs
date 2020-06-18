//---------------------------------------------------------------------
// <copyright file="StreamDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Holds information about stream.
    /// </summary>
    public sealed class StreamDescriptor : Descriptor
    {
        #region Private Fields

        /// <summary>
        /// The Data service stream link object
        /// </summary>
        private DataServiceStreamLink streamLink;

        /// <summary>entity descriptor referring the parent entity.</summary>
        private EntityDescriptor entityDescriptor;

        /// <summary>
        /// transient named stream info, which contains metadata from the response which has not been materialized yet.
        /// </summary>
        private StreamDescriptor transientNamedStreamInfo;

        #endregion

        /// <summary>
        /// Creates a StreamDescriptor class with the given name and other information
        /// </summary>
        /// <param name="name">name of the stream.</param>
        /// <param name="entityDescriptor">instance of entity descriptor that contains this stream.</param>
        internal StreamDescriptor(string name, EntityDescriptor entityDescriptor) : base(EntityStates.Unchanged)
        {
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(entityDescriptor != null, "entityDescriptor != null");
            this.streamLink = new DataServiceStreamLink(name);
            this.entityDescriptor = entityDescriptor;
        }

        /// <summary>
        /// Creates a StreamDescriptor class for the default stream (MR) associated with an entity.
        /// </summary>
        /// <param name="entityDescriptor">instance of entity descriptor that contains this stream.</param>
        internal StreamDescriptor(EntityDescriptor entityDescriptor)
            : base(EntityStates.Unchanged)
        {
            Debug.Assert(entityDescriptor != null, "entityDescriptor != null");
            this.streamLink = new DataServiceStreamLink(null);
            this.entityDescriptor = entityDescriptor;
        }

        /// <summary>The <see cref="Microsoft.OData.Client.DataServiceStreamLink" /> that represents the binary resource stream.</summary>
        /// <returns>Returns <see cref="Microsoft.OData.Client.DataServiceStreamLink" />.</returns>
        public DataServiceStreamLink StreamLink
        {
            get { return this.streamLink; }
        }

        /// <summary>The <see cref="Microsoft.OData.Client.EntityDescriptor" /> that represents the entity to which the named resource stream belongs.</summary>
        /// <returns>The <see cref="Microsoft.OData.Client.EntityDescriptor" /> of the entity.</returns>
        public EntityDescriptor EntityDescriptor
        {
            get
            {
                return this.entityDescriptor;
            }

            set
            {
                this.entityDescriptor = value;
            }
        }

        #region Internal Properties

        /// <summary>
        /// Returns the name of the stream.
        /// </summary>
        internal string Name
        {
            get
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                return this.streamLink.Name;
            }
        }

        /// <summary>
        /// Returns the URI to get the named stream.
        /// </summary>
        internal Uri SelfLink
        {
            get
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                return this.streamLink.SelfLink;
            }

            set
            {
                Debug.Assert(value.IsAbsoluteUri, "self link must be an absolute uri");
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                this.streamLink.SelfLink = value;
            }
        }

        /// <summary>
        /// Returns the URI to update the named stream.
        /// </summary>
        internal Uri EditLink
        {
            get
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                return this.streamLink.EditLink;
            }

            set
            {
                Debug.Assert(value.IsAbsoluteUri, "edit link must be an absolute uri");
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                this.streamLink.EditLink = value;
            }
        }

        /// <summary>
        /// Returns the content type of the named stream.
        /// </summary>
        internal string ContentType
        {
            get
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                return this.streamLink.ContentType;
            }

            set
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                this.streamLink.ContentType = value;
            }
        }

        /// <summary>
        /// Returns the etag for the named stream.
        /// </summary>
        internal string ETag
        {
            get
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                return this.streamLink.ETag;
            }

            set
            {
                Debug.Assert(this.streamLink != null, "Stream Link is not null");
                this.streamLink.ETag = value;
            }
        }

        /// <summary>
        /// Returns the stream associated with this name.
        /// </summary>
        internal DataServiceSaveStream SaveStream
        {
            get;
            set;
        }

        /// <summary>return true, since this class represents entity descriptor.</summary>
        internal override DescriptorKind DescriptorKind
        {
            get { return DescriptorKind.NamedStream; }
        }

        /// <summary>
        /// Transient named stream info, if there are responses which hasn't been fully processed yet.
        /// </summary>
        internal StreamDescriptor TransientNamedStreamInfo
        {
            set
            {
                Debug.Assert(value != null, "you can never set transient named stream to null");

                if (this.transientNamedStreamInfo == null)
                {
                    this.transientNamedStreamInfo = value;
                }
                else
                {
                    StreamDescriptor.MergeStreamDescriptor(this.transientNamedStreamInfo, value);
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Merge the information from the new stream info into the existing one.
        /// </summary>
        /// <param name="existingStreamDescriptor">stream info into which the data needs to be merged.</param>
        /// <param name="newStreamDescriptor">stream info which contains the latest data.</param>
        internal static void MergeStreamDescriptor(StreamDescriptor existingStreamDescriptor, StreamDescriptor newStreamDescriptor)
        {
            // overwrite existing information with new ones (coming from the payload).
            if (newStreamDescriptor.SelfLink != null)
            {
                existingStreamDescriptor.SelfLink = newStreamDescriptor.SelfLink;
            }

            if (newStreamDescriptor.EditLink != null)
            {
                existingStreamDescriptor.EditLink = newStreamDescriptor.EditLink;
            }

            if (newStreamDescriptor.ContentType != null)
            {
                existingStreamDescriptor.ContentType = newStreamDescriptor.ContentType;
            }

            if (newStreamDescriptor.ETag != null)
            {
                existingStreamDescriptor.ETag = newStreamDescriptor.ETag;
            }
        }

        /// <summary>
        /// clears all the changes - like closes the save stream, clears the transient entity descriptor.
        /// This method is called when the client is done with sending all the pending requests.
        /// </summary>
        internal override void ClearChanges()
        {
            this.transientNamedStreamInfo = null;
            this.CloseSaveStream();
        }

        /// <summary>return the most recent edit link for the named stream</summary>
        /// <returns>the uri to edit the named stream.</returns>
        internal Uri GetLatestEditLink()
        {
            if (this.transientNamedStreamInfo != null && this.transientNamedStreamInfo.EditLink != null)
            {
                return this.transientNamedStreamInfo.EditLink;
            }

            return this.EditLink;
        }

        /// <summary>return the most recent etag for the named stream</summary>
        /// <returns>the etag for the named stream.</returns>
        internal string GetLatestETag()
        {
            if (this.transientNamedStreamInfo != null && this.transientNamedStreamInfo.ETag != null)
            {
                return this.transientNamedStreamInfo.ETag;
            }

            return this.ETag;
        }

        /// <summary>
        /// Closes the save stream if there's any and sets it to null
        /// </summary>
        internal void CloseSaveStream()
        {
            if (this.SaveStream != null)
            {
                DataServiceSaveStream stream = this.SaveStream;
                this.SaveStream = null;
                stream.Close();
            }
        }

        #endregion
    }
}
