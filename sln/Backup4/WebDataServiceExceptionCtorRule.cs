//---------------------------------------------------------------------
// <copyright file="WebDataServiceExceptionCtorRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// This rule checks that HashSet`1 constructor take an explicit
    /// comparer object.
    /// </summary>
    public sealed class DataServiceExceptionCtorRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="HashShetCtorRule"/> instance.</summary>
        public DataServiceExceptionCtorRule()
            : base("DataServiceExceptionCtorRule")
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
                    x.DeclaringType.FullName.StartsWith("Microsoft.OData.Service.DataServiceException") &&
                    !HasParameterType(x, "System.Int32"),
                methodUnderCheck.FullName);

            base.VisitConstruct(cons);
        }
    }
}
