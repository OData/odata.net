//---------------------------------------------------------------------
// <copyright file="PayloadKindDetectionResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Class representing a payload kind detection test result.
    /// </summary>
    public sealed class PayloadKindDetectionResult
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="payloadKind">The detected payload kind.</param>
        /// <param name="format">The format for the detected payload kind.</param>
        internal PayloadKindDetectionResult(ODataPayloadKind payloadKind, ODataFormat format)
        {
            this.PayloadKind = payloadKind;
            this.Format = format;
        }

        /// <summary>
        /// The detected payload kind.
        /// </summary>
        public ODataPayloadKind PayloadKind { get; set; }

        /// <summary>
        /// The format for the detected payload kind.
        /// </summary>
        public ODataFormat Format { get; set; }

        /// <summary>
        /// The model to use when reading payloads.
        /// </summary>
        public IEdmModel Model { get; set; }

        /// <summary>
        /// The type to use when reading payloads to specify as the expected type for the reader.
        /// </summary>
        public IEdmTypeReference ExpectedType { get; set; }

        /// <summary>
        /// If set the test is expected to fail with the specified exception.
        /// If left null, the test is not expected to throw an exception.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }
    }
}
