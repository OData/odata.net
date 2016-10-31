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

namespace System.Data.Services.Serializers
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// Provides support for serializing responses in text format.
    /// </summary>
    internal struct TextSerializer
    {
        /// <summary>Writer to which output is sent.</summary>
        private readonly ODataMessageWriter writer;

        /// <summary>Initializes a new <see cref="TextSerializer"/> for writing the response.</summary>
        /// <param name="messageWriter">ODataMessageWriter instance which needs to be used to write the response.</param>
        internal TextSerializer(ODataMessageWriter messageWriter)
        {
            this.writer = messageWriter;
        }

        /// <summary>Handles the complete serialization for the specified content.</summary>
        /// <param name="content">Single Content to write..</param>
        /// <remarks><paramref name="content"/> should be a byte array.</remarks>
        internal void WriteRequest(object content)
        {
            Debug.Assert(content != null, "content != null");
            Debug.Assert(this.writer != null, "this.writer != null");

            object primitiveValue = Serializer.GetPrimitiveValue(content);
            this.writer.WriteValue(primitiveValue);
        }
    }
}
