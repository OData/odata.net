//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
