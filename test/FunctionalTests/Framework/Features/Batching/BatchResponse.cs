//---------------------------------------------------------------------
// <copyright file="BatchResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Collections.Generic;

    public class BatchResponse : AstoriaResponse
    {
        public List<AstoriaResponse> Responses
        {
            get;
            private set;
        }

        public BatchResponse(BatchRequest request, AstoriaResponse response)
            : base(request)
        {
            if (request.ExpectedStatusCode == HttpStatusCode.Accepted)
            {
                try
                {
                    ResponseVerification.VerifyStatusCode(response);
                }
                catch (Exception e)
                {
                    ResponseVerification.LogFailure(response, e);
                }
            }

            Responses = new List<AstoriaResponse>();

            this.Headers = response.Headers;
            this.Payload = response.Payload;
            this.ETagHeaderFound = response.ETagHeaderFound;
            this.ActualStatusCode = response.ActualStatusCode;
            this.Exception = response.Exception;

            if (request.ExpectedStatusCode == HttpStatusCode.Accepted)
                BatchReader.ParseBatchResponse(this);
        }
    }
}
