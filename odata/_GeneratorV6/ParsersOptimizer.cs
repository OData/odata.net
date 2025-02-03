namespace _GeneratorV6
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using AbnfParserGenerator;

    public sealed class ParsersOptimizer
    {
        public ParsersOptimizer()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) Optimize(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            return
                (
                    new Namespace(
                        cstNodes.RuleCstNodes.Name,
                        Optimize(cstNodes.RuleCstNodes.Classes).ToList(),
                        cstNodes.RuleCstNodes.UsingDeclarations),
                    new Namespace(
                        cstNodes.InnerCstNodes.Name,
                        Optimize(cstNodes.InnerCstNodes.Classes).ToList(),
                        cstNodes.InnerCstNodes.UsingDeclarations)
                );
        }

        private IEnumerable<Class> Optimize(IEnumerable<Class> parsers)
        {
            foreach (var parser in parsers)
            {
                var instanceProperty = parser
                    .Properties
                    .Where(property => property.IsStatic && property.Name == "Instance")
                    .First();

                var initializer = instanceProperty.Initializer;
                if (initializer == null)
                {
                    continue;
                }

                var lines = initializer
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                if (lines.Count > 2)
                {
                    var returnType = lines[lines.Count - 1];
                    var selectNewDelimiter = "select new ";
                    if (returnType.StartsWith(selectNewDelimiter))
                    {
                        returnType = returnType.Substring(selectNewDelimiter.Length);
                        returnType = returnType.Substring(0, returnType.IndexOf("("));
                    }
                    else
                    {
                        var selectDelimiter = "select ";
                        returnType = returnType.Substring(selectDelimiter.Length);
                        returnType = returnType.Substring(0, returnType.IndexOf(".Instance"));
                    }

                    var blocks =
                        GenerateBlocks(
                            lines
                                .Take(lines.Count - 1)
                                .Select(SplitLine),
                            returnType)
                        .ToList();

                    var parseBody = blocks
                        .Take(blocks.Count - 1)
                        .Append(
                            GenerateLastLine(lines[lines.Count - 1], blocks[blocks.Count - 1]));

                    var nestedParser = new Class(
                        AccessModifier.Private,
                        ClassModifier.Sealed,
                        "Parser",
                        Enumerable.Empty<string>(),
                        instanceProperty.Type,
                        new[]
                        {
                            new ConstructorDefinition(
                                AccessModifier.Public,
                                Enumerable.Empty<MethodParameter>(),
                                Enumerable.Empty<string>()),
                        },
                        new[]
                        {
                            new MethodDefinition(
                                AccessModifier.Public,
                                ClassModifier.None,
                                false,
                                "IOutput" + instanceProperty.Type.Substring("IParser".Length),
                                Enumerable.Empty<string>(),
                                "Parse",
                                new[]
                                {
                                    new MethodParameter("IInput<char>?", "input"),
                                },
                                string.Join(
                                    Environment.NewLine,
                                    parseBody)),
                        },
                        Enumerable.Empty<Class>(),
                        Enumerable.Empty<PropertyDefinition>());
                    yield return new Class(
                        parser.AccessModifier,
                        parser.ClassModifier,
                        parser.Name,
                        parser.GenericTypeParameters,
                        parser.BaseType,
                        parser.Constructors,
                        parser.Methods,
                        parser
                            .NestedClasses
                            .Append(nestedParser),
                        parser
                            .Properties
                            .Where(property => !property.IsStatic && property.Name != "Instance")
                            .Append(
                                new PropertyDefinition(
                                    AccessModifier.Public,
                                    true,
                                    instanceProperty.Type,
                                    "Instance",
                                    true,
                                    false,
                                    "new Parser();")));
                }
                else
                {
                    yield return parser;
                }
            }
        }

        private (string Name, string Parser) SplitLine(string line)
        {
            var fromDelimiter = "from ";
            var inDelimiter = " in ";

            var nameStartIndex = fromDelimiter.Length;
            var nameEndIndex = line.IndexOf(inDelimiter, nameStartIndex);
            var name = line.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

            var parserStartIndex = nameEndIndex + inDelimiter.Length;
            var parser = line.Substring(parserStartIndex);

            return (name, parser);
        }

        private IEnumerable<string> GenerateBlocks(IEnumerable<(string Name, string Parser)> splitLines, string returnType)
        {
            var remainderName = "input";
            foreach (var splitLine in splitLines)
            {
                yield return
$$"""
var {{splitLine.Name}} = {{splitLine.Parser}}.Parse({{remainderName}});
if (!{{splitLine.Name}}.Success)
{
    return Output.Create(false, default({{returnType}})!, input);
}

""";
                remainderName = $"{splitLine.Name}.Remainder";
            }

            yield return remainderName;
        }

        private string GenerateLastLine(string line, string remainder)
        {
            if (line.StartsWith("select new "))
            {
                var result = "return Output.Create(true, new ";

                var typeStartIndex = "select new ".Length;
                var typeEndIndex = line.IndexOf("(");
                var typeName = line.Substring(typeStartIndex, typeEndIndex - typeStartIndex);

                result += typeName;
                result += "(";

                var first = true;
                while (true)
                {
                    if (!first)
                    {
                        result += ", ";
                    }

                    first = false;

                    var commaIndex = line.IndexOf(", ", typeEndIndex + 1);
                    if (commaIndex < 0)
                    {
                        var parenIndex = line.IndexOf(")", typeEndIndex + 1);
                        result += line.Substring(typeEndIndex, parenIndex - typeEndIndex + 1);
                        result += ".Parsed";
                        break;
                    }

                    result += line.Substring(typeEndIndex + 1, commaIndex - typeEndIndex - 1);
                    result += ".Parsed";

                    typeEndIndex = commaIndex + 2;
                }

                result += "), ";
                result += remainder;
                result += ");";

                return result;
            }
            else
            {
                var singleton = line.Substring("select ".Length);
                singleton = singleton.Substring(0, singleton.Length - 1);
                return $"return Output.Create(true, {singleton}, {remainder});";
            }
        }
    }
}
