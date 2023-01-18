//---------------------------------------------------------------------
// <copyright file="CustomerCollectionOutputFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExperimentsLib;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace TestServer;

/// <summary>
/// An output formatter that writes <see cref="IEnumerable{Customer}"/>
/// payloads using the <see cref="IPayloadWriter{T}"/> specified
/// in the request route.
/// </summary>
public class CustomerCollectionOutputFormatter : TextOutputFormatter
{
    public CustomerCollectionOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type)
        => typeof(IEnumerable<Customer>).IsAssignableFrom(type);

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;

        var writers = services.GetRequiredService<WriterCollection<IEnumerable<Customer>>>();

        // the writer is specified in the route: customers/{writer}
        var writerName = httpContext.Request.RouteValues["writer"] as string;
        var writer = writers.GetWriter(writerName);

        var customers = (IEnumerable<Customer>)context.Object!;

        var stream = httpContext.Response.Body;

        bool includeRawValues = false;
        if (httpContext.Request.Query.TryGetValue("includeRawValues", out var rawValueQuery) && rawValueQuery.Count >= 1)
        {
            includeRawValues = rawValueQuery[0] == "true";
        }

        await writer.WritePayloadAsync(customers, stream, includeRawValues);
    }
}

