//---------------------------------------------------------------------
// <copyright file="LocalWebServer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.EntityClient;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.ServiceModel.Web;

    #region Local web service management.

    public class CachedConnections
    {
        public enum ConnectionType
        {
            Northwind,
            Aruba,
            CustomObjectContext
        }

        public static EntityConnection GetConnectionForType(Type type)
        {
            if (type == typeof(northwind.northwindContext))
            {
                return GetConnectionForType(ConnectionType.Northwind);
            }
            else if (type == typeof(Aruba.ArubaContainer))
            {
                return GetConnectionForType(ConnectionType.Aruba);
            }
            else if (type == typeof(ClrNamespace.CustomObjectContext))
            {
                return GetConnectionForType(ConnectionType.CustomObjectContext);
            }

            throw new ArgumentException("Unsupported Type.", "type");
        }

        public static IDisposable SetupConnection(ConnectionType type)
        {
            return new ClearConnection(type);
        }

        private class ClearConnection : IDisposable
        {
            private ConnectionType type;

            public ClearConnection(ConnectionType type)
            {
                this.type = type;
                GetConnectionForType(type);
            }

            void IDisposable.Dispose()
            {
                EntityConnection connection = connections[this.type];
                connections[this.type] = null;
                connection.Dispose();
            }
        }

        public static bool IsSqlExpressInstalled
        {
            get { return isSqlExpressInstalled; }
        }

        private static EntityConnection GetConnectionForType(ConnectionType type)
        {
            if (!connections.ContainsKey(type) || connections[type] == null)
            {
                string dbName = Enum.GetName(typeof(ConnectionType), type);

                string dataSource = DataUtil.DefaultDataSource;
                if (!IsSqlExpressInstalled)
                {
                    dataSource = "tcp:fxbvt3,1432";
                }

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = dataSource;
                builder.IntegratedSecurity = true;
                builder.MultipleActiveResultSets = true;

                EntityConnection entityConnection = new EntityConnection(DataUtil.BuildEntityConnection(dbName, builder.ConnectionString, System.Reflection.Assembly.GetCallingAssembly()));
                entityConnection.Open();

                CreateDatabase(entityConnection, dbName);
                connections[type] = entityConnection;
            }

            return connections[type];
        }

        private static bool isSqlExpressInstalled = SqlExpressInstalled();

        private static bool SqlExpressInstalled()
        {
            const string SqlExpressServiceName = "MSSQL$SQLEXPRESS";
            var controllers = System.ServiceProcess.ServiceController.GetServices();
            return controllers.Any(c => c.ServiceName == SqlExpressServiceName);
        }

        private static Dictionary<ConnectionType, EntityConnection> connections = new Dictionary<ConnectionType, EntityConnection>(2);

        private static void CreateDatabase(EntityConnection connection, string dbName)
        {
            Assembly assembly = typeof(SimpleWorkspace).Assembly;
            string createSchemaResource = "AstoriaUnitTests.Create" + dbName + ".sql";
            string createSchemaCmd = IOUtil.ReadResourceText(assembly, createSchemaResource).Replace("###INSERT#GUID#HERE###", Guid.NewGuid().ToString());

            DbCommand cmd = connection.StoreConnection.CreateCommand();
            cmd.CommandText = createSchemaCmd;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = TestConstants.MaxTestTimeout;
            cmd.ExecuteNonQuery();
        }
    }

    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SimpleDataService<T> : DataService<T>, IServiceProvider
    {
        public static ODataProtocolVersion MaxProtocolVersion = ODataProtocolVersion.V4;

        public static void InitializeService(DataServiceConfiguration configuration)
        {
            configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            configuration.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);

            configuration.UseVerboseErrors = true;

            if (SimpleDataServiceHelper.PageSizeCustomizer != null)
            {
                SimpleDataServiceHelper.PageSizeCustomizer(configuration as DataServiceConfiguration);
            }

            configuration.DataServiceBehavior.MaxProtocolVersion = MaxProtocolVersion;
        }

        protected override T CreateDataSource()
        {
            if (SimpleDataServiceHelper<T>.CreateDataSourceCustomizer != null)
            {
                return SimpleDataServiceHelper<T>.CreateDataSourceCustomizer();
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T), CachedConnections.GetConnectionForType(typeof(T)));
            }
        }

        public object GetService(Type serviceType)
        {
            if (SimpleDataServiceHelper.GetServiceCustomizer != null)
            {
                return SimpleDataServiceHelper.GetServiceCustomizer(serviceType);
            }

            return null;
        }
    }

    public class SimpleDataServiceHelper
    {
        public static Action<DataServiceConfiguration> PageSizeCustomizer;
        public static Func<Type, object> GetServiceCustomizer;
    }

    public class SimpleDataServiceHelper<T>
    {
        public static Func<T> CreateDataSourceCustomizer;
    }

    public class NWServiceOpService : SimpleDataService<northwind.northwindContext>
    {
        [WebGet]
        [SingleResult]
        public IQueryable<northwind.Customers> GetCustomerById(String customerId)
        {
            return this.CurrentDataSource.Customers.Where(c => c.CustomerID == customerId);
        }

        [WebGet]
        public IEnumerable<northwind.Customers> GetCustomersEnumerable()
        {
            return this.CurrentDataSource.Customers.Take(5).AsEnumerable();
        }

        [WebGet]
        [SingleResult]
        public IQueryable<String> GetCustomerNameById(String customerId)
        {
            return (from c in this.CurrentDataSource.Customers where c.CustomerID == customerId select c.CompanyName).Take(5);
        }

        [WebGet]
        public IEnumerable<String> GetCustomerNamesEnumerable()
        {
            return (from c in this.CurrentDataSource.Customers select c.CompanyName).Take(5).AsEnumerable();
        }

        [WebInvoke(Method = "POST")]
        public IQueryable<northwind.Customers> GetCustomersByIdPOST(String customerId)
        {
            return this.CurrentDataSource.Customers.Where(c => c.CustomerID == customerId);
        }

        [WebGet]
        public IQueryable<northwind.Customers> GetAllCustomers()
        {
            return this.CurrentDataSource.Customers.Take(5);
        }
    }

    public class CustomObjectService : SimpleDataService<ClrNamespace.CustomObjectContext>, IServiceProvider
    {
        public static Action<ClrNamespace.Customer, UpdateOperations> CustomerChangeInterceptorOverride;
        public static Func<Expression<Func<ClrNamespace.Customer, bool>>> CustomerQueryInterceptorOverride;

        public static Action<ClrNamespace.Order, UpdateOperations> OrderChangeInterceptorOverride;
        public static Func<Expression<Func<ClrNamespace.Order, bool>>> OrderQueryInterceptorOverride;

        public static Action<ClrNamespace.Worker, UpdateOperations> WorkerChangeInterceptorOverride;
        public static Func<Expression<Func<ClrNamespace.Worker, bool>>> WorkerQueryInterceptorOverride;

        public static Action<ClrNamespace.Office, UpdateOperations> OfficeChangeInterceptorOverride;
        public static Func<Expression<Func<ClrNamespace.Office, bool>>> OfficeQueryInterceptorOverride;

        [ChangeInterceptor("Customers")]
        public void OnChangeCustomer(ClrNamespace.Customer c, UpdateOperations operation)
        {
            if (CustomerChangeInterceptorOverride != null)
            {
                CustomerChangeInterceptorOverride(c, operation);
            }
        }

        [QueryInterceptor("Customers")]
        public Expression<Func<ClrNamespace.Customer, bool>> OnQueryCustomers()
        {
            if (CustomerQueryInterceptorOverride != null)
            {
                return CustomerQueryInterceptorOverride();
            }
            else
            {
                return c => true;
            }
        }

        [ChangeInterceptor("Orders")]
        public void OnChangeOrder(ClrNamespace.Order o, UpdateOperations operation)
        {
            if (OrderChangeInterceptorOverride != null)
            {
                OrderChangeInterceptorOverride(o, operation);
            }
        }

        [QueryInterceptor("Orders")]
        public Expression<Func<ClrNamespace.Order, bool>> OnQueryOrders()
        {
            if (OrderQueryInterceptorOverride != null)
            {
                return OrderQueryInterceptorOverride();
            }
            else
            {
                return o => true;
            }
        }

        [ChangeInterceptor("Workers")]
        public void OnChangeWorker(ClrNamespace.Worker w, UpdateOperations operation)
        {
            if (WorkerChangeInterceptorOverride != null)
            {
                WorkerChangeInterceptorOverride(w, operation);
            }
        }

        [QueryInterceptor("Workers")]
        public Expression<Func<ClrNamespace.Worker, bool>> OnQueryWorkers()
        {
            if (WorkerQueryInterceptorOverride != null)
            {
                return WorkerQueryInterceptorOverride();
            }
            else
            {
                return w => true;
            }
        }

        [ChangeInterceptor("Offices")]
        public void OnChangeOffice(ClrNamespace.Office o, UpdateOperations operation)
        {
            if (OfficeChangeInterceptorOverride != null)
            {
                OfficeChangeInterceptorOverride(o, operation);
            }
        }

        [QueryInterceptor("Offices")]
        public Expression<Func<ClrNamespace.Office, bool>> OnQueryOffices()
        {
            if (OfficeQueryInterceptorOverride != null)
            {
                return OfficeQueryInterceptorOverride();
            }
            else
            {
                return o => true;
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return new DataServiceStreamProvider();
            }

            return null;
        }
    }

    #endregion Local web service management.
}