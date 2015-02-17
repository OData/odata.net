//---------------------------------------------------------------------
// <copyright file="ODataUriMinimumVersionRequiredCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Represents a enum for selecting types of BagProperties
    /// </summary>
    [ImplementationName(typeof(IODataUriMinimumVersionRequiredCalculator), "Default")]
    public class ODataUriMinimumVersionRequiredCalculator : IODataUriMinimumVersionRequiredCalculator
    {
        /// <summary>
        /// Calculates the version based on the ODataUri provided
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns>Data Service Protocol Version</returns>
        public DataServiceProtocolVersion CalculateMinRequestVersion(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.Assert(maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified when calculating the MinVersion");

            string contentType = request.GetHeaderValueIfExists(HttpHeaders.ContentType);
            DataServiceProtocolVersion dataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.DataServiceVersion));
            DataServiceProtocolVersion maxDataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.MaxDataServiceVersion));
            HttpVerb effectiveVerb = request.GetEffectiveVerb();

            if (contentType == null)
            {
                contentType = MimeTypes.Any;
            }

            // No real request payload for Delete so its automatically version 1
            if (request.GetEffectiveVerb() == HttpVerb.Delete)
            {
                return DataServiceProtocolVersion.V4;
            }

            if (effectiveVerb.IsUpdateVerb() || effectiveVerb == HttpVerb.Post)
            {
                EntitySet entitySet = null;
                if (request.Uri.TryGetExpectedEntitySet(out entitySet))
                {
                    // Determine if the operation and Uri combined yields something that we need to look at the metadata to determine the version or not
                    bool processTypeMetadata = false;

                    // Typically for all posts there is some type of payload so we should analyze the metadata
                    if (effectiveVerb == HttpVerb.Post)
                    {
                        processTypeMetadata = true;
                    }
                    else if (dataServiceVersion != DataServiceProtocolVersion.Unspecified && request.PreferHeaderApplies(maxProtocolVersion))
                    {
                        processTypeMetadata = true;
                    }

                    IEnumerable<EntityType> entityTypes = VersionHelper.GetEntityTypes(entitySet);

                    // Whenever there is an update operation and EPM is involved we need to check the metadata version
                    if (entityTypes.SelectMany(et => et.Annotations.OfType<PropertyMappingAnnotation>()).Where(fma => fma.KeepInContent == false).Any())
                    {
                        processTypeMetadata = true;
                    }

                    if (processTypeMetadata)
                    {
                        return VersionHelper.GetMaximumVersion(entityTypes.Select(et => et.CalculateEntityPropertyMappingProtocolVersion(VersionCalculationType.Request, contentType, maxProtocolVersion, maxDataServiceVersion)).ToArray());
                    }
                }

                return DataServiceProtocolVersion.V4;
            }

            return VersionHelper.CalculateUriMinRequestProtocolVersion(request.Uri, contentType, maxProtocolVersion, maxDataServiceVersion);
        }

        /// <summary>
        /// Calculates the version based on the ODataUri provided
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns>Data Service Protocol Version</returns>
        public DataServiceProtocolVersion CalculateMinResponseVersion(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.Assert(maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified when calculating the MinVersion");

            string contentType = request.GetHeaderValueIfExists(HttpHeaders.Accept);
            DataServiceProtocolVersion maxDataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.MaxDataServiceVersion));
            HttpVerb effectiveVerb = request.GetEffectiveVerb();

            if (contentType == null)
            {
                contentType = MimeTypes.Any;
            }

            if (effectiveVerb == HttpVerb.Post || effectiveVerb.IsUpdateVerb())
            {
                DataServiceProtocolVersion version = DataServiceProtocolVersion.V4;

                string preferHeaderValue = request.GetHeaderValueIfExists(HttpHeaders.Prefer);

                // Bump to Version 3 if prefer header is specified and its > V3 server
                if (preferHeaderValue != null && request.PreferHeaderApplies(maxProtocolVersion))
                {
                    version = version.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
                }

                // for insert or update, versioning is specific to the type
                EntityType expectedEntityType;
                if (TryGetExpectedType(request, out expectedEntityType))
                {
                    version = VersionHelper.IncreaseVersionIfRequired(version, VersionHelper.CalculateProtocolVersion(expectedEntityType, contentType, VersionCalculationType.Response, maxProtocolVersion, maxDataServiceVersion));
                }

                return version;
            }

            return VersionHelper.CalculateUriResponseMinProtocolVersion(request.Uri, contentType, maxProtocolVersion, maxDataServiceVersion);
        }

        internal static bool TryGetExpectedType(ODataRequest request, out EntityType entityType)
        {
            EntitySet entitySet;
            if (request.Uri.TryGetExpectedEntitySet(out entitySet))
            {
                var possibleTypes = entitySet.Container.Model.EntityTypes.Where(t => t.IsKindOf(entitySet.EntityType)).ToList();
                if (possibleTypes.Count == 1)
                {
                    entityType = possibleTypes[0];
                    return true;
                }

                string typeName = null;
                if (entitySet.EntityType.HasStream())
                {
                    typeName = request.GetHeaderValueIfExists(HttpHeaders.MediaLinkEntryEntityTypeHint);
                }
                else if (request.Body != null && request.Body.RootElement.ElementType == ODataPayloadElementType.EntityInstance)
                {
                    typeName = ((EntityInstance)request.Body.RootElement).FullTypeName;
                }

                if (typeName != null)
                {
                    entityType = possibleTypes.SingleOrDefault(t => t.FullName == typeName);
                    return entityType != null;
                }
            }

            entityType = null;
            return false;
        }
    }
}
