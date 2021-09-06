//---------------------------------------------------------------------
// <copyright file="CSharpLanguageStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.ProgrammingLanguages
{
    using System.CodeDom.Compiler;
    using System.Security;
    using Microsoft.CSharp;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="IProgrammingLanguageStrategy"/> for C#.
    /// </summary>
    [ImplementationName(typeof(IProgrammingLanguageStrategy), "CSharp", HelpText = "Generate C# source code")]
    public class CSharpLanguageStrategy : LanguageStrategyBase, IProgrammingLanguageStrategy
    {
        /// <summary>
        /// Initializes a new instance of the CSharpLanguageStrategy class.
        /// </summary>
        public CSharpLanguageStrategy()
            : base("C#", ".cs", ".csproj")
        {
        }

        /// <summary>
        /// Creates the code generator based on CodeDOM.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="ICodeGenerator"/> capable of generating source code in the target language.
        /// </returns>
        public override ExtendedCodeGenerator CreateCodeGenerator()
        {
            return new ExtendedCSharpCodeGenerator();
        }

        /// <summary>
        /// Creates the CodeDOM code provider for the language.
        /// </summary>
        /// <returns>CodeDOM provider for the language.</returns>
        [SecuritySafeCritical]
        protected override CodeDomProvider CreateCodeProvider()
        {
            return new CSharpCodeProvider();
        }
    }
}
