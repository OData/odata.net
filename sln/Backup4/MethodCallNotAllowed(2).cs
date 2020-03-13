//---------------------------------------------------------------------
// <copyright file="MethodCallNotAllowed.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    #region Namespaces

    using System;
    using System.Linq;
    using Microsoft.FxCop.Sdk;

    #endregion Namespaces

    /// <summary>This rule checks all the rules declared statically.</summary>
    public class MethodCallNotAllowed : BaseDataWebRule
    {
        private static readonly MethodCallToDisallow[] Items = new MethodCallToDisallow[]
        {
            new MethodCallToDisallow
            {
                Description = "Static Create should be used to ensure no instances with null are created.",
                NamespacesToCheck = "*",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Client.DataServiceQueryContinuation",
                NameOfMethodToExclude = ".ctor"
            },
            new MethodCallToDisallow
            {
                Description = "Static Create should be used to ensure no instances with null are created.",
                NamespacesToCheck = "*",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Client.DataServiceQueryContinuation`1",
                NameOfMethodToExclude = ".ctor"
            },
            new MethodCallToDisallow
            {
                Description = "ODataLib should not call IEdmModel.FindType() directly because the type resolver is required for the WcfDataServicesClientBehavior.",
                NamespacesToCheck = "Microsoft.OData.Core",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Edm.ExtensionMethods",
                NameOfMethodToExclude = "FindType"
            },
            new MethodCallToDisallow
            {
                Description = "ODataLib should not call IEdmModel.FindDeclaredType() directly because the type resolver is required for the WcfDataServicesClientBehavior.",
                NamespacesToCheck = "Microsoft.OData.Core",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Edm.IEdmModel",
                NameOfMethodToExclude = "FindDeclaredType"
            },
            new MethodCallToDisallow
            {
                Description = "ODataLib should not call IEdmEntitySet.get_ElementType() directly because the type resolver is required for the WcfDataServicesClientBehavior.",
                NamespacesToCheck = "Microsoft.OData.Core",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Edm.IEdmEntitySet",
                NameOfMethodToExclude = "get_ElementType"
            },
            new MethodCallToDisallow
            {
                Description = "ODataLib should not call IEdmOperationParameter.get_Type() directly because the type resolver is required for the WcfDataServicesClientBehavior.",
                NamespacesToCheck = "Microsoft.OData.Core",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Edm.IEdmOperationParameter",
                NameOfMethodToExclude = "get_Type"
            },
            new MethodCallToDisallow
            {
                Description = "ODataLib should not call IEdmFunctionBase.get_ReturnType() directly because the type resolver is required for the WcfDataServicesClientBehavior.",
                NamespacesToCheck = "Microsoft.OData.Core",
                DeclaringTypeOfMethodToExclude = "Microsoft.OData.Edm.IEdmFunctionBase",
                NameOfMethodToExclude = "get_ReturnType"
            },
        };

        private Method methodUnderCheck;

        /// <summary>Initializes a new <see cref="MethodCallNotAllowed"/> instance.</summary>
        public MethodCallNotAllowed()
            : base("MethodCallNotAllowed")
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

            this.methodUnderCheck = method;
            Visit(method);
            return Problems.Count > 0 ? Problems : null;
        }

        /// <summary>Visits a constructor invocation.</summary>
        /// <param name="cons">Construction.</param>
        /// <returns>Resulting expression to visit.</returns>
        public override void VisitConstruct(Construct cons)
        {
            if (cons != null)
            {
                MemberBinding constructorBinding = cons.Constructor as MemberBinding;
                if (constructorBinding != null)
                {
                    Method method = constructorBinding.BoundMember as Method; // More likely: InstanceInitializer;
                    this.CheckMethodUsage(method);
                }
            }

            base.VisitConstruct(cons);
        }

        private void CheckMethodUsage(Method method)
        {
            foreach (MethodCallToDisallow item in Items)
            {
                if (item.MatchesMethodToDisallow(this.methodUnderCheck, method))
                {
                    Problem problem = new Problem(GetResolution(
                        item.DeclaringTypeOfMethodToExclude,
                        item.NameOfMethodToExclude,
                        this.methodUnderCheck.DeclaringType.FullName,
                        this.methodUnderCheck.Name.Name, item.Description));
                    this.Problems.Add(problem);
                }
            }
        }

        /// <summary>Visits a method call.</summary>
        /// <param name="methodCall">Call.</param>
        /// <returns>Resulting expression to visit.</returns>
        public override void VisitMethodCall(MethodCall methodCall)
        {
            if (methodCall != null)
            {
                MemberBinding callBinding = methodCall.Callee as MemberBinding;
                if (callBinding != null)
                {
                    Method method = callBinding.BoundMember as Method;
                    this.CheckMethodUsage(method);
                }
            }

            base.VisitMethodCall(methodCall);
        }

        private class MethodCallToDisallow
        {
            /// <summary>
            /// Namespaces to perform the check. null indicates to check all namespaces.
            /// </summary>
            private string[] namespacesToCheck;

            /// <summary>
            /// Description of the rule.
            /// </summary>
            internal string Description { get; set; }

            /// <summary>
            /// Semi-colon delimited namespaces where this rule applies. "*" indicates to check all namespaces.
            /// </summary>
            internal string NamespacesToCheck
            {
                set
                {
                    if (!string.IsNullOrEmpty(value) && value != "*")
                    {
                        this.namespacesToCheck = value.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }

            /// <summary>
            /// Declaring type of the method to exclude. If the value is "*", exclude the matching method name declared under all types.
            /// </summary>
            internal string DeclaringTypeOfMethodToExclude { get; set; }

            /// <summary>
            /// Method name to exclude. If the value is "*", exclude all methods declared under the type DeclaringTypeOfMethodToExclude.
            /// </summary>
            internal string NameOfMethodToExclude { get; set; }

            internal bool MatchesMethodToDisallow(Method callerMethod, Method calleeMethod)
            {
                if (this.ShouldCheckMethodOnType(callerMethod.DeclaringType.FullName))
                {
                    return NamesMatchMethod(this.DeclaringTypeOfMethodToExclude, this.NameOfMethodToExclude, calleeMethod);
                }

                return false;
            }

            private bool ShouldCheckMethodOnType(string callerMethodDeclaringTypeFullName)
            {
                if (this.namespacesToCheck == null)
                {
                    return true;
                }

                return this.namespacesToCheck.Any(namespaceToCheck => callerMethodDeclaringTypeFullName.StartsWith(namespaceToCheck + "."));
            }

            private static bool NamesMatchMethod(string typeName, string methodName, Method method)
            {
                if (method == null)
                {
                    return false;
                }

                if (typeName != "*" && typeName != method.DeclaringType.FullName)
                {
                    return false;
                }

                return methodName == "*" || methodName == method.Name.Name;
            }
        }
    }
}
