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

        if (uri.AbsoluteUri.Contains("RemapBase"))
        {
            var newBaseUri = new Uri("http://potato");
            context.Request.Scheme = newBaseUri.Scheme;
            context.Request.Host = new HostString(newBaseUri.Host);
            context.Request.Path = "/odata/Customers";
        }
        else if (uri.AbsoluteUri.Contains("RemapBaseAndPathSeparately"))
        {
            context.Request.Path = "/odata/Customers";

            var newBaseUri = new Uri("http://potato");
            context.Request.Scheme = newBaseUri.Scheme;
            context.Request.Host = new HostString(newBaseUri.Host);
        }
        else if (uri.AbsoluteUri.Contains("BasesDontMatchFail"))
        {
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("potato/odata");
            context.Request.Path = "/DontFailMeService/Customers";
        }
        else if (uri.AbsoluteUri.Contains("People"))
        {
            context.Request.QueryString = new QueryString(context.Request.QueryString + "?$top=3");
        }

        await _next(context);
    }
}

