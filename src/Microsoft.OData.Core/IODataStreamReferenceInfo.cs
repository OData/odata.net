//---------------------------------------------------------------------
// <copyright file="IODataStreamReferenceInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    internal interface IODataStreamReferenceInfo
    {
        Uri EditLink { get; set; }

        Uri ReadLink { get; set; }

        string ContentType { get; set; }

        string ETag { get; set; }
    }
}