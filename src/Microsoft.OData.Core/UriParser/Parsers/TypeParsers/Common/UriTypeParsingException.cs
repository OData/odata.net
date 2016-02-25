//---------------------------------------------------------------------
// <copyright file="UriTypeParsingException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common
{
    #region Namespaces

    using System;
    using System.Diagnostics;

    #endregion

    ////#if ORCAS // Not working in .Net3.5 solution, why ??
    ////    [Serializable]
    ////#endif

    /// <summary>
    /// Throw this expcetion when the parser can parse the target type but failed to do so.
    /// Do not throw when parser is not able to parse the target type.
    /// </summary>
    [DebuggerDisplay("{Message};{ParsingFailureReason}")]
    public sealed class UriTypeParsingException : ODataException
    {
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.UriTypeParsingException" /> class with default values.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public UriTypeParsingException()
            : base()
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.UriTypeParsingException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public UriTypeParsingException(string message)
            : base(message)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.UriTypeParsingException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public UriTypeParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception when parsing is failed.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="parsingFailureReason">The reason of parsing failure</param>
        public UriTypeParsingException(string message, string parsingFailureReason)
            : this(message, null, parsingFailureReason)
        {
        }

        /// <summary>
        /// Creates an exception when parsing is failed.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner Exception</param>
        /// <param name="parsingFailureReason">The reason of parsing failure</param>
        public UriTypeParsingException(string message, Exception innerException, string parsingFailureReason)
            : this(message, innerException)
        {
            this.ParsingFailureReason = parsingFailureReason;
        }

        /// <summary>
        /// The reason describing why the parsing process has failed.
        /// </summary>
        public string ParsingFailureReason { get; private set; }
    }
}
