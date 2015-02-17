//---------------------------------------------------------------------
// <copyright file="ODataUriVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a enum for selecting types of BagProperties
    /// </summary>
    [ImplementationName(typeof(IODataUriVersionCalculator), "Default")]
    public class ODataUriVersionCalculator : IODataUriVersionCalculator
    {
        /// <summary>
        /// Calculates the protocol version based on the ODataUri provided
        /// </summary>
        /// <param name="uri">OData Uri to analyze</param>
        /// <param name="contentType">Content Type</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="dataServiceVersion">The data service version of the request</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Data Service Protocol Version</returns>
        public DataServiceProtocolVersion CalculateProtocolVersion(ODataUri uri, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion dataServiceVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            ExceptionUtilities.Assert(maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified");

            if (uri.IsMetadata())
            {
                throw new TaupoNotSupportedException("Context uri should be processed by Entity Model Version Calculator instead");
            }

            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;

            if (uri.IncludesInlineCountAllPages())
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            }

            if (uri.IsCount())
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            }
            else if (uri.IsEntityReferenceLink() || uri.IsNamedStream() || uri.IsPropertyValue())
            {
                // If the uri points to a Link, $value or a named stream their response DSV's are V1 because their payloads are understood by V1
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            }
            else if (uri.HasOpenProperties())
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.GetMinimumVersion(maxProtocolVersion, maxDataServiceVersion));
            }
            else if (uri.IsProperty())
            {
                var propertySegment = (PropertySegment)uri.LastSegment;
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.CalculateProtocolVersion(propertySegment.Property));
            }
            else if (uri.IsAction())
            {
                var action = (FunctionSegment)uri.LastSegment;
                if (action.Function.ReturnType != null)
                {
                    var collectionReturnType = action.Function.ReturnType as CollectionDataType;

                    // If its a collection this doesn't bump things to V3 as this is normal behavior from ServiceOperations V1, so only determining version based on non data type
                    DataType versionType = action.Function.ReturnType;
                    
                    var entityVersionType = action.Function.ReturnType as EntityDataType;
                    if (collectionReturnType != null)
                    {
                        entityVersionType = collectionReturnType.ElementDataType as EntityDataType;
                        if (entityVersionType != null)
                        {
                            versionType = entityVersionType;
                        }
                    }

                    // Bump if its an open type
                    if (entityVersionType != null && entityVersionType.Definition.IsOpen)
                    {
                        expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.GetMinimumVersion(maxProtocolVersion, maxDataServiceVersion));
                    }

                    expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.CalculateDataTypeVersion(versionType));
                }
            }
            else
            {
                ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
                List<DataServiceProtocolVersion> dataServiceProtocolVersions = uri.GetAllEntitySetsIncludingExpands().Select(es => es.CalculateEntitySetProtocolVersion(contentType, VersionCalculationType.Response, maxProtocolVersion, maxDataServiceVersion)).ToList();
                dataServiceProtocolVersions.Add(expectedVersion);
                expectedVersion = VersionHelper.GetMaximumVersion(dataServiceProtocolVersions.ToArray());
            }

            // determine if the uri MIGHT result in there being a next link, and increase version if so
            expectedVersion = VersionHelper.IncreaseVersionIfTrue(ResponseMightIncludeNextLink(uri), expectedVersion, DataServiceProtocolVersion.V4);

            return expectedVersion;
        }

        internal static bool ResponseMightIncludeNextLink(ODataUri uri)
        {
            var payloadsWithNextLinks = new[] { ODataPayloadElementType.EntitySetInstance, ODataPayloadElementType.LinkCollection };
            var expectedPayloadType = uri.GetExpectedPayloadType();
            if (!payloadsWithNextLinks.Contains(expectedPayloadType))
            {
                return false;
            }

            EntitySet expectedSet;
            ExceptionUtilities.Assert(uri.TryGetExpectedEntitySet(out expectedSet), "Could not get expected entity set");

            var pageSize = expectedSet.GetEffectivePageSize();
            if (!pageSize.HasValue)
            {
                return false;
            }

            // if the value of $top is less than 1 page size, no next link will be included
            return !uri.Top.HasValue || uri.Top.Value > pageSize.Value;
        }
    }
}