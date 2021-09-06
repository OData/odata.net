//---------------------------------------------------------------------
// <copyright file="ResponseVerificationException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The exception type thrown whenever response verification fails
    /// </summary>
    [Serializable]
    public class ResponseVerificationException : TestFailedException
    {
        internal const string ErrorMessage = "Response verification failed";

        /// <summary>
        /// Initializes a new instance of the ResponseVerificationException class
        /// </summary>
        public ResponseVerificationException()
            : base(ErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResponseVerificationException class
        /// </summary>
        /// <param name="inner">The inner exception</param>
        public ResponseVerificationException(Exception inner)
            : base(ErrorMessage, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResponseVerificationException class
        /// </summary>
        /// <param name="message">The message</param>
        public ResponseVerificationException(string message)
            : base(ErrorMessage + ": " + message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResponseVerificationException class
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="inner">The inner exception</param>
        public ResponseVerificationException(string message, Exception inner)
            : base(ErrorMessage + ": " + message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResponseVerificationException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected ResponseVerificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
