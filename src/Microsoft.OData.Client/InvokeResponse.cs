//---------------------------------------------------------------------
// <copyright file="InvokeResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Client;

    /// <summary> Response from an Invoke call. </summary>
    public class InvokeResponse : OperationResponse
    {
        /// <summary> Consutrcts an InvokeResponse identical to an OperationResponse. </summary>
        /// <param name="headers">The HTTP headers.</param>
        public InvokeResponse(Dictionary<string, string> headers)
            : base(new HeaderCollection(headers))
        {
        }
    }
}
