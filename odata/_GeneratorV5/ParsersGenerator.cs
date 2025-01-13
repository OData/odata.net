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
            //// TODO create a test that illustrates the innner vs rule name conflicts that can happen

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
            return @namespace
                .Classes
                .Select(
                    @class =>
                        GenerateParser(@class, @namespace.Name, ruleCstNodesNamespace, innerCstNodesNamespace));
        }

        private Class GenerateParser(Class @class, string cstNodeNamespace, string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {
            var nonStaticProperties = @class.Properties.Where(property => !property.IsStatic);

            IEnumerable<Class> nestedClasses;
            IEnumerable<PropertyDefinition> propertyDefinition;
            if (nonStaticProperties.Any())
            {
                if (@class.BaseType == null && @class.Name.Length == 5 && @class.Name.StartsWith("_Ⰳx"))
                {
                    //// TODO document from transcribers generator
                    nestedClasses = Enumerable.Empty<Class>();
                    var initializer = string
                        .Concat(
                            $"from {@class.Name} in Parse.Char((char)0{@class.Name.Substring(2)}) select new {cstNodeNamespace}.{@class.Name}(",
                            string.Join(
                                ", ",
                                @class.Properties.Select(
                                    property =>
                                        $"{property.Type}.Instance")),
                            ");");
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{cstNodeNamespace}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            initializer),
                    };
                }
                else if (@class.Name.Length == 3 && @class.Name[0] == '_' && char.IsAsciiHexDigit(@class.Name[1]) && char.IsAsciiHexDigit(@class.Name[2]))
                {
                    //// TODO document from transcribers generator
                    nestedClasses = Enumerable.Empty<Class>();
                    var initializer = string
                        .Concat(
                            $"from {@class.Name} in Parse.Char((char)0x{@class.Name.Substring(1)}) select new {cstNodeNamespace}.{@class.Name}(",
                            string.Join(
                                ", ",
                                @class.Properties.Select(
                                    property =>
                                        $"{property.Type}.Instance")),
                            ");");
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{cstNodeNamespace}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            initializer),
                    };
                }
                else
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
                                            $"from {property.Name} in {UpdatePropertyType(property.Type, ruleCstNodesNamespace, innerCstNodesNamespace)}Parser.Instance{(property.Type.StartsWith("System.Collections.Generic.IEnumerable<") ? ".Many()" : string.Empty)}{(property.Type.EndsWith("?") ? ".Optional()" : string.Empty)}")), //// TODO how to handle different ranges of ienumerable (at most two, at least 3, etc.) //// TODO what are the cases where nullable and enumerable are used together?
                            Environment.NewLine,
                            $"select new {cstNodeNamespace}.{@class.Name}(",
                            string.Join(
                                ", ",
                                nonStaticProperties
                                    .Select(property => $"{property.Name}{(property.Type.EndsWith("?") ? ".GetOrElse(null)" : string.Empty)}")),
                            ");");
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{cstNodeNamespace}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            initializer),
                    };
                }
            }
            else if (@class.NestedClasses.Any())
            {
                nestedClasses = @class
                    .NestedClasses
                    .Where(
                        nestedClass =>
                            nestedClass.BaseType?.EndsWith(@class.Name) ?? false)
                    .Select(
                        member =>
                            GenerateParser(
                                member, 
                                $"{cstNodeNamespace}.{@class.Name}", 
                                ruleCstNodesNamespace, 
                                innerCstNodesNamespace));
                var initializer = string
                    .Concat(
                        "(",
                        string.Join(
                            $").Or<{cstNodeNamespace}.{@class.Name}>(",
                            nestedClasses
                                .Select(
                                    nestedClass =>
                                        $"{nestedClass.Name}.Instance")),
                        ");");
                propertyDefinition = new[]
                {
                    new PropertyDefinition(
                        AccessModifier.Public,
                        true,
                        $"Parser<{cstNodeNamespace}.{@class.Name}>",
                        "Instance",
                        true,
                        false,
                        initializer),
                };
            }
            else
            {
                if (@class.Name.Length == 4 && @class.Name[0] == '_' && @class.Name[1] == 'x' && char.IsAsciiHexDigit(@class.Name[2]) && char.IsAsciiHexDigit(@class.Name[3]))
                {
                    //// TODO document from transcribers generator
                    nestedClasses = Enumerable.Empty<Class>();
                    var initializer = $"from {@class.Name} in Parse.Char((char)0{@class.Name.Substring(1)}) select {cstNodeNamespace}.{@class.Name}.Instance;";
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{cstNodeNamespace}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            initializer),
                    };
                }
                else
                {
                    //// TODO document from transcribers generator
                    nestedClasses = Enumerable.Empty<Class>();
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"Parser<{cstNodeNamespace}.{@class.Name}>",
                            "Instance",
                            true,
                            false,
                            null),
                    };
                }
            }

            return new Class(
                AccessModifier.Public,
                ClassModifier.Static,
                $"{@class.Name}Parser",
                Enumerable.Empty<string>(),
                null,
                Enumerable.Empty<ConstructorDefinition>(),
                Enumerable.Empty<MethodDefinition>(),
                nestedClasses,
                propertyDefinition);
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

            if (propertyType.EndsWith("?"))
            {
                return UpdatePropertyType(
                    propertyType.Substring(0, propertyType.Length - 1),
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
