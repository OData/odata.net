//---------------------------------------------------------------------
// <copyright file="PayloadFormatVersionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which marks the version of the payload to write when serializing the annotated payload element.
    /// </summary>
    public class PayloadFormatVersionAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// The version to use when writing the payload.
        /// </summary>
        public DataServiceProtocolVersion? Version { get; set; }

        /// <summary>
        /// true if the payload is to be written as a response, false for a request.
        /// </summary>
        public bool? Response { get; set; }

        /// <summary>
        /// true if the payload element should be wrapper in the "d" response wrapper.
        /// </summary>
        public bool ResponseWrapper { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                string result = string.Empty;
                if (this.Version.HasValue)
                {
                    result += "Payload format version " + this.Version.Value.ToString() + ". ";
                }

                if (this.Response.HasValue)
                {
                    result += this.Response.Value ? "Response. " : "Request. ";
                }

                if (this.ResponseWrapper)
                {
                    result += "Response \"d\" Wrapper. ";
                }

                return result;
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new PayloadFormatVersionAnnotation { Version = this.Version, Response = this.Response, ResponseWrapper = this.ResponseWrapper };
        }
    }
}
