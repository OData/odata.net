namespace _GeneratorV4
{
    using System.Collections.Generic;
    using System.Linq;

    using AbnfParserGenerator;

    public sealed class TranscribersGenerator
    {
        private readonly string rulesNamespace;
        private readonly string innersNamespace;

        public TranscribersGenerator(string rulesNamespace, string innersNamespace)
        {
            this.rulesNamespace = rulesNamespace;
            this.innersNamespace = innersNamespace;
        }

        public IEnumerable<Class> Generate(IEnumerable<Class> cstNodes)
        {
            foreach (var cstNode in cstNodes)
            {
                if (string.Equals(cstNode.Name, "Inners", System.StringComparison.OrdinalIgnoreCase))
                {
                    //// TODO handle the inners
                    continue;
                }

                var transcriberName = $"{cstNode.Name}Transcriber";
                yield return new Class(
                    AccessModifier.Public,
                    ClassModifier.Sealed,
                    transcriberName,
                    Enumerable.Empty<string>(),
                    $"ITranscriber<{cstNode.Name}>",
                    new[]
                    {
                        new ConstructorDefinition(
                            AccessModifier.Private,
                            Enumerable.Empty<MethodParameter>(),
                            Enumerable.Empty<string>()),
                    },
                    new[]
                    {
                        new MethodDefinition(
                            AccessModifier.Public,
                            ClassModifier.None,
                            false,
                            "void",
                            Enumerable.Empty<string>(),
                            "Transcribe",
                            new[]
                            {
                                //// TODO you should be using a fully qualified name here, but `class` doesn't include the namespace
                                new MethodParameter(cstNode.Name, "value"),
                                new MethodParameter("StringBuilder", "builder"),
                            },
                            string.Empty), //// TODO generate the method body
                    },
                    Enumerable.Empty<Class>(), //// TODO sometimes you need a nested visitor
                    new[]
                    {
                        new PropertyDefinition(
                            AccessModifier.Public,
                            true,
                            transcriberName,
                            "Instance",
                            true,
                            false,
                            $"new {transcriberName}();"),
                    });
            }
        }
    }
}
