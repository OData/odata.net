namespace _GeneratorV5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AbnfParserGenerator;

    public sealed class TranscribersGenerator
    {
        private readonly string ruleTranscribersNamespace;
        private readonly string innerTranscribersNamespace;

        public TranscribersGenerator(string ruleTranscribersNamespace, string innerTranscribersNamespace)
        {
            this.ruleTranscribersNamespace = ruleTranscribersNamespace;
            this.innerTranscribersNamespace = innerTranscribersNamespace;
        }

        public (IEnumerable<Class> Rules, IEnumerable<Class> Inners) Generate((Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var innersNodes = cstNodes
                .InnerCstNodes
                .Classes;

            return 
                (
                    GenerateTranscribers(cstNodes.RuleCstNodes, cstNodes.RuleCstNodes.Name, cstNodes.InnerCstNodes.Name), 
                    GenerateTranscribers(cstNodes.InnerCstNodes, cstNodes.RuleCstNodes.Name, cstNodes.InnerCstNodes.Name)
                );
        }

        private IEnumerable<Class> GenerateTranscribers(
            Namespace @namespace,
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace)
        {
            foreach (var cstNode in @namespace.Classes.Where(@class => !@class.Name.StartsWith("HelperRanged")))
            {
                var transcriberName = $"{cstNode.Name}Transcriber";

                var nonStaticProperties = cstNode.Properties.Where(property => !property.IsStatic);
                string methodBody; //// TODO bodies should be lines for whitespace purposes
                IEnumerable<Class> nestedClasses;
                if (nonStaticProperties.Any())
                {
                    if (cstNode.Name.StartsWith("_Ⰳx"))
                    {
                        //// TODO Here's what seems to happen, and i think it's a generator issue. Whenever there's a literal,
                        //// you turn that into hex (not sure why). These nodes end up with properties that are just the
                        //// individual hex digits, but that has nothing to do with the literal anymore, so we have to
                        //// circumvent it here in the transcribers. I think you should probably take those nodes that represent
                        //// literals and just make them terminal nodes.
                        //// 
                        //// The other case is where you have a range of values. In this case, you end up with a discriminated
                        //// union where the base type has the "%x" portion and the derived types don't. These don't represent
                        //// a literal in the same way as the first case, they instead represent an option that the ABNF author
                        //// is allowed to write (rather than a requirement). These probably should be kept more or less as they
                        //// are, but the derived types shouldn't have properties.
                        
                        //// this branch handles part of the first case
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(2)});";
                    }
                    else
                    {
                        // this is a non-terminal node, so call the transcribers for this node's properties
                        methodBody = TranscribeProperties(
                            nonStaticProperties, 
                            ruleCstNodesNamespace, 
                            innerCstNodesNamespace, 
                            "value",
                            "builder");
                    }

                    nestedClasses = Enumerable.Empty<Class>();
                }
                else if (cstNode.NestedClasses.Any())
                {
                    methodBody = "Visitor.Instance.Visit(value, builder);";
                    nestedClasses = new[]
                    {
                        new Class(
                            AccessModifier.Private,
                            ClassModifier.Sealed,
                            "Visitor",
                            Enumerable.Empty<string>(),
                            $"{@namespace.Name}.{cstNode.Name}.Visitor<Root.Void, System.Text.StringBuilder>",
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Private,
                                    Enumerable.Empty<MethodParameter>(),
                                    Enumerable.Empty<string>()),
                            },
                            GenerateVisitorMethods(cstNode, @namespace.Name, ruleCstNodesNamespace, innerCstNodesNamespace),
                            Enumerable.Empty<Class>(),
                            new[]
                            {
                                new PropertyDefinition(
                                    AccessModifier.Public,
                                    true,
                                    "Visitor",
                                    "Instance",
                                    true,
                                    false,
                                    "new Visitor();"),
                            }),
                    };
                }
                else
                {
                    nestedClasses = Enumerable.Empty<Class>();
                    if (cstNode.Name.StartsWith("_x"))
                    {
                        //// TODO Here's what seems to happen, and i think it's a generator issue. Whenever there's a literal,
                        //// you turn that into hex (not sure why). These nodes end up with properties that are just the
                        //// individual hex digits, but that has nothing to do with the literal anymore, so we have to
                        //// circumvent it here in the transcribers. I think you should probably take those nodes that represent
                        //// literals and just make them terminal nodes.
                        //// 
                        //// The other case is where you have a range of values. In this case, you end up with a discriminated
                        //// union where the base type has the "%x" portion and the derived types don't. These don't represent
                        //// a literal in the same way as the first case, they instead represent an option that the ABNF author
                        //// is allowed to write (rather than a requirement). These probably should be kept more or less as they
                        //// are, but the derived types shouldn't have properties.

                        //// this branch handles part of the first case
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(1)});";
                    }
                    else
                    {
                        //// TODO Here's what seems to happen, and i think it's a generator issue. Whenever there's a literal,
                        //// you turn that into hex (not sure why). These nodes end up with properties that are just the
                        //// individual hex digits, but that has nothing to do with the literal anymore, so we have to
                        //// circumvent it here in the transcribers. I think you should probably take those nodes that represent
                        //// literals and just make them terminal nodes.
                        //// 
                        //// The other case is where you have a range of values. In this case, you end up with a discriminated
                        //// union where the base type has the "%x" portion and the derived types don't. These don't represent
                        //// a literal in the same way as the first case, they instead represent an option that the ABNF author
                        //// is allowed to write (rather than a requirement). These probably should be kept more or less as they
                        //// are, but the derived types shouldn't have properties.

                        //// this branch handles the single hex digit transcribers that shouldn't exist and are currently
                        //// going to do no-ops; this branch *should* actually indicate a terminal node if the above TODO were
                        //// addressed

                        methodBody = string.Empty;
                    }
                }

                yield return new Class(
                    AccessModifier.Public,
                    ClassModifier.Sealed,
                    transcriberName,
                    Enumerable.Empty<string>(),
                    $"GeneratorV3.ITranscriber<{@namespace.Name}.{cstNode.Name}>",
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
                                new MethodParameter($"{@namespace.Name}.{cstNode.Name}", "value"),
                                new MethodParameter("System.Text.StringBuilder", "builder"),
                            },
                            methodBody),
                    },
                    nestedClasses,
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

        private IEnumerable<MethodDefinition> GenerateVisitorMethods(
            Class cstNode, 
            string @namespace,
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace)
        {
            foreach (var duMember in cstNode.NestedClasses.Where(member => member.BaseType?.EndsWith(cstNode.Name) ?? false))
            {
                string methodBody;
                if (duMember.Name.Length == 3 && duMember.Name[0] == '_' && char.IsAsciiHexDigit(duMember.Name[1]) && char.IsAsciiHexDigit(duMember.Name[2]))
                {
                    //// TODO Here's what seems to happen, and i think it's a generator issue. Whenever there's a literal,
                    //// you turn that into hex (not sure why). These nodes end up with properties that are just the
                    //// individual hex digits, but that has nothing to do with the literal anymore, so we have to
                    //// circumvent it here in the transcribers. I think you should probably take those nodes that represent
                    //// literals and just make them terminal nodes.
                    //// 
                    //// The other case is where you have a range of values. In this case, you end up with a discriminated
                    //// union where the base type has the "%x" portion and the derived types don't. These don't represent
                    //// a literal in the same way as the first case, they instead represent an option that the ABNF author
                    //// is allowed to write (rather than a requirement). These probably should be kept more or less as they
                    //// are, but the derived types shouldn't have properties.
                    
                    //// this branch is handling the second case
                    methodBody = $"context.Append((char)0x{duMember.Name.TrimStart('_')});";
                }
                else
                {
                    methodBody = TranscribeProperties(duMember.Properties, ruleCstNodesNamespace, innerCstNodesNamespace, "node", "context");
                }

                yield return new MethodDefinition(
                    AccessModifier.Protected | AccessModifier.Internal,
                    ClassModifier.None,
                    true,
                    "Root.Void",
                    Enumerable.Empty<string>(),
                    "Accept",
                    new[]
                    {
                        new MethodParameter($"{@namespace}.{cstNode.Name}.{duMember.Name}", "node"),
                        new MethodParameter("System.Text.StringBuilder", "context"),
                    },
                    methodBody + Environment.NewLine + "return default;");
            }
        }

        private string TranscribeProperties(
            IEnumerable<PropertyDefinition> propertyDefinitions,
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace,
            string nodeName,
            string builderName)
        {
            var builder = new StringBuilder();

            TranscribeProperties(propertyDefinitions, ruleCstNodesNamespace, innerCstNodesNamespace, nodeName, builderName, builder);

            return builder.ToString();
        }

        private void TranscribeProperties(
            IEnumerable<PropertyDefinition> propertyDefinitions, 
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace,
            string nodeName, 
            string builderName, 
            StringBuilder builder)
        {
            foreach (var propertyDefinition in propertyDefinitions)
            {
                if (propertyDefinition.Type.StartsWith("System.Collections.Generic.IEnumerable<") || propertyDefinition.Type.StartsWith($"{innerCstNodesNamespace}.HelperRanged"))
                {
                    HandleCollectionType(propertyDefinition, ruleCstNodesNamespace, innerCstNodesNamespace, nodeName, builderName, builder);
                }
                else if (propertyDefinition.Type.EndsWith("?"))
                {
                    builder.AppendLine($"if ({nodeName}.{propertyDefinition.Name} != null)");
                    builder.AppendLine("{");

                    var propertyType = propertyDefinition.Type.Substring(0, propertyDefinition.Type.Length - 1);

                    builder.AppendLine($"{GetTranscriberType(propertyType, ruleCstNodesNamespace, innerCstNodesNamespace)}Transcriber.Instance.Transcribe({nodeName}.{propertyDefinition.Name}, {builderName});");

                    builder.AppendLine("}");
                }
                else
                {
                    var propertyType = propertyDefinition.Type;

                    builder.AppendLine($"{GetTranscriberType(propertyType, ruleCstNodesNamespace, innerCstNodesNamespace)}Transcriber.Instance.Transcribe({nodeName}.{propertyDefinition.Name}, {builderName});");
                }
            }
        }

        private void HandleCollectionType(
            PropertyDefinition propertyDefinition,
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace,
            string nodeName,
            string builderName,
            StringBuilder builder)
        {
            builder.AppendLine($"foreach (var {propertyDefinition.Name} in {nodeName}.{propertyDefinition.Name})");
            builder.AppendLine("{");
            var genericsStartIndex = propertyDefinition.Type.IndexOf("<");
            var genericsEndIndex = propertyDefinition.Type.IndexOf(">");
            var collectionType = propertyDefinition.Type.Substring(genericsStartIndex + 1, genericsEndIndex - genericsStartIndex - 1);

            builder.AppendLine($"{GetTranscriberType(collectionType, ruleCstNodesNamespace, innerCstNodesNamespace)}Transcriber.Instance.Transcribe({propertyDefinition.Name}, {builderName});");

            builder.AppendLine("}");
        }

        private string GetTranscriberType(string propertyType, string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {
            if (propertyType.StartsWith(ruleCstNodesNamespace))
            {
                return $"{this.ruleTranscribersNamespace}{propertyType.Substring(ruleCstNodesNamespace.Length)}";
            }

            if (propertyType.StartsWith(innerCstNodesNamespace))
            {
                return $"{this.innerTranscribersNamespace}{propertyType.Substring(innerCstNodesNamespace.Length)}";
            }

            return propertyType;
        }
    }
}
