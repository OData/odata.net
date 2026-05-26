//---------------------------------------------------------------------
// <copyright file="MediaAsset.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Models;

[MediaType]
public class MediaAsset
{
    public int Id { get; set; }
}
