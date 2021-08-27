//---------------------------------------------------------------------
// <copyright file="ServiceReferenceFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ServiceReferences
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.WebServices.CompilerService.DotNet;

    /// <summary>
    /// Implementation of the service reference factory contract for the known service types in Taupo.WebServices
    /// </summary>
    [ImplementationName(typeof(IServiceReferenceFactory), "Default")]
    public class ServiceReferenceFactory : IServiceReferenceFactory
    {
        private static readonly Type[] knownTypes = new[] { typeof(CompilerServiceClient), typeof(DataServiceBuilderServiceClient), typeof(DataOracleServiceClient) };

        /// <summary>
        /// Gets or sets the authentication mode for the data service.
        /// </summary>
        [InjectTestParameter("AuthenticationMode")]
        public AuthenticationMode AuthenticationMode { get; set; }

        /// <summary>
        /// Creates an instance of the given service reference client for a service with the given uri
        /// </summary>
        /// <typeparam name="TClient">The service reference client type</typeparam>
        /// <param name="serviceUri">The service uri</param>
        /// <returns>An instance of the client type</returns>
        public TClient CreateInstance<TClient>(Uri serviceUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(serviceUri, "serviceUri");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var type = knownTypes.SingleOrDefault(t => typeof(TClient).IsAssignableFrom(t));
            if (type == null)
            {
                return default(TClient);
            }

            TClient serviceProxy = (TClient)Activator.CreateInstance(type, this.CreateHttpBinding(), new EndpointAddress(serviceUri));

            // get the client credentials property and set its value 
            return serviceProxy;
        }

        /// <summary>
        /// Creates an http binding
        /// </summary>
        /// <returns>An http binding</returns>
        private BasicHttpBinding CreateHttpBinding()
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                OpenTimeout = TimeSpan.FromMinutes(10),
                CloseTimeout = TimeSpan.FromMinutes(10)
            };

            if (this.AuthenticationMode != Taupo.Common.AuthenticationMode.None)
            {
                BasicHttpSecurity basicHttpSecuritySettings = new BasicHttpSecurity() { Mode = BasicHttpSecurityMode.TransportCredentialOnly };
                HttpTransportSecurity transportSecurity = new HttpTransportSecurity();
                if (this.AuthenticationMode == Taupo.Common.AuthenticationMode.Basic)
                {
                    transportSecurity.ClientCredentialType = HttpClientCredentialType.Basic;
                    transportSecurity.ProxyCredentialType = HttpProxyCredentialType.Basic;
                }
                else
                {
                    transportSecurity.ClientCredentialType = HttpClientCredentialType.Windows;
                    transportSecurity.ProxyCredentialType = HttpProxyCredentialType.Windows;
                }

                basicHttpSecuritySettings.Transport = transportSecurity;
                binding.Security = basicHttpSecuritySettings;
            }

            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;

            return binding;
        }
    }
}