//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if !WINDOWS_PHONE && !SILVERLIGHT  && !PORTABLELIB
    using System.Runtime.Serialization;
#endif
    #endregion Namespaces

    /// <summary>
    /// Exception type representing exceptions in the OData library.
    /// </summary>
#if !WINDOWS_PHONE && !SILVERLIGHT  && !PORTABLELIB
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public class ODataException : InvalidOperationException
    {
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with default values.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public ODataException()
            : this(Strings.ODataException_GeneralError)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public ODataException(string message)
            : this(message, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public ODataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !WINDOWS_PHONE && !SILVERLIGHT && !PORTABLELIB
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class from the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> instances.</summary>
        /// <param name="info"> A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing the information required to serialize  the new <see cref="T:Microsoft.Data.OData.ODataException" />. </param>
        /// <param name="context"> A <see cref="T:System.Runtime.Serialization.StreamingContext" /> containing the source of the serialized stream  associated with the new <see cref="T:Microsoft.Data.OData.ODataException" />. </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
