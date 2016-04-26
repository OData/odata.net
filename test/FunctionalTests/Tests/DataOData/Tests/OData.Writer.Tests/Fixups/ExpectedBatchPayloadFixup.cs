//---------------------------------------------------------------------
// <copyright file="ExpectedBatchPayloadFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    /// <summary>
    /// Performs fixups on the payloads in the batch
    /// </summary>
    public class ExpectedBatchPayloadFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the root element of the operation.
        /// </summary>
        /// <param name="operation">The operation to visit</param>
        protected override void VisitResponseOperation(HttpResponseData operation)
        {
            var responseOperation = operation as ODataResponse;
            ExceptionUtilities.CheckObjectNotNull(responseOperation, "Operation must be a response");
            ODataPayloadElement rootElement = null;
            string contentType = null;
            if (responseOperation.RootElement != null)
            {
                rootElement = responseOperation.RootElement;
                contentType = responseOperation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
            }

            if (rootElement != null)
            {
                ODataFormat format = null;
                if (contentType.Contains(MimeTypes.ApplicationJson))
                {
                    format = ODataFormat.Json;
                }

                new ExpectedPayloadFixups().Fixup(rootElement, format);
            }
        }

        protected override void VisitRequestOperation(IHttpRequest operation)
        {
            var requestOperation = operation as ODataRequest;
            ExceptionUtilities.CheckObjectNotNull(requestOperation, "Operation must be request");
            ODataPayloadElement rootElement = null;
            string contentType = null;
            if (requestOperation.Body != null && requestOperation.Body.RootElement != null)
            {
                rootElement = requestOperation.Body.RootElement;
                contentType = requestOperation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
            }

            if (rootElement != null)
            {
                ODataFormat format = null;
                if (contentType.Contains(MimeTypes.ApplicationJson))
                {
                    format = ODataFormat.Json;
                }

                new ExpectedPayloadFixups().Fixup(rootElement, format);
            }
        }
    }
}
