//   Copyright 2011 Microsoft Corporation
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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Data.OData.Atom;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces.

    /// <summary>
    /// Utility class to check feature availability in a certain version of OData.
    /// </summary>
    internal static class ODataVersionChecker
    {
        /// <summary>
        /// Check whether the inline count feature is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        internal static void CheckInlineCount(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            if (version < ODataVersion.V2)
            {
                throw new ODataException(Strings.ODataVersionChecker_InlineCountNotSupported(version.VersionString()));
            }
        }

        /// <summary>
        /// Check whether the inline count feature is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <param name="propertyName">The name of the property which holds the multivalue.</param>
        internal static void CheckMultiValueProperties(ODataVersion version, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            if (version < ODataVersion.V3)
            {
                throw new ODataException(Strings.ODataVersionChecker_MultiValuePropertiesNotSupported(propertyName, version.VersionString()));
            }
        }

        /// <summary>
        /// Check whether the server paging feature is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        internal static void CheckServerPaging(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            if (version < ODataVersion.V2)
            {
                throw new ODataException(Strings.ODataVersionChecker_ServerPagingNotSupported(version.VersionString()));
            }
        }

        /// <summary>
        /// Check whether the named streams feature is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        internal static void CheckNamedStreams(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            if (version < ODataVersion.V3)
            {
                throw new ODataException(Strings.ODataVersionChecker_NamedStreamsNotSupported(version.VersionString()));
            }
        }

        /// <summary>
        /// Check whether the association links feature is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        internal static void CheckAssociationLinks(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            if (version < ODataVersion.V3)
            {
                throw new ODataException(Strings.ODataVersionChecker_AssociationLinksNotSupported(version.VersionString()));
            }
        }

        /// <summary>
        /// Check whether the EPM on the specified resource type is supported in the specified version.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <param name="resourceType">The entity resoure type to check.</param>
        internal static void CheckEntityPropertyMapping(ODataVersion version, ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");

            EpmResourceTypeAnnotation epmAnnotation = resourceType.Epm();
            if (epmAnnotation != null)
            {
                Debug.Assert(epmAnnotation.EpmTargetTree != null, "If the EPM annotation is present the EPM tree must already be initialized.");
                if (version < epmAnnotation.EpmTargetTree.MinimumODataProtocolVersion)
                {
                    throw new ODataException(
                        Strings.ODataVersionChecker_EpmVersionNotSupported(
                            resourceType.FullName, 
                            epmAnnotation.EpmTargetTree.MinimumODataProtocolVersion.VersionString(),
                            version.VersionString()));
                }
            }
        }

        /// <summary>
        /// Checks that the version specified on the request or the response is supported by this library.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <remarks>In internal drops we currently do not support protocol version 3.</remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "version", Justification = "Parameter is used if #INTERNAL_DROP is set.")]
        internal static void CheckVersionSupported(ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

#if INTERNAL_DROP || DISABLE_V3
            if (version >= ODataVersion.V3)
            {
                throw new ODataException(Strings.ODataVersionChecker_ProtocolVersion3IsNotSupported);
            }
#endif
        }
    }
}
