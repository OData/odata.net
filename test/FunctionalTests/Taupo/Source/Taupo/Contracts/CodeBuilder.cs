//---------------------------------------------------------------------
// <copyright file="CodeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.CodeDom;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;

    /// <summary>
    /// Dynamically builds, compiles and executes code.
    /// </summary>
    [ImplementationSelector("CodeBuilder", DefaultImplementation = "Default")]
    public abstract class CodeBuilder
    {
        /// <summary>
        /// Adds the namespace import to the current code.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        public abstract void AddNamespaceImport(string namespaceName);

        /// <summary>
        /// Adds the referenced assembly to the current code.
        /// </summary>
        /// <param name="referencedAssembly">The referenced assembly.</param>
        /// <remarks>Referenced assembly must be unqualified for system assemblies</remarks>
        public abstract void AddReferencedAssembly(string referencedAssembly);

        /// <summary>
        /// Adds the external property to the current code.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyValue">The initial property value.</param>
        /// <returns>CodeDOM <see cref="CodeExpression"/> which can be used to refer to the property in the generated code.</returns>
        public abstract CodeExpression AddExternalProperty(string propertyName, CodeTypeReference propertyType, object propertyValue);

        /// <summary>
        /// Adds new custom method to the current code.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>
        /// CodeDOM <see cref="CodeMemberMethod"/> which can be used to customize generated method.
        /// </returns>
        public abstract CodeMemberMethod BeginCustomMethod(string methodName);

        /// <summary>
        /// Ends the current custom method.
        /// </summary>
        public abstract void EndCustomMethod();

        /// <summary>
        /// Adds the code to declare a variable in the current scope.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="type">The type of the variable.</param>
        /// <param name="initExpression">The initialization expression.</param>
        /// <returns>CodeDom <see cref="CodeExpression"/> which can be used to refer to declared variable.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Declare", Justification = "Declares a variable.")]
        public abstract CodeExpression Declare(string variableName, CodeTypeReference type, CodeExpression initExpression);

        /// <summary>
        /// Begins new synchronous variation.
        /// </summary>
        /// <param name="name">The name of the variation (will be the name of the method in the generated code).</param>
        public abstract void BeginVariation(string name);

        /// <summary>
        /// Begins the asynchronous variation.
        /// </summary>
        /// <param name="name">The name of the variation (will be the name of the method in the generated code).</param>
        /// <returns>CodeDom <see cref="CodeExpression"/> which can be used to refer to asynchronous continuation parameter in the generated code.</returns>
        public abstract CodeExpression BeginAsyncVariation(string name);

        /// <summary>
        /// Ends current variation.
        /// </summary>
        public abstract void EndVariation();

        /// <summary>
        /// Gets the variable name unique to the generated code and containing string variableName 
        /// </summary>
        /// <param name="variableName">base name to construct unique variable name</param>
        /// <returns>the generated unique variable name</returns>
        public abstract string GetUniqueVariableName(string variableName);

        /// <summary>
        /// Runs all variations synchronously.
        /// </summary>
        public abstract void RunVariations();

        /// <summary>
        /// Runs all variations and executes the specified continuation at the end.
        /// </summary>
        /// <param name="continuation">The continuation to execute at the end of execution.</param>
        public abstract void RunVariations(IAsyncContinuation continuation);

        /// <summary>
        /// Resets the state of the code builder and prepares for new code to be built.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Adds the specified statement to current scope.
        /// </summary>
        /// <param name="statement">The statement to add.</param>
        public abstract void Add(CodeStatement statement);

        /// <summary>
        /// Adds an assertion to the current block
        /// </summary>
        /// <param name="assertionMethod">Method to be called on assertion handler</param>
        /// <param name="arguments">Arguments to pass to method</param>
        public void AddAssertion(string assertionMethod, params CodeExpression[] arguments)
        {
            this.Add(Code.This().Property("Assert").Call(assertionMethod, arguments));
        }
       
        /// <summary>
        /// Adds the conditional statement to the current scope and enters "then" scope.
        /// </summary>
        /// <param name="condition">The conditional expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "If", Justification = "This is to construct if() { }")]
        public abstract void If(CodeExpression condition);

        /// <summary>
        /// Exits current "then" scope and enters the "else" scope of current conditional statement.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Else", Justification = "This is to construct else { } block")]
        public abstract void Else();

        /// <summary>
        /// Ends current conditional statement scope ("then" or "else") and moves back to the previous scope.
        /// </summary>
        public abstract void EndIf();

        /// <summary>
        /// Begins the new scope which will add statements to given lambda body.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        public abstract void BeginLambda(CodeLambdaExpression lambda);

        /// <summary>
        /// Ends the lambda scope.
        /// </summary>
        public abstract void EndLambda();

        /// <summary>
        /// Adds the external property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>CodeDOM <see cref="CodeExpression"/> which can be used to refer to the property in the generated code.</returns>
        public CodeExpression AddExternalProperty<TProperty>(string propertyName, TProperty propertyValue)
        {
            return this.AddExternalProperty(propertyName, Code.TypeRef<TProperty>(), propertyValue);
        }

        /// <summary>
        /// Adds the specified expression code to current scope.
        /// </summary>
        /// <param name="expression">The expression to add.</param>
        public void Add(CodeExpression expression)
        {
            this.Add(new CodeExpressionStatement(expression));
        }

        /// <summary>
        /// Adds the specified logging statement to the scope.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="arguments">The message arguments.</param>
        public void AddLog(LogLevel logLevel, string message, params CodeExpression[] arguments)
        {
            this.AddLog(logLevel, Code.Primitive(message), arguments);
        }

        /// <summary>
        /// Adds the specified logging statement to the scope.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="arguments">The message arguments.</param>
        public void AddLog(LogLevel logLevel, CodeExpression message, params CodeExpression[] arguments)
        {
            List<CodeExpression> expressions = new List<CodeExpression>();
            expressions.Add(Code.EnumValue(logLevel));
            expressions.Add(message);
            expressions.AddRange(arguments);
            this.Add(Code.This().Property("Log").Call("WriteLine", expressions.ToArray()));
        }

        /// <summary>
        /// Declares and initializes the implicitly typed variable ('var' in C#)
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="initExpression">The initialization expression.</param>
        /// <returns>CodeDom <see cref="CodeExpression"/> which can be used to refer to declared variable.</returns>
        public CodeExpression Declare(string variableName, CodeExpression initExpression)
        {
            return this.Declare(variableName, new CodeImplicitTypeReference(), initExpression);
        }

        /// <summary>
        /// Adds a comment to the generated source
        /// </summary>
        /// <param name="comment">Comment to be added</param>
        public void AddComment(string comment)
        {
            this.Add(new CodeCommentStatement(comment));
        }
    }
}