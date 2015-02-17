//---------------------------------------------------------------------
// <copyright file="ODataBatchResponseRequestAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// This annotation is used to represent the request associated with a batch response. This is used in ODataLib Writer tests to do test deserialization.
    /// </summary>
    public sealed class ODataBatchResponseRequestAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the Batch Request
        /// </summary>
        public BatchRequestPayload BatchRequest { get; set; }

        /// <summary>
        /// Returns the string representation of the ODataBatchResponseRequestAnnotation
        /// </summary>
        public override string StringRepresentation
        {
            get { return "BatchRequest: " + BatchRequest.ToString(); }
        }

        /// <summary>
        /// Clones the ODataBatchResponseRequestAnnotation
        /// </summary>
        /// <returns></returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new ODataBatchResponseRequestAnnotation() { BatchRequest = this.BatchRequest.DeepCopy() };
        }
    }
}
