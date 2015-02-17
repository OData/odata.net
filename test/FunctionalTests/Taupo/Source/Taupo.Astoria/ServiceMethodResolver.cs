//---------------------------------------------------------------------
// <copyright file="ServiceMethodResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of IServiceMethodResolver, resolves runtime dependencies for service methods.
    /// </summary>
    public class ServiceMethodResolver : IServiceMethodResolver
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [InjectDependency(IsRequired = true)]
        public IProgrammingLanguageStrategy Language { get; set; }

        /// <summary>
        /// Gets or sets the identity generator.
        /// </summary>
        /// <value>The identity generator.</value>
        [InjectDependency(IsRequired = true)]
        public IIdentifierGenerator IdentifierGenerator { get; set; }

        /// <summary>
        /// Resolves service method body.
        /// </summary>
        /// <param name="serviceMethod">the service method</param>
        public void ResolveServiceMethodBody(Function serviceMethod)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            FunctionBodyAnnotation body = serviceMethod.Annotations.OfType<FunctionBodyAnnotation>().SingleOrDefault();

            if (body != default(FunctionBodyAnnotation) && body.FunctionBodyGenerator != null)
            {
                body.FunctionBody = body.FunctionBodyGenerator(serviceMethod.Model);
            }
        }

        /// <summary>
        /// Generates service method code.
        /// </summary>
        /// <param name="serviceMethod">the service method</param>
        public void GenerateServiceMethodCode(Function serviceMethod)
        {
            var body = serviceMethod.Annotations.OfType<FunctionBodyAnnotation>().SingleOrDefault();

            serviceMethod.Annotations.RemoveAll(a => a is MethodCodeAnnotation);
            string methodCode = string.Empty;

            // Generate the method code - CodeDom annotations take preference over FunctionBodyAnnotations (QueryExpressions)
            CodeDomBodyAnnotation codeAnnotation = serviceMethod.Annotations.OfType<CodeDomBodyAnnotation>().SingleOrDefault();
            if (codeAnnotation != default(CodeDomBodyAnnotation))
            {
                methodCode = this.GenerateMethodCode(codeAnnotation.Statements, serviceMethod.Model);
            }
            else if (body != default(FunctionBodyAnnotation))
            {
                methodCode = this.GenerateMethodCode(body.FunctionBody, serviceMethod.Model);
            }

            serviceMethod.Add(new MethodCodeAnnotation { Code = methodCode });
        }

        /// <summary>
        /// Generates service method code from a QueryExpression
        /// </summary>
        /// <param name="expression">the QueryExpression</param>
        /// <param name="model">the EntityModelSchema</param>
        /// <returns>the service method code</returns>
        protected virtual string GenerateMethodCode(QueryExpression expression, EntityModelSchema model)
        {
            // Generate this code:
            //
            // return this.<context>.<expression>;
            var codeExpression = this.GenerateCodeExpressionFromQuery(expression, this.GetContextExpression());

            var rewritten = this.RewriteCodeIfNeeded(codeExpression.Expression, model);
            var finalExpression = Code.Return(rewritten);

            var codeGenerator = this.Language.CreateCodeGenerator();
            return codeGenerator.GenerateCodeFromStatement(finalExpression);
        }

        /// <summary>
        /// Gets the context expression.
        /// </summary>
        /// <returns>The context expression</returns>
        protected virtual CodeExpression GetContextExpression()
        {
            // this.CurrentDataSource;
            return Code.This().Property("CurrentDataSource");
        }

        /// <summary>
        /// Rewrites the code if needed.
        /// </summary>
        /// <param name="toRewrite">The code expression to rewrite.</param>
        /// <param name="model">The model.</param>
        /// <returns>The rewritten expression</returns>
        protected virtual CodeExpression RewriteCodeIfNeeded(CodeExpression toRewrite, EntityModelSchema model)
        {
            return toRewrite;
        }

        /// <summary>
        /// Generates the service method code from CodeDom
        /// </summary>
        /// <param name="statements">a collection of CodeDom statements</param>
        /// <param name="model">the EntityModelSchema</param>
        /// <returns>the service method code</returns>
        protected virtual string GenerateMethodCode(IEnumerable<CodeStatement> statements, EntityModelSchema model)
        {
            ExtendedCodeGenerator codeGen = this.Language.CreateCodeGenerator();
            return string.Join(Environment.NewLine, statements.Select(s => codeGen.GenerateCodeFromStatement(s)));
        }

        /// <summary>
        /// Generates CodeDom expression from a QueryExpression
        /// </summary>
        /// <param name="query">the QueryExpression</param>
        /// <param name="context">the context in which the query occurs</param>
        /// <returns>the CodeDom expression</returns>
        protected CodeExpressionWithFreeVariables GenerateCodeExpressionFromQuery(QueryExpression query, CodeExpression context)
        {
            ExceptionUtilities.CheckArgumentNotNull(this.Language, "language");
            ExceptionUtilities.CheckObjectNotNull(query, "Function Body has not been specified");

            var methodSyntaxGenerator = new ClientQueryMethodSyntaxCodeGenerator();
            var parameterResolver = new LinqToAstoriaParameterNameResolutionVisitor(this.IdentifierGenerator);

            return methodSyntaxGenerator.Generate(parameterResolver.ResolveParameterNames(query), context);
        }
    }
}
