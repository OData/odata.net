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

namespace System.Spatial
{
#if !PORTABLELIB
    using System.Runtime.Serialization;
#endif

    /// <summary>The exception that is thrown on an unsuccessful parsing of the serialized format.</summary>
#if !SILVERLIGHT && !PORTABLELIB
    [Serializable]
#endif
    public class ParseErrorException : Exception
    {
        /// <summary>Creates a new instance of the <see cref="T:System.Spatial.ParseErrorException" /> class.</summary>
        public ParseErrorException()
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Spatial.ParseErrorException" /> class from a message and previous exception.</summary>
        /// <param name="message">The message about the exception.</param>
        /// <param name="innerException">The exception that preceeded this one.</param>
        public ParseErrorException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Spatial.ParseErrorException" /> class from a message.</summary>
        /// <param name="message">The message about the exception.</param>
        public ParseErrorException(string message) 
            : base(message)
        {
        }

#if !SILVERLIGHT && !PORTABLELIB
        /// <summary>Creates a new instance of the <see cref="T:System.Spatial.ParseErrorException" /> class from a serialized data.</summary>
        /// <param name="info">The instance that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The instance that contains contextual information about the source or destination.</param>
        protected ParseErrorException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
#endif
    }
}
