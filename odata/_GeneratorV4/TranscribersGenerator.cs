namespace _GeneratorV4
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
                            TranscribeProperties(cstNode.Properties.Where(property => !property.IsStatic))),
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

        private string TranscribeProperties(IEnumerable<PropertyDefinition> propertyDefinitions)
        {
            var builder = new StringBuilder();

            TranscribeProperties(propertyDefinitions, builder);

            return builder.ToString();
        }

        private void TranscribeProperties(IEnumerable<PropertyDefinition> propertyDefinitions, StringBuilder builder)
        {
            foreach (var propertyDefinition in propertyDefinitions)
            {
                if (propertyDefinition.Type.StartsWith("IEnumerable<"))
                {
                    builder.AppendLine($"foreach (var {propertyDefinition.Name} in value.{propertyDefinition.Name})");
                    builder.AppendLine("{");
                    var genericsStartIndex = propertyDefinition.Type.IndexOf("<");
                    var genericsEndIndex = propertyDefinition.Type.IndexOf(">");
                    var collectionType = propertyDefinition.Type.Substring(genericsStartIndex + 1, genericsEndIndex - genericsStartIndex);
                    if (collectionType.StartsWith("Inners."))
                    {
                        collectionType = this.innersNamespace + collectionType.Substring("Inners.".Length);
                    }

                    builder.AppendLine($"{collectionType}Transcriber.Instance.Transcribe({propertyDefinition.Name}, builder);");
                    builder.AppendLine("}");
                }
                else
                {
                    builder.AppendLine($"{propertyDefinition.Type}Transcriber.Instance.Transcribe(value.{propertyDefinition}, builder);");
                }
            }
        }
    }
}
