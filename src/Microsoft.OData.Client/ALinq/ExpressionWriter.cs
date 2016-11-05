//---------------------------------------------------------------------
// <copyright file="ExpressionWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Special visitor to serialize supported expression as query parameters
    /// in the generated URI.
    /// </summary>
    internal class ExpressionWriter : DataServiceALinqExpressionVisitor
    {
        #region Private fields

        /// <summary>Internal buffer.</summary>
        private readonly StringBuilder builder;

        /// <summary>Data context used to generate type names for types.</summary>
        private readonly DataServiceContext context;

        /// <summary>Stack of expressions being visited.</summary>
        private readonly Stack<Expression> expressionStack;

        /// <summary>Whether or not the expression being written is part of the path of the URI.</summary>
        private readonly bool inPath;

        /// <summary>set if can't translate expression</summary>
        private bool cantTranslateExpression;

        /// <summary>Parent expression of the current expression (expression.Peek()); possibly null.</summary>
        private Expression parent;

        /// <summary>the request data service version for the uri</summary>
        private Version uriVersion;

        /// <summary>number of sub scopes (any/all calls) on stack</summary>
        private int scopeCount;

        /// <summary>
        /// currently writing functions in query options
        /// </summary>
        private bool writingFunctionsInQuery;

        #endregion Private fields

        /// <summary>
        /// Creates an ExpressionWriter for the specified <paramref name="context"/>.
        /// </summary>
        /// <param name='context'>Data context used to generate type names for types.</param>
        /// <param name='inPath'>Whether or not the expression being written is part of the path of the URI.</param>
        private ExpressionWriter(DataServiceContext context, bool inPath)
        {
            Debug.Assert(context != null, "context != null");
            this.context = context;
            this.inPath = inPath;
            this.builder = new StringBuilder();
            this.expressionStack = new Stack<Expression>();
            this.expressionStack.Push(null);
            this.uriVersion = Util.ODataVersion4;
            this.scopeCount = 0;
            this.writingFunctionsInQuery = false;
        }

        /// <summary>
        /// An enumeration indicating the direction of a child operand
        /// </summary>
        private enum ChildDirection
        {
            /// <summary>The operand is the left child</summary>
            Left,

            /// <summary>The operand is the right child</summary>
            Right
        }

        /// <summary>Whether inside any/all lambda or not</summary>
        private bool InSubScope
        {
            get
            {
                return this.scopeCount > 0;
            }
        }

        /// <summary>
        /// Serializes an expression to a string
        /// </summary>
        /// <param name='context'>Data context used to generate type names for types.</param>
        /// <param name="e">Expression to serialize</param>
        /// <param name='inPath'>Whether or not the expression being written is part of the path of the URI.</param>
        /// <param name="uriVersion">the request data service version for the uri</param>
        /// <returns>serialized expression</returns>
        internal static string ExpressionToString(DataServiceContext context, Expression e, bool inPath, ref Version uriVersion)
        {
            ExpressionWriter ew = new ExpressionWriter(context, inPath);
            string serialized = ew.Translate(e);
            WebUtil.RaiseVersion(ref uriVersion, ew.uriVersion);
            if (ew.cantTranslateExpression)
            {
                throw new NotSupportedException(Strings.ALinq_CantTranslateExpression(e.ToString()));
            }

            return serialized;
        }

        /// <summary>Main visit method.</summary>
        /// <param name="exp">Expression to visit</param>
        /// <returns>Visited expression</returns>
        internal override Expression Visit(Expression exp)
        {
            this.parent = this.expressionStack.Peek();
            this.expressionStack.Push(exp);
            Expression result = base.Visit(exp);
            this.expressionStack.Pop();
            return result;
        }

        /// <summary>
        /// ConditionalExpression visit method
        /// </summary>
        /// <param name="c">The ConditionalExpression expression to visit</param>
        /// <returns>The visited ConditionalExpression expression </returns>
        internal override Expression VisitConditional(ConditionalExpression c)
        {
            this.cantTranslateExpression = true;
            return c;
        }

        /// <summary>
        /// LambdaExpression visit method
        /// </summary>
        /// <param name="lambda">The LambdaExpression to visit</param>
        /// <returns>The visited LambdaExpression</returns>
        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            this.cantTranslateExpression = true;
            return lambda;
        }

        /// <summary>
        /// NewExpression visit method
        /// </summary>
        /// <param name="nex">The NewExpression to visit</param>
        /// <returns>The visited NewExpression</returns>
        internal override NewExpression VisitNew(NewExpression nex)
        {
            this.cantTranslateExpression = true;
            return nex;
        }

        /// <summary>
        /// MemberInitExpression visit method
        /// </summary>
        /// <param name="init">The MemberInitExpression to visit</param>
        /// <returns>The visited MemberInitExpression</returns>
        internal override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.cantTranslateExpression = true;
            return init;
        }

        /// <summary>
        /// ListInitExpression visit method
        /// </summary>
        /// <param name="init">The ListInitExpression to visit</param>
        /// <returns>The visited ListInitExpression</returns>
        internal override Expression VisitListInit(ListInitExpression init)
        {
            this.cantTranslateExpression = true;
            return init;
        }

        /// <summary>
        /// NewArrayExpression visit method
        /// </summary>
        /// <param name="na">The NewArrayExpression to visit</param>
        /// <returns>The visited NewArrayExpression</returns>
        internal override Expression VisitNewArray(NewArrayExpression na)
        {
            this.cantTranslateExpression = true;
            return na;
        }

        /// <summary>
        /// InvocationExpression visit method
        /// </summary>
        /// <param name="iv">The InvocationExpression to visit</param>
        /// <returns>The visited InvocationExpression</returns>
        internal override Expression VisitInvocation(InvocationExpression iv)
        {
            this.cantTranslateExpression = true;
            return iv;
        }

        /// <summary>
        /// Input resource set references are intentionally omitted from the URL string for the top level
        /// refences to input parameter (i.e. outside of any/all methods).
        /// For parameter references to input (range variable for Where) inside any/all methods we write "$it".
        /// </summary>
        /// <param name="ire">The input reference</param>
        /// <returns>The same input reference expression</returns>
        internal override Expression VisitInputReferenceExpression(InputReferenceExpression ire)
        {
            // This method intentionally does not write anything to the URI for implicit references to the input parameter ($it).
            // This is how 'Where(<input>.Id == 5)' becomes '$filter=Id eq 5'.
            Debug.Assert(ire != null, "ire != null");
            if (this.parent == null || (!this.InSubScope && this.parent.NodeType != ExpressionType.MemberAccess && this.parent.NodeType != ExpressionType.TypeAs && this.parent.NodeType != ExpressionType.Call))
            {
                // Ideally we refer to the parent expression as the un-translatable one,
                // because we cannot reference 'this' as a standalone expression; however
                // if the parent is null for any reasonn, we fall back to the expression itself.
                string expressionText = (this.parent != null) ? this.parent.ToString() : ire.ToString();
                throw new NotSupportedException(Strings.ALinq_CantTranslateExpression(expressionText));
            }

            // Write "$it" for input parameter reference inside any/all methods
            if (this.InSubScope || this.parent.NodeType == ExpressionType.Call)
            {
                this.builder.Append(XmlConstants.ImplicitFilterParameter);
            }

            return ire;
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            string methodName;
            if (TypeSystem.TryGetQueryOptionMethod(m.Method, out methodName))
            {
                this.builder.Append(methodName);
                this.builder.Append(UriHelper.LEFTPAREN);

                // There is a single function, 'contains', which reorders its argument with 
                // respect to the CLR method. Thus handling it as a special case rather than
                // using a more general argument reordering mechanism.
                if (methodName == "contains")
                {
                    Debug.Assert(m.Method.Name == "Contains", "m.Method.Name == 'Contains'");
                    Debug.Assert(m.Object != null, "m.Object != null");
                    Debug.Assert(m.Arguments.Count == 1, "m.Arguments.Count == 1");
                    this.Visit(m.Object);
                    this.builder.Append(UriHelper.COMMA);
                    this.Visit(m.Arguments[0]);
                }
                else
                {
                    if (m.Object != null)
                    {
                        this.Visit(m.Object);
                    }

                    if (m.Arguments.Count > 0)
                    {
                        if (m.Object != null)
                        {
                            this.builder.Append(UriHelper.COMMA);
                        }

                        for (int ii = 0; ii < m.Arguments.Count; ii++)
                        {
                            this.Visit(m.Arguments[ii]);
                            if (ii < m.Arguments.Count - 1)
                            {
                                this.builder.Append(UriHelper.COMMA);
                            }
                        }
                    }
                }

                this.builder.Append(UriHelper.RIGHTPAREN);
            }
            else if (m.Method.Name == "HasFlag")
            {
                Debug.Assert(m.Method.Name == "HasFlag", "m.Method.Name == 'HasFlag'");
                Debug.Assert(m.Object != null, "m.Object != null");
                Debug.Assert(m.Arguments.Count == 1, "m.Arguments.Count == 1");
                this.Visit(m.Object);
                this.builder.Append(UriHelper.SPACE);
                this.builder.Append(UriHelper.HAS);
                this.builder.Append(UriHelper.SPACE);
                this.Visit(m.Arguments[0]);
            }
            else
            {
                SequenceMethod sequenceMethod;
                if (ReflectionUtil.TryIdentifySequenceMethod(m.Method, out sequenceMethod))
                {
                    if (ReflectionUtil.IsAnyAllMethod(sequenceMethod))
                    {
                        // Raise the uriVersion each time we write any or all methods to the uri.
                        WebUtil.RaiseVersion(ref this.uriVersion, Util.ODataVersion4);

                        this.Visit(m.Arguments[0]);
                        this.builder.Append(UriHelper.FORWARDSLASH);
                        if (sequenceMethod == SequenceMethod.All)
                        {
                            this.builder.Append(XmlConstants.AllMethodName);
                        }
                        else
                        {
                            this.builder.Append(XmlConstants.AnyMethodName);
                        }

                        this.builder.Append(UriHelper.LEFTPAREN);
                        if (sequenceMethod != SequenceMethod.Any)
                        {
                            // SequenceMethod.Any represents Enumerable.Any(), which has only source argument
                            // AnyPredicate and All has a second parameter which is the predicate lambda.
                            Debug.Assert(m.Arguments.Count() == 2, "m.Arguments.Count() == 2");
                            LambdaExpression le = (LambdaExpression)m.Arguments[1];
                            string rangeVariable = le.Parameters[0].Name;
                            this.builder.Append(rangeVariable);
                            this.builder.Append(UriHelper.COLON);
                            this.scopeCount++;
                            this.Visit(le.Body);
                            this.scopeCount--;
                        }

                        this.builder.Append(UriHelper.RIGHTPAREN);
                        return m;
                    }
                    else if (sequenceMethod == SequenceMethod.OfType && this.parent != null)
                    {
                        // check to see if this is an OfType filter for Any or All. 
                        // e.g. ctx.CreateQuery<Movie>("Movies").Where(m=>m.Actors.OfType<MegaStar>().Any())
                        //      which translates to /Movies()?$filter=Actors/MegaStar/any()
                        MethodCallExpression mce = this.parent as MethodCallExpression;
                        if (mce != null && ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod) && ReflectionUtil.IsAnyAllMethod(sequenceMethod))
                        {
                            Type filteredType = mce.Method.GetGenericArguments().SingleOrDefault();
                            if (ClientTypeUtil.TypeOrElementTypeIsEntity(filteredType))
                            {
                                this.Visit(m.Arguments[0]);
                                this.builder.Append(UriHelper.FORWARDSLASH);

                                UriHelper.AppendTypeSegment(this.builder, filteredType, this.context, this.inPath, ref this.uriVersion);

                                return m;
                            }
                        }
                    }
                    else if (sequenceMethod == SequenceMethod.Count && this.parent != null)
                    {
                        if (m.Arguments.Any() && m.Arguments[0] != null)
                        {
                            this.Visit(m.Arguments[0]);
                        }

                        this.builder.Append(UriHelper.FORWARDSLASH).Append(UriHelper.DOLLARSIGN).Append(UriHelper.COUNT);
                        return m;
                    }
                }
                else
                {
                    if (m.Object != null)
                    {
                        this.Visit(m.Object);
                    }

                    if (m.Method.Name != "GetValue" && m.Method.Name != "GetValueAsync")
                    {
                        this.builder.Append(UriHelper.FORWARDSLASH);

                        // writing functions in query options
                        writingFunctionsInQuery = true;
                        string declaringType = this.context.ResolveNameFromTypeInternal(m.Method.DeclaringType);
                        if (string.IsNullOrEmpty(declaringType))
                        {
                            throw new NotSupportedException(Strings.ALinq_CantTranslateExpression(m.ToString()));
                        }

                        int index = declaringType.LastIndexOf('.');
                        string fullNamespace = declaringType.Remove(index + 1);
                        string serverMethodName = ClientTypeUtil.GetServerDefinedName(m.Method);
                        this.builder.Append(fullNamespace + serverMethodName);
                        this.builder.Append(UriHelper.LEFTPAREN);
                        string[] argumentNames = m.Method.GetParameters().Select(p => p.Name).ToArray();
                        for (int i = 0; i < m.Arguments.Count; ++i)
                        {
                            this.builder.Append(argumentNames[i]);
                            this.builder.Append(UriHelper.EQUALSSIGN);
                            this.scopeCount++;
                            this.Visit(m.Arguments[i]);
                            this.scopeCount--;
                            this.builder.Append(UriHelper.COMMA);
                        }

                        if (m.Arguments.Any())
                        {
                            this.builder.Remove(this.builder.Length - 1, 1);
                        }

                        this.builder.Append(UriHelper.RIGHTPAREN);
                        writingFunctionsInQuery = false;
                    }

                    return m;
                }

                this.cantTranslateExpression = true;
            }

            return m;
        }

        /// <summary>
        /// Serializes an MemberExpression to a string
        /// </summary>
        /// <param name="m">Expression to serialize</param>
        /// <returns>MemberExpression</returns>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member is FieldInfo)
            {
                throw new NotSupportedException(Strings.ALinq_CantReferToPublicField(m.Member.Name));
            }

            Expression e = this.Visit(m.Expression);

            // if this is a Nullable<T> instance, don't write out /Value since not supported by server
            if (m.Member.Name == "Value" && m.Member.DeclaringType.IsGenericType()
                && m.Member.DeclaringType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return m;
            }

            // if this is a GetValueAsync().Result call in async scenario, don't write out /Result
            MethodCallExpression methodCallExpression = m.Expression as MethodCallExpression;
            if (methodCallExpression != null && methodCallExpression.Method.Name == "GetValueAsync" && m.Member.Name == "Result")
            {
                return m;
            }

            if (!this.IsImplicitInputReference(e) || writingFunctionsInQuery)
            {
                this.builder.Append(UriHelper.FORWARDSLASH);
            }

            // If the member is collection with count property, it will be parsed as $count segment
            var parentType = m.Member.DeclaringType;
            Type collectionType = ClientTypeUtil.GetImplementationType(parentType, typeof(ICollection<>));
            if (!PrimitiveType.IsKnownNullableType(parentType) && collectionType != null &&
                m.Member.Name.Equals(ReflectionUtil.COUNTPROPNAME))
            {
                this.builder.Append(UriHelper.DOLLARSIGN).Append(UriHelper.COUNT);
            }
            else
            {
                this.builder.Append(ClientTypeUtil.GetServerDefinedName(m.Member));
            }

            return m;
        }

        /// <summary>
        /// ConstantExpression visit method
        /// </summary>
        /// <param name="c">The ConstantExpression expression to visit</param>
        /// <returns>The visited ConstantExpression expression </returns>
        internal override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                this.builder.Append(UriHelper.NULL);
                return c;
            }

            // DEVNOTE: 
            // Rather than forcing every other codepath to have the 'Try...' pattern for formatting, 
            // we catch the InvalidOperationException here to change the exception type.
            // This is exceedingly rare, and not a scenario where performance is meaningful, so the 
            // reduced complexity in all other call sites is worth the extra logic here.
            string result;
            BinaryExpression b = this.parent as BinaryExpression;
            MethodCallExpression m = this.parent as MethodCallExpression;
            if ((b != null && HasEnumInBinaryExpression(b)) || (m != null && m.Method.Name == "HasFlag"))
            {
                c = this.ConvertConstantExpressionForEnum(c);
                ClientEdmModel model = this.context.Model;
                IEdmType edmType = model.GetOrCreateEdmType(c.Type.IsEnum() ? c.Type : c.Type.GetGenericArguments()[0]);
                ClientTypeAnnotation typeAnnotation = model.GetClientTypeAnnotation(edmType);
                string typeNameInEdm = this.context.ResolveNameFromTypeInternal(typeAnnotation.ElementType);
                MemberInfo member = typeAnnotation.ElementType.GetField(c.Value.ToString());
                string memberValue = ClientTypeUtil.GetServerDefinedName(member);
                ODataEnumValue enumValue = new ODataEnumValue(memberValue, typeNameInEdm ?? typeAnnotation.ElementTypeName);
                result = ODataUriUtils.ConvertToUriLiteral(enumValue, CommonUtil.ConvertToODataVersion(this.uriVersion), null);
            }
            else
            {
                try
                {
                    result = LiteralFormatter.ForConstants.Format(c.Value);
                }
                catch (InvalidOperationException)
                {
                    if (this.cantTranslateExpression)
                    {
                        // there's already a problem in the parents.
                        // we should just return here, because caller somewhere up the stack will throw a better exception
                        return c;
                    }

                    throw new NotSupportedException(Strings.ALinq_CouldNotConvert(c.Value));
                }
            }
            
            Debug.Assert(result != null, "result != null");
            this.builder.Append(result);
            return c;
        }

        /// <summary>
        /// Serializes an UnaryExpression to a string
        /// </summary>
        /// <param name="u">Expression to serialize</param>
        /// <returns>UnaryExpression</returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this.builder.Append(UriHelper.NOT);
                    this.builder.Append(UriHelper.SPACE);
                    this.VisitOperand(u.Operand);
                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.builder.Append(UriHelper.SPACE);
                    this.builder.Append(UriHelper.NEGATE);
                    this.VisitOperand(u.Operand);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    if (u.Type != typeof(object))
                    {
                        if (IsEnumTypeExpression(u))
                        {
                            this.Visit(u.Operand);
                        }
                        else
                        {
                            this.builder.Append(UriHelper.CAST);
                            this.builder.Append(UriHelper.LEFTPAREN);
                            if (!this.IsImplicitInputReference(u.Operand))
                            {
                                this.Visit(u.Operand);
                                this.builder.Append(UriHelper.COMMA);
                            }

                            this.builder.Append(UriHelper.QUOTE);
                            this.builder.Append(UriHelper.GetTypeNameForUri(u.Type, this.context));
                            this.builder.Append(UriHelper.QUOTE);
                            this.builder.Append(UriHelper.RIGHTPAREN);
                        }
                    }
                    else
                    {
                        if (!this.IsImplicitInputReference(u.Operand))
                        {
                            this.Visit(u.Operand);
                        }
                    }

                    break;
                case ExpressionType.TypeAs:
                    if (u.Operand.NodeType == ExpressionType.TypeAs)
                    {
                        throw new NotSupportedException(Strings.ALinq_CannotUseTypeFiltersMultipleTimes);
                    }

                    this.Visit(u.Operand);

                    if (!this.IsImplicitInputReference(u.Operand))
                    {
                        // InputReferenceExpressions aren't emitted, so no leading slash is required
                        this.builder.Append(UriHelper.FORWARDSLASH);
                    }

                    UriHelper.AppendTypeSegment(this.builder, u.Type, this.context, this.inPath, ref this.uriVersion);

                    break;
                case ExpressionType.UnaryPlus:
                    // no-op always ignore.
                    break;
                default:
                    this.cantTranslateExpression = true;
                    break;
            }

            return u;
        }

        /// <summary>
        /// Serializes an BinaryExpression to a string
        /// </summary>
        /// <param name="b">BinaryExpression to serialize</param>
        /// <returns>serialized expression</returns>
        internal override Expression VisitBinary(BinaryExpression b)
        {
            this.VisitOperand(b.Left, b.NodeType, ChildDirection.Left);
            this.builder.Append(UriHelper.SPACE);
            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    this.builder.Append(UriHelper.AND);
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    this.builder.Append(UriHelper.OR);
                    break;
                case ExpressionType.Equal:
                    this.builder.Append(UriHelper.EQ);
                    break;
                case ExpressionType.NotEqual:
                    this.builder.Append(UriHelper.NE);
                    break;
                case ExpressionType.LessThan:
                    this.builder.Append(UriHelper.LT);
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.builder.Append(UriHelper.LE);
                    break;
                case ExpressionType.GreaterThan:
                    this.builder.Append(UriHelper.GT);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.builder.Append(UriHelper.GE);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    this.builder.Append(UriHelper.ADD);
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    this.builder.Append(UriHelper.SUB);
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    this.builder.Append(UriHelper.MUL);
                    break;
                case ExpressionType.Divide:
                    this.builder.Append(UriHelper.DIV);
                    break;
                case ExpressionType.Modulo:
                    this.builder.Append(UriHelper.MOD);
                    break;
                case ExpressionType.ArrayIndex:
                case ExpressionType.Power:
                case ExpressionType.Coalesce:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                default:
                    this.cantTranslateExpression = true;
                    break;
            }

            this.builder.Append(UriHelper.SPACE);
            this.VisitOperand(b.Right, b.NodeType, ChildDirection.Right);
            return b;
        }

        /// <summary>
        /// Serializes an TypeBinaryExpression to a string
        /// </summary>
        /// <param name="b">TypeBinaryExpression to serialize</param>
        /// <returns>serialized expression</returns>
        internal override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.builder.Append(UriHelper.ISOF);
            this.builder.Append(UriHelper.LEFTPAREN);

            if (!this.IsImplicitInputReference(b.Expression))
            {
                this.Visit(b.Expression);
                this.builder.Append(UriHelper.COMMA);
                this.builder.Append(UriHelper.SPACE);
            }

            this.builder.Append(UriHelper.QUOTE);
            this.builder.Append(UriHelper.GetTypeNameForUri(b.TypeOperand, this.context));
            this.builder.Append(UriHelper.QUOTE);
            this.builder.Append(UriHelper.RIGHTPAREN);

            return b;
        }

        /// <summary>
        /// ParameterExpression visit method.
        /// </summary>
        /// <param name="p">The ParameterExpression expression to visit</param>
        /// <returns>The visited ParameterExpression expression </returns>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            if (this.InSubScope)
            {
                this.builder.Append(p.Name);
            }

            return p;
        }

        /// <summary>
        /// Indicates if two expression types are collapsible, e.g., ((a or b) or c) can be collapsed to (a or b or c).
        /// </summary>
        /// <param name="type">The expression type</param>
        /// <param name="parentType">The expression type of the parent expression</param>
        /// <param name="childDirection">Indicates if the expression is to the left or the right of the parent expression</param>
        /// <returns>True if the two expression types are collapsible, false otherwise</returns>
        private static bool AreExpressionTypesCollapsible(ExpressionType type, ExpressionType parentType, ChildDirection childDirection)
        {
            int precedence = BinaryPrecedence(type);
            int parentPrecedence = BinaryPrecedence(parentType);

            // don't process if operators are not supported
            if (precedence >= 0 && parentPrecedence >= 0)
            {
                if (childDirection == ChildDirection.Left)
                {
                    // Left nodes do not need parentheses if the precedence is equal or higher than the parent, e.g.,
                    //   (1 + 2) + 3 => 1 + 2 + 3
                    //   (1 * 2) + 3 => 1 * 2 + 3
                    if (precedence <= parentPrecedence)
                    {
                        return true;
                    }
                }
                else
                {
                    // Right nodes do not need parentheses if the precedence is higher than the parent
                    if (precedence < parentPrecedence)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the precedence of a binary operator for comparison purposes, or -1 if not applicable.
        /// </summary>
        /// <param name="type">The ExpressionType representing the binary operator</param>
        /// <returns>The precedence of a binary operator for comparison purposes, or -1 if not applicable</returns>
        private static int BinaryPrecedence(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    return 4;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return 3;
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return 2;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return 1;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    return 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Visits operands for Binary and Unary expressions.
        /// Will only output parens if operand is complex expression,
        /// this is so don't have unecessary parens in URI.
        /// </summary>
        /// <param name="e">The operand expression to visit</param>
        private void VisitOperand(Expression e)
        {
            this.VisitOperand(e, null, null);
        }

        /// <summary>
        /// Visits operands for Binary and Unary expressions.
        /// Will only output parens if operand is complex expression,
        /// this is so don't have unecessary parens in URI.
        /// </summary>
        /// <param name="e">The operand expression to visit</param>
        /// <param name="parentType">The node type of the parent expression (if applicable)</param>
        /// <param name="childDirection">Indicates if the expression is to the left or the right of the parent expression</param>
        private void VisitOperand(Expression e, ExpressionType? parentType, ChildDirection? childDirection)
        {
            Debug.Assert(
                parentType.HasValue == childDirection.HasValue,
                "If a parent type is specified, a child direction must also be specified, or both must be unspecified.");
            if (e is BinaryExpression)
            {
                bool requiresParens = !parentType.HasValue ||
                    !AreExpressionTypesCollapsible(e.NodeType, parentType.Value, childDirection.Value);
                
                if (requiresParens)
                {
                    this.builder.Append(UriHelper.LEFTPAREN);
                }

                this.Visit(e);

                if (requiresParens)
                {
                    this.builder.Append(UriHelper.RIGHTPAREN);
                }
            }
            else
            {
                this.Visit(e);
            }
        }

        /// <summary>
        /// Serializes an expression to a string
        /// </summary>
        /// <param name="e">Expression to serialize</param>
        /// <returns>serialized expression</returns>
        private string Translate(Expression e)
        {
            this.Visit(e);
            return this.builder.ToString();
        }

        /// <summary>
        /// The references to parameter for the main predicate (.Where()) is implicit outside any/all methods.
        /// </summary>
        /// <param name="exp">The expression to test</param>
        /// <returns><c>true</c> if the expression represents a reference to the current (resource set) input and it is not in any/all method; otherwise <c>false</c>.</returns>
        private bool IsImplicitInputReference(Expression exp)
        {
            // in subscope (i.e. any/all method), references are explicit.
            if (this.InSubScope)
            {
                return false;
            }

            return (exp is InputReferenceExpression || exp is ParameterExpression);
        }

        /// <summary>
        /// Check whether this BinaryExpression has enum type in it
        /// </summary>
        /// <param name="b">The BinaryExpression to check</param>
        /// <returns>The checked result</returns>
        private static bool HasEnumInBinaryExpression(BinaryExpression b)
        {
            return IsEnumTypeExpression(b.Left) || IsEnumTypeExpression(b.Right);
        }

        /// <summary>
        /// Check whether the type of this Expresion is enum
        /// </summary>
        /// <param name="e">The BinaryExpression to check</param>
        /// <returns>The checked result</returns>
        private static bool IsEnumTypeExpression(Expression e)
        {
            UnaryExpression u = e as UnaryExpression;
            if (u != null)
            {
                return u.Operand.Type.IsEnum() || (u.Operand.Type.IsGenericType() && u.Operand.Type.GetGenericArguments()[0].IsEnum());
            }

            return false;
        }

        /// <summary>
        /// Get the expected enum type from UnaryExpression
        /// </summary>
        /// <param name="e">The Expression to get enum type from</param>
        /// <returns>The extracted enum type</returns>
        private static Type GetEnumType(Expression e)
        {
            UnaryExpression u = e as UnaryExpression;
            if (u != null)
            {
                return u.Operand.Type.IsEnum() ? u.Operand.Type : u.Operand.Type.GetGenericArguments()[0];
            }

            Debug.Assert(e.Type.IsEnum() || e.Type.GetGenericArguments()[0].IsEnum(), "e.Type.IsEnum || e.Type.GetGenericArguments()[0].IsEnum");
            return e.Type.IsEnum() ? e.Type : e.Type.GetGenericArguments()[0];
        }

        /// <summary>
        /// Convert a ConstantExpression into expected enum type
        /// </summary>
        /// <param name="constant">The ConstantExpression to convert</param>
        /// <returns>The converted ConstantExpression</returns>
        private ConstantExpression ConvertConstantExpressionForEnum(ConstantExpression constant)
        {
            Type enumType = null;
            if (this.parent is BinaryExpression)
            {
                BinaryExpression b = this.parent as BinaryExpression;
                if (constant == b.Left)
                {
                    Debug.Assert(b.Right is UnaryExpression, "another binary operand should be UnaryExpression");
                    enumType = GetEnumType(b.Right);
                }
                else
                {
                    Debug.Assert(b.Left is UnaryExpression, "another binary operand should be UnaryExpression");
                    enumType = GetEnumType(b.Left);
                }
            }
            else
            {
                MethodCallExpression m = this.parent as MethodCallExpression;
                if (m != null && m.Method.Name == "HasFlag")
                {
                    enumType = GetEnumType(m.Object);
                }
            }

            Debug.Assert(enumType != null, "enumType != null");
            return Expression.Constant(Enum.Parse(enumType, constant.Value.ToString(), false));
        }
    }
}
