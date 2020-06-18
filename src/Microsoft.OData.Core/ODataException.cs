//---------------------------------------------------------------------
// <copyright file="ODataException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if ORCAS
using System.Runtime.Serialization;
#endif

namespace Microsoft.OData
{
    /// <summary>
    /// Exception type representing exceptions in the OData library.
    /// </summary>
#if ORCAS
    [Serializable]
#endif
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

#if ORCAS
        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataException" /> class from the specified <see cref="System.Runtime.Serialization.SerializationInfo" /> and <see cref="System.Runtime.Serialization.StreamingContext" /> instances.</summary>
        /// <param name="info"> A <see cref="System.Runtime.Serialization.SerializationInfo" /> containing the information required to serialize  the new <see cref="Microsoft.OData.ODataException" />. </param>
        /// <param name="context"> A <see cref="System.Runtime.Serialization.StreamingContext" /> containing the source of the serialized stream  associated with the new <see cref="Microsoft.OData.ODataException" />. </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
