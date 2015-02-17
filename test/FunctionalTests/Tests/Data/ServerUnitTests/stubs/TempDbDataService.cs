//---------------------------------------------------------------------
// <copyright file="TempDbDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region namespaces

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.EntityClient;
    using Microsoft.OData.Service;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Text;
    
    #endregion namespaces

    public class NorthwindTempDbDataService : TempDbDataService<NorthwindModel.NorthwindContext>
    {
        public static IDisposable SetupNorthwind()
        {
            return CachedConnections.SetupConnection(CachedConnections.BaseModelType.Northwind);
        }

        public static string ContextConnectionString;

        public static string MetadataDirectory
        {
            get
            {
                string root = Environment.GetEnvironmentVariable("SDXROOT");
                if (String.IsNullOrEmpty(root))
                {
                    throw new InvalidOperationException("Unable to read MetadataDirectory " +
                        "because the SDXROOT environment variable is not defined.");
                }

                return Path.Combine(root, @"ddsuites\src\fx\DataWeb\UnitTests\ServerUnitTests\stubs\TempDbDataService");
            }
        }
    }

    public class TempDbDataService<T> : OpenWebDataService<T>
    {
        protected override T CreateDataSource()
        {
            return (T)Activator.CreateInstance(typeof(T), CachedConnections.GetConnectionForType(typeof(T)));
        }
    }

    class CachedConnections
    {
        #region Private Fields
        
        private static Dictionary<BaseModelType, EntityConnection> connections = new Dictionary<BaseModelType, EntityConnection>(2);
        private static BaseModelType? databaseToRecycle;
        private const int MaxConnectionTimeout = 600;
        
        #endregion Private Fields

        #region public interface

        public enum BaseModelType
        {
            Northwind,
//            Aruba
        }

        public static IDisposable SetupConnection(BaseModelType type)
        {
            return new ClearConnection(type);
        }

        #endregion public interface

        internal static EntityConnection GetConnectionForType(Type type)
        {
            if (type == typeof(NorthwindModel.NorthwindContext))
            {
                return GetConnectionForType(BaseModelType.Northwind);
            }
            //else if (type == typeof(Aruba.ArubaContainer))
            //{
            //    return GetConnectionForType(ConnectionType.Aruba);
            //}

            throw new ArgumentException("Unsupported Type.", "type");
        }

        /// <summary>Invoke this method to allow the database for a model to be recycled.</summary>
        /// <param name="type">Type of database to recycle.</param>
        /// <remarks>
        /// This is a hint only. It should be used when the data isn't modified; 
        /// the connection strings may be recreated, but the physical database
        /// won't need to be recreated.
        /// </remarks>
        internal static void RecycleDatabase(BaseModelType type)
        {
            // Support more than one whenever we have more databases.
            databaseToRecycle = type;
        }

        #region privates

        private class ClearConnection : IDisposable
        {
            private BaseModelType type;

            public ClearConnection(BaseModelType type)
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

        private static EntityConnection GetConnectionForType(BaseModelType type)
        {
            if (!connections.ContainsKey(type) || connections[type] == null)
            {
                string dbName = Enum.GetName(typeof(BaseModelType), type);
                string dataSource = @".\SQLEXPRESS";

                EntityConnection entityConnection = new EntityConnection(NorthwindTempDbDataService.ContextConnectionString);

                if (string.IsNullOrEmpty(NorthwindTempDbDataService.ContextConnectionString))
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = dataSource;
                    builder.IntegratedSecurity = true;
                    builder.MultipleActiveResultSets = true;
                    entityConnection = new EntityConnection(DataUtil.BuildEntityConnection(NorthwindTempDbDataService.MetadataDirectory, dbName, builder.ConnectionString));
                }

                entityConnection.Open();

                if (!databaseToRecycle.HasValue || databaseToRecycle.Value != type)
                {
                    CreateDatabase(entityConnection, dbName);
                }
                else
                {
                    databaseToRecycle = null;
                }

                connections[type] = entityConnection;
            }

            return connections[type];
        }

        private static void CreateDatabase(EntityConnection connection, string dbName)
        {
            string createSchemaSqlFile = Path.Combine(NorthwindTempDbDataService.MetadataDirectory, "Create" + dbName + ".sql");
            string createSchemaCmd = File.OpenText(createSchemaSqlFile).ReadToEnd().Replace("###INSERT#GUID#HERE###", Guid.NewGuid().ToString());

            DbCommand cmd = connection.StoreConnection.CreateCommand();
            cmd.CommandText = createSchemaCmd;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = MaxConnectionTimeout;
            cmd.ExecuteNonQuery();
        }

        #endregion privates
    }
}