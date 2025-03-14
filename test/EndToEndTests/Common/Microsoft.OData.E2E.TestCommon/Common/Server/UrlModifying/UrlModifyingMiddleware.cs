//---------------------------------------------------------------------
// <copyright file="UrlModifyingMiddleware.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UrlModifying;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

public class UrlModifyingMiddleware
{
    private readonly RequestDelegate _next;

    public UrlModifyingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var uri = new Uri(request.GetDisplayUrl());

        if (uri.AbsoluteUri.EndsWith("RemapPath"))
        {
            context.Request.Path = "/odata/Customers";
        }
        else if (uri.AbsoluteUri.EndsWith("RemapBase"))
        {
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("potato");
            context.Request.Path = "/odata/Customers";
        }
        else if (uri.AbsoluteUri.EndsWith("RemapBaseAndPathSeparately"))
        {
            context.Request.Path = "/odata/Customers";

            context.Request.Scheme = "http";
            context.Request.Host = new HostString("potato");
        }
        else if (uri.AbsoluteUri.EndsWith("BasesDontMatchFail"))
        {
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("potato/odata");
            context.Request.Path = "/NotFound/Customers";
        }
        else if (uri.AbsoluteUri.EndsWith("People"))
        {
            context.Request.QueryString = new QueryString(context.Request.QueryString + "?$top=3");
        }

        await _next(context);
    }
}

