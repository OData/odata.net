//---------------------------------------------------------------------
// <copyright file="DataServicesFeaturesSection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Features section for data services
    /// </summary>
    public sealed class DataServicesFeaturesSection : ConfigurationSection
    {
        /// <summary>
        /// Element to specify whether replace functions should be allowed in url or not.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.ReplaceFunctionFeatureElementName)]
        public DataServicesReplaceFunctionFeature ReplaceFunction
        {
            get
            {
                return (DataServicesReplaceFunctionFeature)base[DataServicesConfigurationConstants.ReplaceFunctionFeatureElementName];
            }

            set
            {
                base[DataServicesConfigurationConstants.ReplaceFunctionFeatureElementName] = value;
            }
        }
    }
}
