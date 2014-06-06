//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Core;
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
