//---------------------------------------------------------------------
// <copyright file="IisDeploymentSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Specifies deployment-level settings specific to IIS
    /// used during data service build process. 
    /// </summary>
    [ImplementationName(typeof(DeploymentSettings), "IIS")]
    public class IisDeploymentSettings : DeploymentSettings
    {
        /// <summary>
        /// Initializes a new instance of the IisDeploymentSettings class.
        /// </summary>
        public IisDeploymentSettings()
            : base("IIS")
        {
            this.TrustLevel = TrustLevel.Medium;
        }

        /// <summary>
        /// Gets or sets the trust level in which to create the Web Service.
        /// </summary>
        [InjectTestParameter("TrustLevel", DefaultValueDescription = "Medium", HelpText = "Specifies the trust level for the deployed service in IIS.")]
        public TrustLevel TrustLevel { get; set; }
    }
}