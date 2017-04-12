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
    using Microsoft.Test.ModuleCore;
    using System.Security;

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
                    _databaseName = String.Format("{0}_{1}_{2}", this.DatabasePrefixName, System.Net.Dns.GetHostName(), Guid.NewGuid().ToString("N"));
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
                CreateEmptyDatabase();
            else
                AttachDatabase();
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
                CreateEmptyDatabase();
            else
                AttachDatabase();
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
                AstoriaTestLog.TraceInfo("Database cleanup failed for " + this.DestinationFolder + this.DatabaseName + ".mdf");
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

        private void CopyDatabaseFiles()
        {
            string sourceMDFFileName = String.Format(@"{0}.mdf", this.DatabasePrefixName);
            string sourceLDFFileName = String.Format(@"{0}.ldf", this.DatabasePrefixName);

            string destinationMDFFileName = String.Format(@"{0}.mdf", this.DatabaseName);
            string destinationLDFFileName = String.Format(@"{0}.ldf", this.DatabaseName);

            string destinationMDF = Path.Combine(this.DestinationFolder , destinationMDFFileName);
            string destinationLDF = Path.Combine(this.DestinationFolder, destinationLDFFileName);

            DeleteDatabaseFiles();

            destinationMDF = ConvertIfIpv6(destinationMDF);
            destinationLDF = ConvertIfIpv6(destinationLDF);

            FileCopy(FindDatabaseFile(sourceMDFFileName), destinationMDF);
            FileCopy(FindDatabaseFile(sourceLDFFileName), destinationLDF);
        }

        /// <summary>Copies an existing file to a new file.</summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="destinationPath">The name of the destination file. This cannot be a directory or an existing file.</param>
        private static void FileCopy(string sourcePath, string destinationPath)
        {
            Debug.Assert(sourcePath != null, "sourcePath != null");
            Debug.Assert(destinationPath != null, "destinationPath != null");
            AstoriaTestLog.WriteLineIgnore("Copying \"" + sourcePath + "\" to \"" + destinationPath + "\"...");

            bool impersonate = true;
            Account account = new Account(null, null, null);

            //If its running local no need to either
            if (AstoriaTestProperties.DataProviderMachineName.ToLower().Equals("local")) {
                impersonate = false;
            }

            ImpersonationWrapper.DoAction(account,impersonate,
               delegate
               {
                   File.Copy(sourcePath, destinationPath);
                   if (CompactUtil.IsFileCompressed(destinationPath))
                   {
                       AstoriaTestLog.WriteLineIgnore("Uncompressing '" + destinationPath + "'...");
                       CompactUtil.UncompressFile(destinationPath);
                   }
               });
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertIfIpv6(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("\\\\"))
                return path;

            //
            //Split string and look for ipv6 portion
            //
            string ipv6Address = null;
            string[] splitPath = path.Split(new Char[] { '\\' });
            foreach (string s in splitPath)
            {
                if (s.Contains(":"))
                {
                    ipv6Address = s;
                    break;
                }
            }

            //
            //  Ensure IpV6 address was actually found to work with
            //
            if (string.IsNullOrEmpty(ipv6Address))
                return path;

            //
            //  When using ipv6 in an UNC, the ipv6 address needs to be converted to a UNC compatible format
            //  for example, given an ipv6 address
            //                          2001:4898:f0:f019:503f:8967:7f95:c381
            //  it  needs to change to the following
            //                          2001-4898-f0-f019-503f-8967-7f95-c381.ipv6-literal.net
            //  Basically, replace all ':' with '-' and append 'ipv6-literal.net' to the address
            //
            string firstStringStep = ipv6Address.Replace(":", "-");
            string lastStringStep = string.Concat(firstStringStep, ".ipv6-literal.net");
            return (path.Replace(ipv6Address, lastStringStep));
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

        private void AttachDatabase()
        {
            if (_isLocal)
            {
                EnsureLocalLoginExists();
            }

            DropDatabase();

            AstoriaTestLog.WriteLineIgnore("-------AttachDb: {0}---------", this.MachineConnectionString);
            using (SqlConnection sqlConn = new SqlConnection(this.MachineConnectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    sqlCmd.CommandTimeout = 0;

                    CopyDatabaseFiles();

                    sqlCmd.CommandText = "sp_attach_db";
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add(new SqlParameter("@dbname", this.DatabaseName));
                    sqlCmd.Parameters.Add(new SqlParameter("@filename1", AttachedMdf));
                    sqlCmd.Parameters.Add(new SqlParameter("@filename2", AttachedLdf));
                    sqlCmd.ExecuteNonQuery();
                    AstoriaTestLog.WriteLineIgnore(String.Format("Completed Attaching Db {0}", this.DatabaseName));
                }
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
