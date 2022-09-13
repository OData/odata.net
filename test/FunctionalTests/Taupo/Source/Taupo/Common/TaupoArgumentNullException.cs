//---------------------------------------------------------------------
// <copyright file="TaupoArgumentNullException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown whenever argument to a method or a property on a Taupo component is invalidly set to null.
    /// </summary>
    [Serializable]
    public class TaupoArgumentNullException : TaupoException
    {
        /// <summary>
        /// Initializes a new instance of the TaupoArgumentNullException class without a message.
        /// </summary>
        public TaupoArgumentNullException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentNullException class with a given parameter name.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        public TaupoArgumentNullException(string argumentName)
            : base(BuildExceptionMessage(argumentName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentNullException class with a given parameter name and inner exception.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="inner">Inner exception.</param>
        public TaupoArgumentNullException(string argumentName, Exception inner)
            : base(BuildExceptionMessage(argumentName), inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaupoArgumentNullException class based on 
        /// <see cref="SerializationInfo"/>
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected TaupoArgumentNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string BuildExceptionMessage(string argumentName)
        {
            return string.Format(CultureInfo.InvariantCulture, "Value cannot be null. Argument name: {0}", argumentName);
        }
    }
}
