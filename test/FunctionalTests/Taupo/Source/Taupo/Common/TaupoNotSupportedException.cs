//---------------------------------------------------------------------
// <copyright file="TaupoNotSupportedException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown for cases not supported by Taupo.
    /// </summary>
    [Serializable]
    public class TaupoNotSupportedException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TaupoNotSupportedException class without a message.
        /// </summary>
        public TaupoNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoNotSupportedException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TaupoNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoNotSupportedException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TaupoNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoNotSupportedException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}