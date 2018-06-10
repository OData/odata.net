//---------------------------------------------------------------------
// <copyright file="DataUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    #endregion Namespaces

    /// <summary>This class provides utility methods for database tasks.</summary>
    public static class DataUtil
    {
        /// <summary>Default data source, assumed to always be available for testing.</summary>
        public static string DefaultDataSource
        {
            [DebuggerStepThrough]
            get
            {
#if USELOCALDB
                return @"(LocalDB)\MSSQLLocalDB";
#else
                return @".\SQLEXPRESS";
#endif
            }
        }

        /// <summary>Provider name, suitable for use with EntityConnectionString.</summary>
        public static string ProviderName
        {
            [DebuggerStepThrough]
            get { return "System.Data.SqlClient"; }
        }

        /// <summary>Builds a connection using integrated security.</summary>
        /// <param name="dataSource">Name of data source ('.' if null).</param>
        /// <param name="path">Path to data file.</param>
        /// <param name="catalogName">Name of database to create.</param>
        public static void AttachDatabase(string dataSource, string path, string logpath, string catalogName)
        {
            string connectionString = BuildTrustedConnection(dataSource, null);

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandTimeout = 0;

                sqlCmd.CommandText = "sp_attach_db";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add(new SqlParameter("@dbname", catalogName));
                sqlCmd.Parameters.Add(new SqlParameter("@filename1", path));
                sqlCmd.Parameters.Add(new SqlParameter("@filename2", logpath));
                sqlCmd.ExecuteNonQuery();
            }
        }

        /// <summary>Builds a connection string for an EntityConnection.</summary>
        /// <param name='baseModelName'>Base name for all files (baseModelName.csdl, baseModelName.msl and baseModelName.ssdl)</param>
        /// <param name='connectionString'>Connection string for a provider (specified for ProviderName).</param>
        /// <param name="assembly">Assembly where the model resources are held</param>
        /// <returns>The connection string to be used with <see cref="System.Data.EntityClient.EntityConnection"/>.</returns>
        public static string BuildEntityConnection(string baseModelName, string connectionString, System.Reflection.Assembly assembly)
        {
            string[] extensions = new string[] { ".csdl", ".msl", ".ssdl" };
            string prefix = String.Empty;
            StringBuilder metaBuilder = new StringBuilder();
            foreach (string extension in extensions)
            {
                metaBuilder.Append(prefix).Append("res://*/").Append(IOUtil.FindResourceNameForPath(assembly, baseModelName + extension));
                prefix = "|";
            }

            System.Data.EntityClient.EntityConnectionStringBuilder builder = new System.Data.EntityClient.EntityConnectionStringBuilder();
            builder.Provider = ProviderName;
            builder.ProviderConnectionString = connectionString;
            builder.Metadata = metaBuilder.ToString();
            return builder.ConnectionString;
        }

        /// <summary>Builds a connection using integrated security.</summary>
        /// <param name="dataSource">Name of data source ('.' if null).</param>
        /// <param name="initialCatalog">Initial catalog on server.</param>
        /// <returns>The connection string to be used with <see cref="SqlConnection"/>.</returns>
        public static string BuildTrustedConnection(string dataSource, string initialCatalog)
        {
            dataSource = dataSource ?? DefaultDataSource;

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            if (initialCatalog != null)
            {
                builder.InitialCatalog = initialCatalog;
            }

            builder.IntegratedSecurity = true;
            return builder.ConnectionString;
        }

        /// <summary>Builds the T-SQL statements required to recreate a database.</summary>
        /// <param name="databaseName">Database name.</param>
        /// <returns>
        /// The statements required to create a database, dropping an existing one if found.
        /// </returns>
        public static IEnumerable<string> BuildTSqlForRecreateDatabase(string databaseName)
        {
            CheckArgumentNotNull(databaseName, "databaseName");
            foreach (string s in BuildTSqlForSafeDropDatabase(databaseName))
            {
                yield return s;
            }

            yield return "CREATE DATABASE " + databaseName + ";";
        }

        /// <summary>Builds the T-SQL statements required to drop a database if it exists.</summary>
        /// <param name="databaseName">Database name.</param>
        /// <returns>
        /// The statements required to drop an existing database if found.
        /// </returns>
        public static IEnumerable<string> BuildTSqlForSafeDropDatabase(string databaseName)
        {
            CheckArgumentNotNull(databaseName, "databaseName");
            yield return
                "IF (0 < (SELECT COUNT(*) FROM sys.databases D WHERE D.name = '" +
                databaseName + "')) DROP DATABASE [" + databaseName + "];";
        }

        /// <summary>Executes the specified <paramref name="statements"/>.</summary>
        /// <param name="connectionString">Connection string on which to execute statements.</param>
        /// <param name="statements">Statements to execute.</param>
        public static void ExecuteNonQueryStatements(string connectionString, IEnumerable<string> statements)
        {
            CheckArgumentNotNull(connectionString, "connectionString");
            CheckArgumentNotNull(statements, "statements");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ExecuteNonQueryStatements(connection, statements);
            }
        }

        /// <summary>Executes the specified <paramref name="statements"/>.</summary>
        /// <param name="connection">Connection on which to execute statements.</param>
        /// <param name="statements">Statements to execute.</param>
        public static void ExecuteNonQueryStatements(SqlConnection connection, IEnumerable<string> statements)
        {
            CheckArgumentNotNull(connection, "connection");
            CheckArgumentNotNull(statements, "statements");
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;

                foreach (string statement in statements)
                {
                    command.CommandText = statement;
                    bool success = false;
                    try
                    {
                        command.ExecuteNonQuery();
                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            Trace.WriteLine("Problem executing this statement:\r\n" + statement);
                        }
                    }
                }
            }
        }

        public static IEnumerable<string> SplitBatchStatements(string batch)
        {
            CheckArgumentNotNull(batch, "batch");
            string[] separators = new string[]
            {
                "\r\ngo\r\n",
                "\r\nGO\r\n",
            };

            return batch.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> SplitBatchStatementsFromFile(string path)
        {
            CheckArgumentNotNull(path, "path");
            string batch = File.ReadAllText(path);
            return SplitBatchStatements(batch);
        }

        public static void TraceTable(SqlConnection connection, string tableName)
        {
            CheckArgumentNotNull(connection, "connection");
            CheckArgumentNotNull(tableName, "tableName");
            TraceStatement(connection, "SELECT * FROM " + tableName, (text) => Trace.Write(text));
        }

        public static void TraceTables(string connectionString, IEnumerable<string> tableNames)
        {
            CheckArgumentNotNull(connectionString, "connectionString");
            CheckArgumentNotNull(tableNames, "tableNames");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (string table in tableNames)
                {
                    Trace.WriteLine("Source: " + table);
                    TraceTable(connection, table);
                }
            }
        }

        public static void TraceStatement(string connectionString, string statement, Action<string> traceAction)
        {
            CheckArgumentNotNull(connectionString, "connectionString");
            CheckArgumentNotNull(statement, "statement");
            CheckArgumentNotNull(traceAction, "traceAction");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                TraceStatement(connection, statement, traceAction);
            }
        }

        public static void TraceStatement(SqlConnection connection, string statement, Action<string> traceAction)
        {
            CheckArgumentNotNull(connection, "connection");
            CheckArgumentNotNull(statement, "statement");
            CheckArgumentNotNull(traceAction, "traceAction");
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = statement;
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    do
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i != 0) traceAction(" ");
                            traceAction(reader.GetName(i));
                        }
                        traceAction(Environment.NewLine);

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (i != 0) traceAction(" ");
                                if (reader.IsDBNull(i))
                                {
                                    traceAction("NULL");
                                }
                                else
                                {
                                    object value = reader.GetValue(i);
                                    if (value is byte[])
                                    {
                                        byte[] bytes = value as byte[];
                                        System.Text.StringBuilder builder = new System.Text.StringBuilder(bytes.Length * 2);
                                        string nibbles = "0123456789ABCDEF";
                                        foreach (byte b in bytes)
                                        {
                                            builder.Append(nibbles[(b & 0xF0) >> 4]);
                                            builder.Append(nibbles[b & 0x0F]);
                                        }
                                        value = builder.ToString();
                                    }

                                    traceAction(value.ToString());
                                }
                            }
                            traceAction(Environment.NewLine);
                        }
                    }
                    while (reader.NextResult());
                }
            }
        }

        private static void CheckArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
