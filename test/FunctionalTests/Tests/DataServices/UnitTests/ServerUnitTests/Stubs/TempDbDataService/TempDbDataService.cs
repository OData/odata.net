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

    public class NorthwindDefaultTempDbService : NorthwindTempDbServiceBase<NorthwindModel.NorthwindContext>
    {
        public static string ContextConnectionString;
    }

    public class NorthwindTempDbServiceBase<T> : TempDbDataService<T>
    {
        public static IDisposable SetupNorthwind()
        {
            return CachedConnections.SetupConnection(CachedConnections.BaseModelType.Northwind);
        }
        
        public static string MetadataDirectory
        {
            get
            {
                return Path.Combine(TestUtil.ServerUnitTestSamples, @"stubs\TempDbDataService");
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
        }

        public static IDisposable SetupConnection(BaseModelType type)
        {
            return new ClearConnection(type);
        }

        #endregion public interface

        internal static EntityConnection GetConnectionForType(Type type)
        {
            if ( type.FullName == "NorthwindModel.NorthwindContext" )
            {
                return GetConnectionForType(BaseModelType.Northwind);
            }

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
            EntityConnection cachedConnection = null;

            public ClearConnection(BaseModelType type)
            {
                this.type = type;

                // if there is an existing connection we need to cache it. Otherwise we will 
                // close a connection we did not open. 
                if (connections.ContainsKey(type))
                {
                    cachedConnection = connections[type];
                    connections.Remove(type);
                }

                GetConnectionForType(type);
            }

            void IDisposable.Dispose()
            {
                EntityConnection connection = connections[this.type];
                // restore cached connection - null is fine and means that connection has to be recreated
                connections[this.type] = cachedConnection;
                connection.Dispose();
            }
        }

        private static EntityConnection GetConnectionForType(BaseModelType type)
        {
            if (!connections.ContainsKey(type) || connections[type] == null)
            {
                string dbName = Enum.GetName(typeof(BaseModelType), type);
                string dataSource = DataUtil.DefaultDataSource;

                EntityConnection entityConnection = new EntityConnection(NorthwindDefaultTempDbService.ContextConnectionString);

                if (string.IsNullOrEmpty(NorthwindDefaultTempDbService.ContextConnectionString))
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = dataSource;
                    builder.IntegratedSecurity = true;
                    builder.MultipleActiveResultSets = true;
                    entityConnection = new EntityConnection(DataUtil.BuildEntityConnection(NorthwindDefaultTempDbService.MetadataDirectory, dbName, builder.ConnectionString));
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
            string createSchemaSqlFile = Path.Combine(NorthwindDefaultTempDbService.MetadataDirectory, "Create" + dbName + ".sql");
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