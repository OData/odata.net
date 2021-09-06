//---------------------------------------------------------------------
// <copyright file="TestRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// A test request message that stores the version and the underlying stream.
    /// </summary>
    public class TestRequestMessage : TestMessage, IODataRequestMessageAsync
    {
        /// <summary>
        /// The URL of the request.
        /// </summary>
        private Uri url;

        /// <summary>
        /// The HTTP method of the request.
        /// </summary>
        private string method;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream with the content of the message.</param>
        /// <param name="flags">The flags modifying the behavior of the message.</param>
        public TestRequestMessage(Stream stream, TestMessageFlags flags = TestMessageFlags.None)
            : base(stream, flags)
        {
            this.method = ODataConstants.MethodGet;
        }

        /// <summary>
        /// The URL of the request.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }

            set
            {
                if (this.StreamRetrieved)
                {
                    throw new InvalidOperationException("Cannot set URL once the stream has been retrieved using GetStream or GetStreamAsync.");
                }

                this.url = value;
            }
        }

        /// <summary>
        /// The HTTP method of the request.
        /// </summary>
        public string Method
        {
            get
            {
                return this.method;
            }

            set
            {
                if (this.StreamRetrieved)
                {
                    throw new InvalidOperationException("Cannot set HTTP method once the stream has been retrieved using GetStream or GetStreamAsync.");
                }

                this.method = value;
            }
        }
    }
}
