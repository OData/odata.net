namespace _GeneratorV6
{
    using System.Collections.Generic;
    using System.Linq;

    using AbnfParserGenerator;

    public sealed class CstNodesOptimizer
    {
        public CstNodesOptimizer()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) Optimize(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var ruleCstNodes = cstNodes.RuleCstNodes.Classes.ToList();
            var innerCstNodes = cstNodes.InnerCstNodes.Classes.ToList();

            var modified = false;
            do
            {
                modified |= Optimize(ruleCstNodes, cstNodes.RuleCstNodes.Name);
                modified |= Optimize(innerCstNodes, cstNodes.InnerCstNodes.Name);
            }
            while (modified);

            return
                (
                    new Namespace(
                        cstNodes.RuleCstNodes.Name,
                        ruleCstNodes,
                        cstNodes.RuleCstNodes.UsingDeclarations),
                    new Namespace(
                        cstNodes.InnerCstNodes.Name,
                        innerCstNodes,
                        cstNodes.InnerCstNodes.UsingDeclarations)
                );
        }

        private bool Optimize(List<Class> cstNodes, string @namespace)
        {
            var modified = false;
            for (int i = 0; i < cstNodes.Count; ++i)
            {
                var cstNode = cstNodes[i];
                if (!IsSingleton(cstNode) && cstNode.Properties.All(property => IsSingleton(property.Type)))
                {
                    var optimized = new Class(
                        cstNode.AccessModifier,
                        cstNode.ClassModifier,
                        cstNode.Name,
                        cstNode.GenericTypeParameters,
                        cstNode.BaseType,
                        new[]
                        {
                            new ConstructorDefinition(
                                AccessModifier.Private,
                                Enumerable.Empty<MethodParameter>(),
                                cstNode
                                    .Properties
                                    .Select(
                                        property => $"this.{property.Name} = {property.Type}.Instance;")),
                        },
                        cstNode.Methods,
                        cstNode.NestedClasses,
                        cstNode
                            .Properties
                            .Append(
                                new PropertyDefinition(
                                    AccessModifier.Public,
                                    true,
                                    $"{@namespace}.{cstNode.Name}",
                                    "Instance",
                                    true,
                                    false,
                                    $"new {@namespace}.{cstNode.Name}"))
                        );

                    cstNodes[i] = optimized;

                    modified = true;
                }
            }

            return modified;
        }

        private bool IsSingleton(string fullyQualifiedType)
        {
            return false;
        }

        private bool IsSingleton(Class @class)
        {
            return false;
        }
    }
}
