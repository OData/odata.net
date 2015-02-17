//---------------------------------------------------------------------
// <copyright file="StaticMethodReplacementStrategyBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// A basic method replacement strategy for replacing a set of static methods defined on a specific type
    /// </summary>
    public abstract class StaticMethodReplacementStrategyBase : IMethodReplacementStrategy
    {
        /// <summary>
        /// Gets the type which contains the static methods to replace
        /// </summary>
        protected internal abstract Type MethodDeclarationType { get; }
        
        /// <summary>
        /// Tries to get a replacement expression for the given method with the given parameters
        /// </summary>
        /// <param name="toReplace">The method to replace</param>
        /// <param name="parameters">The parameters to the method</param>
        /// <param name="replaced">The replaced expression</param>
        /// <returns>True if a replacement was made, false otherwise</returns>
        public virtual bool TryGetReplacement(MethodInfo toReplace, IEnumerable<Expression> parameters, out Expression replaced)
        {
            ExceptionUtilities.CheckArgumentNotNull(toReplace, "toReplace");
            ExceptionUtilities.CheckArgumentNotNull(parameters, "parameters");

            if (!this.CanReplace(toReplace) || !this.ShouldReplaceWithInstanceMethod(toReplace))
            {
                replaced = null;
                return false;
            }

            MethodInfo implementation = this.GetReplacementMethodInfo(toReplace);
            Expression constant = Expression.Constant(this, this.GetType());
            replaced = Expression.Call(constant, implementation, parameters);
            
            return true;
        }

        internal static bool CompareParameters(Type[] toReplaceParameters, Type[] methodInfoParameters)
        {
            if (toReplaceParameters.Length != methodInfoParameters.Length)
            {
                return false;
            }

            for (int i = 0; i < toReplaceParameters.Length; i++)
            {
                var p1 = toReplaceParameters[i];
                var p2 = methodInfoParameters[i];

                if (p1.IsGenericType != p2.IsGenericType)
                {
                    return false;
                }

                if (p1.IsGenericType)
                {
                    if (p1.GetGenericTypeDefinition() != p2.GetGenericTypeDefinition())
                    {
                        return false;
                    }

                    return CompareParameters(p1.GetGenericArguments(), p2.GetGenericArguments());
                }
                else if (p1 != p2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the replacement method info for the given method
        /// </summary>
        /// <param name="toReplace">The method info to get the replacement for</param>
        /// <returns>The replacement method info</returns>
        internal MethodInfo GetReplacementMethodInfo(MethodInfo toReplace)
        {
            ExceptionUtilities.CheckArgumentNotNull(toReplace, "toReplace");
            ExceptionUtilities.Assert(toReplace.DeclaringType == this.MethodDeclarationType, "Given method was not defined on {0}: {1}", this.MethodDeclarationType, toReplace);

            MethodInfo replacement = this.GetType().GetMethods().SingleOrDefault(m => m.Name == toReplace.Name
                && CompareParameters(toReplace.GetParameters().Select(p => p.ParameterType).ToArray(), m.GetParameters().Select(p => p.ParameterType).ToArray()));
            ExceptionUtilities.CheckObjectNotNull(replacement, "Could not find replacement method for method: {0}", toReplace);

            if (replacement.IsGenericMethod)
            {
                replacement = replacement.MakeGenericMethod(toReplace.GetGenericArguments());
            }

            return replacement;
        }

        /// <summary>
        /// Returns whether or not the method needs to be replaced with an instance method.
        /// Note: this can return false either due to the static method already having an implementation,
        /// or because the method should not be replaced with another instance method, but rather by a more complex expression
        /// </summary>
        /// <param name="method">The method to consider replacing</param>
        /// <returns>True if it must be replaced, false otherwise</returns>
        protected internal abstract bool ShouldReplaceWithInstanceMethod(MethodInfo method);

        /// <summary>
        /// Returns whether or not the given method is declared on this strategy's method declaration type
        /// </summary>
        /// <param name="toReplace">The method to replace</param>
        /// <returns>True if it can be replaced, false otherwise</returns>
        protected virtual bool CanReplace(MethodInfo toReplace)
        {
            ExceptionUtilities.CheckArgumentNotNull(toReplace, "toReplace");
            return toReplace.DeclaringType == this.MethodDeclarationType;
        }
    }
}