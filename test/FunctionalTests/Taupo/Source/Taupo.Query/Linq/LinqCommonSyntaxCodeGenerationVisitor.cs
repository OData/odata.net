//---------------------------------------------------------------------
// <copyright file="LinqCommonSyntaxCodeGenerationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// The visitor that creates CodeExpression that represents Linq query built using common syntax from query-based syntax and lambda-based syntax.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is temporarily allowed until further refactoring of current design.")]
    public abstract class LinqCommonSyntaxCodeGenerationVisitor : ILinqExpressionVisitor<CodeExpression>
    {
        private Dictionary<LinqFreeVariableExpression, CodeFreeVariable> freeVariables;

        /// <summary>
        /// Initializes a new instance of the LinqCommonSyntaxCodeGenerationVisitor class.
        /// </summary>
        /// <param name="rootExpression">CodeExpression that is a root of generated query.</param>
        /// <param name="customInitializationCodeGenerator">Code generator for custom initialization.</param>
        protected LinqCommonSyntaxCodeGenerationVisitor(CodeExpression rootExpression, ICustomInitializationCodeGenerator customInitializationCodeGenerator)
        {
            this.RootExpression = rootExpression;
            this.CustomInitializationCodeGenerator = customInitializationCodeGenerator;
            this.freeVariables = new Dictionary<LinqFreeVariableExpression, CodeFreeVariable>();
        }

        /// <summary>
        /// Gets or sets the code generator for custom initialization.
        /// </summary>
        [InjectDependency]
        public ICustomInitializationCodeGenerator CustomInitializationCodeGenerator { get; set; }

        /// <summary>
        /// Gets or sets code expression for the query provider.
        /// </summary>
        protected CodeExpression QueryProvider { get; set; }

        /// <summary>
        /// Gets the root code expression
        /// </summary>
        protected CodeExpression RootExpression { get; private set; }

        /// <summary>
        /// Generates SystemCodeDom.CodeExpression tree representing Linq query using the method syntax along with free variables bound to the query.
        /// </summary>
        /// <param name="queryExpression">Expression from which code is generated.</param>
        /// <returns>Expression with free variables.</returns>
        public CodeExpressionWithFreeVariables GenerateQueryCode(QueryExpression queryExpression)
        {
            CodeExpression query = queryExpression.Accept(this);

            return new CodeExpressionWithFreeVariables(query, this.freeVariables.Values);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqAllExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "All");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqAnyExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "Any");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqAsEnumerableExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);

            return source.Call("AsEnumerable");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqBitwiseAndExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.BitwiseAnd);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqBitwiseOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.BitwiseOr);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqBuiltInFunctionCallExpression expression)
        {
            CodeExpression source = null;

            var builtInFunction = expression.LinqBuiltInFunction;
            var arguments = this.GenerateCodeForArguments(expression.Arguments).ToArray();

            if (builtInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.InstanceMethod)
            {
                ExceptionUtilities.Assert(arguments.Length >= 1, "Wrong number of arguments");
                var args = arguments.Skip(1).ToArray();
                source = arguments[0];
                return source.Call(builtInFunction.MethodName, args);
            }
            else if (expression.LinqBuiltInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.InstanceProperty)
            {
                ExceptionUtilities.Assert(arguments.Length == 1, "Wrong number of arguments");
                source = arguments[0];
                return source.Property(builtInFunction.BuiltInFunction.Name);
            }
            else
            {
                var type = new CodeTypeReferenceExpression(builtInFunction.ClassName);
                ExceptionUtilities.Assert(expression.LinqBuiltInFunction.BuiltInFunctionKind == LinqBuiltInFunctionKind.StaticMethod, "Builtin function kind {0} not supported", expression.LinqBuiltInFunction.BuiltInFunctionKind.ToString());
                return type.Call(builtInFunction.MethodName, arguments);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqCastExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            CodeTypeReference type = this.GetTypeReference(expression.TypeToOperateAgainst);
            return source.Call("Cast", new CodeTypeReference[] { type });
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqConcatExpression expression)
        {
            CodeExpression outer = this.GenerateCode(expression.Outer);
            CodeExpression inner = this.GenerateCode(expression.Inner);

            return outer.Call("Concat", inner);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqContainsExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            CodeExpression value = this.GenerateCode(expression.Value);

            return source.Call("Contains", value);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqCountExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "Count");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqDefaultIfEmptyExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);

            if (expression.DefaultValue != null)
            {
                CodeExpression defaultValue = this.GenerateCode(expression.DefaultValue);
                return source.Call("DefaultIfEmpty", defaultValue);
            }
            else
            {
                return source.Call("DefaultIfEmpty");
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqDistinctExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);

            return source.Call("Distinct");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqExceptExpression expression)
        {
            CodeExpression outer = this.GenerateCode(expression.Outer);
            CodeExpression inner = this.GenerateCode(expression.Inner);

            return outer.Call("Except", inner);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqExclusiveOrExpression expression)
        {
            CodeExpression left = this.GenerateCode(expression.Left);
            CodeExpression right = this.GenerateCode(expression.Right);

            return new CodeExclusiveOrExpression(left, right);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqFirstExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "First");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqFirstOrDefaultExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "FirstOrDefault");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqFreeVariableExpression expression)
        {
            CodeFreeVariable codeFreeVariable;
            if (!this.freeVariables.TryGetValue(expression, out codeFreeVariable))
            {
                IEnumerable<CodeExpression> values = expression.Values.Select(v => this.GenerateCode(v));
                CodeTypeReference typeReference = this.GetTypeReference(expression.ExpressionType);
                codeFreeVariable = new CodeFreeVariable(expression.Name, typeReference, values);
                this.freeVariables.Add(expression, codeFreeVariable);
            }

            return Code.Variable(codeFreeVariable.Name);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqGroupByExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqGroupJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, "GroupJoin");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, "Join");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqLambdaExpression expression)
        {
            CodeExpression body = this.GenerateCode(expression.Body);

            return Code.Lambda().WithBody(body).WithParameters(expression.Parameters.Select(p => p.Name).ToArray());
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqLengthPropertyExpression expression)
        {
            var instance = this.GenerateCode(expression.Instance);

            return Code.Property(instance, "Length");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqLongCountExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "LongCount");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqMaxExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "Max");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>The result of visiting this expression.</returns>
        public virtual CodeExpression Visit(LinqMemberMethodExpression expression)
        {
            var instance = this.GenerateCode(expression.Source);
            var arguments = this.GenerateCodeForArguments(expression.Arguments).ToArray();
            var memberAnnotation = expression.MemberMethod.Annotations.OfType<MemberInSpatialTypeAnnotation>().FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(memberAnnotation, "Currently only spatial types have member methods. The method must include MemberInSpatialTypeAnnotation.");
            if (memberAnnotation.IsStaticMember == true)
            {
                return new CodeTypeReferenceExpression(((QueryMappedScalarTypeWithStructure)expression.Source.ExpressionType).ClrType).Call(expression.MemberMethod.Name, arguments);
            }

            return instance.Call(expression.MemberMethod.Name, arguments);    
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqMinExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "Min");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqNewArrayExpression expression)
        {
            List<CodeExpression> codeExpressions = new List<CodeExpression>();
            foreach (var childQueryExpression in expression.Expressions)
            {
                codeExpressions.Add(this.GenerateCode(childQueryExpression));
            }

            var collectionType = expression.ExpressionType as QueryCollectionType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Expecting expression type to be a collection.");

            var clrQueryType = collectionType.ElementType as IQueryClrType;
            if (clrQueryType != null)
            {
                return new CodeArrayCreateExpression(Code.TypeRef(clrQueryType.ClrType), codeExpressions.ToArray());
            }
            else
            {
                return Code.Array(codeExpressions.ToArray());
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqNewExpression expression)
        {
            CodeCreateAndInitializeObjectExpression result = null;
            result = new CodeCreateAndInitializeObjectExpression();

            for (int i = 0; i < expression.Members.Count; i++)
            {
                CodeExpression member = this.GenerateCode(expression.Members[i]);
                member = this.FixupPropertyForPropertyInitializer(expression, expression.MemberNames[i], expression.Members[i], member);
                result.PropertyInitializers.Add(new KeyValuePair<string, CodeExpression>(expression.MemberNames[i], member));
            }

            return result;
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqNewInstanceExpression expression)
        {
            CodeCreateAndInitializeObjectExpression result = null;
            var constructorArguments = expression.ConstructorArguments.Select(this.GenerateCode);
            result = new CodeCreateAndInitializeObjectExpression(constructorArguments.ToArray());
            var queryClrType = expression.ExpressionType as IQueryClrType;

            ExceptionUtilities.CheckObjectNotNull(queryClrType, "Type must implement " + typeof(IQueryClrType).Name + "interface: " + expression.ExpressionType + ".");

            CodeTypeReference entityTypeReference = Code.TypeRef(queryClrType.ClrType);
            result.ObjectType = entityTypeReference;

            for (int i = 0; i < expression.Members.Count; i++)
            {
                CodeExpression member = this.GenerateCode(expression.Members[i]);
                member = this.FixupPropertyForPropertyInitializer(expression, expression.MemberNames[i], expression.Members[i], member);
                result.PropertyInitializers.Add(new KeyValuePair<string, CodeExpression>(expression.MemberNames[i], member));
            }

            return result;
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqOrderByExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqParameterExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqSelectExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqSelectManyExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqSingleExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "Single");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqSingleOrDefaultExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, "SingleOrDefault");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqSkipExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            CodeExpression skipCount = this.GenerateCode(expression.SkipCount);

            return source.Call("Skip", skipCount);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqTakeExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            CodeExpression skipCount = this.GenerateCode(expression.TakeCount);

            return source.Call("Take", skipCount);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(LinqUnionExpression expression)
        {
            var firstSource = this.GenerateCode(expression.FirstSource);
            var secondSource = this.GenerateCode(expression.SecondSource);

            return firstSource.Call("Union", secondSource);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public abstract CodeExpression Visit(LinqWhereExpression expression);

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryAddExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.Add);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryAndExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.BooleanAnd);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryAsExpression expression)
        {
            CodeExpression argument = this.GenerateCode(expression.Source);
            CodeTypeReference typeRef = this.GetTypeReference(expression.TypeToOperateAgainst);
            return argument.TypeAs(typeRef);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryCastExpression expression)
        {
            CodeExpression argument = this.GenerateCode(expression.Source);
            CodeTypeReference type = this.GetTypeReference(expression.TypeToOperateAgainst);
            return argument.Cast(type);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryConstantExpression expression)
        {
            CodeExpression initializationExpression;
            if (this.CustomInitializationCodeGenerator == null || !this.CustomInitializationCodeGenerator.TryGenerateInitialization(expression.ScalarValue.Value, out initializationExpression))
            {
                initializationExpression = Code.ObjectValue(expression.ScalarValue.Value);
            }

            return initializationExpression;
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryCustomFunctionCallExpression expression)
        {
            var clrMethod = expression.Function.Annotations.OfType<ClrMethodAnnotation>().SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(clrMethod, "Cannot generate Linq syntax for function call expression as there is no information about corresponding Clr method. Function: {0}.", expression.Function.FullName);

            var type = new CodeTypeReferenceExpression(clrMethod.FullTypeName);

            var arguments = new List<CodeExpression>();
            if (expression.IsRoot)
            {
                ExceptionUtilities.CheckObjectNotNull(this.QueryProvider, "Cannot generate custom function call expression as a root as QueryProvider is not setup.");
                arguments.Add(this.QueryProvider);
            }

            arguments.AddRange(this.GenerateCodeForArguments(expression.Arguments));

            return type.Call(clrMethod.MethodName, arguments.ToArray());
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryDivideExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.Divide);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.ValueEquality);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryFunctionImportCallExpression expression)
        {
            var clrMethod = expression.FunctionImport.Annotations.OfType<ClrMethodAnnotation>().SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(clrMethod, "Cannot generate Linq syntax for function import call expression as there is no information about corresponding Clr method. Function: {0}.", expression.FunctionImport.Name);

            var type = new CodeTypeReferenceExpression(clrMethod.FullTypeName);

            var arguments = new List<CodeExpression>();
            if (expression.IsRoot)
            {
                ExceptionUtilities.CheckObjectNotNull(this.QueryProvider, "Cannot generate function import call expression as a root as QueryProvider is not setup.");
                arguments.Add(this.QueryProvider);
            }

            arguments.AddRange(this.GenerateCodeForArguments(expression.Arguments));

            return type.Call(clrMethod.MethodName, arguments.ToArray());
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryFunctionParameterReferenceExpression expression)
        {
            return Code.Variable(expression.ParameterName);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryGreaterThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.GreaterThan);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.GreaterThanOrEqual);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryIsNotNullExpression expression)
        {
            var argument = this.GenerateCode(expression.Argument);

            return argument.NotNull();
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryIsNullExpression expression)
        {
            var argument = this.GenerateCode(expression.Argument);

            return Code.ValueEquals(argument, Code.Null());
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryIsOfExpression expression)
        {
            CodeExpression argument = this.GenerateCode(expression.Source);
            CodeTypeReference type = this.GetTypeReference(expression.TypeToOperateAgainst);
            return argument.TypeIs(type);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryLessThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.LessThan);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryLessThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.LessThanOrEqual);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryModuloExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.Modulus);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryMultiplyExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.Multiply);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryNotEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.IdentityInequality);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryNotExpression expression)
        {
            return this.GenerateCode(expression.Argument).ValueEquals(Code.Primitive(false));
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryNullExpression expression)
        {
            return Code.Null();
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryOfTypeExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            CodeTypeReference type = this.GetTypeReference(expression.TypeToOperateAgainst);
            return source.Call("OfType", new CodeTypeReference[] { type });
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.BooleanOr);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryPropertyExpression expression)
        {
            CodeExpression instance = this.GenerateCode(expression.Instance);

            return instance.Property(expression.Name);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QueryRootExpression expression)
        {
            return this.RootExpression.Property(expression.Name);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public virtual CodeExpression Visit(QuerySubtractExpression expression)
        {
            return this.VisitBinaryExpression(expression, CodeBinaryOperatorType.Subtract);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        protected CodeExpression GenerateCode(QueryExpression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Resolves member Code Expressions to the correct type if needed prior to being assigned to the PropertyInitializer
        /// </summary>
        /// <param name="newExpression">Query Expression</param>
        /// <param name="memberName">Member Name</param>
        /// <param name="memberQueryExpression">New Code Member Expression </param>
        /// <param name="member">Member Code expression</param>
        /// <returns>Code member expression</returns>
        protected virtual CodeExpression FixupPropertyForPropertyInitializer(QueryExpression newExpression, string memberName, QueryExpression memberQueryExpression, CodeExpression member)
        {
            return member;
        }

        /// <summary>
        /// Generates code expressions for the arguments.
        /// </summary>
        /// <param name="arguments">The query expressions for the argumnents.</param>
        /// <returns>The code expressions generated.</returns>
        protected IEnumerable<CodeExpression> GenerateCodeForArguments(IEnumerable<QueryExpression> arguments)
        {
            return arguments.Select(a => this.GenerateCode(a));
        }

        private CodeTypeReference GetTypeReference(QueryType queryType)
        {
            IQueryClrType type = queryType as IQueryClrType;
            ExceptionUtilities.CheckObjectNotNull(type, "IsOf is only valid for types that inherit from IQueryClrType, the passed type is {0}", queryType.StringRepresentation);
            return Code.TypeRef(type.ClrType.FullName);
        }

        private CodeExpression VisitBinaryExpression(QueryBinaryExpression expression, CodeBinaryOperatorType binaryOperator)
        {
            CodeExpression left = this.GenerateCode(expression.Left);
            CodeExpression right = this.GenerateCode(expression.Right);

            return new CodeBinaryOperatorExpression(left, binaryOperator, right);
        }

        private CodeExpression VisitQueryMethodWithLambdaExpression(LinqQueryMethodWithLambdaExpression expression, string methodName)
        {
            CodeExpression source = this.GenerateCode(expression.Source);

            if (expression.Lambda == null)
            {
                return source.Call(methodName);
            }
            else
            {
                var lambda = this.GenerateCode(expression.Lambda) as CodeLambdaExpression;
                return source.Call(methodName, lambda);
            }
        }

        private CodeExpression VisitJoinExpressionBase(LinqJoinExpressionBase expression, string codeMethodName)
        {
            var outer = this.GenerateCode(expression.Outer);
            var inner = this.GenerateCode(expression.Inner);
            var outerKeySelector = this.GenerateCode(expression.OuterKeySelector);
            var innerKeySelector = this.GenerateCode(expression.InnerKeySelector);
            var resultSelector = this.GenerateCode(expression.ResultSelector);

            return outer.Call(codeMethodName, inner, outerKeySelector, innerKeySelector, resultSelector);
        }
    }
}
