//---------------------------------------------------------------------
// <copyright file="CodeCompilerHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.Util
{
    using System.Collections.Generic;
    using Microsoft.CSharp;         //CSharpCodeProvider
    using Microsoft.VisualBasic;    //VBCodeProvider
    using System.CodeDom.Compiler;  //CompilerParameters
    using System.Data.Test.Astoria;
    using System.Linq;

    public class CodeCompilerHelper
    {
        public static Dictionary<string, string> CompilerVersion = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };

        public static void CompileCodeFiles(string[] codeFilePaths, string[] resourceFiles, string dllFilePath, string[] compilerDefines, WorkspaceLanguage language, string[] referencedAssemblies)
        {
            CompileCodeFiles(codeFilePaths, resourceFiles, dllFilePath, compilerDefines, language, CompilerVersion, referencedAssemblies, false);
        }

        public static void CompileCodeFiles(string[] codeFilePaths, string dllFilePath, string[] compilerDefines, WorkspaceLanguage language, string[] referencedAssemblies)
        {
            CompileCodeFiles(codeFilePaths, dllFilePath, compilerDefines, language, CompilerVersion, referencedAssemblies);
        }

        public static void CompileCodeFiles(string[] codeFilePaths, string dllFilePath, string[] compilerDefines, WorkspaceLanguage language, Dictionary<string, string> providerOptions, string[] referencedAssemblies)
        {
            CompileCodeFiles(codeFilePaths, dllFilePath, compilerDefines, language, CompilerVersion, referencedAssemblies, false);
        }

        public static void CompileCodeFiles(string[] codeFilePaths, string dllFilePath, string[] compilerDefines, WorkspaceLanguage language, Dictionary<string, string> providerOptions, string[] referencedAssemblies, bool compileExe)
        {
            CompileCodeFiles(codeFilePaths, null, dllFilePath, compilerDefines, language, CompilerVersion, referencedAssemblies, compileExe);
        }
        public static void CompileCodeFiles(string[] codeFilePaths, string[] resourceFiles, string dllFilePath, string[] compilerDefines, WorkspaceLanguage language, Dictionary<string, string> providerOptions, string[] referencedAssemblies, bool compileExe)
        {
            CodeDomProvider compiler;
            if (language == WorkspaceLanguage.VB)
                compiler = new VBCodeProvider(providerOptions);
            else
                compiler = new CSharpCodeProvider(providerOptions);


            //Options
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = compileExe;
            cp.IncludeDebugInformation = true;
            cp.OutputAssembly = dllFilePath;
            if (!Versioning.Server.SupportsV2Features)
                cp.CompilerOptions = "/Define:ASTORIA_PRE_V2";
            if (compilerDefines != null)
            {
                foreach (string define in compilerDefines)
                    cp.CompilerOptions += " /Define:" + define;
            }

            if (resourceFiles != null && resourceFiles.Length > 0)
            {

                foreach (string embeddedResourceFile in resourceFiles)
                    cp.CompilerOptions += " /res:" + embeddedResourceFile;
            }

            Func<string, string> resolve = delegate(string f)
            {
                if (string.Equals(DataFxAssemblyRef.File.DataServicesClient, f, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(DataFxAssemblyRef.File.ODataLib, f, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(DataFxAssemblyRef.File.DataServices, f, StringComparison.OrdinalIgnoreCase))
                {
                    return Environment.ExpandEnvironmentVariables(System.IO.Path.Combine(DataFxAssemblyRef.File.DS_ReferenceAssemblyPath, f));
                }
				
                return f;
            };

            //References
            //Note: We don't hard-code the assmeblies, but obtain the name from the referenced types
            cp.ReferencedAssemblies.Clear();
            cp.ReferencedAssemblies.AddRange(referencedAssemblies.Select(resolve).Distinct().ToArray());

            //Compile                
            CompilerResults cr = compiler.CompileAssemblyFromFile(cp, codeFilePaths);
            //Compiler Errors
            string completeError = "";
            foreach (CompilerError error in cr.Errors)
            {
                switch (error.ErrorNumber)
                {
                    case "CS0016":  //File is locked (file already loaded)
                    case "CS0042":  //Failure to generate debug information (file already loaded)
                        continue;
                };

                //Otherwise
                if (completeError.Length > 0)
                    completeError += "\r\n" + error.ToString();
                else
                    completeError = error.ToString();
            }
            if (completeError.Length > 0)
            {
                AstoriaTestLog.FailAndThrow(completeError);
            }

        }
    }
}
