namespace _GeneratorV5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
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
            //// TODO how to detect ambiguities
            //// TODO correctly handle "repeats" of values (at least one, at most 5, between 2 and six, etc)

            return
                (
                    new Namespace(
                        this.ruleParsersNamespace,
                        GenerateParsers(
                            cstNodes.RuleCstNodes,
                            cstNodes.RuleCstNodes.Name,
                            cstNodes.InnerCstNodes.Name).ToList(),
                        new[]
                        {
                            "CombinatorParsingV2",
                        }),
                    new Namespace(
                        this.innerParsersNamespace,
                        GenerateParsers(
                            cstNodes.InnerCstNodes,
                            cstNodes.RuleCstNodes.Name,
                            cstNodes.InnerCstNodes.Name).ToList(),
                        new[]
                        {
                            "CombinatorParsingV2",
                        })
                );
        }

        private IEnumerable<Class> GenerateParsers(Namespace @namespace, string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {
            return @namespace
                .Classes
                .Where(
                    @class =>
                        !@class.Name.StartsWith($"HelperRanged"))
                .Select(
                    @class =>
                        GenerateParser(@class, @namespace.Name, ruleCstNodesNamespace, innerCstNodesNamespace));
        }

        private string RangeCount(PropertyDefinition property, string innerCstNodesNamespace)
        {
            if (property.Type.StartsWith("System.Collections.Generic.IEnumerable<"))
            {
                return ".Many()";
            }

            var helperRangedDelimiter = $"{innerCstNodesNamespace}.HelperRanged";
            if (property.Type.StartsWith(helperRangedDelimiter))
            {
                var kindOfHelper = property.Type.Substring(helperRangedDelimiter.Length);
                var exactlyDelimiter = "Exactly";
                if (kindOfHelper.StartsWith(exactlyDelimiter))
                {
                    var countStart = kindOfHelper.Substring(exactlyDelimiter.Length);
                    var genericIndex = countStart.IndexOf("<");
                    var count = countStart.Substring(0, genericIndex);
                    return $".Repeat({count}, {count})";
                }

                var atMostDelimiter = "AtMost";
                if (kindOfHelper.StartsWith(atMostDelimiter))
                {
                    var countStart = kindOfHelper.Substring(atMostDelimiter.Length);
                    var genericIndex = countStart.IndexOf("<");
                    var count = countStart.Substring(0, genericIndex);
                    return $".Repeat(0, {count})";
                }

                var atLeastDelimiter = "AtLeast";
                if (kindOfHelper.StartsWith(atLeastDelimiter))
                {
                    var countStart = kindOfHelper.Substring(atLeastDelimiter.Length);
                    var genericIndex = countStart.IndexOf("<");
                    var count = countStart.Substring(0, genericIndex);
                    return $".Repeat({count}, null)";
                }

                var fromDelimiter = "From";
                if (kindOfHelper.StartsWith(fromDelimiter))
                {
                    var minimumStart = kindOfHelper.Substring(fromDelimiter.Length);
                    var toDelimiter = "To";
                    var toDelimiterIndex = minimumStart.IndexOf(toDelimiter);

                    var minimum = minimumStart.Substring(0, toDelimiterIndex);

                    var maximumStart = minimumStart.Substring(toDelimiterIndex + toDelimiter.Length);
                    var genericIndex = maximumStart.IndexOf("<");

                    var maximum = maximumStart.Substring(0, genericIndex);
                    return $".Repeat({minimum}, {maximum})";
                }

                throw new Exception("TODO");
            }
            
            return string.Empty;
        }

        private string ConstructorArgument(PropertyDefinition property, string innerCstNodesNamespace)
        {
            var builder = new StringBuilder();

            var ranged = false;
            if (property.Type.StartsWith($"{innerCstNodesNamespace}.HelperRanged"))
            {
                ranged = true;
                builder.Append($"new {property.Type}(");
            }

            builder.Append(property.Name);

            if (ranged)
            {
                builder.Append(")");
            }

            if (property.Type.EndsWith("?"))
            {
                builder.Append(".GetOrElse(null)");
            }

            return builder.ToString();
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
                            $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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
                            $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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
                                            $"from {property.Name} in {UpdatePropertyType(property.Type, ruleCstNodesNamespace, innerCstNodesNamespace)}Parser.Instance{RangeCount(property, innerCstNodesNamespace)}{(property.Type.EndsWith("?") ? ".Optional()" : string.Empty)}")), //// TODO how to handle different ranges of ienumerable (at most two, at least 3, etc.) //// TODO what are the cases where nullable and enumerable are used together?
                            Environment.NewLine,
                            $"select new {cstNodeNamespace}.{@class.Name}(",
                            string.Join(
                                ", ",
                                nonStaticProperties
                                    .Select(property => ConstructorArgument(property, innerCstNodesNamespace))),
                            ");");
                    propertyDefinition = new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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
                            $").Or<char, {cstNodeNamespace}.{@class.Name}>(",
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
                        $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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
                            $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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
                            $"IParser<char, {cstNodeNamespace}.{@class.Name}>",
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

            if (propertyType.StartsWith($"{innerCstNodesNamespace}.HelperRanged"))
            {
                var genericIndex = propertyType.IndexOf("<");
                var closingGenericIndex = propertyType.IndexOf(">");
                return UpdatePropertyType(
                    propertyType.Substring(genericIndex + 1, closingGenericIndex - genericIndex - 1),
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
