//---------------------------------------------------------------------
// <copyright file="TestResponseMessage.cs" company="Microsoft">
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
    /// A test response message that stores the version and the underlying stream.
    /// </summary>
    public class TestResponseMessage : TestMessage, IODataResponseMessageAsync
    {
        /// <summary>
        /// The status code of the response.
        /// </summary>
        private int statusCode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to wrap in the message.</param>
        /// <param name="flags">Flags modifying the behavior of the test message.</param>
        public TestResponseMessage(Stream stream, TestMessageFlags flags = TestMessageFlags.None)
            : base(stream, flags)
        {
            this.statusCode = 200;
        }

        /// <summary>
        /// The status code of the response.
        /// </summary>
        public virtual int StatusCode
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                if (this.StreamRetrieved)
                {
                    throw new InvalidOperationException("Cannot set status code once the stream has been retrieved using GetStream or GetStreamAsync.");
                }

                this.statusCode = value;
            }
        }
    }
}
