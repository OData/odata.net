//---------------------------------------------------------------------
// <copyright file="VersionHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Class has some common Version Calculation logic and helper methods
    /// </summary>
    public static class VersionHelper
    {
        public const DataServiceProtocolVersion LatestProtocolVersion = DataServiceProtocolVersion.V4;
        private const string LatestVersionPlusOne = "4.0";

        private static readonly DataServiceProtocolVersion[] allowedVersions = new DataServiceProtocolVersion[] { DataServiceProtocolVersion.V4 };
        private static readonly IDictionary<Func<DataType, bool>, DataServiceProtocolVersion> dataTypeVersionMap = new Dictionary<Func<DataType, bool>, DataServiceProtocolVersion>()
        {
            { t => t is StreamDataType, DataServiceProtocolVersion.V4 },
            { t => t is CollectionDataType, DataServiceProtocolVersion.V4 },
            { t => t is SpatialDataType, DataServiceProtocolVersion.V4 },
        };

        /// <summary>
        /// Calculates the min entityPropertyMapping Version
        /// </summary>
        /// <param name="entitySet">Entity Set to get the min value from</param>
        /// <param name="calculationType">Calculation Type</param>
        /// <param name="contentType">Content Type of the request</param>
        /// <param name="maxProtocolVersion">Max Protocol Version</param>
        /// <param name="maxDataServiceVersion">Max Data Service Version</param>
        /// <returns>Min version</returns>
        public static DataServiceProtocolVersion CalculateMinEntityPropertyMappingVersion(this EntitySet entitySet, VersionCalculationType calculationType, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            IEnumerable<EntityType> entityTypes = VersionHelper.GetEntityTypes(entitySet);
            return VersionHelper.GetMaximumVersion(entityTypes.Select(et => et.CalculateEntityPropertyMappingProtocolVersion(calculationType, contentType, maxProtocolVersion, maxDataServiceVersion)).ToArray());
        }

        /// <summary>
        /// Calculates the DataServiceProtocolVersion of an EntityType by accounting for EPM attributes
        /// </summary>
        /// <param name="entityType">Entity Type to investigate</param>
        /// <param name="calculationType">Calculation type to use, response, request, or metadata</param>
        /// <param name="contentType">Content type determines whether to apply EPM versioning</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Protocol Version</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "maxDataServiceVersion", Justification = "Keeping in case we need the max data service version in the future.")]
        public static DataServiceProtocolVersion CalculateEntityPropertyMappingProtocolVersion(this EntityType entityType, VersionCalculationType calculationType, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtilities.Assert(calculationType == VersionCalculationType.Metadata || maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified for non-metadata cases");

            return DataServiceProtocolVersion.V4;
        }

        /// <summary>
        /// Returns the effective protocol version based on the headers and max protocol version
        /// </summary>
        /// <param name="headers">The headers</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <returns>The effective protocol version</returns>
        public static DataServiceProtocolVersion GetEffectiveProtocolVersion(IDictionary<string, string> headers, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            var requestVersion = GetDataServiceVersion(headers);
            if (requestVersion != DataServiceProtocolVersion.Unspecified)
            {
                return requestVersion;
            }

            string maxDataServiceVersion = null;
            if (headers.ContainsKey(HttpHeaders.MaxDataServiceVersion))
            {
                maxDataServiceVersion = headers[HttpHeaders.MaxDataServiceVersion];
            }

            return CalculateDataServiceVersionIfNotSpecified(maxProtocolVersion, VersionHelper.ConvertToDataServiceProtocolVersion(maxDataServiceVersion));
        }

        /// <summary>
        /// Calculates the Data Service Version's effective version if its not specified based on the mpv and mdsv
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="maxDataServiceVersion">The max data service version</param>
        /// <returns>Effective version</returns>
        public static DataServiceProtocolVersion CalculateDataServiceVersionIfNotSpecified(DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            if (maxDataServiceVersion != DataServiceProtocolVersion.Unspecified && maxDataServiceVersion >= DataServiceProtocolVersion.V4 && maxProtocolVersion >= DataServiceProtocolVersion.V4)
            {
                return VersionHelper.GetMinimumVersion(maxProtocolVersion, maxDataServiceVersion);
            }
            else if (maxDataServiceVersion == DataServiceProtocolVersion.Unspecified)
            {
                return maxProtocolVersion;
            }
            else
            {
                return VersionHelper.GetMinimumVersion(maxProtocolVersion, DataServiceProtocolVersion.V4);
            }
        }

        /// <summary>
        /// Returns the new version if it is greater than the current version and the given condition is true
        /// </summary>
        /// <param name="condition">The condition</param>
        /// <param name="currentVersion">The current version</param>
        /// <param name="newVersion">The new version</param>
        /// <returns>The new version if it is greater than the current version and the condition is true</returns>
        public static DataServiceProtocolVersion IncreaseVersionIfTrue(bool condition, DataServiceProtocolVersion currentVersion, DataServiceProtocolVersion newVersion)
        {
            if (condition)
            {
                return IncreaseVersionIfRequired(currentVersion, newVersion);
            }

            return currentVersion;
        }

        /// <summary>
        /// Increases the ProtocolVersion if the new one provided is greater than the old one.
        /// </summary>
        /// <param name="currentVersion">Current Version</param>
        /// <param name="newVersion">New Version</param>
        /// <returns>The largest version</returns>
        public static DataServiceProtocolVersion IncreaseVersionIfRequired(this DataServiceProtocolVersion currentVersion, DataServiceProtocolVersion newVersion)
        {
            return GetMaximumVersion(currentVersion, newVersion);
        }

        /// <summary>
        /// Finds the minumum version from those given. Will NOT consider 'Unspecified' to be a minimal value.
        /// </summary>
        /// <param name="versions">The versions to get the min from</param>
        /// <returns>The minimum specified version</returns>
        public static DataServiceProtocolVersion GetMinimumVersion(params DataServiceProtocolVersion[] versions)
        {
            ExceptionUtilities.CheckCollectionNotEmpty(versions, "versions");

            DataServiceProtocolVersion min = GetMaximumVersion(versions); // doing this to handle Unspecified safely
            foreach (var version in versions)
            {
                if (version != DataServiceProtocolVersion.Unspecified && version < min)
                {
                    min = version;
                }
            }

            return min;
        }

        /// <summary>
        /// Finds the maximum version from those given
        /// </summary>
        /// <param name="versions">The versions to get the max from</param>
        /// <returns>The maximum version of those given</returns>
        public static DataServiceProtocolVersion GetMaximumVersion(params DataServiceProtocolVersion[] versions)
        {
            ExceptionUtilities.CheckCollectionNotEmpty(versions, "versions");

            DataServiceProtocolVersion max = DataServiceProtocolVersion.Unspecified;
            foreach (var version in versions)
            {
                if (max == DataServiceProtocolVersion.Unspecified || version > max)
                {
                    max = version;
                }
            }

            return max;
        }

        /// <summary>
        /// Gets the value of the 'DataServiceVersion' header for the given headers
        /// </summary>
        /// <param name="headers">The headers</param>
        /// <returns>The value of the 'DataServiceVersion' header from the request</returns>
        public static DataServiceProtocolVersion GetDataServiceVersion(IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            return GetProtocolVersionFromHeader(headers, HttpHeaders.DataServiceVersion);
        }

        /// <summary>
        /// Gets the value of the 'MaxDataServiceVersion' header for the given headers
        /// </summary>
        /// <param name="headers">The headers</param>
        /// <returns>The value of the 'MaxDataServiceVersion' header from the request</returns>
        public static DataServiceProtocolVersion GetMaxDataServiceVersion(IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            return GetProtocolVersionFromHeader(headers, HttpHeaders.MaxDataServiceVersion);
        }

        /// <summary>
        /// Returns the effective protocol version based on the headers and max protocol version
        /// </summary>
        /// <param name="dataServiceProtocolVersion">The DataServiceProtocolVersion to convert to a valid header</param>
        /// <returns>The effective protocol version</returns>
        public static string ConvertToHeaderFormat(this DataServiceProtocolVersion dataServiceProtocolVersion)
        {
            if (dataServiceProtocolVersion == DataServiceProtocolVersion.LatestVersionPlusOne)
            {
                return LatestVersionPlusOne;
            }
            else if (dataServiceProtocolVersion == DataServiceProtocolVersion.Unspecified)
            {
                return null;
            }
            else
            {
                return dataServiceProtocolVersion.ToString().Replace("V", string.Empty) + ".0";
            }
        }

        /// <summary>
        /// Returns the effective protocol version based on the headers and max protocol version
        /// </summary>
        /// <param name="dataServiceProtocolVersion">The DataServiceProtocolVersion to convert to a valid header</param>
        /// <returns>The effective protocol version</returns>
        public static string ConvertToIntegerFormat(this DataServiceProtocolVersion dataServiceProtocolVersion)
        {
            string tempString = dataServiceProtocolVersion.ToString();
            if (dataServiceProtocolVersion == DataServiceProtocolVersion.LatestVersionPlusOne)
            {
                tempString = "V4";
            }
            else if (dataServiceProtocolVersion == DataServiceProtocolVersion.Unspecified)
            {
                return null;
            }

            return tempString.ToString().Replace("V", string.Empty);
        }

        /// <summary>
        /// Returns the effective protocol version based on the headers and max protocol version
        /// </summary>
        /// <param name="dataServiceVersion">The DataServiceProtocolVersion to convert to a valid header, parameter is allowed to be null</param>
        /// <returns>The effective protocol version</returns>
        public static DataServiceProtocolVersion ConvertToDataServiceProtocolVersion(string dataServiceVersion)
        {
            if (dataServiceVersion != null)
            {
                if (dataServiceVersion == LatestVersionPlusOne)
                {
                    return DataServiceProtocolVersion.LatestVersionPlusOne;
                }
                else
                {
                    string tempVersion = dataServiceVersion.Replace(".0", string.Empty).Insert(0, "V");
                    if (tempVersion.Contains(';'))
                    {
                        tempVersion = tempVersion.Substring(0, tempVersion.IndexOf(';'));
                    }

                    return (DataServiceProtocolVersion)Enum.Parse(typeof(DataServiceProtocolVersion), tempVersion, false);
                }
            }

            return DataServiceProtocolVersion.Unspecified;
        }

        /// <summary>
        /// Gets the allowed versions of the DataServiceProtocol
        /// </summary>
        /// <returns>Returns the list of allowed versions</returns>
        public static ReadOnlyCollection<DataServiceProtocolVersion> GetAllowedDataServiceVersions()
        {
            return new ReadOnlyCollection<DataServiceProtocolVersion>(new List<DataServiceProtocolVersion>(allowedVersions));
        }

        /// <summary>
        /// Calculates the protocol version of the EntitySet
        /// </summary>
        /// <param name="entitySet">EntitySet to determine the version of</param>
        /// <param name="contentType">Content Type of the request being made</param>
        /// <param name="calculationType">Calculation type to use, response, request, or metadata</param>
        /// <param name="maxProtocolVersion">max Protocol version of the service</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Data Service Protocol version of the EntitySet</returns>
        public static DataServiceProtocolVersion CalculateEntitySetProtocolVersion(this EntitySet entitySet, string contentType, VersionCalculationType calculationType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");

            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;
            List<EntityType> entityTypes = entitySet.Container.Model.EntityTypes.Where(et => et.GetRootType() == entitySet.EntityType).ToList();
            foreach (EntityType entityType in entityTypes)
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, entityType.CalculateProtocolVersion(contentType, calculationType, maxProtocolVersion, maxDataServiceVersion));
            }

            return expectedVersion;
        }

        /// <summary>
        /// Calculates the data service protocol version of the EntityType
        /// </summary>
        /// <param name="entityType">EntityType to determine version of</param>
        /// <param name="contentType">The content type of the request/response</param>
        /// <param name="calculationType">The type of version calculation</param>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        /// <param name="maxDataServiceVersion">The max data service version header value</param>
        /// <returns>Version of EntityType</returns>
        internal static DataServiceProtocolVersion CalculateProtocolVersion(this EntityType entityType, string contentType, VersionCalculationType calculationType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");

            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;

            // for requests, the type does not actually cause a version bump. Only what is actually used in the uri or payload will cause it.
            if (calculationType == VersionCalculationType.Request)
            {
                return expectedVersion;
            }

            // multivalue, stream, spatial and other data-type versioning is handled this way
            var versionFromProperties = DataTypes.EntityType.WithDefinition(entityType).CalculateDataTypeVersion();
            expectedVersion = expectedVersion.IncreaseVersionIfRequired(versionFromProperties);
            
            // If there are relationships and includeRels = true then at least v3
            if (entityType.AllNavigationProperties.Any())
            {
                bool? includeAssociationLinks = null;
                var dataServiceBehavior = entityType.Model.GetDefaultEntityContainer().GetDataServiceBehavior();
                if (dataServiceBehavior != null)
                {
                    includeAssociationLinks = dataServiceBehavior.IncludeAssociationLinksInResponse;
                }

                if (includeAssociationLinks == true)
                {
                    expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
                }   
            }

            // Calculate expected version based on type's feed mappings.
            expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.CalculateEntityPropertyMappingProtocolVersion(entityType, calculationType, contentType, maxProtocolVersion, maxDataServiceVersion));

            // open types get aggressively versioned due to potentially higher-version property types being returned
            if (entityType.IsOpen)
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, VersionHelper.GetMinimumVersion(maxProtocolVersion, maxDataServiceVersion));
            }

            return expectedVersion;
        }

        /// <summary>
        /// Calculate Data service protocol version based on property type
        /// </summary>
        /// <param name="memberProperty">Member property</param>
        /// <returns>Data Service Protocol version</returns>
        internal static DataServiceProtocolVersion CalculateProtocolVersion(this MemberProperty memberProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");
            return memberProperty.PropertyType.CalculateDataTypeVersion();
        }

        internal static DataServiceProtocolVersion CalculateUriResponseMinProtocolVersion(ODataUri requestUri, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;

            if (requestUri.IncludesInlineCountAllPages())
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }

            if (requestUri.IsCount())
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }
            else if (requestUri.IsProperty())
            {
                var propertySegment = requestUri.LastSegment as PropertySegment;
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(propertySegment.Property.CalculateProtocolVersion());
            }
            else if (!requestUri.IsNamedStream())
            {
                // Check service operations returning non-entity types
                if (requestUri.IsServiceOperation())
                {
                    var serviceOpSegment = requestUri.Segments.OfType<FunctionSegment>().Single();
                    DataType returnType = serviceOpSegment.Function.ReturnType;

                    // Service ops returning complex values or bags of complex values, where the complex type has bag properties, are v3
                    var collectionType = returnType as CollectionDataType;
                    if (collectionType != null)
                    {
                        // for service operations, the fact that a collection is returned does not mean it is a multivalue
                        // so we unwrap the element type before calculating the version
                        returnType = collectionType.ElementDataType;
                    }

                    expectedVersion = expectedVersion.IncreaseVersionIfRequired(returnType.CalculateDataTypeVersion());
                }

                // Check entity types
                List<DataServiceProtocolVersion> dataServiceProtocolVersions = new List<DataServiceProtocolVersion>();
                dataServiceProtocolVersions.Add(expectedVersion);

                if (!(requestUri.IsEntityReferenceLink() || requestUri.IsPropertyValue()))
                {
                    foreach (EntitySet entitySet in requestUri.GetAllEntitySetsIncludingExpands())
                    {
                        DataServiceProtocolVersion entitySetVersion = entitySet.CalculateEntitySetProtocolVersion(contentType, VersionCalculationType.Response, maxProtocolVersion, maxDataServiceVersion);
                        dataServiceProtocolVersions.Add(entitySetVersion);
                        if (entitySetVersion > maxDataServiceVersion)
                        {
                            return VersionHelper.GetMaximumVersion(dataServiceProtocolVersions.ToArray());
                        }
                    }
                }
            }

            return expectedVersion;
        }

        internal static DataServiceProtocolVersion CalculateUriMinRequestProtocolVersion(ODataUri requestUri, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            DataServiceProtocolVersion expectedVersion = DataServiceProtocolVersion.V4;

            string inlineCount;
            if (requestUri.TryGetInlineCountValue(out inlineCount))
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }

            if (requestUri.SelectSegments.Count > 0)
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }

            if (requestUri.HasAnyOrAllInFilter())
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }

            if (requestUri.IsCount())
            {
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            }
            else if (requestUri.IsProperty())
            {
                var propertySegment = requestUri.LastSegment as PropertySegment;
                expectedVersion = expectedVersion.IncreaseVersionIfRequired(propertySegment.Property.CalculateProtocolVersion());
            }
            else
            {
                EntitySet expectedEntitySet = null;
                if (requestUri.TryGetExpectedEntitySet(out expectedEntitySet))
                {
                    List<DataServiceProtocolVersion> dataServiceProtocolVersions = GetEntityTypes(expectedEntitySet).Select(et => et.CalculateEntityPropertyMappingProtocolVersion(VersionCalculationType.Request, contentType, maxProtocolVersion, maxDataServiceVersion)).ToList();
                    dataServiceProtocolVersions.Add(expectedVersion);
                    expectedVersion = VersionHelper.GetMaximumVersion(dataServiceProtocolVersions.ToArray());
                }
            }

            return expectedVersion;
        }

        /// <summary>
        /// This function is used for getting all the types involved in EPM with the entitySet
        /// </summary>
        /// <param name="entitySet">Entity Set to investigate</param>
        /// <returns>List of EntityTypes that need to be reviewed for versioning as it relates to EPM</returns>
        internal static List<EntityType> GetEntityTypes(EntitySet entitySet)
        {
            return entitySet.Container.Model.EntityTypes.Where(et => et.GetRootType().IsKindOf(entitySet.EntityType)).ToList();
        }

        /// <summary>
        /// Returns whether or not the uri contains a type segment that actually filters the result type
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>Returns true if uri contains any EntityTypeSegment that is not of the same type as its preceding segment type</returns>
        internal static bool HasSignificantTypeSegmentInPath(this ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            for (int i = 0; i < uri.Segments.Count; i++)
            {
                var segment = uri.Segments[i];
                if (segment.SegmentType == ODataUriSegmentType.EntityType)
                {
                    // Edit links are never version bumped because a down-level client may send them back to the service
                    if (i > 0 && uri.Segments[i - 1].SegmentType == ODataUriSegmentType.Key)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the uri contains a type segment in either $expand or $select
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri contains a type segment in either $expand or $select</returns>
        internal static bool HasTypeSegmentInExpandOrSelect(this ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            return uri.SelectSegments.Concat(uri.ExpandSegments).Any(c => c.OfType<EntityTypeSegment>().Any());
        }

        /// <summary>
        /// Returns whether or not the uri has any/all operators in its filter
        /// </summary>
        /// <param name="uri">The uri to extend</param>
        /// <returns>whether or not the uri has any/all</returns>
        internal static bool HasAnyOrAllInFilter(this ODataUri uri)
        {
            if (uri.Filter == null)
            {
                return false;
            }

            var operators = new string[] { ODataConstants.AllUriOperator, ODataConstants.AnyUriOperator };
            return operators.Any(o => uri.Filter.Contains("/" + o + "("));
        }

        /// <summary>
        /// Calculates the data type version.
        /// </summary>
        /// <param name="rootDataType">The root data type.</param>
        /// <returns>The version for the data type</returns>
        internal static DataServiceProtocolVersion CalculateDataTypeVersion(this DataType rootDataType)
        {
            var version = DataServiceProtocolVersion.V4;
            foreach (var dataType in GatherDataTypes(rootDataType).Distinct())
            {
                foreach (var mapping in dataTypeVersionMap)
                {
                    if (mapping.Key(dataType))
                    {
                        version = version.IncreaseVersionIfRequired(mapping.Value);
                    }
                }
            }

            return version;
        }

        /// <summary>
        /// Gather data types in a data type
        /// </summary>
        /// <param name="root">root type to gather types from</param>
        /// <returns>list of data types</returns>
        internal static IEnumerable<DataType> GatherDataTypes(DataType root)
        {
            return new DataTypeGatheringVisitor().Gather(root);
        }

        /// <summary>
        /// Gets the value of the given header from the given headers and converts it into a DataServiceProtocolVersion
        /// </summary>
        /// <param name="headers">The headers</param>
        /// <param name="header">The header to convert</param>
        /// <returns>The converted value of the header from the request</returns>
        private static DataServiceProtocolVersion GetProtocolVersionFromHeader(IDictionary<string, string> headers, string header)
        {
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            string version;
            if (!headers.TryGetValue(header, out version) || string.IsNullOrEmpty(version))
            {
                return DataServiceProtocolVersion.Unspecified;
            }

            string stringValue = "V" + version.Substring(0, 1);
            if (!Enum.IsDefined(typeof(DataServiceProtocolVersion), stringValue))
            {
                return DataServiceProtocolVersion.Unspecified;
            }

            return (DataServiceProtocolVersion)Enum.Parse(typeof(DataServiceProtocolVersion), stringValue, false);
        }

        /// <summary>
        /// Visitor which gathers all data types recursively.
        /// </summary>
        private class DataTypeGatheringVisitor : IDataTypeVisitor<IEnumerable<DataType>>
        {
            /// <summary>
            /// Gathers the related data types. This includes collection element types and sub-property types. Does not follow navigations.
            /// </summary>
            /// <param name="dataType">The type to start from.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Gather(DataType dataType)
            {
                ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");
                return dataType.Accept(this);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type to visit.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Visit(CollectionDataType dataType)
            {
                return this.Gather(dataType.ElementDataType).Concat(dataType);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type to visit.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Visit(ComplexDataType dataType)
            {
                return this.VisitProperties(dataType.Definition.AllProperties).Concat(dataType);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type to visit.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Visit(EntityDataType dataType)
            {
                var properties = dataType.Definition.AllProperties;
                if (dataType.Definition.IsOpen)
                {
                    properties = properties.Where(p => p.IsMetadataDeclaredProperty());
                }

                return this.VisitProperties(properties).Concat(dataType);
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type to visit.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Visit(PrimitiveDataType dataType)
            {
                yield return dataType;
            }

            /// <summary>
            /// Visits the specified data type.
            /// </summary>
            /// <param name="dataType">The data type to visit.</param>
            /// <returns>The data types</returns>
            public IEnumerable<DataType> Visit(ReferenceDataType dataType)
            {
                throw new TaupoNotSupportedException("Not supported");
            }

            private IEnumerable<DataType> VisitProperties(IEnumerable<MemberProperty> properties)
            {
                return properties.SelectMany(p => this.Gather(p.PropertyType));
            }
        }
    }
}
