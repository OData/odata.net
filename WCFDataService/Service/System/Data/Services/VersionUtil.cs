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
    using System;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A set of utility methods for dealing with versions and versioning.
    /// </summary>
    internal static class VersionUtil
    {
        /// <summary>Version 1.0.</summary>
        internal static readonly Version Version1Dot0 = new Version(1, 0);

        /// <summary>Version 2.0.</summary>
        internal static readonly Version Version2Dot0 = new Version(2, 0);

        /// <summary>Version 3.0.</summary>
        internal static readonly Version Version3Dot0 = new Version(3, 0);

        /// <summary>
        /// Default set of known data service versions, currently 1.0, 2.0 and 3.0
        /// </summary>
        internal static readonly Version[] KnownDataServiceVersions = new Version[] { Version1Dot0, Version2Dot0, Version3Dot0 };

        /// <summary>
        /// The default response version of the data service. If no version is set for a particular response
        /// The DataService will respond with this version (1.0)
        /// </summary>
        internal static readonly Version DataServiceDefaultResponseVersion = Version1Dot0;

        /// <summary>Given a <see cref="DataServiceProtocolVersion"/> enumeration returns the <see cref="Version"/> instance with the same version number.</summary>
        /// <param name="protocolVersion">The protocol version enum value to convert.</param>
        /// <returns>The version instance with the version number for the specified protocol version.</returns>
        internal static Version ToVersion(this DataServiceProtocolVersion protocolVersion)
        {
            switch (protocolVersion)
            {
                case DataServiceProtocolVersion.V1:
                    return Version1Dot0;
                case DataServiceProtocolVersion.V2:
                    return Version2Dot0;
                default:
                    Debug.Assert(protocolVersion == DataServiceProtocolVersion.V3, "Did you add a new version?");
                    return Version3Dot0;
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
        /// <param name="requestMinVersion">The MinDataServiceVersion of the request.</param>
        /// <param name="requestMaxVersion">The MaxDataServiceVersion of the request.</param>
        /// <param name="maxProtocolVersion">The max protocol version as specified in the config.</param>
        /// <returns>The response version to be used for an error payload.</returns>
        /// <remarks>
        /// This function is specific to exceptions. For V1 and V2, if the request header has a MinDSV, we will still return 
        /// RequestDescription.DataServiceDefaultResponseVersion. This helps avoid breaking changes. For V3, we return
        /// what is in the request header, provided the version is valid.
        /// </remarks>
        internal static Version GetResponseVersionForError(string acceptableContentTypes, Version requestMinVersion, Version requestMaxVersion, Version maxProtocolVersion)
        {
            Debug.Assert(requestMinVersion != null, "requestMinVersion != null");
            Debug.Assert(maxProtocolVersion != null, "maxProtocolVersion != null");

            Version responseVersion = DataServiceDefaultResponseVersion;

            // To avoid breaking changes, if MPV is less than 3.0, just return the default response version, i.e. 1.0.
            if (maxProtocolVersion >= Version3Dot0)
            {
                // If response content type is JSON light, then the response version needs to be 3.0.
                Version effectiveMaxResponseVersion = GetEffectiveMaxResponseVersion(maxProtocolVersion, requestMaxVersion);
                if (ContentTypeUtil.IsResponseMediaTypeJsonLight(acceptableContentTypes, false /*entityTarget*/, effectiveMaxResponseVersion))
                {
                    Debug.Assert(effectiveMaxResponseVersion >= Version3Dot0, "effectiveMaxResponseVersion should be at least Version3Dot0 to match JSON Light.");
                    responseVersion = Version3Dot0;
                }

                // If the MinDSV header is specified, the response version needs to be at least MinDSV.
                if (requestMinVersion > responseVersion && IsKnownRequestVersion(requestMinVersion))
                {
                    responseVersion = requestMinVersion;
                }
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
            return KnownDataServiceVersions.Contains(requestVersion);
        }

        /// <summary>
        /// Determines the highest known version which is less than or equal to the given version.
        /// </summary>
        /// <param name="version">The version to match.</param>
        /// <returns>The highest known version which is less than or equal to the given version.</returns>
        internal static Version MatchToKnownVersion(Version version)
        {
            return KnownDataServiceVersions.Where(v => v <= version).Max();
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

            metadataVersion = Version1Dot0;
            edmSchemaVersion = MetadataEdmSchemaVersion.Version1Dot0;

            //// Copied the following appendixes from the MC-CSDL documentation as reference here.
            ////
            //// 6 Appendix B: Differences Between CSDL Version 1.0 and CSDL Version 1.1
            //// CSDL version 1.1 is a superset of CSDL version 1.0.
            //// This section outlines the differences between CSDL version 1.0 and CSDL version 1.1.
            //// For CSDL version 1.0, the following rules apply.
            ////   ComplexType MUST NOT define an Abstract attribute.
            ////   ComplexType MUST NOT define a BaseType attribute.
            ////   ReturnType for a <FunctionImport> MUST be a Collection.
            ////   ReturnType for a <FunctionImport> MUST NOT be a Collection of ComplexType.
            ////   <Property> MUST NOT define a CollectionKind attribute.
            ////   <Property> of type ComplexType MUST NOT be Nullable.
            ////
            //// 7 Appendix C: Differences Between CSDL Version 1.1 and CSDL Version 1.2
            //// CSDL version 1.2 is a superset of CSDL version 1.1. 
            //// This section outlines the differences between CSDL version 1.1 and CSDL version 1.2.
            //// For CSDL version 1.1, the following rules apply:
            ////   <EntityType> MUST NOT define an <OpenType> attribute.
            ////
            //// 8 Appendix D: Differences Between CSDL Version 1.2 and CSDL Version 2.0
            //// CSDL version 2.0 is a superset of CSDL version 1.2. 
            //// This section outlines the differences between CSDL version 1.2 and CSDL version 2.0.
            //// For CSDL version 1.2, the following rules apply:
            ////   <Schema> MUST NOT contain any <Function> sub-elements
            ////   Entity <Key> MUST NOT define any <AnnotationElement> elements
            ////   In CSDL 1.2 and lower versions binary data type is not supported for defining <Key>.
            ////   Entity <PropertyRef> MUST NOT define any <AnnotationElement> elements
            ////   <ReferentialConstraints>, <Role> MUST NOT define any <AnnotationElement> elements
            ////   <EntityContainer> MUST NOT define any <AnnotationElement> elements
            ////   <FunctionImport> MUST NOT define any <AnnotationElement> elements
            ////   <ReferentialConstraint> MUST only exist between the key properties of associated entities

            if (!provider.HasReflectionOrEFProviderQueryBehavior)
            {
                // Always v1.1+ schemas for custom providers -- this is the shipped behavior for AstoriaV2.
                edmSchemaVersion = RaiseMetadataEdmSchemaVersion(edmSchemaVersion, MetadataEdmSchemaVersion.Version1Dot1);
            }

            // Metadata versioning should only be impacted by visible types. A resource type is visible only when it is reachable from
            // a resource set with EntitySetRights != 'None' or a service op with ServiceOperationRights != None or it is a complex type
            // made visible through DataServiceConfiguration.EnableAccess().
            foreach (ResourceType resourceType in provider.GetVisibleTypes())
            {
                UpdateMetadataVersionForResourceType(resourceType, ref metadataVersion, ref edmSchemaVersion);
            }

            // If the metadata has annotations, the Edm schema version has to be at least v3
            if (provider.Configuration.HasAnnotations())
            {
                edmSchemaVersion = RaiseMetadataEdmSchemaVersion(edmSchemaVersion, MetadataEdmSchemaVersion.Version3Dot0);
                metadataVersion = RaiseVersion(metadataVersion, Version3Dot0);
            }
            
            foreach (OperationWrapper so in provider.GetVisibleOperations())
            {
                Debug.Assert(
                    so.ResultKind == ServiceOperationResultKind.DirectValue ||
                    so.ResultKind == ServiceOperationResultKind.Enumeration ||
                    so.ResultKind == ServiceOperationResultKind.QueryWithMultipleResults ||
                    so.ResultKind == ServiceOperationResultKind.QueryWithSingleResult ||
                    so.ResultKind == ServiceOperationResultKind.Void,
                    "Otherwise we have introduced a new value for ServiceOperationResultKind, we might need to update the 'if' statement below.");

                if (so.Kind == OperationKind.Action)
                {
                    edmSchemaVersion = RaiseMetadataEdmSchemaVersion(edmSchemaVersion, MetadataEdmSchemaVersion.Version3Dot0);
                    metadataVersion = RaiseVersion(metadataVersion, Version3Dot0);
                }

                if (so.ResultKind == ServiceOperationResultKind.Void /*Non-collection return type requires v1.1*/ ||
                    so.ResultKind == ServiceOperationResultKind.QueryWithSingleResult /*Non-collection return type requires v1.1*/ ||
                    so.ResultKind == ServiceOperationResultKind.DirectValue /*Non-collection return type requires v1.1*/ ||
                    so.ResultType.ResourceTypeKind == ResourceTypeKind.ComplexType /*Collection of ComplexType requires v1.1*/ ||
                    so.ResultType.ResourceTypeKind == ResourceTypeKind.Primitive /*Collection of Primitives does NOT require v1.1, but I'm leaving the shipped behavior as it is here*/)
                {
                    edmSchemaVersion = RaiseMetadataEdmSchemaVersion(edmSchemaVersion, MetadataEdmSchemaVersion.Version1Dot1);
                }
            }
        }

        /// <summary>
        /// Update the various versions based on the metadata of the given resource type
        /// </summary>
        /// <param name="resourceType">resource type whose metadata needs to be looked at.</param>
        /// <param name="metadataVersion">Reference to the metadata version to be updated.</param>
        /// <param name="edmSchemaVersion">Reference to the edm schema version to be updated.</param>
        private static void UpdateMetadataVersionForResourceType(ResourceType resourceType, ref Version metadataVersion, ref MetadataEdmSchemaVersion edmSchemaVersion)
        {
            //// Note that we never write the Abstract and BaseType attributes for complex type in our edmx.
            //// If that changes in the future, we need to bump the edm schema version to v1.1 here.

            if (resourceType.IsOpenType)
            {
                // Open types force schema version to be 1.2.
                edmSchemaVersion = RaiseMetadataEdmSchemaVersion(edmSchemaVersion, MetadataEdmSchemaVersion.Version1Dot2);
            }

            if (resourceType.EpmMinimumDataServiceProtocolVersion.ToVersion() > metadataVersion)
            {
                metadataVersion = RaiseVersion(metadataVersion, resourceType.EpmMinimumDataServiceProtocolVersion.ToVersion());
            }

            // Raise the metadata version to the resource type metadata version.
            metadataVersion = RaiseVersion(metadataVersion, resourceType.MetadataVersion);

            // Raise the schema version to the resource type schema version.
            edmSchemaVersion = RaiseVersion(edmSchemaVersion, resourceType.SchemaVersion);
        }
    }
}
