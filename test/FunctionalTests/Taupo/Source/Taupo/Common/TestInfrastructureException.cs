//---------------------------------------------------------------------
// <copyright file="TestInfrastructureException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Exception thrown when a test infrastructure failure is detected.
    /// </summary>
    [Serializable]
    public class TestInfrastructureException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TestInfrastructureException class without a message.
        /// </summary>
        public TestInfrastructureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestInfrastructureException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public TestInfrastructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestInfrastructureException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public TestInfrastructureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestInfrastructureException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TestInfrastructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}