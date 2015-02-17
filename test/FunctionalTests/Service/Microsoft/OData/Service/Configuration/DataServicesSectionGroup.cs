//---------------------------------------------------------------------
// <copyright file="DataServicesSectionGroup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Configuration section group for data services
    /// </summary>
    public sealed class DataServicesSectionGroup : ConfigurationSectionGroup
    {
        /// <summary>
        /// Features section whether you can turn on/off specific data services features.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.FeaturesSectionName)]
        public DataServicesFeaturesSection Features
        {
            get { return (DataServicesFeaturesSection)this.Sections[DataServicesConfigurationConstants.FeaturesSectionName]; }
        }
    }
}
