namespace _GeneratorV6
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using AbnfParserGenerator;

    public sealed class CstNodesOptimizer
    {
        public CstNodesOptimizer()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) Optimize(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var ruleCstNodes =
                new Namespace(
                    cstNodes.RuleCstNodes.Name,
                    cstNodes.RuleCstNodes.Classes.ToList(),
                    cstNodes.RuleCstNodes.UsingDeclarations);
            var innerCstNodes =
                new Namespace(
                    cstNodes.InnerCstNodes.Name,
                    cstNodes.InnerCstNodes.Classes.ToList(),
                    cstNodes.InnerCstNodes.UsingDeclarations);

            bool modified;
            do
            {
                modified = false;
                modified |= Optimize(ruleCstNodes, innerCstNodes);
                modified |= Optimize(innerCstNodes, ruleCstNodes);
            }
            while (modified);

            return
                (
                    ruleCstNodes,
                    innerCstNodes
                );
        }

        private bool Optimize(Namespace toOptimize, Namespace other)
        {
            var modified = false;
            for (int i = 0; i < toOptimize.Classes.Count; ++i)
            {
                var cstNode = toOptimize.Classes[i];
                if (!IsSingleton(cstNode) && cstNode.Properties.All(property => IsSingleton(property.Type, toOptimize, other)))
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
                                    $"{toOptimize.Name}.{cstNode.Name}",
                                    "Instance",
                                    true,
                                    false,
                                    $"new {toOptimize.Name}.{cstNode.Name}"))
                        );

                    toOptimize.Classes[i] = optimized;

                    modified = true;
                }
            }

            return modified;
        }

        private bool IsSingleton(string fullyQualifiedType, Namespace someNodes, Namespace moreNodes)
        {
            if (fullyQualifiedType.StartsWith(someNodes.Name))
            {
                if (IsSingleton(fullyQualifiedType, someNodes))
                {
                    return true;
                }
            }

            if (fullyQualifiedType.StartsWith(moreNodes.Name))
            {
                if (IsSingleton(fullyQualifiedType, moreNodes))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSingleton(string fullyQualifiedType, Namespace nodes)
        {
            var typeName = fullyQualifiedType.Substring(nodes.Name.Length + 1);

            foreach (var @class in nodes.Classes)
            {
                if (string.Equals(typeName, @class.Name))
                {
                    return IsSingleton(@class);
                }
            }

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
