//---------------------------------------------------------------------
// <copyright file="ParseErrorException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>The exception that is thrown on an unsuccessful parsing of the serialized format.</summary>
    public class ParseErrorException : Exception
    {
        /// <summary>Creates a new instance of the <see cref="Microsoft.Spatial.ParseErrorException" /> class.</summary>
        public ParseErrorException()
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.Spatial.ParseErrorException" /> class from a message and previous exception.</summary>
        /// <param name="message">The message about the exception.</param>
        /// <param name="innerException">The exception that preceded this one.</param>
        public ParseErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.Spatial.ParseErrorException" /> class from a message.</summary>
        /// <param name="message">The message about the exception.</param>
        public ParseErrorException(string message)
            : base(message)
        {
        }
    }
}
