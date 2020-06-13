//---------------------------------------------------------------------
// <copyright file="DataServiceStreamLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>Represents the URL of a binary resource stream.</summary>
    public sealed class DataServiceStreamLink : INotifyPropertyChanged
    {
        /// <summary>name of the stream whose link needs to be populated.</summary>
        private readonly string name;

        /// <summary>self link for the stream.</summary>
        /// <remarks>This should always be an absolute uri, if specified. If the payload contains an relative uri,
        /// we always use the context base uri to convert this into an absolute uri.</remarks>
        private Uri selfLink;

        /// <summary>edit link for the stream.</summary>
        /// <remarks>This should always be an absolute uri, if specified. If the payload contains an relative uri,
        /// we always use the context base uri to convert this into an absolute uri.</remarks>
        private Uri editLink;

        /// <summary>content type of the stream.</summary>
        private string contentType;

        /// <summary>etag for the stream.</summary>
        private string etag;

        /// <summary>
        /// Internal constructor to be used by the projection plan compiler, so that we capture the ri
        /// </summary>
        /// <param name="name">name of the stream.</param>
        internal DataServiceStreamLink(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Public Properties

        /// <summary>The name of the binary resource stream.</summary>
        /// <returns>The name of the binary resource stream.</returns>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>The URI that returns the binary resource stream.</summary>
        /// <returns>The URI of the stream.</returns>
        public Uri SelfLink
        {
            get
            {
                return this.selfLink;
            }

            internal set
            {
                Debug.Assert(value == null || value.IsAbsoluteUri, "self link must be an absolute uri");
                this.selfLink = value;
                this.OnPropertyChanged(nameof(SelfLink));
            }
        }

        /// <summary>Gets the URI used to edit the binary resource stream.</summary>
        /// <returns>The URI used to edit the stream.</returns>
        public Uri EditLink
        {
            get
            {
                return this.editLink;
            }

            internal set
            {
                Debug.Assert(value.IsAbsoluteUri, "edit link must be an absolute uri");
                this.editLink = value;
                this.OnPropertyChanged(nameof(EditLink));
            }
        }

        /// <summary>Gets the MIME Content-Type of the binary resource stream. </summary>
        /// <returns>The Content-Type value for the stream.</returns>
        public string ContentType
        {
            get
            {
                return this.contentType;
            }

            internal set
            {
                this.contentType = value;
                this.OnPropertyChanged(nameof(ContentType));
            }
        }

        /// <summary>The eTag value that is used to determine concurrency for a binary resource stream.</summary>
        /// <returns>The value of the eTag header for the stream.</returns>
        public string ETag
        {
            get
            {
                return this.etag;
            }

            internal set
            {
                this.etag = value;
                this.OnPropertyChanged(nameof(ETag));
            }
        }

        #endregion

        /// <summary>
        /// One of the properties changed its value
        /// </summary>
        /// <param name="propertyName">property name</param>
        private void OnPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
