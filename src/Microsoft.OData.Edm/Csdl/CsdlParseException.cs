//---------------------------------------------------------------------
// <copyright file="CsdlParseException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Exception type representing a failure to parse a CSDL document.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public class CsdlParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlParseException"/> class.
        /// </summary>
        public CsdlParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlParseException"/> class.
        /// </summary>
        public CsdlParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlParseException"/> class.
        /// </summary>
        public CsdlParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}