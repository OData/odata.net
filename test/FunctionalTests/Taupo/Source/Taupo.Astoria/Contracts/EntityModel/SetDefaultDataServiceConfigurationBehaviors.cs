//---------------------------------------------------------------------
// <copyright file="SetDefaultDataServiceConfigurationBehaviors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Entity model fixup for setting one of the entity containers as the default
    /// Configures the defaults based on the MajorReleaseVersion if annotations are 
    /// not preconfigured
    /// </summary>
    public class SetDefaultDataServiceConfigurationBehaviors : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the SetDefaultDataServiceConfigurationBehaviors class to a DefaultEntityContainer
        /// </summary>
        public SetDefaultDataServiceConfigurationBehaviors()
        {
        }

        /// <summary>
        /// Gets or sets the MajorReleaseVersion
        /// </summary>
        public DataServiceProtocolVersion MaxProtocolVersion { get; set; }

        /// <summary>
        /// Sets the EntityContainer to default DataServiceConfiguration settings if not specified
        /// Default settings are all permissions for all entitysets
        /// All permissions for ServiceOperations
        /// Visible permissions for all actions
        /// and UseVerboseErrors = true
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            EntityContainer container = model.GetDefaultEntityContainer();
            if (!container.Annotations.OfType<EntitySetRightsAnnotation>().Any())
            {
                container.Annotations.Add(new EntitySetRightsAnnotation() { Value = EntitySetRights.All });
            }

            if (!container.Annotations.OfType<ServiceOperationRightsAnnotation>().Any())
            {
                container.Annotations.Add(new ServiceOperationRightsAnnotation() { Value = ServiceOperationRights.All });
            }

            if (!container.Annotations.OfType<ActionOperationRightsAnnotation>().Any())
            {
                container.Annotations.Add(new ActionOperationRightsAnnotation() { Value = ActionOperationRights.Invoke });
            }

            DataServiceConfigurationAnnotation dataServiceConfigurationAnnotation = container.GetDataServiceConfiguration();
            if (dataServiceConfigurationAnnotation == null)
            {
                dataServiceConfigurationAnnotation = new DataServiceConfigurationAnnotation();
                dataServiceConfigurationAnnotation.UseVerboseErrors = true;
                container.Annotations.Add(dataServiceConfigurationAnnotation);
            }
            
            DataServiceBehaviorAnnotation dataServiceBehaviorAnnotation = container.GetDataServiceBehavior();
            if (dataServiceBehaviorAnnotation == null)
            {
                dataServiceBehaviorAnnotation = new DataServiceBehaviorAnnotation();
                dataServiceBehaviorAnnotation.MaxProtocolVersion = this.MaxProtocolVersion;
                container.Annotations.Add(dataServiceBehaviorAnnotation);
            }
        }
    }
}