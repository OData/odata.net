//---------------------------------------------------------------------
// <copyright file="TextTemplateTransformationRunner.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Text template transformation (T4) runner which uses 'TextTransform.exe'.
    /// </summary>
    public class TextTemplateTransformationRunner : ITextTemplateTransformation
    {
        private string executablePath;

        /// <summary>
        /// Initializes a new instance of the TextTemplateTransformationRunner class.
        /// </summary>
        public TextTemplateTransformationRunner()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public Logger Logger { get; set; }

        /// <summary>
        /// Runs the specified T4 template.
        /// </summary>
        /// <param name="templateFile">T4 template to run</param>
        /// <param name="outputFile">Output file</param>
        /// <param name="parameters">Named parameters to the template</param>
        /// <remarks>Parameter value can be read by the template with Host.ResolveParameterValue("","","parameterName")</remarks>
        [SecuritySafeCritical]
        public void Transform(string templateFile, string outputFile, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            sb.Append(templateFile);
            sb.Append("\" -out \"");
            sb.Append(outputFile);
            sb.Append("\"");

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    sb.Append(" -a \"!!");
                    sb.Append(kvp.Key);
                    sb.Append("!");
                    sb.Append(kvp.Value);
                    sb.Append("\"");
                }
            }

            ProcessStartInfo psi = new ProcessStartInfo(this.FindExecutablePath(), sb.ToString());
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            this.Logger.WriteLine(LogLevel.Verbose, "Running {0} {1} in {2}", psi.FileName, psi.Arguments, psi.WorkingDirectory);
            using (Process proc = Process.Start(psi))
            {
                string errors = proc.StandardError.ReadToEnd();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new TaupoInvalidOperationException("Error processing template:\n\n" + errors);
                }
            }
        }

        /// <summary>
        /// Attempts to locate TextTransform.exe in order to process the T4 templates.
        /// </summary>
        /// <returns>The full file path of TextTransform.exe, if found. Otherwise, null.</returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CommonProgramFiles", Justification = "Name of environment variable")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TextTemplating", Justification = "Name of a folder")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TextTransform", Justification = "Name of a file")]
        [AssertJustification("Calling Environment.ExpandEnvironmentVariables demands EnvironmentPermission (Read flag) for all environment variables that this method reads, and calling File.Exists demands FileIOPermission (Read flag) for the file path in question.")]
        private string FindExecutablePath()
        {
            if (this.executablePath != null)
            {
                return this.executablePath;
            }
            
            string[] possibleLocations = new string[] 
            {
                @"%CommonProgramFiles%\microsoft shared\TextTemplating\10.0\TextTransform.exe",
                @"%CommonProgramFiles(x86)%\microsoft shared\TextTemplating\10.0\TextTransform.exe",
            };

            foreach (string location in possibleLocations)
            {
                string fullName = Environment.ExpandEnvironmentVariables(location);
                if (File.Exists(fullName))
                {
                    this.executablePath = fullName;
                    return fullName;
                }
            }

            throw new TaupoInfrastructureException("TextTransform.exe not found in the following locations:\n\n" + string.Join("\n", possibleLocations));
        }
    }
}
