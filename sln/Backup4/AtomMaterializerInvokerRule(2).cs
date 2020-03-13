//---------------------------------------------------------------------
// <copyright file="AtomMaterializerInvokerRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataWebRules
{
    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// This rule checks that all methods used in AtomMaterializerInvoker
    /// are available and that there aren't unused methods.
    /// </summary>
    public class AtomMaterializerInvokerRule : BaseDataWebRule
    {
        private System.Collections.ArrayList methodsUsedByCompiler;

        /// <summary>Initializes a new <see cref="AtomMaterializerInvokerRule"/> instance.</summary>
        public AtomMaterializerInvokerRule()
            : base("AtomMaterializerInvokerRule")
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

        /// <summary>Checks the client module for consistency.</summary>
        /// <param name="module">Module to check.</param>
        /// <returns>Problems in the module.</returns>
        public override ProblemCollection Check(ModuleNode module)
        {
            base.Check(module);
            if (!module.Name.EndsWith("Data.Services.Client", System.StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            TypeNode compiler = null;
            TypeNode materializer = null;
            TypeNode invoker = null;
            foreach (TypeNode node in module.Types)
            {
                if (node.Name.Name.EndsWith("ODataEntityMaterializerInvoker"))
                {
                    invoker = node;
                }
                else if (node.Name.Name.EndsWith("ODataEntityMaterializer"))
                {
                    materializer = node;
                }
                else if (node.Name.Name.EndsWith("ProjectionPlanCompiler"))
                {
                    compiler = node;
                }

                if (invoker != null && materializer != null && compiler != null)
                {
                    break;
                }
            }

            if (invoker == null || materializer == null || compiler == null)
            {
                Resolution resolution = new Resolution("Unable to find AtomMaterializerInvoker/AtomMaterializer/ProjectionPlanCompiler - check assembly or update rule.");
                Problems.Add(new Problem(resolution));
                return Problems;
            }

            string[] methodNames = GetMethodNamesFromInvoker(invoker);
            string[] usedMethods = GetMethodsUsedByCompiler(compiler);

            // Look for methods that aren't used by compiler.
            for (int i = 0; i < methodNames.Length; i++)
            {
                string methodName = methodNames[i];
                if (methodName == "DirectMaterializePlan" || methodName == "ShallowMaterializePlan")
                {
                    // These are referenced directly so a lambda can be compiled and passed around,
                    // but are not part of projection plan building.
                    continue;
                }

                bool found = System.Array.BinarySearch(usedMethods, methodName) >= 0;
                if (!found)
                {
                    Problems.Add(new Problem(GetResolution(methodName)));
                }
            }

            // TODO: look for methods that aren't implemented by invoker.
            // Harder to do because we gather collection information by just
            // picking up string literals, which gets asserts and other
            // information. A more careful analysis requred code flow analysis,
            // for example the wrappers for DataServiceCollection creation determine
            // the method name through some branching statements.
            //
            // Heuristic: things with no spaces or some other non-identifier characters are method names.
            for (int i = 0; i < usedMethods.Length; i++)
            {
                string usedMethod = usedMethods[i];
                if (usedMethod.Contains(" ") || usedMethod.Contains("|") || usedMethod.Contains("("))
                {
                    continue;
                }

                // There are method name checks in the projection plan compiler used
                // to recognize certain patterns. Skip those.
                if (usedMethod == "Select" || usedMethod == "Create" ||
                    usedMethod == "CreateTracked" || usedMethod == "ToList" ||
                    usedMethod == "ReferenceEquals")
                {
                    continue;
                }

                // There are literals used to create parameter names.
                if (usedMethod == "entry" || usedMethod == "subentry" || usedMethod == "type")
                {
                    continue;
                }

                bool found = System.Array.BinarySearch(methodNames, usedMethod) >= 0;
                if (!found)
                {
                    Resolution resolution = new Resolution("Implement " + usedMethod + " on AtomMaterializerInvoker");
                    Problems.Add(new Problem(resolution));
                }
            }

            return Problems.Count > 0 ? Problems : null;
        }

        private string[] GetMethodsUsedByCompiler(TypeNode compiler)
        {
            System.Collections.ArrayList result = new System.Collections.ArrayList();
            this.methodsUsedByCompiler = result;
            foreach (Member member in compiler.Members)
            {
                if (member.NodeType == NodeType.Method)
                {
                    this.Visit(member);
                }
            }

            this.methodsUsedByCompiler = null;
            result.Sort();
            return (string[])result.ToArray(typeof(string));
        }

        private static string[] GetMethodNamesFromInvoker(TypeNode invoker)
        {
            System.Collections.ArrayList result = new System.Collections.ArrayList(invoker.Members.Count);
            foreach (Member member in invoker.Members)
            {
                if (member.NodeType == NodeType.Method)
                {
                    result.Add(member.Name.Name);
                }
            }

            result.Sort();
            return (string[])result.ToArray(typeof(string));
        }

        public override void VisitLiteral(Literal literal)
        {
            if (literal.Type != null && literal.Type.FullName == "System.String")
            {
                this.methodsUsedByCompiler.Add(literal.Value as string);
            }
            
            base.VisitLiteral(literal);
        }
    }
}
