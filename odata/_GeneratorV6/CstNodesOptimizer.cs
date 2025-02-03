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
                modified |= Optimize(ruleCstNodes, cstNodes.RuleCstNodes.Name, cstNodes);
                modified |= Optimize(innerCstNodes, cstNodes.InnerCstNodes.Name, cstNodes);
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

        private bool Optimize(List<Class> toOptimize, string @namespace, (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var modified = false;
            for (int i = 0; i < toOptimize.Count; ++i)
            {
                var cstNode = toOptimize[i];
                if (!IsSingleton(cstNode) && cstNode.Properties.All(property => IsSingleton(property.Type, cstNodes)))
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

                    toOptimize[i] = optimized;

                    modified = true;
                }
            }

            return modified;
        }

        private bool IsSingleton(string fullyQualifiedType, (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            return false;
        }

        private bool IsSingleton(Class @class)
        {
            if (
                @class.Constructors.Skip(1).Any() ||
                @class.Constructors.Where(constructor => constructor.AccessModifier != AccessModifier.Private && constructor.Parameters.Any()).Any())
            {
                return false;
            }

            return true;
        }
    }
}
