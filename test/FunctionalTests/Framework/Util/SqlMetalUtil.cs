//---------------------------------------------------------------------
// <copyright file="SqlMetalUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Win32;

    /// <summary>This class provides a wrapper for the sqlmetal.exe utility.</summary>
    /// <remarks>
    /// SqlMetal [options] [&lt;input file&gt;]
    /// 
    ///   Generates code and mapping for the LINQ to SQL component of the .NET framework. SqlMetal can:
    ///   - Generate source code and mapping attributes or a mapping file from a database.
    ///   - Generate an intermediate dbml file for customization from the database.
    ///   - Generate code and mapping attributes or mapping file from a dbml file.
    /// 
    /// Options:
    ///   /server:&lt;name&gt;             Database server name.
    ///   /database:&lt;name&gt;           Database catalog on server.
    ///   /user:&lt;name&gt;               Login user ID (default: use Windows Authentication).
    ///   /password:&lt;password&gt;       Login password (default: use Windows Authentication).
    ///   /conn:&lt;connection string&gt;  Database connection string. Cannot be used with /server, /database, /user or /password options.
    ///   /timeout:&lt;seconds&gt;         Timeout value to use when SqlMetal accesses the database (default: 0 which means infinite).
    /// 
    ///   /views                     Extract database views.
    ///   /functions                 Extract database functions.
    ///   /sprocs                    Extract stored procedures.
    /// 
    ///   /dbml[:file]               Output as dbml. Cannot be used with /map option.
    ///   /code[:file]               Output as source code. Cannot be used with /dbml option.
    ///   /map[:file]                Generate mapping file, not attributes. Cannot be used with /dbml option.
    /// 
    ///   /language:&lt;language&gt;       Language for source code: VB or C# (default: derived from extension on code file name).
    ///   /namespace:&lt;name&gt;          Namespace of generated code (default: no namespace).
    ///   /context:&lt;type&gt;            Name of data context class (default: derived from database name).
    ///   /entitybase:&lt;type&gt;         Base class of entity classes in the generated code (default: entities have no base class).
    ///   /pluralize                 Automatically pluralize or singularize class and member names using English language rules.
    ///   /serialization:&lt;option&gt;    Generate serializable classes: None or Unidirectional (default: None).
    ///   /provider:&lt;type&gt;           Provider type: SQLCompact, SQL2000, or SQL2005. (default: provider is determined at run time).
    /// 
    ///   &lt;input file&gt;               May be a SqlExpress mdf file, a SqlCE sdf file, or a dbml intermediate file.
    /// </remarks>
    public class SqlMetalUtil
    {
        /// <summary>Database connection string. Cannot be used with /server, /database, /user or /password options.</summary>
        private string connectionString;

        /// <summary>Output as source code. Cannot be used with /dbml option.</summary>
        private string codeFile;

        /// <summary>Namespace of generated code (default: no namespace).</summary>
        private string namespaceGenerated;

        /// <summary>May be a SqlExpress mdf file, a SqlCE sdf file, or a dbml intermediate file.</summary>
        private string inputFile;

        /// <summary>
        /// Initializes a new <see cref="SqlMetalUtil"/> instance.
        /// </summary>
        public SqlMetalUtil()
        {
        }

        /// <summary>Path to SqlMetal binary file.</summary>
        public static string SqlMetalPath
        {
            get
            {
                return Path.Combine(WindowsSdkInstallPath, @"bin\SqlMetal.exe");
            }
        }

        /// <summary>Path to Windows SDK install path.</summary>
        public static string WindowsSdkInstallPath
        {
            get
            {
                const string KeyName = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0A\WinSDKNetFxTools";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(KeyName))
                {
                    return (string)key.GetValue("InstallationFolder");
                }
            }
        }

        /// <summary>Database connection string. Cannot be used with /server, /database, /user or /password options.</summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }

        /// <summary>Output as source code. Cannot be used with /dbml option.</summary>
        public string CodeFile
        {
            get { return this.codeFile; }
            set { this.codeFile = value; }
        }

        /// <summary>Namespace of generated code (default: no namespace).</summary>
        public string NamespaceGenerated
        {
            get { return this.namespaceGenerated; }
            set { this.namespaceGenerated = value; }
        }

        /// <summary>May be a SqlExpress mdf file, a SqlCE sdf file, or a dbml intermediate file.</summary>
        public string InputFile
        {
            get { return this.inputFile; }
            set { this.inputFile = value; }
        }

        /// <summary>Generates code and/or mapping files..</summary>
        public void Generate()
        {
            RunWithArguments(BuildGeneralArguments());
        }

        /// <summary>
        /// Builds the argument line that is common to all operations.
        /// </summary>
        /// <returns></returns>
        private string BuildGeneralArguments()
        {
            string result = "";
            if (!String.IsNullOrEmpty(this.connectionString)) result += "/conn:\"" + this.connectionString + "\" ";
            if (!String.IsNullOrEmpty(this.codeFile)) result += "/code:\"" + this.codeFile + "\" ";
            if (!String.IsNullOrEmpty(this.namespaceGenerated)) result += "/namespace:\"" + this.namespaceGenerated + "\" ";
            if (!String.IsNullOrEmpty(this.inputFile)) result += "\"" + this.inputFile + "\" ";
            return result;
        }

        private static void RunWithArguments(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = SqlMetalPath;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                string outputText = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        "Running SqlMetal with arguments '" + arguments + "' failed with exit code " +
                        process.ExitCode + ".\r\n" + outputText);
                }
            }
        }
    }
}
