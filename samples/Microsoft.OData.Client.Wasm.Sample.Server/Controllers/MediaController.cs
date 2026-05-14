//---------------------------------------------------------------------
// <copyright file="MediaController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.Wasm.Sample.Server.Models;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Controllers;

public class MediaController : ODataController
{
    private static readonly List<MediaAsset> mediaAssets = new List<MediaAsset>
    {
        new MediaAsset { Id = 1 }
    };

    [EnableQuery]
    public ActionResult<IEnumerable<MediaAsset>> GetMedia()
    {
        return mediaAssets;
    }

    [EnableQuery]
    public ActionResult<MediaAsset> Get([FromODataUri] int key)
    {
        var mediaAsset = mediaAssets.FirstOrDefault(d => d.Id == key);
        if (mediaAsset == null)
        {
            return NotFound();
        }

        return mediaAsset;
    }

    [HttpGet("odata/Media({key})/$value")]
    public ActionResult GetMediaStream(int key)
    {
        var mediaAsset = mediaAssets.FirstOrDefault(d => d.Id == key);
        if (mediaAsset == null)
        {
            return NotFound();
        }

        byte[] mediaContent = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Return the byte array as a FileContentResult
        // application/octet-stream is the default content type for binary data
        return File(mediaContent, "application/octet-stream", $"media_{key}.bin");
    }
}
