//---------------------------------------------------------------------
// <copyright file="ODataMessageKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    /// <summary>
    /// Enum representing whether the payload being read/writen is an OData request or response.
    /// </summary>
    public enum ODataMessageKind
    {
        Request,
        Response
    }
}
