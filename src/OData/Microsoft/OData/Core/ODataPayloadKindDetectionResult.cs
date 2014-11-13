//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>Represents the result of running payload kind detection for a specified payload kind and format.</summary>
        /// <remarks>This class is used to represent the result of running payload kind detection using
        /// <see cref="ODataMessageReader.DetectPayloadKind"/>. See the documentation of that method for more
        /// information.</remarks>
    public sealed class ODataPayloadKindDetectionResult
    {
        /// <summary>The detected payload kind.</summary>
        private readonly ODataPayloadKind payloadKind;

        /// <summary>The format for the detected payload kind.</summary>
        private readonly ODataFormat format;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="payloadKind">The detected payload kind.</param>
        /// <param name="format">The format for the detected payload kind.</param>
        internal ODataPayloadKindDetectionResult(ODataPayloadKind payloadKind, ODataFormat format)
        {
            this.payloadKind = payloadKind;
            this.format = format;
        }

        /// <summary>Gets the detected payload kind.</summary>
        /// <returns>The detected payload kind.</returns>
        public ODataPayloadKind PayloadKind 
        {
            get { return this.payloadKind; }
        }

        /// <summary>Gets the format for the detected payload kind.</summary>
        /// <returns>The format for the detected payload kind.</returns>
        public ODataFormat Format
        {
            get { return this.format; }
        }
    }
}
