namespace _GeneratorV6
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                var lines = instanceProperty
                    .Initializer!
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length > 2)
                {
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
                                "") //// TODO
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
            }
        }
    }
}
