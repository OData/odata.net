namespace _GeneratorV5
{
    using System.Collections.Generic;
    using System.Linq;

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
                        GenerateParsers(cstNodes.RuleCstNodes),
                        new[]
                        {
                            "Sprache",
                        }),
                    new Namespace(
                        this.innerParsersNamespace,
                        GenerateParsers(cstNodes.InnerCstNodes),
                        new[]
                        {
                            "Sprache",
                        })
                );
        }

        private IEnumerable<Class> GenerateParsers(Namespace @namespace)
        {
            foreach (var @class in @namespace.Classes)
            {
                IEnumerable<Class> nestedClasses;
                IEnumerable<PropertyDefinition> property;
                ////if (@class.NestedClasses.Any())
                {
                    //// TODO implement this for dus
                    nestedClasses = Enumerable.Empty<Class>();
                    property = Enumerable.Empty<PropertyDefinition>();
                }
                /*else
                {
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
                }*/

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
    }
}
