//---------------------------------------------------------------------
// <copyright file="EntityDescriptorVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for Calculating the DataServiceVersion for an EntityDescriptor
    /// </summary>
    [ImplementationName(typeof(IEntityDescriptorVersionCalculator), "Default")]
    public class EntityDescriptorVersionCalculator : IEntityDescriptorVersionCalculator
    {
        private EntityModelSchema entityModelSchema;

        /// <summary>
        /// Initializes a new instance of the EntityDescriptorVersionCalculator class.
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        public EntityDescriptorVersionCalculator(EntityModelSchema entityModelSchema)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityModelSchema, "entityModelSchema");
            this.entityModelSchema = entityModelSchema;
        }

        /// <summary>
        /// Calculates the DataServiceVersion for a particular EntityDescriptor
        /// </summary>
        /// <param name="entityDescriptorData">Entity Descriptor Data</param>
        /// <param name="maxProtocolVersion">The client's max protocol version</param>
        /// <returns>A Data service protocol version</returns>
        public DataServiceProtocolVersion CalculateDataServiceVersion(EntityDescriptorData entityDescriptorData, DataServiceProtocolVersion maxProtocolVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");

            Type entityType = entityDescriptorData.Entity.GetType();
            EntityType testEntityType = this.entityModelSchema.EntityTypes.Single(et => et.FullName == entityType.FullName);

            // Calculate expected version based on type's feed mappings.
            DataServiceProtocolVersion expectedVersion = VersionHelper.CalculateEntityPropertyMappingProtocolVersion(testEntityType, VersionCalculationType.Response, MimeTypes.ApplicationAtomXml, maxProtocolVersion, maxProtocolVersion);

            // Commenting out this code pending a quick versioning discussion with pratikp and ahmed
            // Cannot check in as with code it breaks BVT's, want to get this in and update pending discussion 12-7-10
            // if (testEntityType.AllProperties.Any(p => p.IsStream()))
            // {
            //    expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            // }
            // If there are bag properties then its at least v3
            if (testEntityType.HasMultiValue(true) || 
                testEntityType.HasSpatialProperties())
            {
                expectedVersion = VersionHelper.IncreaseVersionIfRequired(expectedVersion, DataServiceProtocolVersion.V4);
            }

            return expectedVersion;
        }
    }
}
