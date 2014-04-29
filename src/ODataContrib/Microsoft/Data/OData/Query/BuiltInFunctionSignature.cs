//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class describing a built in function signature.
    /// </summary>
    internal sealed class BuiltInFunctionSignature : FunctionSignature
    {
        /// <summary>
        /// The return type of the function.
        /// </summary>
        private IEdmTypeReference returnType;

        /// <summary>
        /// Function which takes expression for all the arguments and returns an expression which evaluates to the function call.
        /// </summary>
        private Func<Expression[], Expression> buildExpression;

        /// <summary>
        /// Helper storage for most of the built-in functions. Stores the member info for the member to call in the LINQ expression.
        /// </summary>
        private MemberInfo memberInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buildExpression">Function which takes expression for all the arguments and returns an expression which evaluates to the function call.</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        internal BuiltInFunctionSignature(Func<Expression[], Expression> buildExpression, IEdmTypeReference returnType, params IEdmTypeReference[] argumentTypes)
            : base(argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(buildExpression != null, "buildExpression != null");
            Debug.Assert(returnType != null, "returnType != null");
            Debug.Assert(returnType.IsODataPrimitiveTypeKind(), "Only primitive values are supported as built-in function return types.");

            this.returnType = returnType;
            this.buildExpression = buildExpression;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        private BuiltInFunctionSignature(IEdmTypeReference returnType, params IEdmTypeReference[] argumentTypes)
            : base(argumentTypes)
        {
            Debug.Assert(returnType != null, "returnType != null");
            Debug.Assert(returnType.IsODataPrimitiveTypeKind(), "Only primitive values are supported as built-in function return types.");

            this.returnType = returnType;
        }

        /// <summary>
        /// The return type of the function.
        /// </summary>
        internal IEdmTypeReference ReturnType
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.returnType;
            }
        }

        /// <summary>
        /// Creates a signature for a function which is executed by calling an instance method.
        /// </summary>
        /// <param name="methodName">The name of the CLR method to call.</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <param name="argumentTypes">The types of the arguments of the function.</param>
        /// <returns>Newly created function signature.</returns>
        /// <remarks>Note that arguments are always treated as non-nullable for the CLR method search, even if the ones specified are nullable.</remarks>
        internal static BuiltInFunctionSignature CreateFromInstanceMethodCall(string methodName, IEdmPrimitiveTypeReference returnType, IEdmModel model, params IEdmPrimitiveTypeReference[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            MethodInfo method = argumentTypes[0].GetInstanceType(model).GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                argumentTypes.Skip(1).Select(argumentType => TypeUtils.GetNonNullableType(argumentType.GetInstanceType(model))).ToArray(),
                null);
            Debug.Assert(method != null, "Could not find the specified method on the type of the first argument.");

            BuiltInFunctionSignature signature = new BuiltInFunctionSignature(returnType, argumentTypes);
            signature.memberInfo = method;
            signature.buildExpression = signature.BuildInstanceMethodCallExpression;
            return signature;
        }

        /// <summary>
        /// Creates a signature for a function which is executed by calling a static method.
        /// </summary>
        /// <param name="targetType">The type on which to invoke the method.</param>
        /// <param name="methodName">The name of the CLR method to call.</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <param name="argumentTypes">The types of the arguments of the function.</param>
        /// <returns>Newly created function signature.</returns>
        /// <remarks>Note that arguments are always treated as non-nullable for the CLR method search, even if the ones specified are nullable.</remarks>
        internal static BuiltInFunctionSignature CreateFromStaticMethodCall(Type targetType, string methodName, IEdmPrimitiveTypeReference returnType, IEdmModel model, params IEdmPrimitiveTypeReference[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(model != null, "model != null");

            MethodInfo method = targetType.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                argumentTypes.Select(argumentType => TypeUtils.GetNonNullableType(argumentType.GetInstanceType(model))).ToArray(),
                null);
            Debug.Assert(method != null, "Could not find the specified method on the target type.");

            BuiltInFunctionSignature signature = new BuiltInFunctionSignature(returnType, argumentTypes);
            signature.memberInfo = method;
            signature.buildExpression = signature.BuildStaticMethodCallExpression;
            return signature;
        }

        /// <summary>
        /// Creates a signature for a function which is executed by accessing a property.
        /// </summary>
        /// <param name="propertyName">The name of the CLR property to access on the first argument.</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <param name="argumentTypes">The types of the arguments of the function. (must be exactly one).</param>
        /// <returns>Newly created function signature.</returns>
        /// <remarks>Note that arguments are always treated as non-nullable for the CLR method search, even if the ones specified are nullable.</remarks>
        internal static BuiltInFunctionSignature CreateFromPropertyAccess(string propertyName, IEdmPrimitiveTypeReference returnType, IEdmModel model, params IEdmPrimitiveTypeReference[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(argumentTypes != null, "argumentTypes != null");
            Debug.Assert(argumentTypes.Length == 1, "Property access built-in function is only allowed to have one argument.");

            PropertyInfo property = TypeUtils.GetNonNullableType(argumentTypes[0].GetInstanceType(model)).GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);
            Debug.Assert(property != null, "Could not find the specified property on the target type.");

            BuiltInFunctionSignature signature = new BuiltInFunctionSignature(returnType, argumentTypes);
            signature.memberInfo = property;
            signature.buildExpression = signature.BuildPropertyAccessExpression;
            return signature;
        }

        /// <summary>
        /// Builds the expression for the function call.
        /// </summary>
        /// <param name="argumentExpressions">Argument expressions.</param>
        /// <returns>Expression which evaluates to the function call.</returns>
        internal Expression BuildExpression(Expression[] argumentExpressions)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(argumentExpressions != null, "argumentExpressions != null");
            Debug.Assert(argumentExpressions.Length == this.ArgumentTypes.Length, "argumentExpressions.Length == this.ArgumentTypes.Length");

            return this.buildExpression(argumentExpressions);
        }

        /// <summary>
        /// Builds expression in the shape of an instance method call.
        /// </summary>
        /// <param name="argumentExpressions">The argument expressions, first evaluates to the instance to execute the method on.</param>
        /// <returns>The call expression.</returns>
        private Expression BuildInstanceMethodCallExpression(Expression[] argumentExpressions)
        {
            Expression[] methodArguments = new Expression[argumentExpressions.Length - 1];
            Array.Copy(argumentExpressions, 1, methodArguments, 0, argumentExpressions.Length - 1);
            return Expression.Call(argumentExpressions[0], (MethodInfo)this.memberInfo, methodArguments);
        }

        /// <summary>
        /// Builds expression in the shape of a static method call.
        /// </summary>
        /// <param name="argumentExpressions">The argument expressions.</param>
        /// <returns>The call expression.</returns>
        private Expression BuildStaticMethodCallExpression(Expression[] argumentExpressions)
        {
            return Expression.Call(null, (MethodInfo)this.memberInfo, argumentExpressions);
        }

        /// <summary>
        /// Builds expression in the shape of a property access.
        /// </summary>
        /// <param name="argumentExpressions">The argument expressions.</param>
        /// <returns>The call expression.</returns>
        private Expression BuildPropertyAccessExpression(Expression[] argumentExpressions)
        {
            Debug.Assert(argumentExpressions.Length == 1, "Property access can only be applied to a single argument.");
            return Expression.Property(argumentExpressions[0], (PropertyInfo)this.memberInfo);
        }
    }
}
