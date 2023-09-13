// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData.Routing;

namespace DynamicService
{
#if false
    public class DynamicODataRoute : ODataRoute
    {
        private static readonly string _escapedHashMark = Uri.HexEscape('#');
        private static readonly string _escapedQuestionMark = Uri.HexEscape('?');

        private bool _canGenerateDirectLink;

        public DynamicODataRoute(string routePrefix, ODataPathRouteConstraint pathConstraint)
            : base(routePrefix, pathConstraint)
        {
            _canGenerateDirectLink = routePrefix != null && RoutePrefix.IndexOf('{') == -1;
        }

        public override IHttpVirtualPathData GetVirtualPath(
            HttpRequestMessage request,
            IDictionary<string, object> values)
        {
            if (values == null || !values.Keys.Contains(HttpRoute.HttpRouteKey, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            object odataPathValue;
            if (!values.TryGetValue(ODataRouteConstants.ODataPath, out odataPathValue))
            {
                return null;
            }

            string odataPath = odataPathValue as string;
            if (odataPath != null)
            {
                return GenerateLinkDirectly(request, odataPath) ?? base.GetVirtualPath(request, values);
            }

            return null;
        }

        internal HttpVirtualPathData GenerateLinkDirectly(HttpRequestMessage request, string odataPath)
        {
            HttpConfiguration configuration = request.GetConfiguration();
            if (configuration == null || !_canGenerateDirectLink)
            {
                return null;
            }

            string dataSource = request.Properties[DynamicRouteConstraint.DataSourceNameProperty] as string;
            string link = CombinePathSegments(RoutePrefix, dataSource);
            link = CombinePathSegments(link, odataPath);
            link = UriEncode(link);

            return new HttpVirtualPathData(this, link);
        }

        private static string CombinePathSegments(string routePrefix, string odataPath)
        {
            return string.IsNullOrEmpty(routePrefix)
                ? odataPath
                : (string.IsNullOrEmpty(odataPath) ? routePrefix : routePrefix + '/' + odataPath);
        }

        private static string UriEncode(string str)
        {
            string escape = Uri.EscapeUriString(str);
            escape = escape.Replace("#", _escapedHashMark);
            escape = escape.Replace("?", _escapedQuestionMark);
            return escape;
        }
    }
#endif
}