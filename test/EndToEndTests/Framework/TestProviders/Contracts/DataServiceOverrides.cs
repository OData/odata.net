//---------------------------------------------------------------------
// <copyright file="DataServiceOverrides.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    /// <summary>
    /// Class used to set specific actions on Data Service Providers
    /// </summary>
    public static class DataServiceOverrides
    {
        private static DataServiceActionProviderOverrides dataServiceActionProviderOverrides = new DataServiceActionProviderOverrides();
        private static DataServiceUpdatable2Overrides dataServiceUpdatable2Overrides = new DataServiceUpdatable2Overrides();

        /// <summary>
        /// Gets the Action Provider Overrides
        /// </summary>
        public static DataServiceActionProviderOverrides ActionProvider
        {
            get { return dataServiceActionProviderOverrides; }
        }

        /// <summary>
        /// Gets the UpdateProvider2 overrides
        /// </summary>
        public static DataServiceUpdatable2Overrides UpdateProvider2
        {
            get { return dataServiceUpdatable2Overrides; }
        }
    }
}
