//---------------------------------------------------------------------
// <copyright file="DatabaseHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;
    using System.Data.SqlClient;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;

    /// <summary>
    /// Helper to create the database for EF service
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Ensure the database needed by EF and Hybrid service is created
        /// </summary>
        public static void EnsureDatabaseCreated()
        {
            const string databaseName = "AstoriaDefaultServiceDB";
            const string resourceName = "Microsoft.Test.OData.Services.Astoria.PublicProvider.AstoriaDefaultServiceDB.bak";
            Log.Trace(string.Format("Ensure database {0} exists", databaseName));
            var entityConnBuilder = new EntityConnectionStringBuilder(ConnectionString);
            var sqlConnBuilder = new SqlConnectionStringBuilder(entityConnBuilder.ProviderConnectionString)
                {
                    InitialCatalog = "master"
                };
            using (var conn = new SqlConnection(sqlConnBuilder.ToString()))
            {
                conn.Open();
                bool databaseExist;
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @name", conn))
                {
                    cmd.Parameters.AddWithValue("@name", databaseName);
                    databaseExist = (int)cmd.ExecuteScalar() > 0;
                }
                if (!databaseExist)
                {
                    Log.Trace(string.Format("Database {0} does not exist", databaseName));
                    using (Stream resource = typeof(DatabaseHelper).Assembly
                                  .GetManifestResourceStream(resourceName))
                    {
                        if (resource == null)
                        {
                            throw new ArgumentException("No such resource", resourceName);
                        }
                        const string temp = "Temp";
                        if (!Directory.Exists(temp))
                        {
                            Directory.CreateDirectory(temp);
                        }
                        DirectorySecurity sec = Directory.GetAccessControl(temp);
                        var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                        sec.AddAccessRule(
                            new FileSystemAccessRule(everyone,
                                                     FileSystemRights.Modify |
                                                     FileSystemRights.Synchronize,
                                                     InheritanceFlags.ContainerInherit |
                                                     InheritanceFlags.ObjectInherit, PropagationFlags.None,
                                                     AccessControlType.Allow));
                        Directory.SetAccessControl(temp, sec);
                        string file = Path.GetFullPath(Path.Combine(temp, databaseName + ".bak"));
                        string mdf = Path.GetFullPath(Path.Combine(temp, databaseName + ".mdf"));
                        string ldf = Path.GetFullPath(Path.Combine(temp, databaseName + ".ldf"));
                        Log.Trace(string.Format("Output database backup to {0}.", file));
                        using (Stream output = File.OpenWrite(file))
                        {
                            resource.CopyTo(output);
                        }
                        string restorecmd = string.Format(
                            "RESTORE DATABASE AstoriaDefaultServiceDB FROM DISK = '{0}' WITH MOVE 'AstoriaDefaultServiceDB' TO '{1}', MOVE 'AstoriaDefaultServiceDB_log' TO '{2}', REPLACE", file, mdf, ldf);
                        Log.Trace(restorecmd);
                        using (var cmd = new SqlCommand(restorecmd, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        internal static string ConnectionString
        {
            get { return connectionString; }
        }

        private static string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings["AstoriaDefaultServiceDBEntities"].ConnectionString;
#if USELOCALDB
            connStr = connStr.Replace(@".\SQLExpress", @"(LocalDB)\MSSQLLocalDB");
#endif
            return connStr;
        }

    }
}