//---------------------------------------------------------------------
// <copyright file="EdmGenUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Win32;

    /// <summary>This class provides a wrapper for the EdmGenUtil.exe utility.</summary>
    /// <remarks>
    /// /mode:EntityClassGeneration             Generate objects from a csdl file
    /// /mode:FromSsdlGeneration                Generate msl, csdl, and objects from an ssdl file
    /// /mode:ValidateArtifacts                 Validate the ssdl, msl, and csdl files
    /// /mode:ViewGeneration                    Generate mapping views from ssdl, msl, and csdl files
    /// /mode:FullGeneration                    Generate ssdl, msl, csdl, and objects from the database
    /// /project:<string>                       The base name to be used for all the artifact files (short form: /p)
    /// /provider:<string>                      The name of the Ado.Net data provider to be used for ssdl generation. (short form: /prov)
    /// /connectionstring:<connection string>   The connection string to the database that you would like to connect to (short form: /c)
    /// /incsdl:<file>                          The file to read the conceptual model from
    /// /refcsdl:<file>                         A csdl file that contains types that the /incsdl file is dependent upon
    /// /inmsl:<file>                           The file to read the mapping from
    /// /inssdl:<file>                          The file to read the storage model from
    /// /outcsdl:<file>                         The file to write the generated conceptual model to
    /// /outmsl:<file>                          The file to write the generated mapping to
    /// /outssdl:<file>                         The file to write the generated storage model to
    /// /outobjectlayer:<file>                  The file to write the generated object layer to
    /// /outviews:<file>                        The file to write the pre generated view objects to
    /// /language:CSharp                        Generate code using the C# language
    /// /language:VB                            Generate code using the VB language
    /// /namespace:<string>                     The namespace name to use for the conceptual model types
    /// /entitycontainer:<string>               The name to use for the EntityContainer in the conceptual model
    /// /help                                   Display the usage message (short form: /?)
    /// /nologo                                 Suppress copyright message
    /// </remarks>
    public class EdmGenUtil
    {
        /// <summary>
        /// Initializes a new <see cref="EdmGenUtil"/> instance.
        /// </summary>
        public EdmGenUtil()
        {
        }

        public string EdmConnectionString
        {
            get
            {
                System.Data.EntityClient.EntityConnectionStringBuilder builder = new System.Data.EntityClient.EntityConnectionStringBuilder();
                builder.Metadata = 
                    (this.OutCsdl ?? this.InCsdl) + "|" +
                    (this.OutMsl ?? this.InMsl) + "|" +
                    (this.OutSsdl ?? this.InSsdl);
                builder.Provider = this.Provider;
                builder.ProviderConnectionString = this.ConnectionString;
                return builder.ToString();
            }
        }

        public EdmGenUtilMode Mode { get; set; }

        /// <summary>The base name to be used for all the artifact files (short form: /p)</summary>
        public string Project { get; set; }
        
        /// <summary>The name of the Ado.Net data provider to be used for ssdl generation. (short form: /prov)</summary>
        public string Provider { get; set; }
        
        /// <summary>The connection string to the database that you would like to connect to (short form: /c)</summary>
        public string ConnectionString { get; set; }
        
        /// <summary>The file to read the conceptual model from</summary>
        public string InCsdl { get; set; }
        
        /// <summary>A csdl file that contains types that the /incsdl file is dependent upon</summary>
        public string RefCsdl { get; set; }
        
        /// <summary>The file to read the mapping from</summary>
        public string InMsl { get; set; }
        
        /// <summary>The file to read the storage model from</summary>
        public string InSsdl { get; set; }
        
        /// <summary>The file to write the generated conceptual model to</summary>
        public string OutCsdl { get; set; }
        
        /// <summary>The file to write the generated mapping to</summary>
        public string OutMsl { get; set; }
        
        /// <summary>The file to write the generated storage model to</summary>
        public string OutSsdl { get; set; }
        
        /// <summary>The file to write the generated object layer to</summary>
        public string OutObjectLayer { get; set; }
        
        /// <summary>The file to write the pre generated view objects to</summary>
        public string OutViews { get; set; }
        
        /// <summary>The namespace name to use for the conceptual model types</summary>
        public string Namespace { get; set; }
        
        /// <summary>The name to use for the EntityContainer in the conceptual model</summary>
        public string EntityContainer { get; set; }
        
        /// <summary>Path to EdmGen binary file.</summary>
        public static string EdmGenPath
        {
            get
            {
                return Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "EdmGen.exe");
            }
        }

        /// <summary>Generates code and/or mapping files..</summary>
        public void Generate()
        {
            RunWithArguments(BuildGeneralArguments());
        }

        public void SetupEntityClassGenerationFiles(string projectName, string path, string baseName)
        {
            TestUtil.CheckArgumentNotNull(projectName, "projectName");
            TestUtil.CheckArgumentNotNull(path, "path");
            TestUtil.CheckArgumentNotNull(baseName, "baseName");

            this.Mode = EdmGenUtilMode.EntityClassGeneration;
            this.Project = projectName;
            this.InCsdl = Path.Combine(path, baseName + ".csdl");
            this.InMsl = Path.Combine(path, baseName + ".msl");
            this.InSsdl = Path.Combine(path, baseName + ".ssdl");
            this.OutObjectLayer = Path.Combine(path, projectName + ".cs");
        }

        /// <summary>Sets up file names for output files.</summary>
        /// <param name="projectName"></param>
        /// <param name="path"></param>
        public void SetupOutputFiles(string projectName, string path)
        {
            TestUtil.CheckArgumentNotNull(projectName, "projectName");
            TestUtil.CheckArgumentNotNull(path, "path");

            this.Project = projectName;
            this.OutCsdl = Path.Combine(path, projectName + ".csdl");
            this.OutMsl = Path.Combine(path, projectName + ".msl");
            this.OutObjectLayer = Path.Combine(path, projectName + ".cs");
            this.OutViews = Path.Combine(path, projectName + ".Views.cs");
            this.OutSsdl = Path.Combine(path, projectName + ".ssdl");
        }

        /// <summary>
        /// Builds the argument line that is common to all operations.
        /// </summary>
        /// <returns></returns>
        private string BuildGeneralArguments()
        {
            string result = "";
            result += "/mode:" + this.Mode.ToString() + " ";
            if (!String.IsNullOrEmpty(this.Project)) result += "/project:\"" + this.Project + "\" ";
            if (!String.IsNullOrEmpty(this.Provider)) result += "/provider:\"" + this.Provider + "\" ";
            if (!String.IsNullOrEmpty(this.ConnectionString)) result += "/connectionstring:\"" + this.ConnectionString + "\" ";
            if (!String.IsNullOrEmpty(this.InCsdl)) result += "/incsdl:\"" + this.InCsdl + "\" ";
            if (!String.IsNullOrEmpty(this.RefCsdl)) result += "/refcsdl:\"" + this.RefCsdl + "\" ";
            if (!String.IsNullOrEmpty(this.InMsl)) result += "/inmsl:\"" + this.InMsl + "\" ";
            if (!String.IsNullOrEmpty(this.InSsdl)) result += "/inssdl:\"" + this.InSsdl + "\" ";
            if (!String.IsNullOrEmpty(this.OutCsdl)) result += "/outcsdl:\"" + this.OutCsdl + "\" ";
            if (!String.IsNullOrEmpty(this.OutMsl)) result += "/outmsl:\"" + this.OutMsl + "\" ";
            if (!String.IsNullOrEmpty(this.OutSsdl)) result += "/outssdl:\"" + this.OutSsdl + "\" ";
            if (!String.IsNullOrEmpty(this.OutObjectLayer)) result += "/outobjectlayer:\"" + this.OutObjectLayer + "\" ";
            if (!String.IsNullOrEmpty(this.OutViews)) result += "/outviews:\"" + this.OutViews + "\" ";
            if (!String.IsNullOrEmpty(this.Namespace)) result += "/namespace:\"" + this.Namespace + "\" ";
            if (!String.IsNullOrEmpty(this.EntityContainer)) result += "/entitycontainer:\"" + this.EntityContainer + "\" ";
            return result;
        }

        private static void RunWithArguments(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = EdmGenPath;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                string outputText = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        "Running EdmGen with arguments '" + arguments + "' failed with exit code " +
                        process.ExitCode + ".\r\n" + outputText);
                }
            }
        }

        public enum EdmGenUtilMode
        {
            EntityClassGeneration,
            FromSsdlGeneration,
            ValidateArtifacts,
            ViewGeneration,
            FullGeneration,
        }

        public enum EdmGenUtilLanguage
        {
            CSharp,
            VB
        }
    }
}
