//---------------------------------------------------------------------
// <copyright file="IProgrammingLanguageStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Strategy pattern for a given programming language (C#, VB, ...)
    /// </summary>
    [ImplementationSelector("Language", DefaultImplementation = "CSharp", HelpText = "Programming language of the generated code")]
    public interface IProgrammingLanguageStrategy
    {
        /// <summary>
        /// Gets human-readable name of the language.
        /// </summary>
        string LanguageName { get; }

        /// <summary>
        /// Gets file extension for the language with a leading period.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets project file extension for the language with a leading period.
        /// </summary>
        string ProjectFileExtension { get; }

        /// <summary>
        /// Compiles given set of source files to an assembly (dll).
        /// </summary>
        /// <param name="outputFile">Output file name (*.dll)</param>
        /// <param name="sourceContent">Content of the source files to be compiled.</param>
        /// <param name="referenceAssemblies">Reference assemblies to be used.</param>
        /// <param name="resourceFiles">Embedded resource files</param>
        void CompileAssemblyFromSource(string outputFile, string[] sourceContent, string[] referenceAssemblies, params string[] resourceFiles);

        /// <summary>
        /// Asynchronously compiles given set of source files to an assembly (dll).
        /// </summary>
        /// <param name="outputAssemblyName">Name of the output assembly.</param>
        /// <param name="sourceContent">Content of the source files to be compiled.</param>
        /// <param name="referenceAssemblies">Reference assemblies to be used.</param>
        /// <param name="callback">Action to be invoked on completion. First argument will be the assembly that has been loaded, second - exceptions (if any)</param>
        /// <param name="resourceFiles">Embedded resource files</param>
        /// <remarks>This method will not throw exceptions.</remarks>
        void CompileAssemblyAsync(string outputAssemblyName, string[] sourceContent, string[] referenceAssemblies, Action<Assembly, Exception> callback, params string[] resourceFiles);

        /// <summary>
        /// Creates the code generator based on CodeDOM.
        /// </summary>
        /// <returns>Instance of <see cref="ExtendedCodeGenerator"/> capable of generating source code in the target language.</returns>
        ExtendedCodeGenerator CreateCodeGenerator();
    }
}
