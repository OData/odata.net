//---------------------------------------------------------------------
// <copyright file="BaseDataWebRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using System.Diagnostics;
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// Inherit from this class to implement a rule in the DataWebRules
    /// assembly.
    /// </summary>
    public abstract class BaseDataWebRule: BaseIntrospectionRule
    {
        /// <summary>Initializes a new <see cref="BaseDataWebRule"/>.</summary>
        /// <param name="name">Name of the rule, typically the class name.</param>
        public BaseDataWebRule(string name)
            : base(name, "DataWebRules.Rules", typeof(BaseDataWebRule).Assembly)
        {
        }

        internal InstanceInitializer ConstructAsInstanceInitializer(Construct cons)
        {
            MemberBinding constructorBinding = cons.Constructor as MemberBinding;
            return (constructorBinding != null) ?
                constructorBinding.BoundMember as InstanceInitializer : null;
        }

        internal void CheckMethod(Method method, Predicate<Method> checkHasProblem, params string[] args)
        {
            if (method != null && checkHasProblem(method))
            {
                this.Problems.Add(new Problem(GetResolution(args)));
            }
        }

        internal bool HasParameterType(Method method, string typeNamePrefix)
        {
            Debug.Assert(method != null, "method != null");
            Debug.Assert(typeNamePrefix != null, "typeNamePrefix != null");

            foreach (Parameter parameter in method.Parameters)
            {
                if (parameter.Type.FullName.StartsWith(typeNamePrefix))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
