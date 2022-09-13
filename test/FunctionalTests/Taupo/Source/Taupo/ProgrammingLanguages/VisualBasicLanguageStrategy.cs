//---------------------------------------------------------------------
// <copyright file="VisualBasicLanguageStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.ProgrammingLanguages
{
    using System.CodeDom.Compiler;
    using System.Security;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Implementation of <see cref="IProgrammingLanguageStrategy"/> for Visual Basic.
    /// </summary>
    [ImplementationName(typeof(IProgrammingLanguageStrategy), "VB", HelpText = "Generate Visual Basic source code")]
    public class VisualBasicLanguageStrategy : LanguageStrategyBase, IProgrammingLanguageStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicLanguageStrategy"/> class.
        /// </summary>
        public VisualBasicLanguageStrategy()
            : base("VB", ".vb", ".vbproj")
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
            return new ExtendedVBCodeGenerator();
        }

        /// <summary>
        /// Creates the CodeDOM code provider for the language.
        /// </summary>
        /// <returns>CodeDOM provider for the language.</returns>
        [SecuritySafeCritical]
        protected override CodeDomProvider CreateCodeProvider()
        {
            return new VBCodeProvider();
        }
    }
}