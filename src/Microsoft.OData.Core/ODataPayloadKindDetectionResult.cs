//---------------------------------------------------------------------
// <copyright file="ODataPayloadKindDetectionResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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
