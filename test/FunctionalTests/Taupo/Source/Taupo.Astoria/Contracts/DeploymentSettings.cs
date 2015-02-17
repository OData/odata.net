//---------------------------------------------------------------------
// <copyright file="DeploymentSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Specifies deployment-level settings used during data service build process. 
    /// </summary>
    [ImplementationSelector("DataServiceDeployment", DefaultImplementation = "IIS")]
    public abstract class DeploymentSettings : ServiceBuilderSettingsBase
    {
        [IgnoreDataMember]
        public const string RelayServiceDeployerPrefix = "Relay:";

        /// <summary>
        /// Initializes a new instance of the DeploymentSettings class.
        /// </summary>
        /// <param name="serviceDeployerKind">Kind of the service deployer.</param>
        protected DeploymentSettings(string serviceDeployerKind)
        {
            this.ServiceDeployerKind = serviceDeployerKind;
        }

        /// <summary>
        /// Gets the name of the deployment provider.
        /// </summary>
        [IgnoreDataMember]
        public string ServiceDeployerKind { get; private set; }

        /// <summary>
        /// Gets or sets the authentication mode for the data service.
        /// </summary>
        [InjectTestParameter("AuthenticationMode")]
        public AuthenticationMode AuthenticationMode { get; set; }
    }
}