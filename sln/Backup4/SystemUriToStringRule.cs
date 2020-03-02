//---------------------------------------------------------------------
// <copyright file="SystemUriToStringRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using System;
    using Microsoft.FxCop.Sdk;

    public class SystemUriToStringRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="HashShetCtorRule"/> instance.</summary>
        public SystemUriToStringRule()
            : base("SystemUriToStringRule")
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

        public override void VisitMethodCall(MethodCall call)
        {
            MemberBinding callBinding = call.Callee as MemberBinding;
            if (callBinding != null)
            {
                Method method = callBinding.BoundMember as Method;

                if (callBinding.TargetObject != null && callBinding.TargetObject.Type != null && callBinding.TargetObject.Type.FullName == "System.Uri")
                {
                    if (method.FullName == "System.Object.ToString" ||
                        method.FullName == "System.Uri.get_OriginalString")
                    {
                        Problem problem = new Problem(GetResolution());
                        this.Problems.Add(problem);
                    }
                }
            }

            base.VisitMethodCall(call);
        }

    }
}
