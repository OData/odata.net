//---------------------------------------------------------------------
// <copyright file="CodeTypeReferenceRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;
    public class CodeTypeReferenceRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="CodeTypeReferenceRule"/> instance.</summary>
        public CodeTypeReferenceRule()
            : base("CodeTypeReferenceRule")
        {
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
            InstanceInitializer init = ConstructAsInstanceInitializer(cons);

            if (init != null)
            {
                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeTypeReference" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReferenceOptions"),
                    methodUnderCheck.FullName);

                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeTypeReferenceExpression" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);

                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeParameterDeclaration" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);

                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeMemberField" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);

                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeArrayCreateExpression" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);
                
                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeCastExpression" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);

                CheckMethod(init,
                    x => x.DeclaringType.FullName == "System.CodeDom.CodeObjectCreateExpression" &&
                        !HasParameterType(x, "System.CodeDom.CodeTypeReference"),
                        methodUnderCheck.FullName);
            }

            base.VisitConstruct(cons);
        }
    }
}
