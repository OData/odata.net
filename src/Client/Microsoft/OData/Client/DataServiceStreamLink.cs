//   OData .NET Libraries ver. 6.8.1
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
                this.OnPropertyChanged("SelfLink");
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
                this.OnPropertyChanged("EditLink");
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
                this.OnPropertyChanged("ContentType");
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
                this.OnPropertyChanged("ETag");
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
