//---------------------------------------------------------------------
// <copyright file="FunctionExpressionBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Service.Providers;
    using Enumerable = System.Linq.Enumerable;
    using OpenTypeMethods = Microsoft.OData.Service.Providers.OpenTypeMethods;

    /// <summary>
    /// Component for binding functions into LINQ expressions.
    /// </summary>
    internal class FunctionExpressionBinder
    {
        /// <summary>Callback to resolve a resource type by name.</summary>
        private readonly Func<string, ResourceType> tryResolveResourceType;

        /// <summary>A mapping from constant expressions back to their original string value in case they need to be converted.</summary>
        private readonly IDictionary<Expression, string> literals;

        /// <summary>A type that is not numeric.</summary>
        private const int NumericTypeNotNumeric = 0;

        /// <summary>A type that is a char, single, double or decimal.</summary>
        private const int NumericTypeNotIntegral = 1;

        /// <summary>A type that is a signed integral.</summary>
        private const int NumericTypeSignedIntegral = 2;

        /// <summary>A type that is an unsigned integral.</summary>
        private const int NumericTypeUnsignedIntegral = 3;

        /// <summary>
        /// Initializes a new instance of <see cref="FunctionExpressionBinder"/>.
        /// </summary>
        /// <param name="tryResolveResourceType">Callback to resolve a resource type by name.</param>
        internal FunctionExpressionBinder(Func<string, ResourceType> tryResolveResourceType)
        {
            Debug.Assert(tryResolveResourceType != null, "tryResolveResourceType != null");
            this.tryResolveResourceType = tryResolveResourceType;
            this.literals = new Dictionary<Expression, string>(ReferenceEqualityComparer<Expression>.Instance);
        }

        /// <summary>Finds the best fitting function for the specified arguments.</summary>
        /// <param name="functions">Functions to consider.</param>
        /// <param name="arguments">Arguments; if a best function is found, promoted arguments.</param>
        /// <returns>The best fitting function; null if none found or ambiguous.</returns>
        internal FunctionDescription FindBestFunction(FunctionDescription[] functions, ref Expression[] arguments)
        {
            Debug.Assert(functions != null, "functions != null");
            List<FunctionDescription> applicableFunctions = new List<FunctionDescription>(functions.Length);
            List<Expression[]> applicablePromotedArguments = new List<Expression[]>(functions.Length);

            // Build a list of applicable functions (and cache their promoted arguments).
            foreach (FunctionDescription candidate in functions)
            {
                if (candidate.ParameterTypes.Length != arguments.Length)
                {
                    continue;
                }

                Expression[] promotedArguments = new Expression[arguments.Length];
                bool argumentsMatch = true;
                for (int i = 0; i < candidate.ParameterTypes.Length; i++)
                {
                    promotedArguments[i] = this.PromoteExpression(arguments[i], candidate.ParameterTypes[i], true);
                    if (promotedArguments[i] == null)
                    {
                        argumentsMatch = false;
                        break;
                    }
                }

                if (argumentsMatch)
                {
                    applicableFunctions.Add(candidate);
                    applicablePromotedArguments.Add(promotedArguments);
                }
            }

            // Return the best applicable function.
            if (applicableFunctions.Count == 0)
            {
                // No matching function.
                return null;
            }
            else if (applicableFunctions.Count == 1)
            {
                arguments = applicablePromotedArguments[0];
                return applicableFunctions[0];
            }
            else
            {
                // Find a single function which is better than all others.
                int bestFunctionIndex = -1;
                for (int i = 0; i < applicableFunctions.Count; i++)
                {
                    bool betterThanAllOthers = true;
                    for (int j = 0; j < applicableFunctions.Count; j++)
                    {
                        if (i != j && IsBetterThan(arguments, applicableFunctions[j].ParameterTypes, applicableFunctions[i].ParameterTypes))
                        {
                            betterThanAllOthers = false;
                            break;
                        }
                    }

                    if (betterThanAllOthers)
                    {
                        Debug.Assert(bestFunctionIndex == -1, "bestFunctionIndex == -1");
                        bestFunctionIndex = i;
                        break;
                    }
                }

                if (bestFunctionIndex == -1)
                {
                    return null;
                }

                arguments = applicablePromotedArguments[bestFunctionIndex];
                return applicableFunctions[bestFunctionIndex];
            }
        }

        /// <summary>Checks that the operand (possibly promoted) is valid for the specified operation.</summary>
        /// <param name="signatures">Type with signatures to match.</param>
        /// <param name="operatorKind">Kind of operation for error reporting.</param>
        /// <param name="expr">Expression for operand.</param>
        internal void CheckAndPromoteOperand(Type signatures, UnaryOperatorKind operatorKind, ref Expression expr)
        {
            Debug.Assert(signatures != null, "signatures != null");
            Expression[] args = new Expression[] { expr };
            MethodBase method;
            if (this.FindMethod(signatures, "F", args, out method) != 1)
            {
                string message = Strings.RequestQueryParser_IncompatibleOperand(operatorKind, WebUtil.GetTypeName(args[0].Type));
                throw DataServiceException.CreateSyntaxError(message);
            }

            expr = args[0];
        }

        /// <summary>Checks that the operands (possibly promoted) are valid for the specified operation.</summary>
        /// <param name="signatures">Type with signatures to match.</param>
        /// <param name="operatorKind">Kind of operation for error reporting.</param>
        /// <param name="left">Expression for left operand.</param>
        /// <param name="right">Expression for right operand.</param>
        internal void CheckAndPromoteOperands(Type signatures, BinaryOperatorKind operatorKind, ref Expression left, ref Expression right)
        {
            Expression[] args = new Expression[] { left, right };
            MethodBase method;
            if (this.FindMethod(signatures, "F", args, out method) != 1)
            {
                string message = Strings.RequestQueryParser_IncompatibleOperands(
                    operatorKind,
                    WebUtil.GetTypeName(left.Type),
                    WebUtil.GetTypeName(right.Type));
                throw DataServiceException.CreateSyntaxError(message);
            }

            left = args[0];
            right = args[1];
        }

        /// <summary>
        /// Tracks the literals original text for use in converting it's type during method call binding.
        /// </summary>
        /// <param name="expr">The literal's constant expression.</param>
        /// <param name="text">The original text of the literal.</param>
        internal void TrackOriginalTextOfLiteral(ConstantExpression expr, string text)
        {
            this.literals.Add(expr, text);
        }

        /// <summary>Checks whether the specified type is a signed integral type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is a signed integral type; false otherwise.</returns>
        private static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == NumericTypeSignedIntegral;
        }

        /// <summary>Checks whether the specified type is an unsigned integral type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is an unsigned integral type; false otherwise.</returns>
        private static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == NumericTypeUnsignedIntegral;
        }

        /// <summary>Gets a flag for the numeric kind of type.</summary>
        /// <param name="type">Type to get numeric kind for.</param>
        /// <returns>
        /// One of NumericTypeNotNumeric; NumericTypeNotIntegral if it's char,
        /// single, double or decimal; NumericTypeSignedIntegral, or NumericTypeUnsignedIntegral.
        /// </returns>
        private static int GetNumericTypeKind(Type type)
        {
            type = WebUtil.GetNonNullableType(type);
            Debug.Assert(!type.IsEnum, "!type.IsEnum");
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return NumericTypeNotIntegral;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return NumericTypeSignedIntegral;
                case TypeCode.Byte:
                    return NumericTypeUnsignedIntegral;
                default:
                    return NumericTypeNotNumeric;
            }
        }

        /// <summary>Returns an object that can enumerate the specified type and its supertypes.</summary>
        /// <param name="type">Type to based enumeration on.</param>
        /// <returns>An object that can enumerate the specified type and its supertypes.</returns>
        private static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }

            return SelfAndBaseClasses(type);
        }

        /// <summary>Returns an object that can enumerate the specified type and its supertypes.</summary>
        /// <param name="type">Type to based enumeration on.</param>
        /// <returns>An object that can enumerate the specified type and its supertypes.</returns>
        private static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        /// <summary>Adds an interface type to a list of types, including inherited interfaces.</summary>
        /// <param name="types">Types list ot add to.</param>
        /// <param name="type">Interface type to add.</param>
        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces())
                {
                    AddInterface(types, t);
                }
            }
        }

        /// <summary>Finds the best applicable methods from the specified array that match the arguments.</summary>
        /// <param name="applicable">Candidate methods.</param>
        /// <param name="args">Argument expressions.</param>
        /// <returns>Best applicable methods.</returns>
        private static MethodData[] FindBestApplicableMethods(MethodData[] applicable, Expression[] args)
        {
            Debug.Assert(applicable != null, "applicable != null");

            List<MethodData> result = new List<MethodData>();
            foreach (MethodData method in applicable)
            {
                bool betterThanAllOthers = true;
                foreach (MethodData otherMethod in applicable)
                {
                    if (otherMethod != method && IsBetterThan(args, otherMethod, method))
                    {
                        betterThanAllOthers = false;
                        break;
                    }
                }

                if (betterThanAllOthers)
                {
                    result.Add(method);
                }
            }

            return result.ToArray();
        }

        /// <summary>Parses the specified text into a number.</summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="type">Type to parse into.</param>
        /// <returns>The parsed number.</returns>
        private static object ParseNumber(string text, Type type)
        {
            TypeCode tc = Type.GetTypeCode(WebUtil.GetNonNullableType(type));
            switch (tc)
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (SByte.TryParse(text, out sb))
                    {
                        return sb;
                    }

                    break;
                case TypeCode.Byte:
                    byte b;
                    if (Byte.TryParse(text, out b))
                    {
                        return b;
                    }

                    break;
                case TypeCode.Int16:
                    short s;
                    if (Int16.TryParse(text, out s))
                    {
                        return s;
                    }

                    break;
                case TypeCode.Int32:
                    int i;
                    if (Int32.TryParse(text, out i))
                    {
                        return i;
                    }

                    break;
                case TypeCode.Int64:
                    long l;
                    if (Int64.TryParse(text, out l))
                    {
                        return l;
                    }

                    break;
                case TypeCode.Single:
                    float f;
                    if (Single.TryParse(text, out f))
                    {
                        return f;
                    }

                    break;
                case TypeCode.Double:
                    double d;
                    if (Double.TryParse(text, out d))
                    {
                        return d;
                    }

                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (Decimal.TryParse(text, out e))
                    {
                        return e;
                    }

                    break;
            }

            return null;
        }

        /// <summary>Checks whether the source type is compatible with the value type.</summary>
        /// <param name="source">Source type.</param>
        /// <param name="target">Target type.</param>
        /// <returns>true if source can be used in place of target; false otherwise.</returns>
        private static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target)
            {
                return true;
            }

            if (!target.IsValueType)
            {
                return target.IsAssignableFrom(source);
            }

            Type sourceType = WebUtil.GetNonNullableType(source);
            Type targetType = WebUtil.GetNonNullableType(target);

            //// This rule stops the parser from considering nullable types as incompatible
            //// with non-nullable types. We have however implemented this rule because we just
            //// access underlying rules. C# requires an explicit .Value access, and EDM has no
            //// nullablity on types and (at the model level) implements null propagation.
            ////
            //// if (sourceType != source && targetType == target)
            //// {
            ////     return false;
            //// }

            TypeCode sourceCode = sourceType.IsEnum ? TypeCode.Object : Type.GetTypeCode(sourceType);
            TypeCode targetCode = targetType.IsEnum ? TypeCode.Object : Type.GetTypeCode(targetType);
            switch (sourceCode)
            {
                case TypeCode.SByte:
                    switch (targetCode)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Byte:
                    switch (targetCode)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int16:
                    switch (targetCode)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int32:
                    switch (targetCode)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int64:
                    switch (targetCode)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Single:
                    switch (targetCode)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }

                    break;
                default:
                    if (sourceType == targetType)
                    {
                        return true;
                    }

                    break;
            }

            // Anything can be converted to something that's *exactly* an object.
            if (target == typeof(object))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether one type list is a better fit than other for the 
        /// specified expressions.
        /// </summary>
        /// <param name="args">Expressions for arguments.</param>
        /// <param name="firstCandidate">First type list to check.</param>
        /// <param name="secondCandidate">Second type list to check.</param>
        /// <returns>
        /// true if <paramref name="firstCandidate"/> has better parameter matching than <paramref name="secondCandidate"/>.
        /// </returns>
        private static bool IsBetterThan(Expression[] args, IEnumerable<Type> firstCandidate, IEnumerable<Type> secondCandidate)
        {
            bool better = false;

            using (IEnumerator<Type> first = firstCandidate.GetEnumerator())
            using (IEnumerator<Type> second = secondCandidate.GetEnumerator())
            {
                for (int i = 0; i < args.Length; i++)
                {
                    first.MoveNext();
                    second.MoveNext();
                    int c = CompareConversions(args[i].Type, first.Current, second.Current);
                    if (c < 0)
                    {
                        return false;
                    }

                    if (c > 0)
                    {
                        better = true;
                    }
                }
            }

            return better;
        }

        /// <summary>
        /// Checks whether one method is a better fit than other for the 
        /// specified expressions.
        /// </summary>
        /// <param name="args">Expressions for arguments.</param>
        /// <param name="m1">First method to check.</param>
        /// <param name="m2">Second method to check.</param>
        /// <returns>
        /// true if <paramref name="m1"/> has better parameter matching than <paramref name="m2"/>.
        /// </returns>
        private static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            Debug.Assert(args != null, "args != null");
            Debug.Assert(m1 != null, "m1 != null");
            Debug.Assert(m2 != null, "m2 != null");
            return IsBetterThan(args, m1.ParameterTypes, m2.ParameterTypes);
        }

        /// <summary>Checks which conversion is better.</summary>
        /// <param name="source">Source type.</param>
        /// <param name="targetA">First candidate type to convert to.</param>
        /// <param name="targetB">Second candidate type to convert to.</param>
        /// <returns>
        /// Return 1 if s -> t1 is a better conversion than s -> t2
        /// Return -1 if s -> t2 is a better conversion than s -> t1
        /// Return 0 if neither conversion is better
        /// </returns>
        private static int CompareConversions(Type source, Type targetA, Type targetB)
        {
            // If both types are exactly the same, there is no preference.
            if (targetA == targetB)
            {
                return 0;
            }

            // Look for exact matches.
            if (source == targetA)
            {
                return 1;
            }

            if (source == targetB)
            {
                return -1;
            }

            // If one is compatible and the other is not, choose the compatible type.
            bool compatibleT1AndT2 = IsCompatibleWith(targetA, targetB);
            bool compatibleT2AndT1 = IsCompatibleWith(targetB, targetA);
            if (compatibleT1AndT2 && !compatibleT2AndT1)
            {
                return 1;
            }

            if (compatibleT2AndT1 && !compatibleT1AndT2)
            {
                return -1;
            }

            // Prefer to keep the original nullability.
            bool sourceNullable = WebUtil.IsNullableType(source);
            bool typeNullableA = WebUtil.IsNullableType(targetA);
            bool typeNullableB = WebUtil.IsNullableType(targetB);
            if (sourceNullable == typeNullableA && sourceNullable != typeNullableB)
            {
                return 1;
            }

            if (sourceNullable != typeNullableA && sourceNullable == typeNullableB)
            {
                return -1;
            }

            // Prefer signed to unsigned.
            if (IsSignedIntegralType(targetA) && IsUnsignedIntegralType(targetB))
            {
                return 1;
            }

            if (IsSignedIntegralType(targetB) && IsUnsignedIntegralType(targetA))
            {
                return -1;
            }

            // Prefer non-object to object.
            if (targetA != typeof(object) && targetB == typeof(object))
            {
                return 1;
            }

            if (targetB != typeof(object) && targetA == typeof(object))
            {
                return -1;
            }

            return 0;
        }

        /// <summary>Finds the named method in the specifid type.</summary>
        /// <param name="type">Type to look in.</param>
        /// <param name="methodName">Name of method to look for.</param>
        /// <param name="args">Arguments to method.</param>
        /// <param name="method">Best method found.</param>
        /// <returns>Number of matching methods.</returns>
        private int FindMethod(Type type, string methodName, Expression[] args, out MethodBase method)
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance;
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Method, Flags, Type.FilterName, methodName);
                int count = this.FindBestMethod(Enumerable.Cast<MethodBase>(members), args, out method);
                if (count != 0)
                {
                    return count;
                }
            }

            method = null;
            return 0;
        }

        /// <summary>Finds all applicable methods from the specified enumeration that match the arguments.</summary>
        /// <param name="methods">Enumerable object of candidate methods.</param>
        /// <param name="args">Argument expressions.</param>
        /// <returns>Methods that apply to the specified arguments.</returns>
        private MethodData[] FindApplicableMethods(IEnumerable<MethodBase> methods, Expression[] args)
        {
            List<MethodData> result = new List<MethodData>();
            foreach (MethodBase method in methods)
            {
                MethodData methodData = new MethodData(method, method.GetParameters());
                if (this.IsApplicable(methodData, args))
                {
                    result.Add(methodData);
                }
            }

            return result.ToArray();
        }

        /// <summary>Finds the best methods for the specified arguments given a candidate method enumeration.</summary>
        /// <param name="methods">Enumerable object for candidate methods.</param>
        /// <param name="args">Argument expressions to match.</param>
        /// <param name="method">Best matched method.</param>
        /// <returns>The number of "best match" methods.</returns>
        private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            MethodData[] applicable = this.FindApplicableMethods(methods, args);
            if (applicable.Length > 1)
            {
                applicable = FindBestApplicableMethods(applicable, args);
            }

            int result = applicable.Length;
            method = null;
            if (applicable.Length == 1)
            {
                // If we started off with all non-OpenType expressions and end with all-OpenType
                // expressions, we've been too aggresive - the transition from non-open-types
                // to open types should initially happen only as a result of accessing open properties.
                MethodData md = applicable[0];
                bool originalArgsDefined = true;
                bool promotedArgsOpen = true;
                for (int i = 0; i < args.Length; i++)
                {
                    originalArgsDefined = originalArgsDefined && !OpenTypeMethods.IsOpenPropertyExpression(args[i]);
                    promotedArgsOpen = promotedArgsOpen && md.Parameters[i].ParameterType == typeof(object);
                    args[i] = md.Args[i];
                }

                method = (originalArgsDefined && promotedArgsOpen) ? null : md.MethodBase;
                result = (method == null) ? 0 : 1;
            }
            else if (applicable.Length > 1)
            {
                // We may have the case for operators (which C# doesn't) in which we have a nullable operand
                // and a non-nullable operand. We choose to convert the one non-null operand to nullable in that
                // case (the binary expression will lift to null).
                if (args.Length == 2 && applicable.Length == 2 &&
                    WebUtil.GetNonNullableType(applicable[0].Parameters[0].ParameterType) ==
                    WebUtil.GetNonNullableType(applicable[1].Parameters[0].ParameterType))
                {
                    MethodData nullableMethod =
                        WebUtil.TypeAllowsNull(applicable[0].Parameters[0].ParameterType) ?
                                                                                              applicable[0] :
                                                                                                                applicable[1];
                    args[0] = nullableMethod.Args[0];
                    args[1] = nullableMethod.Args[1];
                    return this.FindBestMethod(methods, args, out method);
                }
            }

            return result;
        }

        /// <summary>Checks whether the specified method is applicable given the argument expressions.</summary>
        /// <param name="method">Method to check.</param>
        /// <param name="args">Argument expressions.</param>
        /// <returns>true if the method is applicable; false otherwise.</returns>
        private bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
            {
                return false;
            }

            Expression[] promotedArgs = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                Debug.Assert(!pi.IsOut, "!pi.IsOut");
                Expression promoted = this.PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null)
                {
                    return false;
                }

                promotedArgs[i] = promoted;
            }

            method.Args = promotedArgs;
            return true;
        }

        /// <summary>Promotes the specified expression to the given type if necessary.</summary>
        /// <param name="expr">Expression to promote.</param>
        /// <param name="type">Type to change expression to.</param>
        /// <param name="exact">Whether an exact type is required; false implies a compatible type is OK.</param>
        /// <returns>Expression with the promoted type.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        private Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            Debug.Assert(expr != null, "expr != null");
            Debug.Assert(type != null, "type != null");
            if (expr.Type == type)
            {
                return expr;
            }

            ConstantExpression ce = null;
            if (type == typeof(DateTime))
            {
                // Sometimes if the property type is DateTime? in provider, we pass the type as DateTimeOffset?
                // to ODataLib and it puts a Convert node to convert DateTimeOffset to DateTimeOffset?. Hence
                // to reach to the constant value, we need to handle the Convert node.
                if (expr.Type == typeof(DateTimeOffset) || expr.Type == typeof(DateTimeOffset?))
                {
                    if (expr.NodeType == ExpressionType.Convert)
                    {
                        ce = ((UnaryExpression)expr).Operand as ConstantExpression;
                    }
                }
            }

            if (ce == null)
            {
                ce = expr as ConstantExpression;
            }

            if (ce != null)
            {
                if (ce == ExpressionUtils.NullLiteral)
                {
                    if (WebUtil.TypeAllowsNull(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    string text;
                    if (this.literals.TryGetValue(ce, out text))
                    {
                        Type target = WebUtil.GetNonNullableType(type);
                        object value = null;
                        if (ce.Type == typeof(string) && (target == typeof(Type) || target == typeof(ResourceType)))
                        {
                            if (WebConvert.TryRemoveQuotes(ref text))
                            {
                                ResourceType resourceType = this.tryResolveResourceType(text);
                                if (resourceType != null)
                                {
                                    if (target == typeof(Type))
                                    {
                                        if (resourceType.CanReflectOnInstanceType)
                                        {
                                            value = resourceType.InstanceType;
                                        }
                                    }
                                    else
                                    {
                                        if (resourceType.CanReflectOnInstanceType == false)
                                        {
                                            value = resourceType;
                                        }
                                    }
                                }
                            }
                        }
                        else if ((type == typeof(DateTime) || type == typeof(DateTime?)) &&
                                 (expr.Type == typeof(DateTimeOffset) || expr.Type == typeof(DateTimeOffset?)))
                        {
                            // Since the URI parser in ODataLib will always convert Constants as DateTimeOffset,
                            // and WCF DS Server supports DateTime clr type, we need to do the conversion whenever required.
                            value = WebUtil.ConvertDateTimeOffsetToDateTime((DateTimeOffset)ce.Value);
                        }
                        else
                        {
                            switch (Type.GetTypeCode(ce.Type))
                            {
                                case TypeCode.Int32:
                                case TypeCode.Int64:
                                    value = ParseNumber(text, target);
                                    break;
                                case TypeCode.Double:
                                    if (target == typeof(decimal))
                                    {
                                        value = ParseNumber(text, target);
                                    }

                                    break;
                            }
                        }

                        if (value != null)
                        {
                            return Expression.Constant(value, type);
                        }
                    }
                }
            }

            if (IsCompatibleWith(expr.Type, type))
            {
                // (type != typeof(object) || expr.Type.IsValueType) part is added
                // to prevent cast to System.Object from non-value types (objects).
                if (type.IsValueType || exact && (type != typeof(object) || expr.Type.IsValueType))
                {
                    return Expression.Convert(expr, type);
                }

                return expr;
            }

            // Allow promotion from nullable to non-nullable by directly accessing underlying value.
            if (WebUtil.IsNullableType(expr.Type) && type.IsValueType)
            {
                Expression valueAccessExpression = Expression.Property(expr, "Value");
                valueAccessExpression = this.PromoteExpression(valueAccessExpression, type, exact);
                return valueAccessExpression;
            }

            return null;
        }

        /// <summary>Use this class to encapsulate method information.</summary>
        [DebuggerDisplay("MethodData {methodBase}")]
        private class MethodData
        {
            #region Private fields

            /// <summary>Described method.</summary>
            private readonly MethodBase methodBase;

            /// <summary>Parameters for method.</summary>
            private readonly ParameterInfo[] parameters;

            /// <summary>Argument expressions.</summary>
            private Expression[] args;

            #endregion Private fields

            #region Constructors

            /// <summary>Initializes a new <see cref="MethodData"/> instance.</summary>
            /// <param name="method">Described method</param>
            /// <param name="parameters">Parameters for method.</param>
            public MethodData(MethodBase method, ParameterInfo[] parameters)
            {
                this.methodBase = method;
                this.parameters = parameters;
            }

            #endregion Constructors

            #region Properties

            /// <summary>Argument expressions.</summary>
            public Expression[] Args
            {
                get { return this.args; }
                set { this.args = value; }
            }

            /// <summary>Described method.</summary>
            public MethodBase MethodBase
            {
                get { return this.methodBase; }
            }

            /// <summary>Parameters for method.</summary>
            public ParameterInfo[] Parameters
            {
                get { return this.parameters; }
            }

            /// <summary>Enumeration of parameter types.</summary>
            public IEnumerable<Type> ParameterTypes
            {
                get
                {
                    foreach (ParameterInfo parameter in this.Parameters)
                    {
                        yield return parameter.ParameterType;
                    }
                }
            }

            #endregion Properties
        }
    }
}