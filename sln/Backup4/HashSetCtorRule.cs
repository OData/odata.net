//---------------------------------------------------------------------
// <copyright file="HashSetCtorRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// This rule checks that HashSet`1 constructor take an explicit
    /// comparer object.
    /// </summary>
    public class HashSetCtorRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="HashShetCtorRule"/> instance.</summary>
        public HashSetCtorRule(): base("HashSetCtorRule")
        {
        }

        /// <summary>Visibility of targets to which this rule commonly applies.</summary>
        public override TargetVisibilities TargetVisibility
        {
            get
            {
                return TargetVisibilities.All;
            }
        }

        /// <summary>Checks type members.</summary>
        /// <param name="member">Member being checked.</param>
        /// <returns>A collection of problems found in <paramref name="member"/> or null.</returns>
        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (method == null)
            {
                return null;
            }

            methodUnderCheck = method;
            Visit(method);
            return Problems.Count > 0 ? Problems : null;
        }

        /// <summary>Visits a constructor invocation.</summary>
        /// <param name="cons">Construction.</param>
        /// <returns>Resulting expression to visit.</returns>
        public override void VisitConstruct(Construct cons)
        {
            CheckMethod(
                ConstructAsInstanceInitializer(cons),
                x =>
                    x.Template != null &&
                    x.Template.FullName == "System.Collections.Generic.HashSet`1.#ctor" &&
                    !HasParameterType(x, "System.Collections.Generic.IEqualityComparer`1"),
                methodUnderCheck.FullName);

            CheckMethod(
                ConstructAsInstanceInitializer(cons),
                x =>
                    x.Template != null &&
                    x.Template.FullName == "System.Collections.Generic.Dictionary`2.#ctor" &&
                    !HasParameterType(x, "System.Collections.Generic.IEqualityComparer`1"),
                methodUnderCheck.FullName);

            base.VisitConstruct(cons);
        }
    }
}
