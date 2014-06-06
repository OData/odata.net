//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_SERVER
namespace Microsoft.OData.Service
#else
namespace Microsoft.OData.Client
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
#if ASTORIA_LIGHT
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// This class introduced because of a bug in SL3 which prevents using non-public (e.g anonymous) types as return types for lambdas
    /// We should be able to remove this for SL4.
    /// devnote (sparra): This still seems to be an issue in SL4, but does not repro in Win8 .NETCore framework.
    /// </summary>
    internal static class ExpressionHelpers
    {
        /// <summary>Lambda function.</summary>
        private static MethodInfo lambdaFunc;

        /// <summary>
        /// Create a lambda function given the expression and the parameters.
        /// </summary>
        /// <param name="body">Expression for the lambda function.</param>
        /// <param name="parameters">Parameters for the lambda function.</param>
        /// <returns>An instance of LambdaExpression.</returns>
        internal static LambdaExpression CreateLambda(Expression body, params ParameterExpression[] parameters)
        {
            return CreateLambda(InferDelegateType(body, parameters), body, parameters);
        }

        /// <summary>
        /// This creates a tree and compiles it just for the purposes of creating the real lambda.
        /// </summary>
        /// <param name="delegateType">Generic type for the Lambda function.</param>
        /// <param name="body">Expression for the lambda function.</param>
        /// <param name="parameters">Parameters for the lambda function.</param>
        /// <returns>An instance of LambdaExpression.</returns>
        internal static LambdaExpression CreateLambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            // Expression.Lambda() doesn't work directly if "body" is a non-public type
            // Work around this by calling the factory from a DynamicMethod.
            var args = new[] { Expression.Parameter(typeof(Expression), "body"), Expression.Parameter(typeof(ParameterExpression[]), "parameters") };

            var lambdaFactory = Expression.Lambda<Func<Expression, ParameterExpression[], LambdaExpression>>(
                Expression.Call(GetLambdaFactoryMethod(delegateType), args), args);

            return lambdaFactory.Compile().Invoke(body, parameters);
        }

        /// <summary>
        /// Returns the generic type of the lambda function.
        /// </summary>
        /// <param name="body">Expression for the lambda function.</param>
        /// <param name="parameters">Parameters for the lambda function.</param>
        /// <returns>The generic type of the lambda function.</returns>
        private static Type InferDelegateType(Expression body, params ParameterExpression[] parameters)
        {
            bool isVoid = body.Type == typeof(void);
            int length = (parameters == null) ? 0 : parameters.Length;

            var typeArgs = new Type[length + (isVoid ? 0 : 1)];
            for (int i = 0; i < length; i++)
            {
                typeArgs[i] = parameters[i].Type;
            }

            if (isVoid)
            {
                return Expression.GetActionType(typeArgs);
            }
            else
            {
                typeArgs[length] = body.Type;
                return Expression.GetFuncType(typeArgs);
            }
        }

        /// <summary>
        /// Returns the methodinfo for the lambda function.
        /// </summary>
        /// <param name="delegateType">Generic type of the lambda function.</param>
        /// <returns>MethodInfo which binds the generic type to the lambda function.</returns>
        private static MethodInfo GetLambdaFactoryMethod(Type delegateType)
        {
            // Gets the MethodInfo for Expression.Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
            if (lambdaFunc == null)
            {
                lambdaFunc = new Func<Expression, ParameterExpression[], Expression<Action>>(Expression.Lambda<Action>).Method.GetGenericMethodDefinition();
            }

            // Create a throwaway delegate to bind to the right Labda function with a specific delegate type.
            return lambdaFunc.MakeGenericMethod(delegateType);
        }
    }
#endif

    /// <summary>
    /// base vistor class for walking an expression tree bottom up.
    /// </summary>
    internal abstract class ALinqExpressionVisitor
    {
        /// <summary>
        /// Main visit method for ALinqExpressionVisitor
        /// </summary>
        /// <param name="exp">The expression to visit</param>
        /// <returns>The visited expression </returns>
        internal virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.UnaryPlus:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
#if !ASTORIA_CLIENT
                case ExpressionType.Power:
#endif
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new NotSupportedException(Strings.ALinq_UnsupportedExpression(exp.NodeType.ToString()));
            }
        }

        /// <summary>
        /// MemberBinding visit method
        /// </summary>
        /// <param name="binding">The MemberBinding expression to visit</param>
        /// <returns>The visited MemberBinding expression </returns>
        internal virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new NotSupportedException(Strings.ALinq_UnsupportedExpression(binding.BindingType.ToString()));
            }
        }

        /// <summary>
        /// ElementInit visit method
        /// </summary>
        /// <param name="initializer">The ElementInit expression to visit</param>
        /// <returns>The visited ElementInit expression </returns>
        internal virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="u">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        internal virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }

                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }

            return b;
        }

        /// <summary>
        /// TypeBinaryExpression visit method
        /// </summary>
        /// <param name="b">The TypeBinaryExpression expression to visit</param>
        /// <returns>The visited TypeBinaryExpression expression </returns>
        internal virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        /// <summary>
        /// ConstantExpression visit method
        /// </summary>
        /// <param name="c">The ConstantExpression expression to visit</param>
        /// <returns>The visited ConstantExpression expression </returns>
        internal virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// ConditionalExpression visit method
        /// </summary>
        /// <param name="c">The ConditionalExpression expression to visit</param>
        /// <returns>The visited ConditionalExpression expression </returns>
        internal virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression iftrue = this.Visit(c.IfTrue);
            Expression iffalse = this.Visit(c.IfFalse);
            if (test != c.Test || iftrue != c.IfTrue || iffalse != c.IfFalse)
            {
                return Expression.Condition(test, iftrue, iffalse, iftrue.Type.IsAssignableFrom(iffalse.Type) ? iftrue.Type : iffalse.Type);
            }

            return c;
        }

        /// <summary>
        /// ParameterExpression visit method
        /// </summary>
        /// <param name="p">The ParameterExpression expression to visit</param>
        /// <returns>The visited ParameterExpression expression </returns>
        internal virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// MemberExpression visit method
        /// </summary>
        /// <param name="m">The MemberExpression expression to visit</param>
        /// <returns>The visited MemberExpression expression </returns>
        internal virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = this.Visit(m.Expression);
            return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        internal virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            return obj != m.Object || args != m.Arguments ? Expression.Call(obj, m.Method, args) : m;
        }

        /// <summary>
        /// Expression list visit method
        /// </summary>
        /// <param name="original">The expression list to visit</param>
        /// <returns>The visited expression list</returns>
        internal virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            if (list != null)
            {
                return new ReadOnlyCollection<Expression>(list);
            }

            return original;
        }

        /// <summary>
        /// MemberAssignment visit method
        /// </summary>
        /// <param name="assignment">The MemberAssignment to visit</param>
        /// <returns>The visited MemberAssignmentt</returns>
        internal virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
        }

        /// <summary>
        /// MemberMemberBinding visit method
        /// </summary>
        /// <param name="binding">The MemberMemberBinding to visit</param>
        /// <returns>The visited MemberMemberBinding</returns>
        internal virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        /// <summary>
        /// MemberListBinding visit method
        /// </summary>
        /// <param name="binding">The MemberListBinding to visit</param>
        /// <returns>The visited MemberListBinding</returns>
        internal virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        /// <summary>
        /// Binding List visit method
        /// </summary>
        /// <param name="original">The Binding list to visit</param>
        /// <returns>The visited Binding list</returns>
        internal virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        /// <summary>
        /// ElementInit expression list visit method
        /// </summary>
        /// <param name="original">The ElementInit expression list  to visit</param>
        /// <returns>The visited ElementInit expression list </returns>
        internal virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        /// <summary>
        /// LambdaExpression visit method
        /// </summary>
        /// <param name="lambda">The LambdaExpression to visit</param>
        /// <returns>The visited LambdaExpression</returns>
        internal virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
#if !ASTORIA_LIGHT
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
#else
                ParameterExpression[] parameters = new ParameterExpression[lambda.Parameters.Count];
                lambda.Parameters.CopyTo(parameters, 0);
                return ExpressionHelpers.CreateLambda(lambda.Type, body, parameters);
#endif
            }

            return lambda;
        }

        /// <summary>
        /// NewExpression visit method
        /// </summary>
        /// <param name="nex">The NewExpression to visit</param>
        /// <returns>The visited NewExpression</returns>
        internal virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                return nex.Members != null
                           ? Expression.New(nex.Constructor, args, nex.Members)
                           : Expression.New(nex.Constructor, args);
            }

            return nex;
        }

        /// <summary>
        /// MemberInitExpression visit method
        /// </summary>
        /// <param name="init">The MemberInitExpression to visit</param>
        /// <returns>The visited MemberInitExpression</returns>
        internal virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
        }

        /// <summary>
        /// ListInitExpression visit method
        /// </summary>
        /// <param name="init">The ListInitExpression to visit</param>
        /// <returns>The visited ListInitExpression</returns>
        internal virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
        }

        /// <summary>
        /// NewArrayExpression visit method
        /// </summary>
        /// <param name="na">The NewArrayExpression to visit</param>
        /// <returns>The visited NewArrayExpression</returns>
        internal virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                return na.NodeType == ExpressionType.NewArrayInit ? Expression.NewArrayInit(na.Type.GetElementType(), exprs) : Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
            }

            return na;
        }

        /// <summary>
        /// InvocationExpression visit method
        /// </summary>
        /// <param name="iv">The InvocationExpression to visit</param>
        /// <returns>The visited InvocationExpression</returns>
        internal virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
        }
    }
}
