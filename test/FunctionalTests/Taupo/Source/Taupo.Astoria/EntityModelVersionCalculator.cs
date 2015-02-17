//---------------------------------------------------------------------
// <copyright file="EntityModelVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a class for calculating the Version
    /// </summary>
    [ImplementationName(typeof(IEntityModelVersionCalculator), "Default")]
    public class EntityModelVersionCalculator : IEntityModelVersionCalculator
    {
        /// <summary>
        /// Calculates the Max Protocol version based on the schema
        /// </summary>
        /// <param name="entityModelSchema">EntityModel Schema</param>
        /// <returns>A Protocol Version</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Conditions expanded for readability.")]
        public DataServiceProtocolVersion CalculateProtocolVersion(EntityModelSchema entityModelSchema)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityModelSchema, "entityModelSchema");

            DataServiceProtocolVersion expectedDataServiceVersion = DataServiceProtocolVersion.V4;

            foreach (EntityType t in entityModelSchema.EntityTypes)
            {
                var entityTypeVersion = VersionHelper.CalculateDataTypeVersion(DataTypes.EntityType.WithDefinition(t));
                expectedDataServiceVersion = VersionHelper.IncreaseVersionIfRequired(expectedDataServiceVersion, entityTypeVersion);
                expectedDataServiceVersion = VersionHelper.IncreaseVersionIfRequired(expectedDataServiceVersion, VersionHelper.CalculateEntityPropertyMappingProtocolVersion(t, VersionCalculationType.Metadata, MimeTypes.ApplicationXml, VersionHelper.LatestProtocolVersion, VersionHelper.LatestProtocolVersion));
            }

            if (entityModelSchema.Functions.Any(f => f.IsAction()))
            {
                expectedDataServiceVersion = VersionHelper.IncreaseVersionIfRequired(expectedDataServiceVersion, DataServiceProtocolVersion.V4);
            }

            return expectedDataServiceVersion;
        }
    }
}