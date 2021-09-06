//---------------------------------------------------------------------
// <copyright file="AssertionFailedException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown whenever there's an assertion failure in the test code.
    /// </summary>
    [Serializable]
    public class AssertionFailedException : TestFailedException
    {
        /// <summary>
        /// Initializes a new instance of the AssertionFailedException class without a message.
        /// </summary>
        public AssertionFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssertionFailedException class with a given message.
        /// </summary>
        /// <param name="message">Exception message</param>
        public AssertionFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssertionFailedException class with a given message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception.</param>
        public AssertionFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssertionFailedException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected AssertionFailedException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }
    }
}
