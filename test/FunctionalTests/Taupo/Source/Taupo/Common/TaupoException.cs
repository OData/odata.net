//---------------------------------------------------------------------
// <copyright file="TaupoException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for all Taupo exceptions also used for generic error messages from within the test framework.
    /// </summary>
    [Serializable]
    public abstract class TaupoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TaupoException class without a message.
        /// </summary>
        protected TaupoException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        protected TaupoException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        protected TaupoException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}