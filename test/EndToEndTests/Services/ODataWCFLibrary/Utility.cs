//---------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    internal static class Utility
    {
        public static Uri RebuildUri(Uri original)
        {
            if (!HostingEnvironment.IsHosted)
            {
                return original;
            }

            var uri = default(Uri);

            if (ODataSessionIdManager.IsSharedRequest)
            {
                uri = original;
            }
            else
            {
                var match = Regex.Match(original.AbsoluteUri, @"/\(S\((\w+)\)\)");
                if (match.Success)
                {
                    uri = original;
                }
                else
                {
                    var builder = new UriBuilder(original.Scheme, original.Host, original.Port, HttpContext.Current.Request.ApplicationPath);
                    var beforeSessionSegement = new Uri(builder.ToString(), UriKind.Absolute).AbsoluteUri;
                    var afterSessionSegment = original.AbsoluteUri.Substring(beforeSessionSegement.Length);

                    var sessionSegment = string.Format("(S({0}))", HttpContext.Current.Session.SessionID);
                    var path = CombineUriPaths(beforeSessionSegement, sessionSegment);
                    path = CombineUriPaths(path, afterSessionSegment);
                    uri = new Uri(path);
                }
            }

            var uriBuilder = new UriBuilder(uri);
            var baseAddressUri = new Uri(Utility.GetServiceBaseAddress(), UriKind.Absolute);
            uriBuilder.Host = baseAddressUri.Host;
            uri = new Uri(uriBuilder.ToString());

            return uri;
        }

        public static string GetServiceBaseAddress()
        {
            var result = default(string);

            if (HostingEnvironment.IsHosted)
            {
                // TODO: when using Azure API, change the following code with Azure API. if the code is deployed on the Azure, read web.config
                result = ODataSessionIdManager.IsSharedRequest ? WebConfigurationManager.AppSettings["TripPinServiceBaseAddress"] : WebConfigurationManager.AppSettings["TripPinServiceRWBaseAddress"];
            }

            if (string.IsNullOrEmpty(result))
            {
                // TODO: We should not use it, BaseUri should be self contain in the context url
                result = OperationContext.Current.Host.BaseAddresses.First().AbsoluteUri.TrimEnd('/') + "/";
            }

            return result;
        }

        public static object GetRootQuery(IODataDataSource dataSource, IEdmNavigationSource navigationSource)
        {
            return dataSource.GetType().GetProperty(navigationSource.Name).GetValue(dataSource, null);
        }

        public static object QuickCreateInstance(Type type)
        {
            return Expression.Lambda<Func<object>>(Expression.New(type), new ParameterExpression[0]).Compile()();
        }

        public static object CreateResource(IEdmType type)
        {
            var targetType = EdmClrTypeUtils.GetInstanceType(type.FullTypeName());
            return QuickCreateInstance(targetType);
        }

        public static Uri BuildLocationUri(QueryContext context, object target)
        {
            if (context.Target.NavigationSource == null)
            {
                throw new InvalidOperationException("Building Location URI for non-entity resource is not supported.");
            }

            if (context.Target.IsEntitySet)
            {
                var keySegment = BuildKeySegment(context.Target.ElementType as IEdmEntityType, context.Target.NavigationSource as IEdmEntitySetBase, target);
                return context.Target.BuildCanonicalUri(context.RootUri, keySegment);
            }

            return context.Target.BuildCanonicalUri(context.RootUri, null);
        }

        public static bool IsMediaEntity(Type type)
        {
            return typeof(MediaEntity).IsAssignableFrom(type);
        }

        public static bool IsETagProperty(object entity, string propertyName)
        {
            var etagPropertyName = GetETagPropertyName(entity);
            return (!string.IsNullOrEmpty(etagPropertyName) && etagPropertyName == propertyName);
        }

        public static string FormatETagValueWeak(object rawValue)
        {
            var value = rawValue;

            if (rawValue is long)
            {
                value = ((long)rawValue).ToString("X16");
            }

            return string.Format("W/\"{0}\"", value);
        }

        public static string GetETagValue(object clrObject)
        {
            // TODO: we will consider how to support multiple ETag fields

            var propertyName = GetETagPropertyName(clrObject);

            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            var rawValue = clrObject.GetType().GetProperty(propertyName).GetValue(clrObject, null);

            // weak ETag value
            return FormatETagValueWeak(rawValue);
        }

        public static string GetETagPropertyName(object clrObject)
        {
            // TODO: we will consider how to support multiple ETag fields

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
            var property = clrObject.GetType().GetProperties(flags).SingleOrDefault(p => p.GetCustomAttributes(typeof(ETagFieldAttribute), true).Any());
            return property == null ? null : property.Name;
        }

        public static bool TryGetIfMatch(IDictionary<string, string> requestHeaders, out string etag)
        {
            return TryGetIfMatchOrIfNoneMatch(ServiceConstants.HttpHeaders.IfMatch, requestHeaders, out etag);
        }

        public static bool TryGetIfNoneMatch(IDictionary<string, string> requestHeaders, out string etag)
        {
            return TryGetIfMatchOrIfNoneMatch(ServiceConstants.HttpHeaders.IfNoneMatch, requestHeaders, out etag);
        }

        public static void SetStatusCode(this IODataResponseMessage message, HttpStatusCode code)
        {
            message.StatusCode = (int)code;
        }

        public static void SetStatusCode(this IODataResponseMessage message, int code)
        {
            message.StatusCode = code;
        }

        public static void AddPreferenceApplied(this IODataResponseMessage message, string appliedPref)
        {
            if (!string.IsNullOrEmpty(appliedPref))
            {
                if (string.IsNullOrEmpty(message.GetHeader(ServiceConstants.HttpHeaders.PreferenceApplied)))
                {
                    message.SetHeader(ServiceConstants.HttpHeaders.PreferenceApplied, appliedPref);
                }
                else
                {
                    message.SetHeader(ServiceConstants.HttpHeaders.PreferenceApplied,
                        string.Format("{0};{1}", message.GetHeader(ServiceConstants.HttpHeaders.PreferenceApplied), appliedPref));
                }
            }
        }

        public static HttpMethod CreateHttpMethod(string method)
        {
            try
            {
                return (HttpMethod)Enum.Parse(typeof(HttpMethod), method, true);
            }
            catch
            {
                return HttpMethod.Unknown;
            }
        }

        public static Dictionary<string, string> ToDictionary(this WebHeaderCollection collection)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var key in collection.AllKeys)
            {
                result.Add(key, collection[key]);
            }

            return result;
        }

        public static ODataServiceException BuildException(HttpStatusCode statusCode)
        {
            return BuildException(statusCode, "An server side error occured.", null);
        }

        public static ODataServiceException BuildException(HttpStatusCode statusCode, string message, Exception exception)
        {
            return new ODataServiceException(statusCode, message, exception);
        }

        public static bool IsReadOnly(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(ReadOnlyFieldAttribute), true);
            return attributes.Length > 0;
        }

        private static bool TryGetIfMatchOrIfNoneMatch(string httpHeader, IDictionary<string, string> requestHeaders, out string etag)
        {
            etag = null;

            if (!requestHeaders.ContainsKey(httpHeader))
            {
                return false;
            }

            etag = requestHeaders[httpHeader];

            return true;
        }

        private static KeySegment BuildKeySegment(IEdmEntityType entityType, IEdmEntitySetBase entitySet, object target)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            if (entitySet == null)
            {
                throw new ArgumentNullException("entitySet");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            KeySegment keySegment = new KeySegment(
                entityType.Key().Select(
                    (key) =>
                    {
                        var keyValue = target.GetType().GetProperty(key.Name).GetValue(target, null);
                        return new KeyValuePair<string, object>(key.Name, keyValue);
                    }),
                entityType,
                entitySet);

            return keySegment;
        }

        private static string CombineUriPaths(string path1, string path2)
        {
            if (path1.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                if (path2.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    path2 = path2.Substring(1);
                }
            }
            else
            {
                if (!path2.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    path2 = "/" + path2;
                }
            }

            return path1 + path2;
        }
    }
}