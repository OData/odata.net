//---------------------------------------------------------------------
// <copyright file="UriTypeParsingException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks; 

    #endregion

#if ORCAS
    [Serializable]
#endif
    /// <summary>
    /// Throw this expcetion when the parser can parse the target type but failed to do so.
    /// Do not throw when parser is not able to parse the target type.
    /// </summary>
    [DebuggerDisplay("{Message};{ParsingFailureReason}")]
    public class UriTypeParsingException : ODataException
    {
        /// <summary>
        /// Creates an Excpetion when parsing is failed
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="parsingFailureReason">The reason of parsing failure</param>
        public UriTypeParsingException(string message, string parsingFailureReason = "")
            : this(message, null, parsingFailureReason)
        {
        }

        /// <summary>
        /// Creates an Excpetion when parsing is failed
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner Exception</param>
        /// <param name="parsingFailureReason">The reason of parsing failure</param>
        public UriTypeParsingException(string message, Exception innerException, string parsingFailureReason = "")
            : base(message, innerException)
        {
            this.ParsingFailureReason = parsingFailureReason;
        }

        /// <summary>
        /// The reason describing why the parsing process has failed.
        /// </summary>
        public string ParsingFailureReason { get; private set; }
    }
}
