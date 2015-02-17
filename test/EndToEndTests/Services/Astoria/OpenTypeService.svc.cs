//---------------------------------------------------------------------
// <copyright file="OpenTypeService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Spatial;

namespace Microsoft.Test.OData.Services.OpenTypesService 
{
    using Microsoft.OData.Service;
#if TESTPROVIDERS || TEST_ODATA_SERVICES_ASTORIA || TEST_ODATA_SERVICES_ASTORIA_NOPUBLICPROVIDER
    using Microsoft.Spatial;
#else
    using System.Spatial; 
    using System.Data.Services.Common; 
#endif

    [System.ServiceModel.ServiceBehaviorAttribute(IncludeExceptionDetailInFaults=true, Namespace="http://microsoft.com/test/taupo/generated/")]
    public class OpenTypeService : DataService<DefaultContainer>, Microsoft.Test.OData.Framework.TestProviders.Contracts.DataOracle.IDataServiceDataSourceCreator, System.IServiceProvider {
        
        private DefaultContainer contextInstance;
        
        public OpenTypeService() {
            this.contextInstance = new DefaultContainer(this);
        }
        
        public static void InitializeService(DataServiceConfiguration config) {
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.AcceptSpatialLiteralsInQuery = false;
            config.DataServiceBehavior.MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;
            config.SetEntitySetAccessRule("*", Microsoft.OData.Service.EntitySetRights.All);
            config.SetServiceActionAccessRule("*", Microsoft.OData.Service.ServiceActionRights.Invoke);
            config.SetServiceOperationAccessRule("*", Microsoft.OData.Service.ServiceOperationRights.All);
            SpatialImplementation.CurrentImplementation.Operations = new Microsoft.Test.OData.Framework.TestProviders.Common.PseudoDistanceImplementation();
            config.EnableTypeAccess("*");
        }
        
        public virtual object GetService(System.Type serviceType) {
            if (((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider)) 
                        || (serviceType == typeof(Microsoft.OData.Service.IUpdatable)))) {
                return this.contextInstance;
            }
            if ((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceQueryProvider))) {
                return this.contextInstance;
            }
            if ((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceMetadataProvider))) {
                return this.contextInstance;
            }
            return null;
        }
        
        object Microsoft.Test.OData.Framework.TestProviders.Contracts.DataOracle.IDataServiceDataSourceCreator.CreateDataSource() {
            return this.CreateDataSource();
        }
    }
}
