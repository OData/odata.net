//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaTypeResolutionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Visitor which resolves types of Linq to Astoria specific nodes in the query expressions.
    /// </summary>
    public class LinqToAstoriaTypeResolutionVisitor : LinqTypeResolutionVisitor, ILinqToAstoriaExpressionVisitor<QueryExpression>
    {
        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaTypeResolutionVisitor class.
        /// </summary>
        /// <param name="typeLibrary">The query type library.</param>
        public LinqToAstoriaTypeResolutionVisitor(QueryTypeLibrary typeLibrary)
            : base(typeLibrary)
        {
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaAddQueryOptionExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqToAstoriaAddQueryOptionExpression(source, expression.QueryOption, expression.QueryValue, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaConditionalExpression expression)
        {
            var condition = this.ResolveTypes(expression.Condition);
            var ifTrue = this.ResolveTypes(expression.IfTrue);
            var ifFalse = this.ResolveTypes(expression.IfFalse);

            ExceptionUtilities.Assert(ifTrue.ExpressionType.IsAssignableFrom(ifFalse.ExpressionType), "The ifTrue and ifFalse expression types should be the same");
            return new LinqToAstoriaConditionalExpression(condition, ifTrue, ifFalse, ifTrue.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaExpandExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqToAstoriaExpandExpression(source, expression.ExpandString, source.ExpressionType, expression.IsImplicit);
        }

        /// <summary>
        /// Visits a LinqToAstoriaExpandLambdaExpression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public QueryExpression Visit(LinqToAstoriaExpandLambdaExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqToAstoriaExpandLambdaExpression(expression.Source, expression.Lambda, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaKeyExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            // rebuild the key in case any of the properties or values were unresolved
            return source.Key(expression.KeyProperties.Select(pair => new NamedValue(pair.Key.Name, pair.Value.ScalarValue.Value)));
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaLinksExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqToAstoriaLinksExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqToAstoriaValueExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqToAstoriaValueExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public override QueryExpression Visit(QueryCustomFunctionCallExpression expression)
        {
            var resolvedArguments = this.ResolveTypesForArguments(expression.Arguments);

            var serviceOpAnnotation = expression.Function.Annotations.OfType<LegacyServiceOperationAnnotation>().SingleOrDefault();
            if (serviceOpAnnotation != null && serviceOpAnnotation.ReturnTypeQualifier == ServiceOperationReturnTypeQualifier.Void)
            {
                // Void operations have no type to resolve
                return expression;
            }

            QueryExpression functionBody = null;
            if (expression.FunctionBody != null)
            {
                ExceptionUtilities.Assert(!this.CustomFunctionsCallStack.Contains(expression.Function), "Recursive function calls are not supported. Function: '{0}'.", expression.Function.FullName);

                this.CustomFunctionsCallStack.Push(expression.Function);
                functionBody = this.ResolveTypes(expression.FunctionBody);
                this.CustomFunctionsCallStack.Pop();
            }

            QueryType resolvedResultType = null;
            if (expression.Function.ReturnType != null)
            {
                resolvedResultType = this.GetDefaultQueryType(expression.Function.ReturnType);
            }
            else
            {
                resolvedResultType = new QueryVoidType(this.EvaluationStrategy);
            }

            var resolved = new QueryCustomFunctionCallExpression(resolvedResultType, expression.Function, functionBody, expression.IsRoot, expression.IsCalledByNameOnly, resolvedArguments);

            return resolved;
        }

        /// <summary>
        /// Resolves types for instance of QueryPropertyExpresssion.
        /// </summary>
        /// <param name="expression">Instance expression to resolve types for</param>
        /// <returns>Instance expression with resolved types</returns>
        /// <remarks>For Astoria we recgnize specific patterns to modify instance type accordingly.</remarks>
        protected override QueryExpression ResolvePropertyInstance(QueryExpression expression)
        {
            // recognize pattern: KeyExpression followed by Property - treat type of KeyExpression as singleton, rather than collection
            var keyExpression = expression as LinqToAstoriaKeyExpression;
            if (keyExpression != null)
            {
                return this.ResolveKeyExpressionForProperty(keyExpression);
            }

            // recognize pattern: KeyExpression followed by Links followed by Property - treat type of Key().Links() as singleton, rather than collection
            var linksExpression = expression as LinqToAstoriaLinksExpression;
            if (linksExpression != null)
            {
                return this.ResolvePropertyInstance(linksExpression.Source).Links();
            }

            // recognize pattern: KeyExpression followed by OfType followed by Property - treat type of Key().OfType() as singleton, rather than collection
            var typeExpression = expression as QueryOfTypeExpression;
            if (typeExpression != null)
            {
                return this.ResolvePropertyInstance(typeExpression.Source).As(typeExpression.TypeToOperateAgainst);
            }

            var asExpression = expression as QueryAsExpression;
            if (asExpression != null)
            {
                return this.ResolvePropertyInstance(asExpression.Source).As(asExpression.TypeToOperateAgainst);
            }

            return base.ResolvePropertyInstance(expression);
        }

        private LinqToAstoriaKeyExpression ResolveKeyExpressionForProperty(LinqToAstoriaKeyExpression keyExpression)
        {
            keyExpression = (LinqToAstoriaKeyExpression)this.ResolveTypes(keyExpression);
            var instanceType = this.GetInstanceTypeFromKeyExpression(keyExpression);
            return new LinqToAstoriaKeyExpression(keyExpression.Source, keyExpression.KeyProperties, instanceType);
        }

        private QueryStructuralType GetInstanceTypeFromKeyExpression(LinqToAstoriaKeyExpression keyExpression)
        {
            var collectionType = keyExpression.ExpressionType as QueryCollectionType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Key expression type was not a collection type. Type was: {0}", keyExpression.ExpressionType);

            var instanceType = collectionType.ElementType as QueryStructuralType;
            ExceptionUtilities.CheckObjectNotNull(instanceType, "Collection element type was not a structural type. Type was: {0}", collectionType.ElementType);

            return instanceType;
        }
    }
}