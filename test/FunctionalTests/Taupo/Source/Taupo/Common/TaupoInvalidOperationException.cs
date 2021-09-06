//---------------------------------------------------------------------
// <copyright file="TaupoInvalidOperationException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when a method call is invalid for the current state of Taupo object.
    /// </summary>
    [Serializable]
    public class TaupoInvalidOperationException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TaupoInvalidOperationException class without a message.
        /// </summary>
        public TaupoInvalidOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInvalidOperationException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TaupoInvalidOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInvalidOperationException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TaupoInvalidOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInvalidOperationException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoInvalidOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}