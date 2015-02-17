//---------------------------------------------------------------------
// <copyright file="ClientQueryMethodSyntaxCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Generaters the expression tree of CodeExpression nodes that represents Linq to Astoria query for the given <see cref="QueryExpression"/> tree.
    /// </summary>
    [ImplementationName(typeof(IClientQueryCodeGenerator), "MethodSyntax", HelpText = "Method syntax query generator")]
    public class ClientQueryMethodSyntaxCodeGenerator : IClientQueryCodeGenerator
    {
        /// <summary>
        /// Gets or sets the code generator for custom initialization.
        /// </summary>
        [InjectDependency]
        public ICustomInitializationCodeGenerator CustomInitializationCodeGenerator { get; set; }

        /// <summary>
        /// Generates the tree of CodeExpression nodes that represents Linq to Astoria baseline query along with free variables bound to the query.
        /// </summary>
        /// <param name="expression">The root node of the expression tree that the resulting tree will be built from.</param>
        /// <param name="context">CodeExpression representing the context for the query.</param>
        /// <returns>Result of code generation.</returns>
        public CodeExpressionWithFreeVariables Generate(QueryExpression expression, CodeExpression context)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            var visitor = new LinqToAstoriaMethodSyntaxCodeGenerationVisitor(context, this.CustomInitializationCodeGenerator);

            return visitor.GenerateQueryCode(expression);
        }

        /// <summary>
        /// The visitor that creates CodeExpression that represents Linq query build using lambda-based syntax.
        /// </summary>    
        private class LinqToAstoriaMethodSyntaxCodeGenerationVisitor : LinqMethodSyntaxCodeGenerationVisitor, ILinqToAstoriaExpressionVisitor<CodeExpression>
        {
            internal LinqToAstoriaMethodSyntaxCodeGenerationVisitor(CodeExpression rootExpression, ICustomInitializationCodeGenerator customInitializationCodeGenerator)
                : base(rootExpression, customInitializationCodeGenerator)
            {
            }

            /// <summary>
            /// Generates System.CodeDom.CodeExpression from the given expression.
            /// </summary>
            /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
            /// <returns>
            /// Generated System.CodeDom.CodeExpression.
            /// </returns>
            public override CodeExpression Visit(LinqBuiltInFunctionCallExpression expression)
            {
                if (expression.LinqBuiltInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.InstanceProperty)
                {
                    var type = this.GetClrType(expression.Arguments[0]);
                    if (Nullable.GetUnderlyingType(type) != null)
                    {
                        var instance = this.GenerateCode(expression.Arguments[0]);
                        string propertyName = expression.LinqBuiltInFunction.MethodName;
                        return instance.Property("Value").Property(propertyName);
                    }
                }

                return base.Visit(expression);
            }

            /// <summary>
            /// Visits a LinqAddQueryOptionExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaAddQueryOptionExpression expression)
            {
                Type clrType = this.GetClrType(expression.Source);
                var source = this.GenerateQueryCode(expression.Source).Expression;

                return source.Cast(Code.GenericType("DataServiceQuery", clrType.FullName)).Call("AddQueryOption", Code.Primitive(expression.QueryOption), Code.Primitive(expression.QueryValue));
            }

            /// <summary>
            /// Visits a LinqToAstoriaConditionalExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaConditionalExpression expression)
            {
                return Code.Ternary(
                    this.GenerateQueryCode(expression.Condition).Expression,
                    this.GenerateQueryCode(expression.IfTrue).Expression,
                    this.GenerateQueryCode(expression.IfFalse).Expression);
            }

            /// <summary>
            /// Visits a LinqExpandExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaExpandExpression expression)
            {
                var source = this.GenerateQueryCode(expression.Source).Expression;

                if (!expression.IsImplicit)
                {
                    Type clrType = this.GetClrType(expression.Source);
                    return source.Cast(Code.GenericType("DataServiceQuery", clrType.FullName)).Call("Expand", Code.Primitive(expression.ExpandString));
                }

                return source;
            }

            /// <summary>
            /// Visits a LinqToAstoriaExpandLambdaExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaExpandLambdaExpression expression)
            {
                var source = this.GenerateQueryCode(expression.Source).Expression;
                var lambda = this.GenerateQueryCode(expression.Lambda).Expression as CodeLambdaExpression;

                Type clrType = this.GetClrType(expression.Source);
                return source.Cast(Code.GenericType("DataServiceQuery", clrType.FullName)).Call("Expand", lambda);
            }

            /// <summary>
            /// Visits a LinqKeyExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaKeyExpression expression)
            {
                CodeExpression source = this.GenerateQueryCode(expression.Source).Expression;

                var lambda = this.GenerateQueryCode(expression.Lambda).Expression as CodeLambdaExpression;

                return source.Call("Where", lambda);
            }

            /// <summary>
            /// Visits a LinqToAstoriaLinksExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaLinksExpression expression)
            {
                throw new TaupoNotSupportedException("Not supported");
            }

            /// <summary>
            /// Visits a LinqToAstoriaValueExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public CodeExpression Visit(LinqToAstoriaValueExpression expression)
            {
                throw new TaupoNotSupportedException("Not supported");
            }

            /// <summary>
            /// Override the default code generation for Astoria service operations
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public override CodeExpression Visit(QueryCustomFunctionCallExpression expression)
            {
                return expression.GenerateClientCode(this.RootExpression);
            }
            
            /// <summary>
            /// Gets the CLR type of the expression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The Clr type.</returns>
            private Type GetClrType(QueryExpression expression)
            {
                var type = expression.ExpressionType as IQueryClrType;
                if (type == null)
                {
                    var collectionType = expression.ExpressionType as QueryCollectionType;
                    while (collectionType != null)
                    {
                        type = collectionType.ElementType as IQueryClrType;
                        collectionType = collectionType.ElementType as QueryCollectionType;
                    }
                }

                ExceptionUtilities.CheckObjectNotNull(type, "Type is null");
                return type.ClrType;
            }
        }
    }
}
