//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
