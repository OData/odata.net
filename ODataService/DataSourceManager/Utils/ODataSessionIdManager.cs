// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace DataSourceManager.Utils
{
    ///// <summary>
    ///// The default SessionIdManager in Azure will cause to loop 302, use custom SessionIdManager to avoid this.
    ///// </summary>
    //public class ODataSessionIdManager : ISessionIDManager
    //{
    //    private static InternalSessionIdManager _internalManager = new InternalSessionIdManager();

    //    public string CreateSessionID(HttpContext context)
    //    {
    //        return _internalManager.CreateSessionID(context);
    //    }

    //    public string GetSessionID(HttpContext context)
    //    {
    //        var id = context.Items["AspCookielessSession"] as string;

    //        // Azure web site does not support header "AspFilterSessionId", so we cannot get context.Items["AspCookielessSession"]
    //        // for azure web site use, Headers["X-Original-URL"] format: /(S(xxx))/odata/path.
    //        var originalUrl = context.Request.Headers["X-Original-URL"];

    //        if (!string.IsNullOrEmpty(originalUrl))
    //        {
    //            var match = Regex.Match(context.Request.Headers["X-Original-URL"], @"/\(S\((\w+)\)\)");
    //            if (match.Success)
    //            {
    //                id = match.Groups[1].Value;
    //            }
    //        }

    //        return id;
    //    }

    //    public void Initialize()
    //    {
    //        _internalManager.Initialize();
    //    }

    //    public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIdReissue)
    //    {
    //        return _internalManager.InitializeRequest(context, suppressAutoDetectRedirect, out supportSessionIdReissue);
    //    }

    //    public void RemoveSessionID(HttpContext context)
    //    {
    //        _internalManager.RemoveSessionID(context);
    //    }

    //    public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
    //    {
    //        _internalManager.SaveSessionID(context, id, out redirected, out cookieAdded);
    //    }

    //    public bool Validate(string id)
    //    {
    //        return _internalManager.Validate(id);
    //    }

    //    private class InternalSessionIdManager : SessionIDManager
    //    {
    //        public override bool Validate(string id)
    //        {
    //            return !string.IsNullOrEmpty(id);
    //        }
    //    }
    //}
}
