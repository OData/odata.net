//---------------------------------------------------------------------
// <copyright file="AstoriaDatabase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.TestExecutionLayer
{
    #region Namespaces

    using System.Data.SqlClient;
    using System.Data.Test.Astoria.Util;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Security;
    using System.Text;
    using Microsoft.Test.ModuleCore;

    #endregion Namespaces

    [SecuritySafeCritical]
    [DnsPermission(Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
    public class AstoriaDatabase : IDisposable
    {
        private string _machineConnectionString = null;
        private string _machine = null;
        private string _databaseName = null;
        private string _databasePrefixName = null;
        private string _sourceFolder = null;
        private string _destinationFolder = null;
        private string _attachFolder = null;
        private bool _isLocal = false;
        const string databaseFolder = @"workspaces\databases\";

        /// <summary>Initializes a new <see cref="AstoriaDatabase"/> instance.</summary>
        /// <param name="databasePrefixName">Database prefix name to be composed into a unique name.</param>
        public AstoriaDatabase(string databasePrefixName, bool populate)
        {
            if (String.IsNullOrEmpty(databasePrefixName))
            {
                throw new ArgumentException("String.IsNullOrEmpty(databasePrefixName)", "databasePrefixName");
            }
            
            _databasePrefixName = databasePrefixName;
            CreateDatabase();
            this.IsEmpty = !populate;
        }

        public AstoriaDatabase(string databasePrefixName)
            : this(databasePrefixName, true)
        { }

        private string GetInsertString(Workspace workspace, ResourceType resourceType, fxLanguage language, Random random)
        {
            string commandText = "INSERT INTO {0} ({1}) VALUES ({2});";
            string tableName = resourceType.Name;
            string columns = "";
            string values = "";

            ResourceInstanceKey rik = ResourceInstanceUtil.CreateUniqueKey(workspace.ServiceContainer.ResourceContainers[resourceType.Name], resourceType);
            foreach (ResourceInstanceProperty rip in rik.KeyProperties)
            {
                columns += rip.Name + ",";
                string propertyValue = AstoriaUnitTests.Data.TypeData.XmlValueFromObject(((ResourceInstanceSimpleProperty)rip).PropertyValue);
                values += propertyValue + ",";
            }

            foreach (ResourceProperty prop in resourceType.Properties)
            {
                if (!prop.IsComplexType && !prop.IsNavigation && !(prop.Type is CollectionType) && prop.PrimaryKey == null)
                {
                    columns += prop.Name + ",";

                    if (prop.Type is ClrString)
                    {
                        int maxSize = (int)(prop.Facets["MaxSize"].Value.ClrValue);
                        if( maxSize > 1000 ) maxSize = 1000;

                        values += "'" + language.CreateData(random, maxSize).Replace("'", "''") + "',";
                    }
                    else
                        values += prop.Type.CreateValue().ToString() + ",";
                }
            }

            columns = columns.TrimEnd(',');
            values = values.TrimEnd(',');
            
            string fullTableName = String.Format("[{0}].[{1}].[{2}]", this.DatabaseName, "dbo", tableName);
            return string.Format(commandText, fullTableName, columns, values);
        }

        internal void InsertLanguageData(Workspace workspace, ResourceType resourceType)
        {
            fxLanguages languages = new fxLanguages();
            fxLanguage language = languages.Choose();
            AstoriaTestLog.TraceInfo("Language Data: " + language.GetType().Name);

            // do them one at a time so that if one fails, we know which
            using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
            {
                sqlConn.Open();
                for (int i = 0; i < 5; i++)
                {
                    using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                    {
                        sqlCmd.CommandTimeout = 0;
                        sqlCmd.CommandText = GetInsertString(workspace, resourceType, language, TestUtil.Random);
                        try
                        {
                            sqlCmd.ExecuteNonQuery();
                        }
                        catch (SqlException error)
                        {
                            AstoriaTestLog.WriteLineIgnore("Error while executing: " + sqlCmd.CommandText);
                            throw;
                        }
                    }
                }
            }
        }

        public bool Remote
        {
            get { return (!_isLocal); }
        }

        public string Machine
        {
            get
            {
                if (_machine == null)
                {
                    switch (AstoriaTestProperties.DataProviderMachineName)
                    {
                        case "Lab":
                            break;

                        case "Local":
#if !USELOCALDB
                            if (ServiceUtil.IsLocalSqlServerExpressRunning)
#endif
                            {
                                _isLocal = true;
                                _machine = DataUtil.DefaultDataSource;
                            }
                            break;

                        default:
                            _machine = AstoriaTestProperties.DataProviderMachineName;
                            break;
                    }
                    _machine = _machine.Trim();
                }
                return _machine;
            }
        }

        public string DatabaseConnectionString
        {
            get
            {
                return MachineConnectionString + ";database=" + this.DatabaseName;
            }
        }

        public string DatabaseName
        {
            get
            {
                if (_databaseName == null)
                {
                    _databaseName = String.Format("{0}_{1}_{2}",
                        this.DatabasePrefixName,
                        System.Net.Dns.GetHostName().Replace('-', '_'),
                        Guid.NewGuid().ToString("N"));
                }

                return _databaseName;
            }
        }

        public string DatabasePrefixName
        {
            get
            {
                return _databasePrefixName;
            }
        }

        public string MachineConnectionString
        {
            get
            {
                if (_machineConnectionString == null)
                {
                    string machineName = this.Machine;
                    if (_isLocal)
                    {
                        _machineConnectionString = "server=" + machineName + ";Integrated Security=true;";
                    }
                    _machineConnectionString += "Connection Timeout=120;multipleactiveresultsets=true;";
                }

                return _machineConnectionString;
            }
        }

        private void CreateDatabase()
        {
            AstoriaTestLog.WriteLineIgnore("-------Create Database ---------");
            AstoriaTestLog.WriteLineIgnore("-------     DataLayerProviderMachineName: {0}---------", this.Machine);
            AstoriaTestLog.WriteLineIgnore("-------     MachineConnectionString: {0}---------", this.MachineConnectionString);
            AstoriaTestLog.WriteLineIgnore("-------     DatabaseName: {0}---------", this.DatabaseName);
            AstoriaTestLog.WriteLineIgnore("-------     DatabaseConnectionString: {0}---------", this.DatabaseConnectionString);
            if (IsEmpty)
            {
                CreateEmptyDatabase();
            }
            else
            {
                CreatePopulatedDatabase();
            }
        }

        public void Dispose()
        {
            if (Remote)
            {
                DropDatabase();
            }
        }

        public void Restore()
        {
            AstoriaTestLog.WriteLineIgnore("-------Restoring Database ---------");
            AstoriaTestLog.WriteLineIgnore("-------     DatabaseConnectionString: {0}---------", this.DatabaseConnectionString);

            if (IsEmpty)
            {
                CreateEmptyDatabase();
            }
            else
            {
                CreatePopulatedDatabase();
            }
        }

        public bool IsEmpty
        {
            get;
            private set;
        }

        private string AttachedMdf
        {
            get
            {
                return Path.Combine(this.AttachFolder, this.DatabaseName + ".mdf");
            }
        }

        private string AttachedLdf
        {
            get
            {
                return Path.Combine(this.AttachFolder, this.DatabaseName + ".ldf");
            }
        }

        private void DeleteDatabaseFiles()
        {
            try
            {
                IOUtil.EnsureFileDeleted(Path.Combine(this.DestinationFolder, this.DatabaseName + ".mdf"));
            }
            catch (Exception e)
            {
                AstoriaTestLog.TraceInfo("Database cleanup failed for " + this.DestinationFolder + this.DatabaseName + ".mdf");
            }

            try
            {
                IOUtil.EnsureFileDeleted(Path.Combine(this.DestinationFolder, this.DatabaseName + ".ldf"));
            }
            catch (Exception e)
            {
                AstoriaTestLog.TraceInfo("Database cleanup failed for " + this.DestinationFolder + this.DatabaseName + ".ldf");
            }
        }

        private string FindDatabaseFile(string name)
        {
            string[] basePaths = new string[]
            {
                this.SourceFolder,
                Environment.GetEnvironmentVariable("DD_SuitesTarget"),
                Environment.GetEnvironmentVariable("DD_BuiltTarget")
            };

            foreach (string basePath in basePaths)
            {
                if (basePath != null)
                {
                    if (File.Exists(Path.Combine(basePath, name)))
                    {
                        return Path.Combine(basePath, name);
                    }
                    else if (File.Exists(Path.Combine(Path.Combine(basePath, "Workspaces\\Databases"), name)))
                    {
                        return Path.Combine(Path.Combine(basePath, "Workspaces\\Databases"), name);
                    }
                }
            }

            throw new TestException(TestResult.Failed, "Unable to find '" + name + "' in any of " + string.Join(",", basePaths));
        }

        /// <summary>Creates a local login for network services.</summary>
        private void CreateLocalLoginForNetworkService()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + this.Machine + ";Integrated Security=true"))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        @"IF (0 = (SELECT COUNT(*) from sys.server_principals where name = 'NT AUTHORITY\NETWORK SERVICE'))" +
                        @" CREATE LOGIN [NT AUTHORITY\NETWORK SERVICE] FROM WINDOWS";
                    command.ExecuteNonQuery();

                    command.CommandText = @"EXEC sp_addsrvrolemember 'NT AUTHORITY\NETWORK SERVICE', 'sysadmin'";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Ensures that a valid local login exists.</summary>
        private void EnsureLocalLoginExists()
        {
            CreateLocalLoginForNetworkService();
        }

        /// <summary>Create an empty database.</summary>
        private void CreateEmptyDatabase()
        {
            if (_isLocal)
            {
                EnsureLocalLoginExists();
            }

            DropDatabase();

            AstoriaTestLog.WriteLineIgnore("-------CreateEmptyDb: {0}---------", this.MachineConnectionString);
            using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    sqlCmd.CommandTimeout = 0;
                    sqlCmd.CommandText = String.Format("CREATE DATABASE [{0}];", this.DatabaseName);
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Create a populated database.</summary>
        private void CreatePopulatedDatabase()
        {
            if (_isLocal)
            {
                EnsureLocalLoginExists();
            }

            DropDatabase();

            AstoriaTestLog.WriteLineIgnore("-------CreatePopulatedDb: {0}---------", this.MachineConnectionString);
            using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    sqlCmd.CommandTimeout = 0;
                    sqlCmd.CommandText = String.Format(
                        "CREATE DATABASE [{0}]\n" +
                        " CONTAINMENT = NONE\n" +
                        " ON PRIMARY\n" +
                        "(NAME = N'{0}', FILENAME = N'{1}', SIZE = 4288KB, MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB)\n" +
                        " LOG ON\n" +
                        "(NAME = N'{0}_log', FILENAME = N'{2}', SIZE = 3456KB, MAXSIZE = 2048GB, FILEGROWTH = 10 %)",
                        this.DatabaseName,
                        this.AttachedMdf,
                        this.AttachedLdf);
                    sqlCmd.ExecuteNonQuery();
                }

                // Note: SSMS will create a file that ends with GO\n. The code
                // below expects that and won't execute an SQL fragment unless it is
                // followed by GO\n.
                String filePath = FindDatabaseFile(this.DatabasePrefixName + ".sql");
                using (StreamReader reader = new StreamReader(filePath))
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    // Read from the file until it is empty.
                    StringBuilder sqlStatements = new StringBuilder();
                    String fileLine = reader.ReadLine();
                    while (fileLine != null)
                    {
                        // If we find a "GO", execute everything we received up
                        // to this point and ignore the "GO".
                        if (fileLine == "GO")
                        {
                            if (sqlStatements.Length > 0)
                            {
                                // Get command and clean buffer.
                                String sqlStatement = sqlStatements.ToString();
                                sqlStatements.Clear();

                                // Replace database name if needed.
                                if (sqlStatement.Contains("{0}"))
                                {
                                    sqlStatement = String.Format(sqlStatement, this.DatabaseName);
                                }

                                // Execute.
                                sqlCmd.CommandText = sqlStatement;
                                sqlCmd.CommandTimeout = 0;
                                sqlCmd.ExecuteNonQuery();
                            }
                        }
                        else if (!String.IsNullOrEmpty(fileLine))
                        {
                            // Buffer all statements up until "GO".
                            sqlStatements.AppendLine(fileLine);
                        }

                        fileLine = reader.ReadLine();
                    }
                }

                AstoriaTestLog.WriteLineIgnore(String.Format("Completed Create Populated Db {0}", this.DatabaseName));
            }
        }

        public void DropDatabase()
        {
            // Ensure there are no pooled collections before we drop the database.
            SqlConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    sqlCmd.CommandTimeout = 0;

                    sqlCmd.CommandText = String.Format("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') " +
                        "BEGIN; ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}]; END;", this.DatabaseName.Replace("]", "]]"));

                    sqlCmd.ExecuteNonQuery();
                }
            }

            //AstoriaTestLog.WriteLineIgnore(String.Format("Dropped Db {0}", this.DatabaseName));
            DeleteDatabaseFiles();
        }

        private string SourceFolder
        {
            get
            {
                if (_sourceFolder == null)
                {

                    switch (AstoriaTestProperties.DataProviderMachineName)
                    {
                        case "Lab":
                            _sourceFolder = Path.Combine(Environment.CurrentDirectory, databaseFolder);
                            break;

                        case "Local":
                            _sourceFolder = Path.Combine(Environment.CurrentDirectory, databaseFolder);
                            break;

                        default:
                            _sourceFolder = Path.Combine(Environment.CurrentDirectory, databaseFolder);
                            break;
                    }
                }
                return _sourceFolder;
            }
        }
        private string DestinationFolder
        {
            get
            {
                if (_destinationFolder == null)
                {
                    switch (AstoriaTestProperties.DataProviderMachineName)
                    {
                        case "Lab":
                            _destinationFolder = Path.Combine(@"\\" + this.Machine, "attachdb");
                            break;

                        case "Local":
                            _destinationFolder = Environment.CurrentDirectory;
                            break;

                        default:
                            _destinationFolder = Path.Combine(@"\\" + this.Machine, "attachdb");
                            break;
                    }
                }
                return _destinationFolder;
            }
        }

        private string getRemoteSystemDrive()
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = sqlConn.CreateCommand();
                    sqlCmd.CommandTimeout = 0;
                    sqlCmd.CommandText = "exec master..xp_cmdshell N'echo %SYSTEMDRIVE%'";
                    String driveLetter = (String)sqlCmd.ExecuteScalar();
                    return (driveLetter);
                }
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndThrow("Unable to retrieve %SYSTEMDRIVE% of remote DB Machine");
            }
            return null;
        }

        private string AttachFolder
        {
            get
            {
                if (_attachFolder == null)
                {
                    switch (AstoriaTestProperties.DataProviderMachineName)
                    {
                        case "Lab":
                            string driveLetter = getRemoteSystemDrive();
                            _attachFolder = driveLetter + @"\AttachDB\";
                            break;

                        case "Local":
                            _attachFolder = this.DestinationFolder;
                            break;

                        default:
                            _attachFolder = this.DestinationFolder;
                            break;
                    }
                }
                return _attachFolder;
            }
        }
    }
}
