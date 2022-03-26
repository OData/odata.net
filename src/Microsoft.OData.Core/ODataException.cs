//---------------------------------------------------------------------
// <copyright file="ODataException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.OData
{
    /// <summary>
    /// Exception type representing exceptions in the OData library.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public class ODataException : InvalidOperationException
    {
        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataException" /> class with default values.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public ODataException()
            : this(Strings.ODataException_GeneralError)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public ODataException(string message)
            : this(message, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public ODataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
