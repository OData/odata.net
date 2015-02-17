//---------------------------------------------------------------------
// <copyright file="VersionUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// A set of utility methods for dealing with versions and versioning.
    /// </summary>
    internal static class VersionUtil
    {
        /// <summary>Version 4.0.</summary>
        internal static readonly Version Version4Dot0 = new Version(4, 0);

        /// <summary>
        /// Default set of known odata versions, currently 4.0
        /// </summary>
        internal static readonly Version[] KnownODataVersions = new Version[] { Version4Dot0 };

        /// <summary>
        /// The default response version of the data service. If no version is set for a particular response
        /// The DataService will respond with this version (4.0)
        /// </summary>
        internal static readonly Version DataServiceDefaultResponseVersion = Version4Dot0;

        /// <summary>Given a <see cref="ODataProtocolVersion"/> enumeration returns the <see cref="Version"/> instance with the same version number.</summary>
        /// <param name="protocolVersion">The protocol version enum value to convert.</param>
        /// <returns>The version instance with the version number for the specified protocol version.</returns>
        internal static Version ToVersion(this ODataProtocolVersion protocolVersion)
        {
            switch (protocolVersion)
            {
                default:
                    Debug.Assert(protocolVersion == ODataProtocolVersion.V4, "Did you add a new version?");
                    return Version4Dot0;
            }
        }

        /// <summary>
        /// If necessary raises the metadata edm schema version.
        /// </summary>
        /// <param name="versionToRaise">Version to raise.</param>
        /// <param name="targetVersion">New version to raise to.</param>
        /// <returns>New version if the target version is greater than the existing version.</returns>
        internal static MetadataEdmSchemaVersion RaiseMetadataEdmSchemaVersion(MetadataEdmSchemaVersion versionToRaise, MetadataEdmSchemaVersion targetVersion)
        {
            return targetVersion > versionToRaise ? targetVersion : versionToRaise;
        }

        /// <summary>
        /// If necessary raises version to the version requested by the user.
        /// </summary>
        /// <param name="versionToRaise">Version to raise.</param>
        /// <param name="targetVersion">New version to raise to.</param>
        /// <returns>New version if the requested version is greater than the existing version.</returns>
        internal static Version RaiseVersion(Version versionToRaise, Version targetVersion)
        {
            return targetVersion > versionToRaise ? targetVersion : versionToRaise;
        }

        /// <summary>
        /// If necessary raises version to the version requested by the user.
        /// </summary>
        /// <param name="versionToRaise">Version to raise.</param>
        /// <param name="targetVersion">New version to raise to.</param>
        /// <returns>New version if the requested version is greater than the existing version.</returns>
        internal static MetadataEdmSchemaVersion RaiseVersion(MetadataEdmSchemaVersion versionToRaise, MetadataEdmSchemaVersion targetVersion)
        {
            return targetVersion > versionToRaise ? targetVersion : versionToRaise;
        }

        /// <summary>
        /// Check the feature version with the max protocol version specified in the server configuration.
        /// </summary>
        /// <param name="featureVersion">feature version that is required to understand the current request.</param>
        /// <param name="maxProtocolVersion">maxProtocolVersion that is specified in the server configuration.</param>
        internal static void CheckMaxProtocolVersion(Version featureVersion, Version maxProtocolVersion)
        {
            if (maxProtocolVersion < featureVersion)
            {
                // incorrect versioning should cause a 500 - internal server error
                throw new InvalidOperationException(Strings.DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion(featureVersion.ToString(), maxProtocolVersion.ToString()));
            }
        }

        /// <summary>
        /// Verify that the specified request version is greater than or equal to the required request version
        /// </summary>
        /// <param name="requiredRequestVersion">Minimum version required to process the request.</param>
        /// <param name="actualRequestVersion">Request version as specified in the request header.</param>
        internal static void CheckRequestVersion(Version requiredRequestVersion, Version actualRequestVersion)
        {
            if (actualRequestVersion < requiredRequestVersion)
            {
                string message = Strings.DataService_DSVTooLow(
                    actualRequestVersion.ToString(2),
                    requiredRequestVersion.Major,
                    requiredRequestVersion.Minor);
                throw DataServiceException.CreateBadRequestError(message);
            }
        }

        /// <summary>
        /// Returns the highest response version possible, which is the min of MPV and RequestMaxDSV
        /// </summary>
        /// <param name="maxProtocolVersion">MPV as defined in the server.</param>
        /// <param name="requestMaxVersion">Request MaxDSV header value.</param>
        /// <returns>the minimum of <paramref name="maxProtocolVersion"/> and <paramref name="requestMaxVersion"/>.</returns>
        internal static Version GetEffectiveMaxResponseVersion(Version maxProtocolVersion, Version requestMaxVersion)
        {
            Debug.Assert(maxProtocolVersion != null, "maxProtocolVersion != null");
            return (requestMaxVersion != null && requestMaxVersion < maxProtocolVersion) ? requestMaxVersion : maxProtocolVersion;
        }

        /// <summary>
        /// Gets the response version for an error payload.
        /// </summary>
        /// <param name="acceptableContentTypes">A comma-separated list of client-supported MIME Accept types.</param>
        /// <param name="requestMaxVersion">The OData-MaxVersion of the request.</param>
        /// <param name="maxProtocolVersion">The max protocol version as specified in the config.</param>
        /// <returns>The response version to be used for an error payload.</returns>
        /// <remarks>
        /// This function is specific to exceptions. For V1 and V2, we will still return 
        /// RequestDescription.DataServiceDefaultResponseVersion. This helps avoid breaking changes. For V3, we return
        /// what is in the request header, provided the version is valid.
        /// </remarks>
        internal static Version GetResponseVersionForError(string acceptableContentTypes, Version requestMaxVersion, Version maxProtocolVersion)
        {
            Debug.Assert(maxProtocolVersion != null, "maxProtocolVersion != null");

            Version responseVersion = DataServiceDefaultResponseVersion;

            Version effectiveMaxResponseVersion = GetEffectiveMaxResponseVersion(maxProtocolVersion, requestMaxVersion);
            if (ContentTypeUtil.IsResponseMediaTypeJsonLight(acceptableContentTypes, false /*entityTarget*/, effectiveMaxResponseVersion))
            {
                Debug.Assert(effectiveMaxResponseVersion >= Version4Dot0, "effectiveMaxResponseVersion should be at least Version3Dot0 to match JSON Light.");
                responseVersion = Version4Dot0;
            }

            return responseVersion;
        }

        /// <summary>
        /// Verify that the request version is a version we know.
        /// </summary>
        /// <param name="requestVersion">request version from the header</param>
        /// <returns>returns true if the request version is known</returns>
        internal static bool IsKnownRequestVersion(Version requestVersion)
        {
            return KnownODataVersions.Contains(requestVersion);
        }

        /// <summary>
        /// Determines the highest known version which is less than or equal to the given version.
        /// </summary>
        /// <param name="version">The version to match.</param>
        /// <returns>The highest known version which is less than or equal to the given version.</returns>
        internal static Version MatchToKnownVersion(Version version)
        {
            return KnownODataVersions.Where(v => v <= version).Max();
        }

        /// <summary>
        /// Goes through all visible types in the provider and determine the metadata version.
        /// </summary>
        /// <param name="provider">Provider wrapper instance.</param>
        /// <param name="metadataVersion">Returns the metadata version.</param>
        /// <param name="edmSchemaVersion">Returns the edm schema version.</param>
        internal static void UpdateMetadataVersion(DataServiceProviderWrapper provider, out Version metadataVersion, out MetadataEdmSchemaVersion edmSchemaVersion)
        {
            Debug.Assert(provider != null, "provider != null");

            metadataVersion = Version4Dot0;
            edmSchemaVersion = MetadataEdmSchemaVersion.Version4Dot0;

            // Metadata versioning should only be impacted by visible types. A resource type is visible only when it is reachable from
            // a resource set with EntitySetRights != 'None' or a service op with ServiceOperationRights != None or it is a complex type
            // made visible through DataServiceConfiguration.EnableAccess().
            foreach (ResourceType resourceType in provider.GetVisibleTypes())
            {
                UpdateMetadataVersionForResourceType(resourceType, ref metadataVersion, ref edmSchemaVersion);
            }

#if DEBUG
            // If there are multiple versions in future, add code here to do version-specific things
            foreach (OperationWrapper so in provider.GetVisibleOperations())
            {
                Debug.Assert(
                    so.ResultKind == ServiceOperationResultKind.DirectValue ||
                    so.ResultKind == ServiceOperationResultKind.Enumeration ||
                    so.ResultKind == ServiceOperationResultKind.QueryWithMultipleResults ||
                    so.ResultKind == ServiceOperationResultKind.QueryWithSingleResult ||
                    so.ResultKind == ServiceOperationResultKind.Void,
                    "Otherwise we have introduced a new value for ServiceOperationResultKind, we might need to update the 'if' statement below.");
            }
#endif
        }

        /// <summary>
        /// Update the various versions based on the metadata of the given resource type
        /// </summary>
        /// <param name="resourceType">resource type whose metadata needs to be looked at.</param>
        /// <param name="metadataVersion">Reference to the metadata version to be updated.</param>
        /// <param name="edmSchemaVersion">Reference to the edm schema version to be updated.</param>
        private static void UpdateMetadataVersionForResourceType(ResourceType resourceType, ref Version metadataVersion, ref MetadataEdmSchemaVersion edmSchemaVersion)
        {
            // Raise the metadata version to the resource type metadata version.
            metadataVersion = RaiseVersion(metadataVersion, resourceType.MetadataVersion);

            // Raise the schema version to the resource type schema version.
            edmSchemaVersion = RaiseVersion(edmSchemaVersion, resourceType.SchemaVersion);
        }
    }
}
