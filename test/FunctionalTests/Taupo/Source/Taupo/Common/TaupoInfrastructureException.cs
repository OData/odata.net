//---------------------------------------------------------------------
// <copyright file="TaupoInfrastructureException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when an infrastructure failure is detected.
    /// </summary>
    [Serializable]
    public class TaupoInfrastructureException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TaupoInfrastructureException class without a message.
        /// </summary>
        public TaupoInfrastructureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInfrastructureException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TaupoInfrastructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInfrastructureException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TaupoInfrastructureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoInfrastructureException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoInfrastructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}