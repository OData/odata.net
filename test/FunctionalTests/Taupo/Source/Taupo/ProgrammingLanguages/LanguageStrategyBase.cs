//---------------------------------------------------------------------
// <copyright file="LanguageStrategyBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.ProgrammingLanguages
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.WebServices.CompilerService.DotNet;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Base class for <see cref="CSharpLanguageStrategy"/> and <see cref="VisualBasicLanguageStrategy"/>.
    /// </summary>
    public abstract class LanguageStrategyBase : IProgrammingLanguageStrategy
    {
        /// <summary>
        /// Initializes a new instance of the LanguageStrategyBase class.
        /// </summary>
        /// <param name="languageName">Name of the language.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="projectFileExtension">The project file extension.</param>
        protected LanguageStrategyBase(string languageName, string fileExtension, string projectFileExtension)
        {
            this.Logger = Logger.Null;
            this.LanguageName = languageName;
            this.FileExtension = fileExtension;
            this.ProjectFileExtension = projectFileExtension;
            this.TempSourceFileDirectory = IOHelpers.CreateTempDirectory("SourcesToCompile");
            this.TempAssemblyDirectory = IOHelpers.CreateTempDirectory("CompiledAsemblies");
            this.RemoteCompilerType = "DotNet40";
            this.AssemblyPathResolver = new AssemblyPathResolver();
        }

        /// <summary>
        /// Gets or sets the logger to use to print debug information.
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether compilation should use Release settings.
        /// </summary>
        [InjectTestParameter("IsReleaseBuild", HelpText = "Determines whether compiled assemblies should be optimized.")]
        public bool IsReleaseBuild { get; set; }

        /// <summary>
        /// Gets or sets the name of the remote compiler host.
        /// </summary>
        [InjectDependency]
        public ICompilerService CompilerService { get; set; }

        /// <summary>
        /// Gets or sets the assembly Path Resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAssemblyPathResolver AssemblyPathResolver { get; set; }

        /// <summary>
        /// Gets or sets the type of the remote compiler.
        /// </summary>
        [InjectTestParameter("RemoteCompilerType", HelpText = "Type of the remote compiler (such as 'Silverlight4')")]
        public string RemoteCompilerType { get; set; }

        /// <summary>
        /// Gets language name
        /// </summary>
        public string LanguageName { get; private set; }

        /// <summary>
        /// Gets file extension
        /// </summary>
        public string FileExtension { get; private set; }

        /// <summary>
        /// Gets file extension for project files.
        /// </summary>
        public string ProjectFileExtension { get; private set; }

        /// <summary>
        /// Gets or sets the directory where temporary source files should be stored.
        /// </summary>
        [InjectTestParameter("TempSourceFileDirectory", DefaultValueDescription = @".\tmp\sources", HelpText = "Directory where temporary source files should be stored.")]
        public string TempSourceFileDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory where temporary assembly files should be stored.
        /// </summary>
        [InjectTestParameter("TempAssemblyDirectory", DefaultValueDescription = @".\tmp\assemblies", HelpText = "Directory where temporary assembly files should be stored.")]
        public string TempAssemblyDirectory { get; set; }

        /// <summary>
        /// Creates the code generator based on CodeDOM.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="ExtendedCodeGenerator"/> capable of generating source code in the target language.
        /// </returns>
        public abstract ExtendedCodeGenerator CreateCodeGenerator();

        /// <summary>
        /// Asynchronously compiles given set of source files to an assembly (dll).
        /// </summary>
        /// <param name="outputAssemblyName">Name of the output assembly.</param>
        /// <param name="sourceContent">Content of the source files to be compiled.</param>
        /// <param name="referenceAssemblies">Reference assemblies to be used.</param>
        /// <param name="callback">Action to be invoked on completion. First argument will be the assembly that has been loaded, second - exceptions (if any)</param>
        /// <param name="resourceFiles">Embedded resource files</param>
        /// <remarks>This method will not throw exceptions.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exceptions here.")]
        public void CompileAssemblyAsync(string outputAssemblyName, string[] sourceContent, string[] referenceAssemblies, Action<Assembly, Exception> callback, params string[] resourceFiles)
        {
            if (this.CompilerService == null)
            {
                try
                {
                    this.EnsureTempDirectoriesExist();

                    string outputFile = Path.Combine(this.TempAssemblyDirectory, outputAssemblyName + ".dll");
                    this.Logger.WriteLine(LogLevel.Trace, "Compiling {0} files to {1} using {2} reference assemblies:", sourceContent.Length, outputFile, referenceAssemblies.Length);

                    string[] resolvedReferenceAssemblies = this.ResolveReferenceAssemblies(referenceAssemblies);
                    this.CompileAssemblyFromSource(outputFile, sourceContent, resolvedReferenceAssemblies, resourceFiles);

                    var uniqueReferenceAssemblies = resolvedReferenceAssemblies
                        .Select(c => Path.GetDirectoryName(c))
                        .Concat(new[] { this.TempAssemblyDirectory })
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Distinct()
                        .ToArray();

                    var assembly = AssemblyHelpers.LoadAssembly(outputFile, uniqueReferenceAssemblies);
                    callback(assembly, null);
                }
                catch (Exception ex)
                {
                    callback(null, ex);
                }

                return;
            }

            try
            {
                if (resourceFiles != null && resourceFiles.Length > 0)
                {
                    callback(null, new TaupoArgumentException("cannot handle compiling embedded resources, resource files should not be specified. "));
                    return;
                }

                ICompilerService client = this.CreateRemoteCompiler();

                this.Logger.WriteLine(LogLevel.Trace, "Remotely compiling {0} files to {1} using {2} reference assemblies:", sourceContent.Length, outputAssemblyName, referenceAssemblies.Length);
                foreach (var refAsm in referenceAssemblies)
                {
                    this.Logger.WriteLine(LogLevel.Trace, "   {0}", refAsm);
                }

                client.BeginCompileAssembly(
                    this.RemoteCompilerType,
                    this.FileExtension,
                    this.ProjectFileExtension,
                    outputAssemblyName,
                    sourceContent,
                    referenceAssemblies,
                    result =>
                    {
                        try
                        {
                            string errorLog;
                            byte[] assemblyBytes = client.EndCompileAssembly(out errorLog, result);
                            if (assemblyBytes == null)
                            {
                                callback(null, new TaupoInvalidOperationException("Error compiling assembly: " + errorLog));
                                return;
                            }

                            string cachePath = Path.Combine(this.TempAssemblyDirectory, outputAssemblyName + ".dll");
                            this.EnsureTempDirectoriesExist();

                            File.WriteAllBytes(cachePath, assemblyBytes);
                            var assembly = AssemblyHelpers.LoadAssembly(cachePath, this.TempAssemblyDirectory);
                            callback(assembly, null);
                        }
                        catch (Exception ex)
                        {
                            callback(null, ex);
                        }
                    },
                    null);
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
        }

        /// <summary>
        /// Compiles given set of source files to an assembly (dll).
        /// </summary>
        /// <param name="outputFile">Output file name (*.dll)</param>
        /// <param name="sourceContent">Content of the source files to be compiled.</param>
        /// <param name="referenceAssemblies">Reference assemblies to be used.</param>
        /// <param name="resourceFiles">Embedded resource files</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Writing to files demands FileIOPermission (Write flag) to the path of the file being written.")]
        public void CompileAssemblyFromSource(string outputFile, string[] sourceContent, string[] referenceAssemblies, params string[] resourceFiles)
        {
            referenceAssemblies = referenceAssemblies ?? new string[0];

            if (this.CompilerService != null)
            {
                // invoke remote compiler synchronously
                ICompilerService client = this.CreateRemoteCompiler();
                string errorLog;

                string outputName = Path.GetFileNameWithoutExtension(outputFile);

                this.Logger.WriteLine(LogLevel.Trace, "Remotely compiling {0} files to {1} using {2} reference assemblies:", sourceContent.Length, outputName, referenceAssemblies.Length);
                foreach (var refAsm in referenceAssemblies)
                {
                    this.Logger.WriteLine(LogLevel.Trace, "   {0}", refAsm);
                }

                byte[] assemblyBytes = client.CompileAssembly(out errorLog, this.RemoteCompilerType, this.FileExtension, this.ProjectFileExtension, outputFile, sourceContent, referenceAssemblies);
                if (assemblyBytes == null)
                {
                    throw new TaupoInvalidOperationException("Errors during compilation:\r\n\r\n" + errorLog);
                }

                File.WriteAllBytes(outputFile, assemblyBytes);
            }
            else
            {
                CodeDomProvider codeProvider = this.CreateCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                string[] resolvedReferenceAssemblies = this.ResolveReferenceAssemblies(referenceAssemblies);
                parameters.ReferencedAssemblies.AddRange(resolvedReferenceAssemblies);

                parameters.WarningLevel = 4;
                parameters.OutputAssembly = outputFile;
                parameters.IncludeDebugInformation = !this.IsReleaseBuild;
                parameters.CompilerOptions = this.IsReleaseBuild ? "/optimize" : null;

                List<string> fileNames = new List<string>();
                this.EnsureTempDirectoriesExist();

                foreach (var source in sourceContent)
                {
                    string tempFileName = Path.GetFullPath(Path.Combine(this.TempSourceFileDirectory, Guid.NewGuid().ToString("N") + this.FileExtension));
                    File.WriteAllText(tempFileName, source);
                    fileNames.Add(tempFileName);
                }

                foreach (var resource in resourceFiles)
                {
                    parameters.EmbeddedResources.Add(resource);
                }

                this.Logger.WriteLine(LogLevel.Trace, "Compiling {0} files to {1} using {2} reference assemblies:", sourceContent.Length, outputFile, referenceAssemblies.Length);
                this.Logger.WriteLine(LogLevel.Trace, "Source files:");
                foreach (var sourceFile in fileNames)
                {
                    this.Logger.WriteLine(LogLevel.Trace, "   {0}", sourceFile);
                }

                this.Logger.WriteLine(LogLevel.Trace, "Embedded resource files:");
                foreach (var resource in resourceFiles)
                {
                    this.Logger.WriteLine(LogLevel.Trace, "   {0}", resource);
                }

                this.Logger.WriteLine(LogLevel.Trace, "Reference assemblies:");
                foreach (var refAsm in referenceAssemblies)
                {
                    this.Logger.WriteLine(LogLevel.Trace, "   {0}", refAsm);
                }

                var results = codeProvider.CompileAssemblyFromFile(parameters, fileNames.ToArray());
                if (results.Errors.HasErrors)
                {
                    throw new TaupoInvalidOperationException(this.GetExceptionText(results));
                }
            }
        }

        /// <summary>
        /// Creates the CodeDOM code provider for the language.
        /// </summary>
        /// <returns>CodeDOM provider for the language.</returns>
        protected abstract CodeDomProvider CreateCodeProvider();

        private static string GetErrorMessageText(CompilerError c)
        {
            return c.FileName + "(" + c.Line + "," + c.Column + ") " + "error" + " " + c.ErrorNumber + " " + c.ErrorText;
        }

        private void EnsureTempDirectoriesExist()
        {
            IOHelpers.EnsureDirectoryExists(this.TempAssemblyDirectory);
            IOHelpers.EnsureDirectoryExists(this.TempSourceFileDirectory);
        }

        private string[] ResolveReferenceAssemblies(string[] referenceAssemblies)
        {
            string[] resolvedReferenceAssemblies = referenceAssemblies.Select(asm => this.AssemblyPathResolver.ResolveAssemblyLocation(asm)).ToArray();
            
            this.Logger.WriteLine(LogLevel.Trace, "Reference Assemblies:");
            foreach (var refAsm in resolvedReferenceAssemblies)
            {
                this.Logger.WriteLine(LogLevel.Trace, "   {0}", refAsm);
            }

            return resolvedReferenceAssemblies;
        }

        private string GetExceptionText(CompilerResults results)
        {
            return "Compilation Failed:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().Select(c => GetErrorMessageText(c)).ToArray());
        }

        private ICompilerService CreateRemoteCompiler()
        {
            return this.CompilerService;
        }
    }
}
