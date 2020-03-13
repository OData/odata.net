//---------------------------------------------------------------------
// <copyright file="ThreadGetSetDataRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// This rule checks that Thread.GetData and Thread.SetData aren't used.
    /// </summary>
    public class ThreadGetSetDataRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="ThreadGetSetDataRule"/> instance.</summary>
        public ThreadGetSetDataRule() : base("ThreadGetSetDataRule")
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
        public override void VisitMethodCall(MethodCall methodCall)
        {
            if (methodCall != null)
            {
                MemberBinding callBinding = methodCall.Callee as MemberBinding;
                if (callBinding != null)
                {
                    Method method = callBinding.BoundMember as Method;
                    if (method != null &&
                        method.DeclaringType.Namespace.Name == "System.Threading" &&
                        method.DeclaringType.Name.Name == "Thread" &&
                        (method.Name.Name == "GetData" || method.Name.Name == "SetData"))
                    {
                        this.Problems.Add(new Problem(GetResolution(methodUnderCheck.FullName)));
                    }
                }
            }

            base.VisitMethodCall(methodCall);
        }
    }
}
