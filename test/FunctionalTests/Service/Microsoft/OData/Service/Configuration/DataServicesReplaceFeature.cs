//---------------------------------------------------------------------
// <copyright file="DataServicesReplaceFeature.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Feature for allowing replace functions in url.
    /// </summary>
    public class DataServicesReplaceFunctionFeature : ConfigurationElement
    {
        /// <summary>
        /// Returns the value of the enable attribute for data services replace feature.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.EnableAttributeName)]
        public bool Enable
        {
            get { return (bool)this[DataServicesConfigurationConstants.EnableAttributeName]; }
            set { this[DataServicesConfigurationConstants.EnableAttributeName] = value; }
        }

        /// <summary>
        /// returns true if the element is present otherwise false.
        /// </summary>
        internal virtual bool IsPresent
        {
            get { return this.ElementInformation.IsPresent; }
        }
    }
}
