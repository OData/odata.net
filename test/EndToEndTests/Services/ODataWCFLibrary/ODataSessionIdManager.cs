//---------------------------------------------------------------------
// <copyright file="ODataSessionIdManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.SessionState;
    using Microsoft.Test.OData.Services.ODataWCFService.Extensions;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    public class ODataSessionIdManager : ISessionIDManager
    {
        private static readonly InternalSessionIdManager InternalManager = new InternalSessionIdManager();
        private static readonly List<string> SharedODataServiceNames = new List<string>();

        private const string SharedSessionId = "S_DEFAULT_ID";

        public static bool IsSharedRequest
        {
            get { return IsSharedServiceRequest(HttpContext.Current); }
        }

        public string CreateSessionID(HttpContext context)
        {
            if (IsSharedServiceRequest(context))
            {
                return SharedSessionId;
            }

            return InternalManager.CreateSessionID(context);
        }

        public string GetSessionID(HttpContext context)
        {
            if (IsSharedServiceRequest(context))
            {
                return SharedSessionId;
            }

            var id = HttpContext.Current.Items["AspCookielessSession"] as string;
            // Azure web site does not support header "AspFilterSessionId", so we cannot get context.Items["AspCookielessSession"]
            // for azure web site use, Headers["X-Original-URL"] format: /(S(xxx))/odata/odata.svc
            var originalUrl = HttpContext.Current.Request.Headers["X-Original-URL"];
            if (!string.IsNullOrEmpty(originalUrl))
            {
                var match = Regex.Match(HttpContext.Current.Request.Headers["X-Original-URL"], @"/\(S\((\w+)\)\)");
                if (match.Success)
                {
                    id = match.Groups[1].Value;
                }
            }

            return id;
        }

        public void Initialize()
        {
            InternalManager.Initialize();
        }

        public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIdReissue)
        {
            return InternalManager.InitializeRequest(context, suppressAutoDetectRedirect, out supportSessionIdReissue);
        }

        public void RemoveSessionID(HttpContext context)
        {
            InternalManager.RemoveSessionID(context);
        }

        public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
        {
            if (IsSharedServiceRequest(context))
            {
                // OData service default session does not need to be redirected.
                redirected = false;
                cookieAdded = false;
                return;
            }

            InternalManager.SaveSessionID(context, id, out redirected, out cookieAdded);
        }

        public bool Validate(string id)
        {
            return InternalManager.Validate(id);
        }

        private static bool IsSharedServiceRequest(HttpContext context)
        {
            EnsureSharedODataServiceNames(context);

            var result = false;

            var path = context.Request.CurrentExecutionFilePath;
            if (!string.IsNullOrEmpty(context.Request.ApplicationPath) && context.Request.ApplicationPath != "/")
            {
                path = path.Substring(context.Request.ApplicationPath.Length);
            }

            foreach (var serviceName in SharedODataServiceNames)
            {
                var servicePathNoQuery = string.Format("/{0}", serviceName);
                var servicePathWithQuery = string.Format("/{0}/", serviceName);
                if (string.Equals(path, servicePathNoQuery, StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(servicePathWithQuery, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static void EnsureSharedODataServiceNames(HttpContext context)
        {
            if (SharedODataServiceNames.Count == 0)
            {
                var descriptors = ExtensionManager.Container.GetExportedValues<IODataServiceDescriptor>();
                foreach (var descriptor in descriptors)
                {
                    if (descriptor.ServiceName != "TripPinServiceRW")
                    {
                        SharedODataServiceNames.Add(descriptor.ServiceName);
                    }
                }
            }
        }

        private class InternalSessionIdManager : SessionIDManager
        {
            public override bool Validate(string id)
            {
                return !string.IsNullOrEmpty(id);
            }
        }
    }
}