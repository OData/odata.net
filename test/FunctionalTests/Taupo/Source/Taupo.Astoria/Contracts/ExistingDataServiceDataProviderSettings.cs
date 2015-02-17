//---------------------------------------------------------------------
// <copyright file="ExistingDataServiceDataProviderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Specifies deployment-level settings specific to ExistingDataServiceDeployer
    /// </summary>
    [ImplementationName(typeof(DataProviderSettings), "Existing")]
    public class ExistingDataServiceDataProviderSettings : DataProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the ExistingDataServiceDataProviderSettings class.
        /// </summary>
        public ExistingDataServiceDataProviderSettings()
            : this("Existing")
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExistingDataServiceDataProviderSettings class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public ExistingDataServiceDataProviderSettings(string providerName)
            : base(providerName)
        {
        }

        /// <summary>
        /// Gets or sets the Uri to an existing DataService
        /// </summary>
        [InjectTestParameter("ServiceBaseUri", HelpText = "The Uri to an existing DataService to target the tests at")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is externally injected")]
        public string ServiceBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the Uri to get the ServiceDocument of an existing DataService 
        /// </summary>
        [InjectTestParameter("ServiceDocumentUri", HelpText = "The Uri to get the ServiceDocument of an existing DataService to target the tests at")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is externally injected")]
        public string ServiceDocumentUri { get; set; }

        /// <summary>
        /// Gets or sets the ServiceDocumentParser to parse the Service Document for an OData Service
        /// </summary>
        [InjectTestParameter("ServiceDocumentParser", HelpText = "The servicedocument parser for the Data Service")]
        public string ServiceDocumentParser { get; set; }

        /// <summary>
        /// Gets or sets the authentication provider to authenticate against the data service
        /// </summary>
        [InjectTestParameter("AuthenticationProvider", HelpText = "The authentication provider to authenticate against the data service")]
        public string AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets a wrapperScope to wrap instances of the DataServiceContext
        /// </summary>
        [InjectTestParameter("WrapperScope", DefaultValueDescription = "DataServiceContextTrackingScope")]
        public string WrapperScope { get; set; }

        /// <summary>
        /// Gets or sets a wrapperScope to wrap instances of the DataServiceContext
        /// </summary>
        [InjectTestParameter("PrimitiveValueComparer", DefaultValueDescription = "QueryScalarValueToClrValueComparer")]
        public string PrimitiveValueComparer { get; set; }
    }
}
