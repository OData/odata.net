//---------------------------------------------------------------------
// <copyright file="InvokeResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;

    /// <summary> Response from an Invoke call. </summary>
    public class InvokeResponse : OperationResponse
    {
        /// <summary> Constructs an InvokeResponse identical to an OperationResponse. </summary>
        /// <param name="headers">The HTTP headers.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "<Pending>")]
        public InvokeResponse(Dictionary<string, string> headers)
            : base(new HeaderCollection(headers))
        {
        }
    }
}
