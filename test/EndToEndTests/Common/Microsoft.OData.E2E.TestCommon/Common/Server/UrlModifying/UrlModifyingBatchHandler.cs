//---------------------------------------------------------------------
// <copyright file="UrlModifyingBatchHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UrlModifying;

public class UrlModifyingBatchHandler : DefaultODataBatchHandler
{
    public override async Task ProcessBatchAsync(HttpContext context, RequestDelegate nextHandler)
    {
        // Enable buffering to allow multiple reads of the request body
        context.Request.EnableBuffering();

        // Read the content type and boundary from the request
        var contentType = new ContentType(context.Request.ContentType);
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        // Read the original request body stream
        var originalBody = context.Request.Body;
        var modifiedBodyStream = new MemoryStream();

        try
        {
            // Reset the position of the original body stream to 0
            originalBody.Position = 0;

            var multipartReader = new MultipartReader(boundary, originalBody);
            var section = await multipartReader.ReadNextSectionAsync();

            while (section != null)
            {
                // Read the section content
                using (var sectionStream = new MemoryStream())
                {
                    await section.Body.CopyToAsync(sectionStream);
                    sectionStream.Position = 0;
                    var content = await new StreamReader(sectionStream).ReadToEndAsync();

                    // Modify the URLs in the content
                    if (content.Contains("BatchRequest1"))
                    {
                        content = content.Replace("BatchRequest1", "Customers");
                    }
                    else if (content.Contains("BatchRequest2"))
                    {
                        content = content.Replace("BatchRequest2", "People");
                    }

                    // Write the modified section to the new body stream
                    var headerBuilder = new StringBuilder();
                    foreach (var header in section.Headers)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {header.Value}");
                    }

                    await modifiedBodyStream.WriteAsync(Encoding.UTF8.GetBytes(
                        $"--{boundary}\r\n" +
                        headerBuilder.ToString() +
                        $"\r\n{content}\r\n"));

                    section = await multipartReader.ReadNextSectionAsync();
                }
            }

            // Write the closing boundary to the modified body stream
            var closingBoundaryBytes = Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
            await modifiedBodyStream.WriteAsync(closingBoundaryBytes, 0, closingBoundaryBytes.Length);

            // Reset the position of the modified body stream to 0 for downstream readers
            modifiedBodyStream.Position = 0;

            context.Request.Body = modifiedBodyStream;
            context.Request.ContentLength = modifiedBodyStream.Length;
        }
        finally
        {
            // Restore the original request body stream to avoid side effects
            originalBody.Position = 0;
        }

        await base.ProcessBatchAsync(context, nextHandler);
    }
}
