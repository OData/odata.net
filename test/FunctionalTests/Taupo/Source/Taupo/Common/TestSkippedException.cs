//---------------------------------------------------------------------
// <copyright file="TestSkippedException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Thrown whenever test variation wants to skip execution without signaling warning or a failure.
    /// </summary>
    [Serializable]
    public class TestSkippedException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TestSkippedException class without a message.
        /// </summary>
        public TestSkippedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestSkippedException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TestSkippedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestSkippedException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TestSkippedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestSkippedException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TestSkippedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}