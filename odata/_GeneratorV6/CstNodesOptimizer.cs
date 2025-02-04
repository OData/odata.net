namespace _GeneratorV6
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
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
                var optimized = Optimize(cstNode, toOptimize.Name + ".", toOptimize, other);
                if (optimized != null)
                {
                    toOptimize.Classes[i] = optimized;
                    modified = true;
                }
            }

            return modified;
        }

        private Class? Optimize(Class @class, string namespacePrefix, Namespace first, Namespace second)
        {
            var modified = false;

            var optimizedNested = new List<Class>();
            foreach (var nested in @class.NestedClasses)
            {
                if (nested.Name == "Visitor")
                {
                    optimizedNested.Add(nested);
                    continue;
                }

                var optimized = Optimize(nested, string.Empty, first, second);
                if (optimized == null)
                {
                    optimizedNested.Add(nested);
                }
                else
                {
                    modified = true;
                    optimizedNested.Add(optimized);
                }
            }

            if (!IsSingleton(@class) && @class.Properties.Any() && @class.Properties.All(property => IsSingleton(property.Type, first, second)))
            {
                return new Class(
                    @class.AccessModifier,
                    @class.ClassModifier,
                    @class.Name,
                    @class.GenericTypeParameters,
                    @class.BaseType,
                    new[]
                    {
                            new ConstructorDefinition(
                                AccessModifier.Private,
                                Enumerable.Empty<MethodParameter>(),
                                @class
                                    .Properties
                                    .Select(
                                        property => $"this.{property.Name} = {property.Type}.Instance;")),
                    },
                    @class.Methods,
                    optimizedNested,
                    @class
                        .Properties
                        .Append(
                            new PropertyDefinition(
                                AccessModifier.Public,
                                true,
                                $"{namespacePrefix}{@class.Name}",
                                "Instance",
                                true,
                                false,
                                $"new {namespacePrefix}{@class.Name}();"))
                    );
            }
            else if (modified)
            {
                return new Class(
                    @class.AccessModifier,
                    @class.ClassModifier,
                    @class.Name,
                    @class.GenericTypeParameters,
                    @class.BaseType,
                    @class.Constructors,
                    @class.Methods,
                    optimizedNested,
                    @class.Properties);
            }

            return null;
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
            if (@class.Constructors.Skip(1).Any())
            {
                // more than 1 constructor
                return false;
            }

            var constructor = @class.Constructors.ElementAt(0);
            if (constructor.AccessModifier != AccessModifier.Private)
            {
                return false;
            }

            if (constructor.Parameters.Any())
            {
                return false;
            }

            /*if (@class.NestedClasses.Any())
            {
                // TODO this is capturing visitors; it could probably be more robust
                return false;
            }*/

            if (!@class.Properties.Where(property => property.IsStatic && property.Name == "Instance").Any())
            {
                return false;
            }

            return true;
        }
    }
}
