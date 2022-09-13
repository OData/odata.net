//---------------------------------------------------------------------
// <copyright file="ExtendedCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.ProgrammingLanguages;

    /// <summary>
    /// An extended <see cref="ICodeGenerator"/> that allows use in partial trust.
    /// </summary>
    /// <remarks>
    /// This class intentionally does not extend <see cref="ICodeGenerator"/>.
    /// All of <see cref="ICodeGenerator"/>'s methods have a link demand and inheritance
    /// demand for full trust, which prevents use in partial trust.
    /// </remarks>
    public abstract class ExtendedCodeGenerator
    {
        // string token to identify extension method container class
        public const string StaticClassNameToken = "??STATIC??CLASS??NAME??TOKEN??";

        // string token to identity extension methods
        public const string ExtensionMethodNameToken = "??EXTENSION?METHOD??NAME??TOKEN??";

        private ICodeGenerator wrappedCodeGenerator;
        private CodeDomTreeRewriter rewriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCodeGenerator"/> class.
        /// </summary>
        protected ExtendedCodeGenerator()
        {
        }

        private CodeDomTreeRewriter Rewriter
        {
            get
            {
                if (this.rewriter == null)
                {
                    this.rewriter = this.CreateRewriter();
                }

                return this.rewriter;
            }
        }

        private ICodeGenerator WrappedCodeGenerator
        {
            get
            {
                if (this.wrappedCodeGenerator == null)
                {
                    this.wrappedCodeGenerator = this.CreateCodeGenerator();
                }

                return this.wrappedCodeGenerator;
            }
        }

        /// <summary>
        /// Creates an escaped identifier for the specified value.
        /// </summary>
        /// <param name="value">The string to create an escaped identifier for.</param>
        /// <returns>The escaped identifier for the value.</returns>
        [SecuritySafeCritical]
        public string CreateEscapedIdentifier(string value)
        {
            return this.WrappedCodeGenerator.CreateEscapedIdentifier(value);
        }

        /// <summary>
        /// Creates a valid identifier for the specified value.
        /// </summary>
        /// <param name="value">The string to generate a valid identifier for.</param>
        /// <returns>A valid identifier for the specified value.</returns>
        [SecuritySafeCritical]
        public string CreateValidIdentifier(string value)
        {
            return this.WrappedCodeGenerator.CreateValidIdentifier(value);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and
        /// outputs it to the specified a string.
        /// </summary>
        /// <param name="compileUnit">A <see cref="CodeCompileUnit"/> to generate code for.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit)
        {
            return this.GenerateCodeFromCompileUnit(compileUnit, null);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and
        /// outputs it to a string using the specified options.
        /// </summary>
        /// <param name="compileUnit">A <see cref="CodeCompileUnit"/> to generate code for.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, SafeCodeGeneratorOptions options)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.GenerateCodeFromCompileUnit(compileUnit, writer, options);
                return this.RewriteExtensionMethod(writer.ToString());
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="compileUnit">A <see cref="CodeCompileUnit"/> to generate code for.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        [SecuritySafeCritical]
        public virtual void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, SafeCodeGeneratorOptions options)
        {
            using (var wrappingWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.WrappedCodeGenerator.GenerateCodeFromCompileUnit(this.Rewriter.Rewrite(compileUnit), wrappingWriter, options);
                string rewrittenResults = this.RewriteExtensionMethod(wrappingWriter.ToString());
                writer.Write(rewrittenResults);
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) expression and outputs it to a string.
        /// </summary>
        /// <param name="expression">A <see cref="CodeExpression"/> that indicates the expression to generate code for.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromExpression(CodeExpression expression)
        {
            return this.GenerateCodeFromExpression(expression, null);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) expression and outputs it to a string
        /// using the specified options.
        /// </summary>
        /// <param name="expression">A <see cref="CodeExpression"/> that indicates the expression to generate code for.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromExpression(CodeExpression expression, SafeCodeGeneratorOptions options)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.GenerateCodeFromExpression(expression, writer, options);
                return this.RewriteExtensionMethod(writer.ToString());
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) expression and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="expression">A <see cref="CodeExpression"/> that indicates the expression to generate code for.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        [SecuritySafeCritical]
        public virtual void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, SafeCodeGeneratorOptions options)
        {
            using (StringWriter wrappedWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.WrappedCodeGenerator.GenerateCodeFromExpression(this.Rewriter.Rewrite(expression), wrappedWriter, options);
                writer.Write(this.RewriteExtensionMethod(wrappedWriter.ToString()));
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) namespace and
        /// outputs it to a string.
        /// </summary>
        /// <param name="ns">A <see cref="CodeNamespace"/> that indicates the namespace to generate code for.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromNamespace(CodeNamespace ns)
        {
            return this.GenerateCodeFromNamespace(ns, null);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) namespace and
        /// outputs it to a string using the specified options.
        /// </summary>
        /// <param name="ns">A <see cref="CodeNamespace"/> that indicates the namespace to generate code for.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromNamespace(CodeNamespace ns, SafeCodeGeneratorOptions options)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.GenerateCodeFromNamespace(ns, writer, options);
                return this.RewriteExtensionMethod(writer.ToString());
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) namespace and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="ns">A <see cref="CodeNamespace"/> that indicates the namespace to generate code for.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        [SecuritySafeCritical]
        public virtual void GenerateCodeFromNamespace(CodeNamespace ns, TextWriter writer, SafeCodeGeneratorOptions options)
        {
            using (StringWriter wrappedWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.WrappedCodeGenerator.GenerateCodeFromNamespace(this.Rewriter.Rewrite(ns), wrappedWriter, options);
                writer.Write(this.RewriteExtensionMethod(wrappedWriter.ToString()));
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) statement and
        /// outputs it to a string.
        /// </summary>
        /// <param name="statement">A <see cref="CodeStatement"/> containing the CodeDOM elements to translate.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromStatement(CodeStatement statement)
        {
            return this.GenerateCodeFromStatement(statement, null);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) statement and
        /// outputs it to a string using the specified options.
        /// </summary>
        /// <param name="statement">A <see cref="CodeStatement"/> containing the CodeDOM elements to translate.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromStatement(CodeStatement statement, SafeCodeGeneratorOptions options)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.GenerateCodeFromStatement(statement, writer, options);
                return this.RewriteExtensionMethod(writer.ToString());
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) statement and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="statement">A <see cref="CodeStatement"/> containing the CodeDOM elements to translate.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        [SecuritySafeCritical]
        public virtual void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, SafeCodeGeneratorOptions options)
        {
            using (var wrappedWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.WrappedCodeGenerator.GenerateCodeFromStatement(this.Rewriter.Rewrite(statement), wrappedWriter, options);
                writer.Write(this.RewriteExtensionMethod(wrappedWriter.ToString()));
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) type declaration and
        /// outputs it to a string.
        /// </summary>
        /// <param name="type">A <see cref="CodeTypeDeclaration"/> that indicates the type to generate code for.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromType(CodeTypeDeclaration type)
        {
            return this.GenerateCodeFromType(type, null);
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) type declaration and
        /// outputs it to a string using the specified options.
        /// </summary>
        /// <param name="type">A <see cref="CodeTypeDeclaration"/> that indicates the type to generate code for.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        /// <returns>The generated code.</returns>
        public string GenerateCodeFromType(CodeTypeDeclaration type, SafeCodeGeneratorOptions options)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.GenerateCodeFromType(type, writer, options);
                return this.RewriteExtensionMethod(writer.ToString());
            }
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) type declaration and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="type">A <see cref="CodeTypeDeclaration"/> that indicates the type to generate code for.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        [SecuritySafeCritical]
        public virtual void GenerateCodeFromType(CodeTypeDeclaration type, TextWriter writer, SafeCodeGeneratorOptions options)
        {
           using (StringWriter wrappedWriter = new StringWriter(CultureInfo.InvariantCulture))
           {
                this.WrappedCodeGenerator.GenerateCodeFromType(this.Rewriter.Rewrite(type), wrappedWriter, options);
                writer.Write(this.RewriteExtensionMethod(wrappedWriter.ToString()));
           }
        }

        /// <summary>
        /// Gets the type indicated by the specified <see cref="T:CodeTypeReference"/>.
        /// </summary>
        /// <param name="type">A <see cref="T:CodeTypeReference"/> that indicates the type to return.</param>
        /// <returns>
        /// A text representation of the specified type for the language this code generator is designed to generate code in. For example, in Visual Basic, passing in type System.Int32 will return "Integer".
        /// </returns>
        [SecuritySafeCritical]
        public string GetTypeOutput(CodeTypeReference type)
        {
            return this.RewriteExtensionMethod(this.WrappedCodeGenerator.GetTypeOutput(this.Rewriter.Rewrite(type)));
        }

        /// <summary>
        /// Gets a value that indicates whether the specified value is a valid identifier for the current language.
        /// </summary>
        /// <param name="value">The value to test for being a valid identifier.</param>
        /// <returns>
        /// true if the <paramref name="value"/> parameter is a valid identifier; otherwise, false.
        /// </returns>
        [SecuritySafeCritical]
        public bool IsValidIdentifier(string value)
        {
            return this.WrappedCodeGenerator.IsValidIdentifier(value);
        }

        /// <summary>
        /// Gets a value indicating whether the generator provides support for the language features represented by the specified <see cref="T:Compiler.GeneratorSupport"/> object.
        /// </summary>
        /// <param name="support">The capabilities to test the generator for.</param>
        /// <returns>
        /// true if the specified capabilities are supported; otherwise, false.
        /// </returns>
        [SecuritySafeCritical]
        public bool Supports(GeneratorSupport support)
        {
            return this.WrappedCodeGenerator.Supports(support);
        }

        /// <summary>
        /// Throws an exception if the specified value is not a valid identifier.
        /// </summary>
        /// <param name="value">The identifier to validate.</param>
        [SecuritySafeCritical]
        public void ValidateIdentifier(string value)
        {
            this.WrappedCodeGenerator.ValidateIdentifier(value);
        }

        /// <summary>
        /// Update extension methods in CodeDOM generated code.
        /// </summary>
        /// <param name="code">Generated code.</param>
        /// <returns>Updated code.</returns>
        public string RewriteExtensionMethod(string code)
        {
            ExceptionUtilities.CheckObjectNotNull(code, "CodeDOM generated code is null.");

            if (this.GetType() == typeof(ExtendedCSharpCodeGenerator))
            {
                // remove the [Extension()] attribure line from CSharp code
                code = code.Replace("[Extension()]", null);

                // add "static" in static class declaration
                code = code.Replace(" class " + StaticClassNameToken, " static class ");

                // add "this" for extension method binding type parameter
                code = code.Replace(ExtensionMethodNameToken + "(", "(this ");
            }
            else if (this.GetType() == typeof(ExtendedVBCodeGenerator))
            {
                // remove the tokens from VB code
                code = code.Replace(StaticClassNameToken, string.Empty);
                code = code.Replace(ExtensionMethodNameToken, string.Empty);
            }

            return code;
        }

        /// <summary>
        /// Creates the <see cref="ICodeGenerator"/> that this <see cref="ExtendedCodeGenerator"/> wraps.
        /// </summary>
        /// <returns>The wrapped <see cref="ICodeGenerator"/>.</returns>
        protected abstract ICodeGenerator CreateCodeGenerator();

        /// <summary>
        /// Creates the <see cref="CodeDomTreeRewriter"/> that rewrites
        /// custom CodeDOM expression nodes into a facility that the original
        /// CodeDOM code generator can understand.
        /// </summary>
        /// <returns>The <see cref="CodeDomTreeRewriter"/>.</returns>
        protected abstract CodeDomTreeRewriter CreateRewriter();
    }
}