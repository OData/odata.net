//---------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Text;
    using System.Web;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.UriHandlers;

    /// <summary>
    /// Wrapper class for incoming client request URI/queries.
    /// </summary>
    public class QueryContext
    {
        private QueryTarget target;
        
        // the $skiptoken contains TrackingChanges and SkipCount members
        private readonly dynamic skipToken;

        public QueryContext(Uri rootUri, Uri requestUri, IEdmModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (rootUri == null)
            {
                throw new ArgumentNullException("rootUri");
            }

            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            this.RootUri = rootUri;

            this.Model = model;
            this.QueryUri = requestUri;

            ODataUriParser uriParser = new ODataUriParser(this.Model, ServiceConstants.ServiceBaseUri, requestUri);

            // TODO: [Tiano]: Recover the following when using new delta framework
            // this.QueryPath = uriParser.ParsePath();

            this.UriParser = uriParser;

            this.FormatOption = this.GetAcceptFormatFromUri(requestUri);

            this.DeltaToken = this.GetDeltaTokenFromUri(requestUri);

            this.AsyncToken = this.GetAsyncTokenFromUri(requestUri);

            this.skipToken = this.ParseSkipToken();
        }

        /// <summary>
        /// Gets the model for query context
        /// </summary>
        public IEdmModel Model { get; private set; }

        /// <summary>
        /// Gets the root URI
        /// </summary>
        public Uri RootUri { get; private set; }

        /// <summary>
        /// Gets the request URI
        /// </summary>
        public Uri QueryUri { get; private set; }

        /// <summary>
        /// Gets the Canonical URI if it's not the same as QueryUri
        /// </summary>
        public Uri CanonicalUri { get; set; }

        /// <summary>
        /// Gets the ODL QueryPath for the parsed URI
        /// </summary>
        public ODataPath QueryPath
        {
            get { return this.UriParser.ParsePath(); }
        }

        /// <summary>
        /// Gets the FilterClause of the parsed URI
        /// </summary>
        public FilterClause QueryFilterClause
        {
            get { return this.UriParser.ParseFilter(); }
        }

        /// <summary>
        /// Gets the OrderByClause of the parsed URI
        /// </summary>
        public OrderByClause QueryOrderByClause
        {
            get { return this.UriParser.ParseOrderBy(); }
        }

        /// <summary>
        /// Gets the SelectExpandClause of the parsed URI
        /// </summary>
        public SelectExpandClause QuerySelectExpandClause
        {
            get
            {
                try
                {
                    return this.UriParser.ParseSelectAndExpand();
                }
                catch (ODataException e)
                {
                    throw Utility.BuildException(HttpStatusCode.BadRequest, e.Message, e);
                }
            }
        }

        public SearchClause QuerySearchClause
        {
            get { return this.UriParser.ParseSearch(); }
        }

        ///<summary>
        /// Get the EntityIdSegment of the parsed URI
        /// </summary>
        public EntityIdSegment QueryEntityIdSegment
        {
            get { return this.UriParser.ParseEntityId(); }
        }

        /// <summary>
        /// Get the query count option of the parsed URI 
        /// </summary>
        public bool? CountOption
        {
            get
            {
                try
                {
                    return this.UriParser.ParseCount();
                }
                catch (ODataException ex)
                {
                    // according to the protocol, the service should response 400 status code
                    throw new ODataServiceException(HttpStatusCode.BadRequest, ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Get the $top value of the parsed URI
        /// </summary>
        public long? TopOption
        {
            get { return this.UriParser.ParseTop(); }
        }

        /// <summary>
        /// Get the $skip value of the parsed URI
        /// </summary>
        public long? SkipOption
        {
            get { return this.UriParser.ParseSkip(); }
        }

        /// <summary>
        /// $format value of the parsed URI
        /// </summary>
        public string FormatOption { get; set; }

        /// <summary>
        /// $deltaToken value of the parsed URI
        /// </summary>
        public string DeltaToken { get; set; }

        /// <summary>
        /// $asyncToken value of the parsed URI
        /// </summary>
        public string AsyncToken { get; set; }

        /// <summary>
        /// Gets a value to indicate whether the current request should track changes.
        /// </summary>
        public bool TrackingChanges { get; private set; }

        /// <summary>
        /// if the current request should handle server-driven paging, the field is non-null, otherwise null.
        /// </summary>
        public int? appliedPageSize { get; private set; }

        /// <summary>
        /// If the current response should generate a next page link, returns it, otherwise null.
        /// </summary>
        public Uri NextLink { get; private set; }

        /// <summary>
        /// If the current response should generate a delta link, returns it, otherwise null.
        /// </summary>
        public Uri DeltaLink { get; private set; }

        /// <summary>
        /// Gets the total count of the query. The value is only avialable when count option specified.
        /// </summary>
        public long? TotalCount { get; private set; }

        /// <summary>
        /// Get the UriParser.
        /// </summary>
        public ODataUriParser UriParser { get; private set; }

        public Expression[] ActionInvokeParameters { get; set; }

        // TODO: [jiajyu] Temporary solution for unknown NavigationSource from operation. Should be removed later.
        public IEdmNavigationSource OperationResultSource { get; set; }

        public QueryTarget Target
        {
            get
            {
                if (this.target == null)
                {
                    this.target = QueryTarget.Resolve(this.QueryPath);
                }

                return this.target;
            }
        }

        /// <summary>
        /// Resolves the parsed URI against the data store.
        /// </summary>
        /// <param name="model">The data store model.</param>
        /// <param name="dataContext">The data access context.</param>
        /// <param name="level">The level of segment need to be translated, the default value is -1 means translate all.</param>
        /// <returns>The results of querying the data store.</returns>
        public object ResolveQuery(IODataDataSource dataSource, int level = -1)
        {
            var testExpressionVisitor = new PathSegmentToExpressionTranslator(dataSource, this, this.Model) { ActionInvokeParameters = this.ActionInvokeParameters };

            // build linq expression from ODataPath and execute the expression 
            Expression boundExpression = Expression.Constant(null);

            int levelToTranslate = level == -1 ? this.QueryPath.Count - 1 : level;
            for (int i = 0; i <= levelToTranslate; ++i)
            {
                boundExpression = this.QueryPath.ElementAt(i).TranslateWith(testExpressionVisitor);
            }

            //Handle Action without return type
            var methodCallExpression = boundExpression as MethodCallExpression;
            if (methodCallExpression != null && methodCallExpression.Method.ReturnType == typeof(void))
            {
                Expression<Action> actionLambda = Expression.Lambda<Action>(boundExpression);

                actionLambda.Compile()();
                return null;
            }

            if (this.QueryPath.LastSegment is NavigationPropertyLinkSegment && this.QueryEntityIdSegment != null)
            {
                // We assume the $id always finish with the keysegment, and consistence with the base path.
                ODataUriParser uriParser = new ODataUriParser(this.Model, ServiceConstants.ServiceBaseUri, this.QueryEntityIdSegment.Id);
                boundExpression = uriParser.ParsePath().LastSegment.TranslateWith(testExpressionVisitor);
            }

            // handle $filter query option
            if (this.QueryFilterClause != null)
            {
                boundExpression = boundExpression.ApplyFilter(GetElementTypeForOption(ServiceConstants.QueryOption_Filter), this.UriParser, this.QueryFilterClause);
            }

            //handle $search query option
            if (this.QuerySearchClause != null)
            {
                boundExpression = boundExpression.ApplySearch(GetElementTypeForOption(ServiceConstants.QueryOption_Search), this.UriParser, this.QuerySearchClause);
            }

            //handle $orderby query option
            if (this.QueryOrderByClause != null)
            {
                boundExpression = boundExpression.ApplyOrderBy(GetElementTypeForOption(ServiceConstants.QueryOption_OrderBy), this.UriParser, this.QueryOrderByClause);
            }

            //handle $skip query option
            if (this.SkipOption != null)
            {
                boundExpression = boundExpression.ApplySkip(GetElementTypeForOption(ServiceConstants.QueryOption_Skip), this.SkipOption.Value);
            }

            //handle $top query option
            if (this.TopOption != null)
            {
                boundExpression = boundExpression.ApplyTop(GetElementTypeForOption(ServiceConstants.QueryOption_Top), this.TopOption.Value);
            }

            boundExpression = Expression.Convert(boundExpression, typeof(object));
            Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(boundExpression);

            Func<object> compiled = lambda.Compile();

            var result = default(object);

            try
            {
                result = compiled();
            }
            catch (NullReferenceException)
            {
                // Currently we assume the NRE will lead to NotFound.
                throw Utility.BuildException(HttpStatusCode.NotFound);
            }

            return ProcessQueryResult(result);
        }

        /// <summary>
        /// Call this method to initialize the environment to support server-driven paging.
        /// </summary>
        /// <param name="context">The values of the Prefer HTTP header.</param>
        public void InitializeServerDrivenPaging(PreferenceContext context)
        {
            if (context == null) throw Utility.BuildException(HttpStatusCode.InternalServerError);

            if (this.Target.TypeKind == EdmTypeKind.Collection && this.Target.ElementTypeKind == EdmTypeKind.Entity)
            {
                // If the current request contains odata.maxpagesize preference
                if (context.MaxPageSize.HasValue)
                {
                    var clientSize = context.MaxPageSize.Value;
                    // The client might set odata.maxpagesize as zero or negative
                    if (clientSize < 1)
                    {
                        clientSize = ServiceConstants.DefaultPageSize;
                    }

                    this.appliedPageSize = clientSize;
                }
                else
                {
                    this.appliedPageSize = ServiceConstants.DefaultPageSize;
                }
            }
        }

        /// <summary>
        /// Call this method to detect whether the current request need to track changes.
        /// </summary>
        /// <param name="context">The values of the Prefer HTTP header.</param>
        public void InitializeTrackingChanges(PreferenceContext context)
        {
            if (context == null) throw Utility.BuildException(HttpStatusCode.InternalServerError);

            this.TrackingChanges = context.TrackingChanges || this.skipToken.TrackingChanges;
        }

        /// <summary>
        /// Attempts to return the level of delete source in URL
        /// </summary>
        /// <returns></returns>
        public int ResolveDeleteSourceLevel()
        {
            int level = -1;
            for (int i = this.QueryPath.Count - 1; i >= 0; --i)
            {
                var segment = this.QueryPath.ElementAt(i);
                if (segment as EntitySetSegment != null || segment as NavigationPropertySegment != null)
                {
                    level = i;
                    return level;
                }
            }
            return level;
        }

        /// <summary>
        /// Attempts to resolve the key values for a entity based query.
        /// </summary>
        /// <returns>The key values for the query.</returns>
        public IDictionary<string, object> ResolveKeyValues()
        {
            Dictionary<string, object> keyDictionary = new Dictionary<string, object>();
            KeySegment keySegment = this.QueryPath.OfType<KeySegment>().LastOrDefault();
            if (keySegment != null)
            {
                foreach (var key in keySegment.Keys)
                {
                    keyDictionary.Add(key.Key, key.Value);
                }
            }

            return keyDictionary;
        }

        private Type GetElementTypeForOption(string option)
        {
            if (this.Target.ElementTypeKind == EdmTypeKind.None)
            {
                throw Utility.BuildException(
                    HttpStatusCode.BadRequest,
                    string.Format("The query option '{0}' can only be applied to collection resouces", option),
                    null);
            }

            return EdmClrTypeUtils.GetInstanceType(this.Target.ElementType.FullTypeName());
        }

        private string GetAcceptFormatFromUri(Uri requestUri)
        {
            var format = HttpUtility.ParseQueryString(requestUri.Query).Get(ServiceConstants.QueryOption_Format);
            if (!string.IsNullOrEmpty(format))
            {
                switch (format)
                {
                    case "json":
                        return "application/json";
                    case "atom":
                        return "application/atom+xml";
                    default:
                        return format;
                }
            }

            return null;
        }

        private string GetDeltaTokenFromUri(Uri requestUri)
        {
            return HttpUtility.ParseQueryString(requestUri.Query).Get(ServiceConstants.QueryOption_Delta);
        }

        private string GetAsyncTokenFromUri(Uri requestUri)
        {
            string asyncToken = HttpUtility.ParseQueryString(requestUri.Query).Get(ServiceConstants.QueryOption_AsyncToken);
            if (!string.IsNullOrWhiteSpace(asyncToken))
            {
                return asyncToken;
            }
            return null;
        }

        /// <summary>
        /// Call this method to handle more tasks on the query result.
        /// </summary>
        /// <param name="originalResult">The unprocessed query result.</param>
        /// <returns>The new query result.</returns>
        private object ProcessQueryResult(object originalResult)
        {
            var result = originalResult;

            if (result != null)
            {
                var allResults = default(IEnumerable<object>);
                var collectionResults = result as IEnumerable;
                if (collectionResults != null)
                {
                    allResults = collectionResults.Cast<object>();
                    if (this.CountOption == true)
                    {
                        this.TotalCount = allResults.LongCount();
                    }
                }

                // if the current request should handle server-driven paging
                if (this.appliedPageSize.HasValue)
                {
                    var partialResults = allResults.Skip((int)this.skipToken.SkipCount);

                    // if has next page
                    if (partialResults.Count() > this.appliedPageSize.Value)
                    {
                        partialResults = partialResults.Take(this.appliedPageSize.Value);
                        this.NextLink = CreateNextPageLink();
                    }
                    else
                    {
                        if (this.TrackingChanges)
                        {
                            this.DeltaLink = CreateDeltaLink((IEnumerable)originalResult);
                        }
                    }

                    // change the result to partial
                    result = ConvertToNonGenericCollection(partialResults);
                }
                else
                {
                    // tracking changes without server-driven paging
                    if (this.TrackingChanges)
                    {
                        this.DeltaLink = CreateDeltaLink((IEnumerable)originalResult);
                    }
                }
            }

            return result;
        }

        private Uri CreateNextPageLink()
        {
            // The OData protocol says that the client specifies odata.track-changes on the initial request to the delta link but is not required to repeat it for subsequent pages,
            // so if the current request needs paging, we put the tracking changes flag in the $skiptoken.

            // If the current response has a delta link, the format of $skiptoken is 'DLxxx', otherwise 'xxx'. The xxx is the skip row count of the next request.

            // builds new $skiptoken
            var skipSize = this.skipToken.SkipCount + this.appliedPageSize.Value;
            var format = this.skipToken.TrackingChanges ? "DL{0}" : "{0}";
            var skipToken = string.Format(CultureInfo.InvariantCulture, format, skipSize);

            // creates the query
            var query = this.QueryUri.Query.TrimStart('?');
            var queries = HttpUtility.ParseQueryString(query, Encoding.UTF8);
            queries["$skiptoken"] = skipToken;

            // builds URI
            var path = this.QueryUri.OriginalString.Substring(0, this.QueryUri.OriginalString.Length - this.QueryUri.Query.Length);
            return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?{1}", HttpUtility.UrlPathEncode(path), queries));
        }

        private Uri CreateDeltaLink(IEnumerable results)
        {
            var deltaToken = DeltaContext.GenerateDeltaToken(this.QueryUri, results, this.Target.NavigationSource, this.QuerySelectExpandClause);
            return new Uri(string.Format("{0}?{1}={2}", ServiceConstants.ServiceBaseUri, ServiceConstants.QueryOption_Delta, deltaToken));
        }

        private dynamic ParseSkipToken()
        {
            // If the original request needs to track changes, the format of $skiptoken is 'DLxxx', otherwise 'xxx'. The xxx is the skip row count of the next request.

            dynamic result = new ExpandoObject();
            result.TrackingChanges = false;
            result.SkipCount = 0;

            // Uri.Query property contains '?' symbol.
            var query = this.QueryUri.Query.TrimStart('?');

            var queries = HttpUtility.ParseQueryString(query, Encoding.UTF8);
            var skipToken = queries["$skiptoken"];

            if (skipToken != null)
            {
                if (skipToken.StartsWith("DL", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.TrackingChanges = true;
                    skipToken = skipToken.Substring(2);
                }

                result.SkipCount = int.Parse(skipToken, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            return result;
        }

        private static IEnumerable ConvertToNonGenericCollection(IEnumerable<object> source)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }
}
