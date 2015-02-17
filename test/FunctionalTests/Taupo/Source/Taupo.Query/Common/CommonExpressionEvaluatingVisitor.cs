//---------------------------------------------------------------------
// <copyright file="CommonExpressionEvaluatingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Base implementation of a visitor which computes values of query expressions.
    /// </summary>
    public abstract class CommonExpressionEvaluatingVisitor : ICommonExpressionVisitor<QueryValue>
    {
        private IQueryDataSet dataSet;

        /// <summary>
        /// Initializes a new instance of the CommonExpressionEvaluatingVisitor class.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        protected CommonExpressionEvaluatingVisitor(IQueryDataSet dataSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataSet, "dataSet");
            this.dataSet = dataSet;
        }

        /// <summary>
        /// Gets the dataset for the evaluations.
        /// </summary>
        public IQueryDataSet DataSet
        {
            get { return this.dataSet; }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Evaluate(QueryExpression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryAddExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.Add(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryAndExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);

            // short circuit "And" logic if left is false (and evaluation did not throw any exceptions)
            // TODO: this is sort of a hack, we should have more solid way of check whether given EvaluationError is actual exception or just list of ingnorable exceptions
            bool leftThrewEvaluationException = left.EvaluationError != null && !string.IsNullOrEmpty(left.EvaluationError.ToString());
            if (!left.IsNull && (bool)left.Value == false && !leftThrewEvaluationException)
            {
                return left;
            }

            return left.AndAlso(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryAsExpression expression)
        {
            QueryValue source = this.Evaluate(expression.Source);
            QueryType resultType = expression.TypeToOperateAgainst;

            return source.TreatAs(resultType);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryCastExpression expression)
        {
            var source = this.Evaluate(expression.Source);
            var type = expression.TypeToOperateAgainst;

            return source.Cast(type);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryConstantExpression expression)
        {
            return expression.ScalarValue;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(QueryCustomFunctionCallExpression expression)
        {
            QueryValue result;

            var argumentValues = this.EvaluateArguments(expression.Arguments);

            var customEvaluator = expression.Function.Annotations.OfType<FunctionEvaluatorAnnotation>().Select(a => a.FunctionEvaluator).SingleOrDefault();

            if (customEvaluator != null)
            {
                result = customEvaluator(expression.ExpressionType, argumentValues);
            }
            else if (expression.FunctionBody != null)
            {
                QueryExpression functionBody = expression.FunctionBody;

                // replace parameter refs with arguments
                if (expression.Function.Parameters.Any())
                {
                    // Consider query:
                    //      "select value DefaultNamespace.GetCustomer(c.CustomerId) from [DefaultContainer].[Customer] as c" where GetCustomer is a function.
                    //    After we replace parameter references in the function's body with "c.CustomerId" expression evaluation
                    //    of the body expression fails becuase variable "c" is not in the scope for the function's body.
                    //    Note also that function body itself can have a variable reference with the same name "c".
                    // So the right thing is to evaluate each argument and then replace parameter references with constant expressions.
                    // However QueryConstantExpression currently only supports QueryScalarType/QueryScalarValue.
                    // So for now we only evaluating scalar arguments.
                    // TODO: add supoort for non-scalar constants and change the following code to evaluate each argument
                    //      NOTE: we have similar limitation in LinqToEntitiesEvaluator
                    List<QueryExpression> evaluatedArguments = new List<QueryExpression>();
                    foreach (var argument in expression.Arguments)
                    {
                        QueryScalarType scalarArgumentType = argument.ExpressionType as QueryScalarType;
                        if (scalarArgumentType != null)
                        {
                            QueryScalarValue scalarValue = (QueryScalarValue)this.Evaluate(argument);
                            var constant = CommonQueryBuilder.Constant(scalarValue);
                            evaluatedArguments.Add(constant);
                        }
                        else
                        {
                            evaluatedArguments.Add(argument);
                        }
                    }

                    var visitor = this.CreateFunctionParameterReferenceReplacingVisitor(expression.Function, evaluatedArguments);
                    functionBody = visitor.ReplaceExpression(expression.FunctionBody);
                }

                result = this.Evaluate(functionBody);
            }
            else
            {
                result = expression.ExpressionType.EvaluationStrategy.EvaluateFunction(
                            expression.ExpressionType,
                            expression.Function,
                            argumentValues);
            }

            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryDivideExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.Divide(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(QueryEqualToExpression expression)
        {
            // This for cases like ComplexType = null
            if (expression.Left.ExpressionType is QueryStructuralType)
            {
                QueryStructuralValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.EqualTo(right);
            }
            else if (expression.Left.ExpressionType is QueryReferenceType)
            {
                QueryReferenceValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.EqualTo(right);
            }
            else if (expression.Left.ExpressionType is QueryScalarType)
            {
                QueryScalarValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.EqualTo(right);
            }
            else
            {
                throw new TaupoNotSupportedException("Equality on type" + expression.Left.ExpressionType.ToString() + " is not supported");
            }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryFunctionImportCallExpression expression)
        {
            var argumentValues = this.EvaluateArguments(expression.Arguments);
            var result = expression.ExpressionType.EvaluationStrategy.EvaluateFunctionImport(expression.ExpressionType, expression.FunctionImport, argumentValues);
            var returnType = expression.FunctionImport.ReturnTypes.Single();
            if (returnType.EntitySet != null)
            {
                result = this.FixupFunctionImportResultEntities(result, returnType.EntitySet.Name);
            }

            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryFunctionParameterReferenceExpression expression)
        {
            throw new TaupoInvalidOperationException("Should never evaluate function parameter reference");
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryGreaterThanExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.GreaterThan(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.GreaterThanOrEqual(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryIsNotNullExpression expression)
        {
            // Cannot be a collection
            ExceptionUtilities.Assert(!(expression.Argument.ExpressionType is QueryCollectionType), "IsNotNull expression cannot be a collection");
            var argument = this.Evaluate(expression.Argument);
            return argument.Type.EvaluationStrategy.BooleanType.CreateValue(!argument.IsNull);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryIsNullExpression expression)
        {
            // Cannot be a collection
            ExceptionUtilities.Assert(!(expression.Argument.ExpressionType is QueryCollectionType), "IsNull expression cannot be a collection");
            var argument = this.Evaluate(expression.Argument);
            return argument.Type.EvaluationStrategy.BooleanType.CreateValue(argument.IsNull);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryIsOfExpression expression)
        {
            QueryValue source = this.Evaluate(expression.Source);
            QueryType resultType = expression.TypeToOperateAgainst;

            return source.IsOf(resultType, false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryLessThanExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.LessThan(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryLessThanOrEqualToExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.LessThanOrEqual(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryModuloExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.Modulo(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryMultiplyExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.Multiply(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(QueryNotEqualToExpression expression)
        {
            // This for cases like ComplexType != null
            if (expression.Left.ExpressionType is QueryStructuralType)
            {
                QueryStructuralValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.NotEqualTo(right);
            }
            else if (expression.Left.ExpressionType is QueryReferenceType)
            {
                QueryReferenceValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.NotEqualTo(right);
            }
            else if (expression.Left.ExpressionType is QueryScalarType)
            {
                QueryScalarValue left, right;
                this.EvaluateBinaryArguments(expression, out left, out right);
                return left.NotEqualTo(right);
            }
            else
            {
                throw new TaupoNotSupportedException("Equality on type" + expression.Left.ExpressionType.ToString() + " is not supported");
            }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryNotExpression expression)
        {
            var argumentValue = this.Evaluate(expression.Argument) as QueryScalarValue;
            if (argumentValue == null)
            {
                throw new TaupoInvalidOperationException("Argument must be a " + typeof(QueryScalarValue).Name + ".");
            }

            return argumentValue.LogicalNegate();
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryNullExpression expression)
        {
            return expression.ExpressionType.NullValue;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryOfTypeExpression expression)
        {
            QueryValue source = this.Evaluate(expression.Source);
            QueryEntityType resultType = expression.TypeToOperateAgainst as QueryEntityType;

            var argumentCollectionValue = source as QueryCollectionValue;
            ExceptionUtilities.Assert(argumentCollectionValue != null, "The argument to OfType has to be a collection of structural values");
            ExceptionUtilities.Assert(argumentCollectionValue.Type.ElementType is QueryEntityType, "The argument to OfType has to be a collection of structural values");

            return argumentCollectionValue.OfType(resultType, false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryOrExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);

            // short circuit "Or" logic if left is true (and evaluation did not throw any exceptions)
            // TODO: this is sort of a hack, we should have more solid way of check whether given EvaluationError is actual exception or just list of ingnorable exceptions
            bool leftThrewEvaluationException = left.EvaluationError != null && !string.IsNullOrEmpty(left.EvaluationError.ToString());
            if (left.EvaluationError == null && !left.IsNull && (bool)left.Value && !leftThrewEvaluationException)
            {
                return left;
            }

            return left.OrElse(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(QueryPropertyExpression expression)
        {
            var originalSource = this.Evaluate(expression.Instance);
            var sourceAsStructuralValue = originalSource as QueryStructuralValue;
            var sourceAsReferenceValue = originalSource as QueryReferenceValue;
            var sourceAsRecordValue = originalSource as QueryRecordValue;
            var sourceAsScalarValue = originalSource as QueryScalarValue;
            QueryValue result;

            if (sourceAsReferenceValue != null)
            {
                sourceAsStructuralValue = sourceAsReferenceValue.IsNull ? sourceAsReferenceValue.Type.QueryEntityType.NullValue : sourceAsReferenceValue.EntityValue;
            }

            if (sourceAsStructuralValue != null)
            {
                result = sourceAsStructuralValue.GetValue(expression.Name);
            }
            else if (sourceAsRecordValue != null)
            {
                var matchingMembers = sourceAsRecordValue.Type.Properties.Where(m => m.Name == expression.Name).ToList();
                ExceptionUtilities.Assert(matchingMembers.Count == 1, "Expecting exactly one member with name '{0}'. Actual: {1}.", expression.Name, matchingMembers.Count);

                result = sourceAsRecordValue.GetMemberValue(sourceAsRecordValue.Type.Properties.IndexOf(matchingMembers[0]));
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(sourceAsScalarValue, "Instance must evaluate to a structural or record or scalar value. Actual: '{0}'.", originalSource);
                ExceptionUtilities.Assert(
                    sourceAsScalarValue.Type is IQueryTypeWithProperties,
                    "Only query scalar types implementing IQueryTypeWithProperties are supported as query member property, actual type: {0}.",
                    sourceAsScalarValue.Type.StringRepresentation);

                result = sourceAsScalarValue.Type.EvaluationStrategy.EvaluateMemberProperty(sourceAsScalarValue, expression.ExpressionType, expression.Name);
            }

            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QueryRootExpression expression)
        {
            return this.DataSet[expression.Name];
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(QuerySubtractExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.Subtract(right);
        }

        /// <summary>
        /// Creates a visitor for replacing function parameter references.
        /// </summary>
        /// <param name="customFunction">The custom function.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>Visitor for replacing function parameter references.</returns>
        protected abstract IQueryExpressionReplacingVisitor CreateFunctionParameterReferenceReplacingVisitor(Function customFunction, IEnumerable<QueryExpression> arguments);

        /// <summary>
        /// Evaluates the arguments of binary expression and assigns their values to specified variables.
        /// </summary>
        /// <typeparam name="TArgumentType">The type of the argument type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="left">The result of left expression evaluation.</param>
        /// <param name="right">The result of right expression evaluation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Out parameters enable type inference.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Out parameters enable type inference.")]
        protected void EvaluateBinaryArguments<TArgumentType>(QueryBinaryExpression expression, out TArgumentType left, out TArgumentType right)
            where TArgumentType : QueryValue
        {
            left = null;
            right = null;

            left = this.Evaluate(expression.Left) as TArgumentType;
            if (left == null)
            {
                throw new TaupoInvalidOperationException("Left operand must be a " + typeof(TArgumentType).Name + ".");
            }

            right = this.Evaluate(expression.Right) as TArgumentType;
            if (right == null)
            {
                throw new TaupoInvalidOperationException("Right operand must be a " + typeof(TArgumentType).Name + ".");
            }
        }

        /// <summary>
        /// Evaluates an expression and verifies that it is a collection.
        /// </summary>
        /// <param name="expression">The source to evaluate.</param>
        /// <returns>Evaluated source.</returns>
        protected QueryCollectionValue EvaluateCollection(QueryExpression expression)
        {
            var result = this.Evaluate(expression);
            ExceptionUtilities.Assert(result != null, "Result of evaluation can never be null. Instead QueryValue.IsNull should be set to true.");

            var collectionValue = result as QueryCollectionValue;
            ExceptionUtilities.CheckObjectNotNull(
                collectionValue,
                "Source expression must be a collection. Source expression type: '{0}'. Result type: '{1}'.",
                expression.ExpressionType.StringRepresentation,
                result.Type.StringRepresentation);

            return collectionValue;
        }

        /// <summary>
        /// Evaluates the arguments.
        /// </summary>
        /// <param name="arguments">The arguments to evaluate.</param>
        /// <returns>Evaluated arguments.</returns>
        protected QueryValue[] EvaluateArguments(IEnumerable<QueryExpression> arguments)
        {
            return arguments.Select(a => this.Evaluate(a)).ToArray();
        }

        /// <summary>
        /// Evaluates built-in function.
        /// </summary>
        /// <param name="resultType">The functionresult type.</param>
        /// <param name="function">The built-in function.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>QueryValue which is the result of function evaluation.</returns>
        protected QueryValue EvaluateBuiltInFunction(QueryType resultType, QueryBuiltInFunction function, IEnumerable<QueryExpression> arguments)
        {
            var argumentValues = this.EvaluateArguments(arguments);
            var result = resultType.EvaluationStrategy.EvaluateBuiltInFunction(resultType, function.NamespaceName, function.Name, argumentValues);
            return result;
        }

        /// <summary>
        /// Entities returned from FunctionImport only have their scalar properties set. However, in order to properly evaluate results of queries
        /// which project navigation properties of these entities (such as FunctionImportReturningCustomers().Select(c => c.Orders))
        /// we need to associate these entities with ones that are already stored in the data set (which basically is the full graph)
        /// In order to do that, we look through entity associated with the function import for entity with matching keys and return it instead
        /// </summary>
        /// <param name="result">Collection of fixed to fix-up.</param>
        /// <param name="entitySetName">Name of entity set associated with the given function import.</param>
        /// <returns>Collection of fixed-up entities.</returns>
        private QueryValue FixupFunctionImportResultEntities(QueryValue result, string entitySetName)
        {
            var resultCollection = result as QueryCollectionValue;

            ExceptionUtilities.CheckObjectNotNull(resultCollection, "Expecting collection.");
            ExceptionUtilities.Assert(resultCollection.Type.ElementType is QueryEntityType, "Expecting collection of entities.");

            var fixedElements = new List<QueryValue>();
            foreach (var element in resultCollection.Elements.Cast<QueryStructuralValue>())
            {
                var elementEntityType = (QueryEntityType)element.Type;
                var primaryKeyPropertyNames = elementEntityType.Properties.Where(p => p.IsPrimaryKey).Select(p => p.Name);
                var elementKeyPropertyValuesDictionary = primaryKeyPropertyNames.Select(n => new { Name = n, Value = element.GetScalarValue(n) }).ToDictionary(k => k.Name, e => e.Value);
                QueryValue matchingElementInEntitySet = null;
                foreach (var entitySetElement in this.DataSet[entitySetName].Elements.Cast<QueryStructuralValue>())
                {
                    if (!elementEntityType.EntityType.Equals(((QueryEntityType)entitySetElement.Type).EntityType))
                    {
                        continue;
                    }

                    var elementsMatch = this.AllKeysMatch(entitySetElement, elementKeyPropertyValuesDictionary);
                    if (elementsMatch)
                    {
                        matchingElementInEntitySet = entitySetElement;
                        break;
                    }
                }

                if (matchingElementInEntitySet != null)
                {
                    fixedElements.Add(matchingElementInEntitySet);
                }
                else
                {
                    fixedElements.Add(element);
                }
            }

            return ((QueryCollectionType)result.Type).CreateCollectionWithValues(fixedElements);
        }

        private bool AllKeysMatch(QueryStructuralValue element, Dictionary<string, QueryScalarValue> primaryKeyValuesDictionary)
        {
            var allKeysMatch = true;
            foreach (var entry in primaryKeyValuesDictionary)
            {
                var elementKeyPropertyValue = element.GetScalarValue(entry.Key);
                if (!(bool)entry.Value.EqualTo(elementKeyPropertyValue).Value)
                {
                    allKeysMatch = false;
                    break;
                }
            }

            return allKeysMatch;
        }
    }
}
