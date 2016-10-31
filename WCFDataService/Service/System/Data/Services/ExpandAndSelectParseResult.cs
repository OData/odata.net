//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Parsing;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion Namespaces

    /// <summary>
    /// Component for performing simple syntactic parsing of the $expand and $select query options.
    /// </summary>
    internal class ExpandAndSelectParseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Data.Services.ExpandAndSelectParseResult"/> class.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        /// <param name="dataService">The data service.</param>
        internal ExpandAndSelectParseResult(RequestDescription requestDescription, IDataService dataService)
        {
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(dataService.OperationContext != null, "dataService.OperationContext != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");

            string expand = dataService.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringExpand);
            this.RawSelectQueryOptionValue = dataService.OperationContext.RequestMessage.GetQueryStringItem(XmlConstants.HttpQueryStringSelect);

            if (string.IsNullOrEmpty(expand) && string.IsNullOrEmpty(this.RawSelectQueryOptionValue))
            {
                return;
            }

            ResourceType targetResourceType = requestDescription.TargetResourceType;
            ResourceSetWrapper targetResourceSet = requestDescription.TargetResourceSet;
            if (!string.IsNullOrEmpty(expand))
            {
                if (targetResourceType == null || targetResourceType.ResourceTypeKind != ResourceTypeKind.EntityType || targetResourceSet == null)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryExpandOptionNotApplicable);
                }
            }

            if (!string.IsNullOrEmpty(this.RawSelectQueryOptionValue))
            {
                ValidateSelectIsAllowedForRequest(requestDescription);

                // Throw if $select requests have been disabled by the user
                Debug.Assert(dataService.Configuration != null, "dataService.Configuration != null");
                if (!dataService.Configuration.DataServiceBehavior.AcceptProjectionRequests)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataServiceConfiguration_ProjectionsNotAccepted);
                }
            }

            MetadataProviderEdmModel model = dataService.Provider.GetMetadataProviderEdmModel();
            IEdmEntityType targetType = (IEdmEntityType)model.EnsureSchemaType(targetResourceType);
            IEdmEntitySet targetSet = model.EnsureEntitySet(targetResourceSet);

            var uriParser = new ODataUriParser(model, /*serviceRoot*/ null)
            {
                Settings =
                {
                    MaximumExpansionDepth = dataService.Configuration.MaxExpandDepth,
                    MaximumExpansionCount = dataService.Configuration.MaxExpandCount,
                }
            };

            uriParser.Settings.EnableWcfDataServicesServerBehavior();

            try
            {
                Debug.Assert(model.Mode == MetadataProviderEdmModelMode.Serialization, "Model expected to be in serialization mode by default");
                model.Mode = MetadataProviderEdmModelMode.SelectAndExpandParsing;
                this.Clause = uriParser.ParseSelectAndExpand(this.RawSelectQueryOptionValue, expand, targetType, targetSet);
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
            finally
            {
                model.Mode = MetadataProviderEdmModelMode.Serialization;
            }

            this.HasExpand = this.Clause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any();
            this.HasSelect = HasSelectedItemAtAnyLevel(this.Clause);
        }

        /// <summary>
        /// The actual select/expand clause.
        /// </summary>
        internal SelectExpandClause Clause { get; private set; }

        /// <summary>
        /// Whether there was a $expand in the request.
        /// </summary>
        internal bool HasExpand { get; private set; }

        /// <summary>
        /// Whether there was a $select in the request.
        /// </summary>
        internal bool HasSelect { get; private set; }

        /// <summary>
        /// Gets the raw query option value of $select.
        /// </summary>
        internal string RawSelectQueryOptionValue { get; private set; }

        /// <summary>
        /// Throws exceptions if the $select query option cannot be specified on this request.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        private static void ValidateSelectIsAllowedForRequest(RequestDescription requestDescription)
        {
            // We only allow $select on entity/entityset queries. Queries which return a primitive/complex value don't support $select.
            if (requestDescription.TargetResourceType == null || (requestDescription.TargetResourceType.ResourceTypeKind != ResourceTypeKind.EntityType))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }

            // $select can't be used on $links URIs as it doesn't make sense
            if (requestDescription.SegmentInfos.Any(si => si.TargetKind == RequestTargetKind.Link))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }
        }

        /// <summary>
        /// Determines whether there are any selected items at any level of the tree. Essentially determines whether $select was specified in the request.
        /// </summary>
        /// <param name="clause">The expand/select clause to check for selected items.</param>
        /// <returns>Whether the clause had a select item at any level that was not an expansion.</returns>
        private static bool HasSelectedItemAtAnyLevel(SelectExpandClause clause)
        {
            return !clause.AllSelected || clause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any(i => HasSelectedItemAtAnyLevel(i.SelectAndExpand));
        }
    }
}
