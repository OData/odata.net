namespace _GeneratorV5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using AbnfParserGenerator;

    public sealed class TranscribersGenerator
    {
        private readonly string ruleTranscribersNamespace;
        private readonly string innerTranscribersNamespace;

        private readonly string rulesCstNodesNamespace = "Test.CstNodes"; //// TODO figure out how to do namespaces correctly
        private readonly string innersCstNodesNamespace = "Inners"; //// TODO figure out how to do namespaces correctly

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
            foreach (var cstNode in @namespace.Classes)
            {
                var transcriberName = $"{cstNode.Name}Transcriber";

                var nonStaticProperties = cstNode.Properties.Where(property => !property.IsStatic);
                string methodBody; //// TODO bodies should be lines for whitespace purposes
                IEnumerable<Class> nestedClasses;

                methodBody = string.Empty;
                nestedClasses = Enumerable.Empty<Class>();
                if (nonStaticProperties.Any()) //// TODO you are adding these cases to the rules (you already added to inners)
                {
                    if (cstNode.Name.Length == 3 && cstNode.Name[0] == '_' && char.IsDigit(cstNode.Name[1]) && char.IsDigit(cstNode.Name[2]))
                    {
                        ////methodBody = $"builder.Append((char)0x{cstNode.Name.TrimStart('_')});";
                        //// TODO
                        methodBody = string.Empty;
                    }
                    else if (cstNode.Name.StartsWith("_Ⰳx"))
                    {
                        //// TODO
                        ////methodBody = $"builder.Append((char)0{cstNode.Name.Substring(2)});";
                    }
                    else
                    {
                        methodBody = TranscribeProperties(
                            nonStaticProperties, 
                            ruleCstNodesNamespace, 
                            innerCstNodesNamespace, 
                            "value",
                            "builder");
                    }

                    nestedClasses = Enumerable.Empty<Class>();
                }
                /*else if (cstNode.NestedClasses.Any())
                {
                    methodBody = "Visitor.Instance.Visit(value, builder);";
                    nestedClasses = new[]
                    {
                        new Class(
                            AccessModifier.Private,
                            ClassModifier.Sealed,
                            "Visitor",
                            Enumerable.Empty<string>(),
                            $"GeneratorV3.Abnf.{cstNode.Name}.Visitor<Root.Void, StringBuilder>", //// TODO namespace should be computed
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Private,
                                    Enumerable.Empty<MethodParameter>(),
                                    Enumerable.Empty<string>()),
                            },
                            GenerateVisitorMethods(cstNode, false),
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
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(1)});";
                    }
                    else if (cstNode.Name.StartsWith("_Ⰳx"))
                    {
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(2)});";
                    }
                    else
                    {
                        //// TODO are there other terminal node cases?
                        //// methodBody = $"builder.Append(\"{cstNode.Name.TrimStart('_')}\");";
                        methodBody = $"builder.Append((char)0x{cstNode.Name.TrimStart('_')});";
                    }
                }*/

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

        /*private IEnumerable<Class> GenerateInners(IEnumerable<Class> cstNodes)
        {
            foreach (var cstNode in cstNodes)
            {
                var transcriberName = $"{cstNode.Name}Transcriber";

                var nonStaticProperties = cstNode.Properties.Where(property => !property.IsStatic);
                string methodBody; //// TODO bodies should be lines for whitespace purposes
                IEnumerable<Class> nestedClasses;
                if (nonStaticProperties.Any()) //// TODO add these branches to the `generaterules` method?
                {
                    if (cstNode.Name.Length == 3 && cstNode.Name[0] == '_' && char.IsDigit(cstNode.Name[1]) && char.IsDigit(cstNode.Name[2]))
                    {
                        ////methodBody = $"builder.Append((char)0x{cstNode.Name.TrimStart('_')});";
                        //// TODO
                        methodBody = string.Empty;
                    }
                    else if (cstNode.Name.StartsWith("_Ⰳx"))
                    {
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(2)});";
                    }
                    else
                    {
                        methodBody = TranscribeProperties(cstNode.Properties.Where(property => !property.IsStatic), "value", "builder");
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
                            $"GeneratorV3.Abnf.Inners.{cstNode.Name}.Visitor<Root.Void, StringBuilder>", //// TODO namespace should be computed
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Private,
                                    Enumerable.Empty<MethodParameter>(),
                                    Enumerable.Empty<string>()),
                            },
                            GenerateVisitorMethods(cstNode, true),
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
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(1)});";
                    }
                    else if (cstNode.Name.StartsWith("_Ⰳx"))
                    {
                        methodBody = $"builder.Append((char)0{cstNode.Name.Substring(2)});";
                    }
                    else
                    {
                        //// TODO are there other terminal node cases?
                        //// methodBody = $"builder.Append(\"{cstNode.Name.TrimStart('_')}\");";
                        methodBody = $"builder.Append((char)0x{cstNode.Name.TrimStart('_')});";
                    }
                }

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

        private IEnumerable<MethodDefinition> GenerateVisitorMethods(Class cstNode, bool inners)
        {
            foreach (var duMember in cstNode.NestedClasses.Where(member => member.BaseType?.EndsWith(cstNode.Name) ?? false))
            {
                string methodBody;
                if (duMember.Name.Length == 3 && duMember.Name[0] == '_' && char.IsAsciiHexDigit(duMember.Name[1]) && char.IsAsciiHexDigit(duMember.Name[2]))
                {
                    //// TODO it's weird that this decision is made here
                    methodBody = $"context.Append((char)0x{duMember.Name.TrimStart('_')});";
                }
                else
                {
                    methodBody = TranscribeProperties(duMember.Properties, "node", "context");
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
                        new MethodParameter($"GeneratorV3.Abnf{(inners ? ".Inners" : string.Empty)}.{cstNode.Name}.{duMember.Name}", "node"), //// TODO don't hardcode namespace
                        new MethodParameter("StringBuilder", "context"),
                    },
                    methodBody + Environment.NewLine + "return default;");
            }
        }*/

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
                /*if (propertyDefinition.Type.StartsWith("IEnumerable<"))
                {
                    builder.AppendLine($"foreach (var {propertyDefinition.Name} in {nodeName}.{propertyDefinition.Name})");
                    builder.AppendLine("{");
                    var genericsStartIndex = propertyDefinition.Type.IndexOf("<");
                    var genericsEndIndex = propertyDefinition.Type.IndexOf(">");
                    var collectionType = propertyDefinition.Type.Substring(genericsStartIndex + 1, genericsEndIndex - genericsStartIndex - 1);
                    if (collectionType.StartsWith(this.innersCstNodesNamespace))
                    {
                        builder.Append(this.innersTranscribersNamespace);
                        builder.Append(".");

                        collectionType = collectionType.Substring(this.innersCstNodesNamespace.Length + 1);
                    }
                    else if (collectionType.StartsWith(this.rulesCstNodesNamespace))
                    {
                        builder.Append(this.rulesTranscribersNamespace);
                        builder.Append(".");

                        collectionType = collectionType.Substring(this.rulesCstNodesNamespace.Length + 1);
                    }

                    builder.AppendLine($"{collectionType}Transcriber.Instance.Transcribe({propertyDefinition.Name}, {builderName});");
                    builder.AppendLine("}");
                }
                else */if (propertyDefinition.Type.EndsWith("?"))
                {
                    builder.AppendLine($"if (value.{propertyDefinition.Name} != null)");
                    builder.AppendLine("{");

                    var propertyType = propertyDefinition.Type.Substring(0, propertyDefinition.Type.Length - 1);

                    //// TODO i think you can delete this commetn block
                    /*if (propertyType.StartsWith(this.innersCstNodesNamespace))
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

                    propertyType = propertyType.Substring(0, propertyType.Length - 1);*/

                    //// TODO this line should work
                    builder.AppendLine($"{GetTranscriberType(propertyType, ruleCstNodesNamespace, innerCstNodesNamespace)}Transcriber.Instance.Transcribe({nodeName}.{propertyDefinition.Name}, {builderName});");

                    builder.AppendLine("}");
                }
                /*else
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

                    builder.AppendLine($"{propertyType}Transcriber.Instance.Transcribe({nodeName}.{propertyDefinition.Name}, {builderName});");
                }*/
            }
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
