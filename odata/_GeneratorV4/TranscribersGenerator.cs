namespace _GeneratorV4
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AbnfParserGenerator;

    public sealed class TranscribersGenerator
    {
        private readonly string rulesTranscribersNamespace;
        private readonly string innersTranscribersNamespace;

        private readonly string rulesCstNodesNamespace = "Test.CstNodes"; //// TODO figure out how to do namespaces correctly
        private readonly string innersCstNodesNamespace = "Inners"; //// TODO figure out how to do namespaces correctly

        public TranscribersGenerator(string rulesNamespace, string innersNamespace)
        {
            this.rulesTranscribersNamespace = rulesNamespace;
            this.innersTranscribersNamespace = innersNamespace;
        }

        public (IEnumerable<Class> Rules, IEnumerable<Class> Inners) Generate(IEnumerable<Class> cstNodes)
        {
            var rulesNodes = cstNodes
                .Where(node => !string.Equals(node.Name, "Inners", System.StringComparison.OrdinalIgnoreCase));
            var innersNodes = cstNodes
                .Where(node => string.Equals(node.Name, "Inners", System.StringComparison.OrdinalIgnoreCase))
                .Single()
                .NestedClasses;
            
            return (GenerateRules(rulesNodes), GenerateInners(innersNodes));
        }

        private IEnumerable<Class> GenerateRules(IEnumerable<Class> cstNodes)
        {
            foreach (var cstNode in cstNodes)
            {
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

        private IEnumerable<Class> GenerateInners(IEnumerable<Class> cstNodes)
        {
            foreach (var cstNode in cstNodes)
            {
                var transcriberName = $"{cstNode.Name}Transcriber";
                yield return new Class(
                    AccessModifier.Public,
                    ClassModifier.Sealed,
                    transcriberName,
                    Enumerable.Empty<string>(),
                    $"ITranscriber<GeneratorV3.Abnf.Inners.{cstNode.Name}>", //// TODO don't hardcode the inners namespace
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
                                new MethodParameter($"GeneratorV3.Abnf.Inners.{cstNode.Name}", "value"),
                                new MethodParameter("StringBuilder", "builder"),
                            },
                            TranscribeProperties(cstNode.Properties.Where(property => !property.IsStatic))), //// TODO bodies should be lines for whitespace purposes
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
                    /*var genericsStartIndex = propertyDefinition.Type.IndexOf("<");
                    var genericsEndIndex = propertyDefinition.Type.IndexOf(">");
                    var collectionType = propertyDefinition.Type.Substring(genericsStartIndex + 1, genericsEndIndex - genericsStartIndex - 1);
                    if (collectionType.StartsWith("Inners."))
                    {
                        collectionType = this.innersNamespace + "." + collectionType.Substring("Inners.".Length);
                    }

                    builder.AppendLine($"{collectionType}Transcriber.Instance.Transcribe({propertyDefinition.Name}, builder);");*/
                    builder.AppendLine("}");
                }
                else if (propertyDefinition.Type.EndsWith("?"))
                {
                    builder.AppendLine($"if (value.{propertyDefinition.Name} != null)");
                    builder.AppendLine("{");

                    var propertyType = propertyDefinition.Type;

                    if (propertyType.StartsWith(this.innersCstNodesNamespace))
                    {
                        builder.Append(this.innersTranscribersNamespace);
                        builder.Append(".");

                        propertyType = propertyType.Substring(this.innersCstNodesNamespace.Length + 1);
                    }
                    else if (propertyType.StartsWith(this.rulesCstNodesNamespace))
                    {
                        builder.Append(this.rulesTranscribersNamespace);
                        builder.Append(".");

                        propertyType = propertyType.Substring(this.rulesCstNodesNamespace.Length + 1);
                    }

                    propertyType = propertyType.Substring(0, propertyType.Length - 1);

                    builder.AppendLine($"{propertyType}Transcriber.Instance.Transcribe(value.{propertyDefinition.Name}, builder);");

                    builder.AppendLine("}");
                }
                else
                {
                    var propertyType = propertyDefinition.Type;

                    if (propertyType.StartsWith(this.innersCstNodesNamespace))
                    {
                        builder.Append(this.innersTranscribersNamespace);
                        builder.Append(".");

                        propertyType = propertyType.Substring(this.innersCstNodesNamespace.Length + 1);
                    }
                    else if (propertyType.StartsWith(this.rulesCstNodesNamespace))
                    {
                        builder.Append(this.rulesTranscribersNamespace);
                        builder.Append(".");

                        propertyType = propertyType.Substring(this.rulesCstNodesNamespace.Length + 1);
                    }

                    builder.AppendLine($"{propertyType}Transcriber.Instance.Transcribe(value.{propertyDefinition.Name}, builder);");
                }
            }
        }
    }
}
