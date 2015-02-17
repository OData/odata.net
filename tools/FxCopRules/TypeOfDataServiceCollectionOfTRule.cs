//---------------------------------------------------------------------
// <copyright file="TypeOfDataServiceCollectionOfTRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;
    using System.Diagnostics;

    class TypeOfDataServiceCollectionOfTRule : BaseDataWebRule
    {
        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="TypeOfDataServiceCollectionOfTRule"/> instance.</summary>
        public TypeOfDataServiceCollectionOfTRule()
            : base("TypeOfDataServiceCollectionOfTRule")
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

        /// <summary>Visits a method call.</summary>
        /// <param name="methodCall">Method call.</param>
        /// <returns>Resulting expression to visit.</returns>
        public override void VisitMethodCall(MethodCall methodCall)
        {
            if (methodCall != null)
            {
                MemberBinding callBinding = methodCall.Callee as MemberBinding;
                if (callBinding != null)
                {
                    /* 
                        typeof(DataServiceCollection<>) is compiled to IL as: 
                            ldtoken [Microsoft.OData.Client]Microsoft.OData.Client.DataServiceCollection`1
                            call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
                     */
                    Method method = callBinding.BoundMember as Method;
                    if (method.Name.Name == "GetTypeFromHandle" &&
                        method.DeclaringType.FullName == "System.Type")
                    {
                        Debug.Assert(methodCall.Operands.Count == 1);
                        Debug.Assert(methodCall.Operands[0].NodeType == NodeType.Ldtoken);

                        ClassNode classType = (((Literal)(((UnaryExpression)methodCall.Operands[0]).Operand)).Value) as ClassNode;
                        if (classType != null && classType.FullName == "Microsoft.OData.Client.DataServiceCollection`1")
                        {
                            if (methodUnderCheck.DeclaringType.FullName != "System.Data.Serices.Client.WebUtil" &&
                                methodUnderCheck.Name.Name != "GetDataServiceCollectionOfTType")
                            {
                                this.Problems.Add(new Problem(GetResolution(methodUnderCheck.FullName)));
                            }
                        }
                    }
                }
            }

            base.VisitMethodCall(methodCall);
        }


    }
}
