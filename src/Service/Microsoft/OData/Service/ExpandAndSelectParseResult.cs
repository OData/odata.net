//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>
    /// Component for performing simple syntactic parsing of the $expand and $select query options.
    /// </summary>
    internal class ExpandAndSelectParseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Microsoft.OData.Service.ExpandAndSelectParseResult"/> class.
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

            var uriParser = RequestUriProcessor.CreateUriParserWithBatchReferenceCallback(dataService, dataService.OperationContext.AbsoluteRequestUri);

            try
            {
                Debug.Assert(model.Mode == MetadataProviderEdmModelMode.Serialization, "Model expected to be in serialization mode by default");
                model.Mode = MetadataProviderEdmModelMode.SelectAndExpandParsing;
                this.Clause = uriParser.ParseSelectAndExpand();
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
            finally
            {
                model.Mode = MetadataProviderEdmModelMode.Serialization;
            }

            if (this.Clause != null)
            {
                this.HasExpand = this.Clause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any();
                this.HasSelect = HasSelectedItemAtAnyLevel(this.Clause);
            }
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

            // $select can't be used on $ref URIs as it doesn't make sense
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
