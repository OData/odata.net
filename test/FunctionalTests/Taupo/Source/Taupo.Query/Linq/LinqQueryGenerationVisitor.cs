//---------------------------------------------------------------------
// <copyright file="LinqQueryGenerationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// The visitor that creates System.Linq.Expressions.Expression tree that represents a Linq query.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is temporarily allowed until further refactoring of current design.")]
    public abstract class LinqQueryGenerationVisitor : ILinqExpressionVisitor<Expression>
    {
        private Type expressionGeneratorType = typeof(Queryable);
        private List<object> closures;
        private IAnonymousTypeBuilder anonymousTypeBuilder;
        private Dictionary<LinqParameterExpression, ParameterExpression> parametersInScope;

        /// <summary>
        /// Initializes a new instance of the LinqQueryGenerationVisitor class.
        /// </summary>
        /// <param name="context">Context for the query.</param>
        /// <param name="closures">Closures that are in scope. Properties of the type should match free variable names.</param>
        /// <param name="anonymousTypeBuilder">The anonymous type builder.</param>
        protected LinqQueryGenerationVisitor(object context, IEnumerable<object> closures, IAnonymousTypeBuilder anonymousTypeBuilder)
        {
            this.Context = context;
            this.parametersInScope = new Dictionary<LinqParameterExpression, ParameterExpression>();
            this.closures = new List<object>(closures);
            this.anonymousTypeBuilder = anonymousTypeBuilder;
        }

        /// <summary>
        /// Gets the context for the linq query.
        /// </summary>
        protected object Context { get; private set; }

        /// <summary>
        /// Gets or sets the the query provider for the linq query.
        /// </summary>
        protected IQueryProvider QueryProvider { get; set; }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression GenerateLinqExpression(QueryExpression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqAllExpression expression)
        {
            return this.GenerateAnyAllQueryCallExpression(expression, "All");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqAnyExpression expression)
        {
            return this.GenerateAnyAllQueryCallExpression(expression, "Any");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqAsEnumerableExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            return Expression.Call(typeof(Enumerable), "AsEnumerable", new Type[] { sourceTypeArgument }, source);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqBitwiseAndExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.And);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqBitwiseOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Or);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqBuiltInFunctionCallExpression expression)
        {
            var type = Type.GetType(expression.LinqBuiltInFunction.ClassName);
            ExceptionUtilities.CheckObjectNotNull(type, "Cannot find type '{0}' to find method {1}", expression.LinqBuiltInFunction.ClassName, expression.LinqBuiltInFunction.MethodName);

            var expressionArguments = expression.Arguments.Select(a => this.GenerateLinqExpression(a)).ToList();
            var expressionArgumentTypes = expressionArguments.Select(a => a.Type).ToList();

            if (expression.LinqBuiltInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.InstanceMethod)
            {
                ExceptionUtilities.Assert(expressionArguments.Count > 0, "Need at least one argument");

                var instanceExpression = expressionArguments.First();

                expressionArguments.Remove(instanceExpression);
                expressionArgumentTypes.Remove(expressionArgumentTypes.First());
                var methodInfo = type.GetMethod(expression.LinqBuiltInFunction.MethodName, expressionArgumentTypes.ToArray(), true, false);

                return Expression.Call(instanceExpression, methodInfo, expressionArguments);
            }
            else if (expression.LinqBuiltInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.InstanceProperty)
            {
                ExceptionUtilities.Assert(expressionArguments.Count == 1, "Invalid test expression, cannot use a function that is an InstanceProperty type with multiple arguments");

                var instanceExpression = expressionArguments.Single();

                var propertyGetMethod = type.GetProperty(expression.LinqBuiltInFunction.MethodName).GetGetMethod();
                return Expression.Call(instanceExpression, propertyGetMethod);
            }
            else
            {
                var methodInfo = type.GetMethod(expression.LinqBuiltInFunction.MethodName, expressionArgumentTypes.ToArray(), true, true);
                return Expression.Call(methodInfo, expressionArguments.ToArray());
            }
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqCastExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);

            Type type = this.GetClrType(expression.TypeToOperateAgainst);
            MethodInfo methodInfo = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type);
            return Expression.Call(methodInfo, source);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqConcatExpression expression)
        {
            Expression outer = this.GenerateLinqExpression(expression.Outer);
            Expression inner = this.GenerateLinqExpression(expression.Inner);

            Type typeArgument = this.GetCollectionTypeArgument(outer);

            return Expression.Call(this.expressionGeneratorType, "Concat", new Type[] { typeArgument }, outer, inner);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqContainsExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Expression value = this.GenerateLinqExpression(expression.Value);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            return Expression.Call(this.expressionGeneratorType, "Contains", new Type[] { sourceTypeArgument }, source, value);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqCountExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "Count");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqDefaultIfEmptyExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            if (expression.DefaultValue != null)
            {
                Expression defaultValue = this.GenerateLinqExpression(expression.DefaultValue);
                return Expression.Call(this.expressionGeneratorType, "DefaultIfEmpty", new Type[] { sourceTypeArgument }, source, defaultValue);
            }
            else
            {
                return Expression.Call(this.expressionGeneratorType, "DefaultIfEmpty", new Type[] { sourceTypeArgument }, source);
            }
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqDistinctExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            return Expression.Call(this.expressionGeneratorType, "Distinct", new Type[] { sourceTypeArgument }, source);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqExceptExpression expression)
        {
            Expression outer = this.GenerateLinqExpression(expression.Outer);
            Expression inner = this.GenerateLinqExpression(expression.Inner);

            Type typeArgument = this.GetCollectionTypeArgument(outer);

            return Expression.Call(this.expressionGeneratorType, "Except", new Type[] { typeArgument }, outer, inner);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqExclusiveOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.ExclusiveOr);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqFirstExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "First");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqFirstOrDefaultExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "FirstOrDefault");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqFreeVariableExpression expression)
        {
            // find closure object which has a property matching the free variable name
            object closure = this.closures.Where(c => c.GetType().GetProperty(expression.Name) != null).SingleOrDefault();

            if (closure == null)
            {
                throw new TaupoInvalidOperationException("Could not find closure with property matching free variable name: " + expression.Name);
            }

            PropertyInfo propertyInfo = closure.GetType().GetProperty(expression.Name);
            ExceptionUtilities.Assert(propertyInfo != null, "PropertyInfo should not be null");

            return Expression.Property(Expression.Constant(closure), propertyInfo);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqGroupByExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Expression keySelector = this.GenerateLinqExpression(expression.KeySelector);
            Expression elementSelector = expression.ElementSelector != null ? this.GenerateLinqExpression(expression.ElementSelector) : null;
            Expression resultSelector = expression.ResultSelector != null ? this.GenerateLinqExpression(expression.ResultSelector) : null;

            var genericArguments = new List<Type>(keySelector.Type.GetGenericArguments());
            var arguments = new List<Expression>(new[] { source, keySelector });
            if (elementSelector != null)
            {
                genericArguments.Add(elementSelector.Type.GetGenericArguments()[1]);
                arguments.Add(elementSelector);
            }

            if (resultSelector != null)
            {
                genericArguments.Add(resultSelector.Type.GetGenericArguments()[2]);
                arguments.Add(resultSelector);
            }

            return Expression.Call(this.expressionGeneratorType, "GroupBy", genericArguments.ToArray(), arguments.ToArray());
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqGroupJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, "GroupJoin");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, "Join");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqLambdaExpression expression)
        {
            ParameterExpression[] parameters = expression.Parameters.Select(p => this.GenerateLinqExpression(p)).Cast<ParameterExpression>().ToArray();
            Expression body = this.GenerateLinqExpression(expression.Body);
            return Expression.Lambda(body, parameters);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqLengthPropertyExpression expression)
        {
            var instance = this.GenerateLinqExpression(expression.Instance);

            return Expression.Property(instance, "Length");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqLongCountExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "LongCount");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqMaxExpression expression)
        {
            // Max must take an extra generic type argument when a lamba is given, otherwise Max will throw
            if (expression.Lambda != null)
            {
                return this.GenerateQueryMethodWithLambda(expression, "Max", new Type[] { ((IQueryClrType)expression.Lambda.Body.ExpressionType).ClrType });
            }
            else
            {
                return this.GenerateQueryMethodWithLambda(expression, "Max", new Type[] { });
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMemeberMethodExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        public virtual Expression Visit(LinqMemberMethodExpression expression)
        {
            var instance = this.GenerateLinqExpression(expression.Source);
            string methodName = expression.MemberMethod.Name;
            var arguments = expression.Arguments.Select(a => this.GenerateLinqExpression(a)).ToArray();

            return this.BuildMethodCallExpression(instance, methodName, arguments);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMinExpression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqMinExpression expression)
        {
            // Min must take an extra generic type argument when a lamba is given, otherwise Min will throw
            if (expression.Lambda != null)
            {
                return this.GenerateQueryMethodWithLambda(expression, "Min", new Type[] { ((IQueryClrType)expression.Lambda.Body.ExpressionType).ClrType });
            }
            else
            {
                return this.GenerateQueryMethodWithLambda(expression, "Min", new Type[] { });
            }
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public virtual Expression Visit(LinqNewArrayExpression expression)
        {
            List<Expression> linqExpressions = new List<Expression>();
            foreach (var childExpression in expression.Expressions)
            {
                linqExpressions.Add(this.GenerateLinqExpression(childExpression));
            }

            var collectionType = expression.ExpressionType as QueryCollectionType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Expecting expression type to be a collection.");

            var queryClrType = collectionType.ElementType as IQueryClrType;
            ExceptionUtilities.CheckObjectNotNull(queryClrType, "In order to make a New Array we need to have a clrType, the queryType'{0}' does not implement IQueryClrType", expression.ExpressionType);
            
            return Expression.NewArrayInit(queryClrType.ClrType, linqExpressions.ToArray());
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public virtual Expression Visit(LinqNewExpression expression)
        {
            Expression[] members = expression.Members.Select(this.GenerateLinqExpression).ToArray();
            Type anonymousClrType = null;
            if (expression.ExpressionType is QueryAnonymousStructuralType)
            {
                anonymousClrType = this.anonymousTypeBuilder.GetAnonymousType(((QueryAnonymousStructuralType)expression.ExpressionType).Properties.Select(c => c.Name).ToArray());
            }

            ExceptionUtilities.CheckObjectNotNull(anonymousClrType, "could not discover type of new Expression");
            Type[] genericTypeArguments = members.Select(m => m.Type).ToArray();
            Type concreteClrType = anonymousClrType.MakeGenericType(genericTypeArguments);
            ConstructorInfo ctorInfo = concreteClrType.GetInstanceConstructor(true, genericTypeArguments);

            return Expression.New(ctorInfo, members);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqNewInstanceExpression expression)
        {
            var queryClrType = expression.ExpressionType as IQueryClrType;

            ExceptionUtilities.Assert(queryClrType != null, "could not discover type of new Expression");

            Type clrType = queryClrType.ClrType;

            // If we are creating an instance of a specific type, we need to prepare the memberbind expressions for the binding the values of the 
            // properties of the type to input values
            var memberBindExpressions = new List<MemberBinding>();

            // entity=>
            for (int index = 0; index < expression.Members.Count; index++)
            {
                string propertyName = expression.MemberNames[index];
                Expression memberExpression = this.GenerateLinqExpression(expression.Members[index]);

                // .PropertyName = PropertyExpression1(entity.PropertyName)
                MemberInfo propertyInfo = clrType.GetProperty(propertyName);
                MemberBinding binding = Expression.Bind(propertyInfo, memberExpression);
                memberBindExpressions.Add(binding);
            }

            ExceptionUtilities.Assert(expression.ConstructorArguments.Count == 0, "Cannot create typed instances with non-default constructor");

            // Since we are creating an instance of a known type, we get the default parameter-less constructor
            ConstructorInfo ctorInfo = clrType.GetInstanceConstructor(true, PlatformHelper.EmptyTypes);
            ExceptionUtilities.CheckObjectNotNull(ctorInfo, "Type '{0}' does not have a default , parameter-less constructor", clrType.FullName);

            // entity => new EntityType( constructorArgs ) 
            NewExpression newExpression = Expression.New(ctorInfo, Enumerable.Empty<Expression>());

            // entity => new EntityType( constructorArgs ) { Property1 = PropertyExpression1(entity.Property1) }
            Expression objectWithPropertiesSet = Expression.MemberInit(newExpression, memberBindExpressions);
            return objectWithPropertiesSet;
        }
 
        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqOrderByExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);

            Expression result = source;
            for (int i = 0; i < expression.KeySelectors.Count; i++)
            {
                var keySelector = this.GenerateLinqExpression(expression.KeySelectors[i]);
                Type[] keySelectorGenericArguments = keySelector.Type.GetGenericArguments();

                var isDescending = expression.AreDescending[i];

                string methodName = i == 0 ? "OrderBy" : "ThenBy";

                if (isDescending)
                {
                    methodName += "Descending";
                }

                result = Expression.Call(this.expressionGeneratorType, methodName, new[] { keySelectorGenericArguments[0], keySelectorGenericArguments[1] }, result, keySelector);
            }

            return result;
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqParameterExpression expression)
        {
            ParameterExpression result;
            if (!this.parametersInScope.TryGetValue(expression, out result))
            {
                result = Expression.Parameter(this.GetClrType(expression.ExpressionType), expression.Name);
                this.parametersInScope.Add(expression, result);
            }

            return result;
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqSelectExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Expression selector = this.GenerateLinqExpression(expression.Lambda);

            Type[] selectorGenericArguments = selector.Type.GetGenericArguments();

            return Expression.Call(this.expressionGeneratorType, "Select", new[] { selectorGenericArguments[0], selectorGenericArguments[1] }, source, selector);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqSelectManyExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);

            // a bit of LINQ rocket-science. Basically we need to fake the type of collectionSelector to be always IEnumerable<Foo>
            // even if the body is something else, like IList<Foo>. Otherwise Expression.Call will not work cause it won't be able to find appropriate overload
            // compiler does similar thing when it's constructing Expression from method syntax or comprehension
            // fakeCollectionSelector is used to get generic type arguments, which are then used to generate delegate type that we need.
            Expression fakeCollectionSelector = this.GenerateLinqExpression(expression.CollectionSelector);
            Type[] collectionSelectorGenericArguments = fakeCollectionSelector.Type.GetGenericArguments();
            var sourceTypeArgument = collectionSelectorGenericArguments[0];
            var collectionTypeArgument = collectionSelectorGenericArguments[1].GetInterfaces()
                .Where(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Single().GetGenericArguments().Single();

            ParameterExpression[] collectionSelectorParameters = expression.CollectionSelector.Parameters.Select(p => this.GenerateLinqExpression(p)).Cast<ParameterExpression>().ToArray();
            Expression collectionSelectorBody = this.GenerateLinqExpression(expression.CollectionSelector.Body);
            var delegateType = typeof(Func<,>).MakeGenericType(sourceTypeArgument, typeof(IEnumerable<>).MakeGenericType(collectionTypeArgument));
            var collectionSelector = Expression.Lambda(delegateType, collectionSelectorBody, collectionSelectorParameters);
            var resultSelector = expression.ResultSelector != null ? this.GenerateLinqExpression(expression.ResultSelector) : null;

            if (resultSelector == null)
            {
                return Expression.Call(this.expressionGeneratorType, "SelectMany", new[] { sourceTypeArgument, collectionTypeArgument }, source, collectionSelector);
            }
            else
            {
                var resultTypeArgument = resultSelector.Type.GetGenericArguments().Last();

                return Expression.Call(this.expressionGeneratorType, "SelectMany", new[] { sourceTypeArgument, collectionTypeArgument, resultTypeArgument }, source, collectionSelector, resultSelector);
            }
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqSingleExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "Single");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqSingleOrDefaultExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "SingleOrDefault");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqSkipExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Expression skipCount = this.GenerateLinqExpression(expression.SkipCount);

            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            return Expression.Call(this.expressionGeneratorType, "Skip", new[] { sourceTypeArgument }, source, skipCount);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqTakeExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Expression takeCount = this.GenerateLinqExpression(expression.TakeCount);

            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);

            return Expression.Call(this.expressionGeneratorType, "Take", new[] { sourceTypeArgument }, source, takeCount);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqUnionExpression expression)
        {
            Expression firstSource = this.GenerateLinqExpression(expression.FirstSource);
            Expression secondSource = this.GenerateLinqExpression(expression.SecondSource);

            Type sourceType = firstSource.Type.GetGenericArguments()[0];

            return Expression.Call(this.expressionGeneratorType, "Union", new[] { sourceType }, firstSource, secondSource);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(LinqWhereExpression expression)
        {
            return this.GenerateQueryMethodWithLambda(expression, "Where");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryAddExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Add);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryAndExpression expression)
        {
            Expression left = this.GenerateLinqExpression(expression.Left);
            Expression right = this.GenerateLinqExpression(expression.Right);

            return Expression.And(left, right);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryAsExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type type = this.GetClrType(expression.TypeToOperateAgainst);
            return Expression.TypeAs(source, type);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryCastExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type type = this.GetClrType(expression.TypeToOperateAgainst);
            return Expression.Convert(source, type);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryConstantExpression expression)
        {
            return Expression.Constant(expression.ScalarValue.Value, this.GetClrType(expression.ExpressionType));
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public virtual Expression Visit(QueryCustomFunctionCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not implemented yet.");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryDivideExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Divide);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Equal);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public virtual Expression Visit(QueryFunctionImportCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not implemented yet.");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryFunctionParameterReferenceExpression expression)
        {
            throw new TaupoNotSupportedException("Not implemented yet.");
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryGreaterThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.GreaterThan);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.GreaterThanOrEqual);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryIsNotNullExpression expression)
        {
            var argument = this.GenerateLinqExpression(expression.Argument);

            return Expression.NotEqual(argument, Expression.Constant(null));
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryIsNullExpression expression)
        {
            var argument = this.GenerateLinqExpression(expression.Argument);

            return Expression.Equal(argument, Expression.Constant(null));
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryIsOfExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type type = this.GetClrType(expression.TypeToOperateAgainst);
            return Expression.TypeIs(source, type);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryLessThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.LessThan);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryLessThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.LessThanOrEqual);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryModuloExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Modulo);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryMultiplyExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Multiply);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryNotEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.NotEqual);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryNotExpression expression)
        {
            Expression left = this.GenerateLinqExpression(expression.Argument);

            return Expression.Not(left);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryNullExpression expression)
        {
            return Expression.Constant(null, this.GetClrType(expression.ExpressionType));
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryOfTypeExpression expression)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);

            Type type = this.GetClrType(expression.TypeToOperateAgainst);
            MethodInfo methodInfo = typeof(Queryable).GetMethod("OfType").MakeGenericMethod(type);
            return Expression.Call(methodInfo, source);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryOrExpression expression)
        {
            Expression left = this.GenerateLinqExpression(expression.Left);
            Expression right = this.GenerateLinqExpression(expression.Right);

            return Expression.Or(left, right);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryPropertyExpression expression)
        {
            Expression instance = this.GenerateLinqExpression(expression.Instance);

            return Expression.Property(instance, expression.Name);
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QueryRootExpression expression)
        {
            PropertyInfo propertyInfo = this.Context.GetType().GetProperty(expression.Name);
            IQueryable queryRoot = propertyInfo.GetValue(this.Context, null) as IQueryable;
            ExceptionUtilities.CheckObjectNotNull(queryRoot, "The root of the query should be an IQueryable");
            return queryRoot.Expression;
        }

        /// <summary>
        /// Generates System.Linq.Expressions.Expression from the given expression.
        /// </summary>
        /// <param name="expression">Expression to generate System.Linq.Expressions.Expression from.</param>
        /// <returns>Generated System.Linq.Expressions.Expression.</returns>
        public Expression Visit(QuerySubtractExpression expression)
        {
            return this.VisitBinaryExpression(expression, Expression.Subtract);
        }

        /// <summary>
        /// Builds a method call expression.
        /// </summary>
        /// <param name="instance">The instance expression.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="arguments">The argument expressions.</param>
        /// <returns>The method call expression</returns>
        protected static Expression BuildMethodCallExpression(Expression instance, MethodInfo methodInfo, params Expression[] arguments)
        {
            if (methodInfo.IsStatic)
            {
                return Expression.Call(methodInfo, arguments);
            }
            else
            {
                return Expression.Call(instance, methodInfo, arguments);
            }
        }

        /// <summary>
        /// Builds a method call expression.
        /// </summary>
        /// <param name="instance">The instance expression.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The argument expressions.</param>
        /// <returns>The method call expression</returns>
        protected virtual Expression BuildMethodCallExpression(Expression instance, string methodName, params Expression[] arguments)
        {
            MethodInfo methodInfo = instance.Type.GetMethod(methodName);
            ExceptionUtilities.CheckObjectNotNull(methodInfo, "Could not find method '{0}' on type of expression '{1}'", methodName, instance);

            return BuildMethodCallExpression(instance, methodInfo, arguments);
        }

        /// <summary>
        /// Visits the binary expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="builderMethodCall">The builder method call.</param>
        /// <returns>The result of visiting the expression</returns>
        protected virtual Expression VisitBinaryExpression(QueryBinaryExpression expression, Func<Expression, Expression, BinaryExpression> builderMethodCall)
        {
            Expression left = this.GenerateLinqExpression(expression.Left);
            Expression right = this.GenerateLinqExpression(expression.Right);
            
            return builderMethodCall(left, right);
        }

        private Type GetClrType(QueryType queryType)
        {
            var queryClrType = queryType as IQueryClrType;
            if (queryClrType != null)
            {
                return queryClrType.ClrType;
            }

            var queryCollectionType = queryType as QueryCollectionType;
            if (queryCollectionType != null)
            {
                var elementType = queryCollectionType.ElementType as IQueryClrType;
                if (elementType != null)
                {
                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType.ClrType);
                    return enumerableType;
                }
            }

            throw new TaupoInvalidOperationException("Unable to determine CLR type for " + queryType + ".");
        }

        private Type GetCollectionTypeArgument(Expression expression)
        {
            Type enumerableInterface = expression.Type.GetInterfaces().Where(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Single();

            return enumerableInterface.GetGenericArguments()[0];
        }

        private Expression GenerateQueryMethodWithLambda(LinqQueryMethodWithLambdaExpression expression, string queryMethodName)
        {
            return this.GenerateQueryMethodWithLambda(expression, queryMethodName, new Type[] { });
        }

        private Expression GenerateQueryMethodWithLambda(LinqQueryMethodWithLambdaExpression expression, string queryMethodName, Type[] additionalTypeArguments)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);
            Type[] typeArguments = new Type[] { sourceTypeArgument };

            // Some expressions e.g min and max require additional generic type arguments
            typeArguments = typeArguments.Concat(additionalTypeArguments).ToArray();

            if (expression.Lambda == null)
            {
                return Expression.Call(this.expressionGeneratorType, queryMethodName, typeArguments, source);
            }
            else
            {
                Expression predicate = this.GenerateLinqExpression(expression.Lambda);
                return Expression.Call(this.expressionGeneratorType, queryMethodName, typeArguments, source, predicate);
            }
        }

        private Expression GenerateAnyAllQueryCallExpression(LinqQueryMethodWithLambdaExpression expression, string methodName)
        {
            Expression source = this.GenerateLinqExpression(expression.Source);
            Type sourceTypeArgument = this.GetCollectionTypeArgument(source);
            Type[] typeArguments = new Type[] { sourceTypeArgument };

            if (expression.Lambda == null)
            {
                return Expression.Call(typeof(Enumerable), methodName, typeArguments, source);
            }
            else
            {
                Expression predicate = this.GenerateLinqExpression(expression.Lambda);
                return Expression.Call(typeof(Enumerable), methodName, typeArguments, source, predicate);
            }
        }

        private Expression VisitJoinExpressionBase(LinqJoinExpressionBase expression, string methodName)
        {
            Expression outer = this.GenerateLinqExpression(expression.Outer);
            Expression inner = this.GenerateLinqExpression(expression.Inner);
            Expression outerKeySelector = this.GenerateLinqExpression(expression.OuterKeySelector);
            Expression innerKeySelector = this.GenerateLinqExpression(expression.InnerKeySelector);
            Expression resultSelector = this.GenerateLinqExpression(expression.ResultSelector);

            var outerType = outerKeySelector.Type.GetGenericArguments()[0];
            var innerType = innerKeySelector.Type.GetGenericArguments()[0];
            var keyType = outerKeySelector.Type.GetGenericArguments()[1];
            var resultType = resultSelector.Type.GetGenericArguments().Last();

            return Expression.Call(typeof(Queryable), methodName, new Type[] { outerType, innerType, keyType, resultType }, outer, inner, outerKeySelector, innerKeySelector, resultSelector);
        }
    }
}