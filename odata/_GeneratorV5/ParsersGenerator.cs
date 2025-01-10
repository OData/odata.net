namespace _GeneratorV5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using AbnfParserGenerator;

    public sealed class ParsersGenerator
    {
        private readonly string ruleParsersNamespace;
        private readonly string innerParsersNamespace;

        public ParsersGenerator(string ruleParsersNamespace, string innerParsersNamespace)
        {
            this.ruleParsersNamespace = ruleParsersNamespace;
            this.innerParsersNamespace = innerParsersNamespace;
        }

        public (Namespace RuleParsers, Namespace InnerParsers) Generate((Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            return
                (
                    new Namespace(
                        this.ruleParsersNamespace,
                        GenerateParsers(
                            cstNodes.RuleCstNodes,
                            cstNodes.RuleCstNodes.Name,
                            cstNodes.InnerCstNodes.Name),
                        new[]
                        {
                            "Sprache",
                        }),
                    new Namespace(
                        this.innerParsersNamespace,
                        GenerateParsers(
                            cstNodes.InnerCstNodes,
                            cstNodes.RuleCstNodes.Name,
                            cstNodes.InnerCstNodes.Name),
                        new[]
                        {
                            "Sprache",
                        })
                );
        }

        private IEnumerable<Class> GenerateParsers(Namespace @namespace, string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {
            foreach (var @class in @namespace.Classes)
            {
                var nonStaticProperties = @class.Properties.Where(property => !property.IsStatic);

                IEnumerable<Class> nestedClasses;
                IEnumerable<PropertyDefinition> property;
                if (nonStaticProperties.Any())
                {
                    nestedClasses = Enumerable.Empty<Class>();
                    var initializer = string
                        .Concat(
                            string.Join(
                                Environment.NewLine,
                                    nonStaticProperties
                                    .Select(
                                        property =>
                                        //// TODO this initializer stuff should probably be its own method and use a builder
                                        //// TODO handle nullables
                                            $"from {property.Name} in {UpdatePropertyType(property.Type, ruleCstNodesNamespace, innerCstNodesNamespace)}Parser.Instance{(property.Type.StartsWith("System.Collections.Generic.IEnumerable<") ? ".Many()" : string.Empty)}")), //// TODO how to handle different ranges of ienumerable (at most two, at least 3, etc.)
                            Environment.NewLine,
                            $"select new {@namespace.Name}.{@class.Name}(",
                            string.Join(
                                ", ",
                                nonStaticProperties
                                    .Select(property => property.Name)),
                            ");");
                    property = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{@namespace.Name}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            initializer),
                    };
                }
                else if (@class.NestedClasses.Any())
                {
                    //// TODO implement this for dus
                    nestedClasses = Enumerable.Empty<Class>();
                    property = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{@namespace.Name}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            null), //// TODO initializer
                    };
                }
                else
                {
                    //// TODO implement this for terminal nodes
                    nestedClasses = Enumerable.Empty<Class>();
                    property = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{@namespace.Name}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            null),
                    };
                }

                yield return new Class(
                    AccessModifier.Public,
                    ClassModifier.Static,
                    $"{@class.Name}Parser",
                    Enumerable.Empty<string>(),
                    null,
                    Enumerable.Empty<ConstructorDefinition>(),
                    Enumerable.Empty<MethodDefinition>(),
                    nestedClasses,
                    property);
            }
        }

        private string UpdatePropertyType(string propertyType, string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {
            var collectionDelimiter = "System.Collections.Generic.IEnumerable<";
            if (propertyType.StartsWith(collectionDelimiter))
            {
                return UpdatePropertyType(
                    propertyType.Substring(collectionDelimiter.Length, propertyType.Length - collectionDelimiter.Length - 1),
                    ruleCstNodesNamespace, 
                    innerCstNodesNamespace);
            }

            if (propertyType.StartsWith(ruleCstNodesNamespace))
            {
                return $"{this.ruleParsersNamespace}{propertyType.Substring(ruleCstNodesNamespace.Length)}";
            }

            if (propertyType.StartsWith(innerCstNodesNamespace))
            {
                return $"{this.innerParsersNamespace}{propertyType.Substring(innerCstNodesNamespace.Length)}";
            }

            return propertyType;
        }
    }
}
