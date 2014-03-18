//---------------------------------------------------------------------
// <copyright file="DataServiceRequestArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
// This class represents additional metadata to be applied to a request
// sent from the client to a data service.
// </summary>
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>Represents additional metadata that is included in a request message to WCF Data Services.</summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
    public class DataServiceRequestArgs
    {
#if DEBUG && WINDOWS_PHONE
        /// <summary> True,if this instance is being deserialized via DataContractSerialization, false otherwise </summary>
        private bool deserializing;
#endif
        /// <summary>Creates a new instance of the <see cref="T:System.Data.Services.Client.DataServiceRequestArgs" /> class.</summary>
        public DataServiceRequestArgs()
        {
            this.HeaderCollection = new HeaderCollection();
        }

        /// <summary>Gets or sets the Accept header of the request message.</summary>
        /// <returns>The value of the Accept header.</returns>
        /// <remarks>
        /// Sets the mime type (ex. image/png) to be used when retrieving the stream.
        /// Note that no validation is done on the contents of this property.
        /// It is the responsibility of the user to format it correctly to be used
        /// as the value of an HTTP Accept header.
        /// </remarks>
        public string AcceptContentType
        {
            get
            {
                return this.HeaderCollection.GetHeader(XmlConstants.HttpRequestAccept);
            }

            set
            {
                this.HeaderCollection.SetHeader(XmlConstants.HttpRequestAccept, value);
            }
        }

        /// <summary>Gets or sets the Content-Type header of the request message.</summary>
        /// <returns>The value of the Content-Type header.</returns>
        /// <remarks>
        /// Sets the Content-Type header to be used when sending the stream to the server.
        /// Note that no validation is done on the contents of this property.
        /// It is the responsibility of the user to format it correctly to be used
        /// as the value of an HTTP Content-Type header.
        /// </remarks>
        public string ContentType
        {
            get
            {
                return this.HeaderCollection.GetHeader(XmlConstants.HttpContentType);
            }

            set
            {
                this.HeaderCollection.SetHeader(XmlConstants.HttpContentType, value);
            }
        }

        /// <summary>Gets or sets the value of the Slug header of the request message.</summary>
        /// <returns>A value that is the Slug header of the request. </returns>
        /// <remarks>
        /// Sets the Slug header to be used when sending the stream to the server.
        /// Note that no validation is done on the contents of this property.
        /// It is the responsibility of the user to format it correctly to be used
        /// as the value of an HTTP Slug header.
        /// </remarks>
        public string Slug
        {
            get
            {
                return this.HeaderCollection.GetHeader(XmlConstants.HttpSlug);
            }

            set
            {
                this.HeaderCollection.SetHeader(XmlConstants.HttpSlug, value);
            }
        }

        /// <summary>Gets the headers in the request message.</summary>
        /// <returns>The headers in the request message.</returns>
        /// <remarks>
        /// Dictionary containing all the request headers to be used when retrieving the stream.
        /// The user should take care so as to not alter an HTTP header which will change
        /// the meaning of the request.
        /// No validation is performed on the header names or values.
        /// This class will not attempt to fix up any of the headers specified and
        /// will try to use them "as is".
        /// </remarks>
#if WINDOWS_PHONE
        [DataMember]
#endif
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "The setter is called during de-serialization")]
        public Dictionary<string, string> Headers
        {
            get
            {
                // by mistake in V2 we made some public API not expose the interface, but we don't
                // want the rest of the codebase to use this type, so we only cast it when absolutely
                // required by the public API.
                return (Dictionary<string, string>)this.HeaderCollection.UnderlyingDictionary;
            }

            internal set
            {
#if DEBUG && WINDOWS_PHONE
                Debug.Assert(this.deserializing, "This property can only be set during deserialization");
#endif
                this.HeaderCollection = new HeaderCollection(value);
            }
        }

        /// <summary>Request header collection.</summary>
        internal HeaderCollection HeaderCollection
        {
            get; 
            private set;
        }

#if DEBUG && WINDOWS_PHONE
        /// <summary>
        /// Called during deserialization of this instance by DataContractSerialization
        /// </summary>
        /// <param name="context">Streaming context for this deserialization session</param>
        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
        }

        /// <summary>
        /// Called after this instance has been deserialized by DataContractSerialization
        /// </summary>
        /// <param name="context">Streaming context for this deserialization session</param>
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
        }
#endif
    }
}
