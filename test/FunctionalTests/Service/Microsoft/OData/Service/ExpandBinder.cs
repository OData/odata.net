//---------------------------------------------------------------------
// <copyright file="ExpandBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Query;
    using Microsoft.OData.Core.Query.SemanticAst;

    /// <summary>
    /// Temporary simple expand binder to produce <see cref="Microsoft.OData.Core.Query.SemanticAst.ExpandItem"/>. Will be replaced with the ODL expand/select parser when integration is complete.
    /// </summary>
    internal static class ExpandBinder
    {
        /// <summary>
        /// Binds the expand paths from the requests $expand query option to the sets/types/properties from the metadata provider of the service.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="expandQueryOption">The value of the $expand query option.</param>
        /// <returns>The bound expand segments.</returns>
        internal static IList<IList<ExpandItem>> BindExpandSegments(RequestDescription requestDescription, IDataService dataService, string expandQueryOption)
        {
            Debug.Assert(requestDescription != null, "requestDescription != null");
            Debug.Assert(dataService != null, "dataService != null");

            if (string.IsNullOrWhiteSpace(expandQueryOption))
            {
                return new List<IList<ExpandItem>>();
            }

            ResourceType targetResourceType = requestDescription.TargetResourceType;
            ResourceSetWrapper targetResourceSet = requestDescription.TargetResourceSet;
            if (targetResourceType == null || targetResourceType.ResourceTypeKind != ResourceTypeKind.EntityType || targetResourceSet == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryExpandOptionNotApplicable);
            }

            MetadataProviderEdmModel model = dataService.Provider.GetMetadataProviderEdmModel();
            IEdmEntityType targetType = (IEdmEntityType)model.EnsureSchemaType(targetResourceType);
            IEdmEntitySet targetSet = model.EnsureEntitySet(targetResourceSet);

            SelectExpandClause clause;
            try
            {
                model.Mode = MetadataProviderEdmModelMode.SelectAndExpandParsing;
                clause = ODataUriParser.ParseSelectAndExpand(/*select*/ null, expandQueryOption, model, targetType, targetSet);
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
            finally
            {
                model.Mode = MetadataProviderEdmModelMode.Serialization;
            }

            return new ExpandAndSelectPathExtractor(clause).ExpandPaths;
        }
    }
}
