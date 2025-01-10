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
                        GenerateParsers(cstNodes.RuleCstNodes)),
                    new Namespace(
                        this.innerParsersNamespace,
                        GenerateParsers(cstNodes.InnerCstNodes))
                );
        }

        private IEnumerable<Class> GenerateParsers(Namespace @namespace)
        {
            foreach (var @class in @namespace.Classes)
            {
                yield return new Class(
                    AccessModifier.Public,
                    ClassModifier.Static,
                    $"{@class.Name}Parser",
                    Enumerable.Empty<string>(),
                    null,
                    Enumerable.Empty<ConstructorDefinition>(),
                    Enumerable.Empty<MethodDefinition>(),
                    Enumerable.Empty<Class>(), //// TODO there are nested classes for dus
                    Enumerable.Empty<PropertyDefinition>()); //// TODO the instance property
            }
        }
    }
}
