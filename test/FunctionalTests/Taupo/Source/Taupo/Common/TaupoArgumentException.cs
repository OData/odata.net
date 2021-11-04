//---------------------------------------------------------------------
// <copyright file="TaupoArgumentException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown whenever argument to a method or a property on a Taupo component is set to an invalid value.
    /// </summary>
    [Serializable]
    public class TaupoArgumentException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TaupoArgumentException class without a message.
        /// </summary>
        public TaupoArgumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TaupoArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TaupoArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}