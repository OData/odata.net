//---------------------------------------------------------------------
// <copyright file="DynamicProxyMethodGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection; 
    using System.Reflection.Emit;
    using System.Security; 
    using System.Security.Permissions;
 

    /// <summary>
    /// Generates proxy methods for external callers to call internal methods
    /// All lambda_methods are considered external. When these methods need
    /// to access internal resources, a proxy must be used. Otherwise the call
    /// will fail for partial trust scenario.
    /// </summary>
    internal class DynamicProxyMethodGenerator
    {
#if !ASSEMBLY_ATTRIBUTE_ON_NETSTANDARD_20 
        
        /// <summary>
        /// Dynamically generated proxy methods for external callers (lambda_method are external callers)
        /// </summary>
        private static Dictionary<MethodBase, MethodInfo> dynamicProxyMethods = new Dictionary<MethodBase, MethodInfo>(EqualityComparer<MethodBase>.Default);
#endif

        /// <summary>
        /// Builds an expression to best call the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The original method or constructor</param>
        /// <param name="arguments">The arguments with which to call the method.</param>
        /// <returns>An expression to call the argument method or constructor</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "'this' parameter is required when compiling for the desktop.")]
        internal Expression GetCallWrapper(MethodBase method, params Expression[] arguments)
        {
#if !ASSEMBLY_ATTRIBUTE_ON_NETSTANDARD_20
            if (!this.ThisAssemblyCanCreateHostedDynamicMethodsWithSkipVisibility())
            {
                return WrapOriginalMethodWithExpression(method, arguments);
            }

            return GetDynamicMethodCallWrapper(method, arguments);
#else
            return WrapOriginalMethodWithExpression(method, arguments);
#endif
        }

#if !ASSEMBLY_ATTRIBUTE_ON_NETSTANDARD_20 
        /// <summary>
        /// Determines whether this assembly has enough permissions to create
        /// <see cref="DynamicMethod"/>s that can be hosted within this assembly
        /// and also skip visibility checks (access modifier checks) in order to call potentially
        /// internal user types (e.g., anonymous types).
        /// </summary>
        /// <returns>True if this assembly has enough permissions. Otherwise, false.</returns>
        protected virtual bool ThisAssemblyCanCreateHostedDynamicMethodsWithSkipVisibility()
        {
            return typeof(DynamicProxyMethodGenerator).Assembly.IsFullyTrusted;
        }

        /// <summary>
        /// Build a externally visible <see cref="DynamicMethod"/> to call the argument method.
        /// </summary>
        /// <param name="method">The original method or constructor</param>
        /// <param name="arguments">The arguments with which to call the method.</param>
        /// <returns>An expression to call the argument method or constructor</returns>
        [SecuritySafeCritical]
        private static Expression GetDynamicMethodCallWrapper(MethodBase method, params Expression[] arguments)
        {
            if (method.DeclaringType == null || method.DeclaringType.Assembly != typeof(DynamicProxyMethodGenerator).Assembly)
            {
                // Security filtering: we should only accept methods that are bound to our own assembly
                return WrapOriginalMethodWithExpression(method, arguments);
            }

            string internalMethodName = "_dynamic_" + method.ReflectedType.Name + "_" + method.Name;

            MethodInfo mi = null;

            lock (dynamicProxyMethods)
            {
                dynamicProxyMethods.TryGetValue(method, out mi);
            }

            if (mi != null)
            {
                return Expression.Call(mi, arguments);
            }
            else
            {
                Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                MethodInfo methodInfo = method as MethodInfo;

                // Dynamic Method Signature return type:
                // if method is a constructor, the return type is the reflected type
                // otherwise, the return type is the method's return type
                DynamicMethod dm = CreateDynamicMethod(internalMethodName, methodInfo == null ? method.ReflectedType : methodInfo.ReturnType, parameterTypes);

                ILGenerator g = dm.GetILGenerator();
                for (int i = 0; i < parameterTypes.Length; ++i)
                {
                    switch (i)
                    {
                        case 0:
                            g.Emit(OpCodes.Ldarg_0);
                            break;
                        case 1:
                            g.Emit(OpCodes.Ldarg_1);
                            break;
                        case 2:
                            g.Emit(OpCodes.Ldarg_2);
                            break;
                        case 3:
                            g.Emit(OpCodes.Ldarg_3);
                            break;
                        default:
                            g.Emit(OpCodes.Ldarg, i);
                            break;
                    }
                }

                if (methodInfo == null)
                {
                    // 'method' argument must be either ConstructorInfo or MethodInfo.
                    // since it is not methodInfo emit the constructor call
                    g.Emit(OpCodes.Newobj, (ConstructorInfo)method);
                }
                else
                {
                    // method call
                    g.EmitCall(OpCodes.Call, methodInfo, null);
                }

                g.Emit(OpCodes.Ret);

                lock (dynamicProxyMethods)
                {
                    // DEVNOTE(pqian):
                    // we may waste some cycles creating the dynamic method
                    // on multi-thread scenario, but it's better than locking this method entirely.
                    if (!dynamicProxyMethods.ContainsKey(method))
                    {
                        dynamicProxyMethods.Add(method, dm);
                    }
                }

                return Expression.Call(dm, arguments);
            }
        }

        /// <summary>
        /// Create a new dynamic method
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="returnType">the return type</param>
        /// <param name="parameterTypes">the parameter types</param>
        /// <returns>a new instance of dynamic method</returns>
        [SecurityCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes)
        {
            return new DynamicMethod(name, returnType, parameterTypes, typeof(DynamicProxyMethodGenerator).Module, skipVisibility: true);
        }
#endif

        /// <summary>
        /// Wraps the specified <see cref="MethodBase"/> in an expression that invokes it.
        /// </summary>
        /// <param name="method">The method to wrap in an expression.</param>
        /// <param name="arguments">The arguments with which to invoke the <paramref name="method"/>.</param>
        /// <returns>An expression which invokes the <paramref name="method"/> with the specified <paramref name="arguments"/>.</returns>
        private static Expression WrapOriginalMethodWithExpression(MethodBase method, Expression[] arguments)
        {
            var methodInfo = method as MethodInfo;
            if (methodInfo != null)
            {
                return Expression.Call(methodInfo, arguments);
            }

            return Expression.New((ConstructorInfo)method, arguments);
        }
    }
}