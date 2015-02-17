//---------------------------------------------------------------------
// <copyright file="DataServiceHostDeploymentSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Specifies deployment-level settings specific to IDataServiceHost 
    /// used during data service build process. 
    /// </summary>
    [ImplementationName(typeof(DeploymentSettings), "IDataServiceHost")]
    public class DataServiceHostDeploymentSettings : DeploymentSettings
    {
        /// <summary>
        /// Initializes a new instance of the DataServiceHostDeploymentSettings class.
        /// </summary>
        public DataServiceHostDeploymentSettings()
            : base("IDataServiceHost")
        {
        }
    }
}