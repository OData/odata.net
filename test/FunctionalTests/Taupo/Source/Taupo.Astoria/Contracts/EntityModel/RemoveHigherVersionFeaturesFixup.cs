//---------------------------------------------------------------------
// <copyright file="RemoveHigherVersionFeaturesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Entity model fixup for removing all V3 features if MaxProtocolVersion is less than V3.
    /// </summary>
    public class RemoveHigherVersionFeaturesFixup : IEntityModelFixup
    {
        /// <summary>
        /// maximum version of data service
        /// </summary>
        private DataServiceProtocolVersion version;

        /// <summary>
        /// Initializes a new instance of the RemoveHigherVersionFeaturesFixup class that remove higher version features in the model
        /// </summary>
        /// <param name="version">max dataservice service version</param>
        public RemoveHigherVersionFeaturesFixup(DataServiceProtocolVersion version)
        {
            this.version = version;
        }

        /// <summary>
        /// Remove V3 features 
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            if (this.version < DataServiceProtocolVersion.V4)
            {
                EntityContainer defaultContainer = model.GetDefaultEntityContainer();
                ExceptionUtilities.CheckObjectNotNull(defaultContainer, "Expected a EntityContainer in the model, likely need to execute AddDefaultContainerFixup");
                DataServiceBehaviorAnnotation serviceBehaviorAnnotation = defaultContainer.GetDataServiceBehavior();
                ExceptionUtilities.CheckObjectNotNull(serviceBehaviorAnnotation, "Expected a EntityContainer in the model, likely need to execute SetDefaultDataServiceConfigurationBehaviors fixup");

                new RemoveActionsFixup().Fixup(model);
                new RemoveMultiValueFixup().Fixup(model);
                new RemoveNamedStreamsFixup().Fixup(model);
                new RemoveNavigationPropertiesFromDerivedTypesFixup().Fixup(model);
                serviceBehaviorAnnotation.IncludeAssociationLinksInResponse = null;
            }
        }
    }
}