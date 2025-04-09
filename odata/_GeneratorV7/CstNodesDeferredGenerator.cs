namespace _GeneratorV7
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using __GeneratedOdata.Trancsribers.Rules;
    using AbnfParserGenerator;

    public sealed class CstNodesDeferredGenerator
    {
        public CstNodesDeferredGenerator()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) CreateDeferred(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var translatedRules = Translate(cstNodes.RuleCstNodes, cstNodes.RuleCstNodes, cstNodes.InnerCstNodes);
            var translatedInners = Translate(cstNodes.InnerCstNodes, cstNodes.RuleCstNodes, cstNodes.InnerCstNodes);

            return
                (
                    translatedRules,
                    translatedInners
                );
        }

        private Namespace Translate(Namespace toTranslate, Namespace rules, Namespace inners)
        {
            var translatedClasses = Translate(toTranslate.Classes, rules, inners).ToList();
            return new Namespace(
                toTranslate.Name,
                translatedClasses,
                new[]
                {
                    "System",
                    "CombinatorParsingV3",
                });
        }

        private IEnumerable<Class> Translate(IEnumerable<Class> toTranslate, Namespace rules, Namespace inners)
        {
            foreach (var @class in toTranslate)
            {
                var translatedClasses = Translate(@class, rules, inners);
                foreach (var translatedClass in translatedClasses)
                {
                    yield return translatedClass;
                }
            }
        }

        private IEnumerable<Class> Translate(Class toTranslate, Namespace rules, Namespace inners)
        {
            if (toTranslate.Name.StartsWith("HelperRanged"))
            {
                return Enumerable.Empty<Class>();
            }
            else if (toTranslate.Properties.Where(property => property.Name == "Instance" && property.IsStatic).Any())
            {
                return TranslateTerminal(toTranslate);
            }
            else if (toTranslate.NestedClasses.Where(nestedClass => nestedClass.Name == "Visitor").Any())
            {
                return TranslateDiscriminatedUnion(toTranslate);
            }
            else
            {
                return TranslateInner(toTranslate, rules, inners);
            }
        }

        private string GenerateDeferredFactoryMethodBody(Class toTranslate, Namespace rules, Namespace inners)
        {
            var properties = toTranslate.Properties.ToList();

            if (properties.Count == 0)
            {
                throw new Exception("TODO");
            }

            var builder = new StringBuilder();
            GenerateDeferredFactoryMethodBodyPropertyInitialization(properties[0], "previousNodeRealizationResult", rules, inners, builder);
            builder.AppendLine();
            for (int i = 1; i < properties.Count; ++i)
            {
                GenerateDeferredFactoryMethodBodyPropertyInitialization(
                    properties[i], 
                    $"Future.Create(() => {properties[i - 1].Name}.Value.Realize())", 
                    rules,
                    inners,
                    builder);
                builder.AppendLine();
            }

            builder.Append($"return new {toTranslate.Name}<ParseMode.Deferred>(");
            builder.AppendJoin(", ", properties.Select(property => property.Name));
            builder.Append(");");

            return builder.ToString();
        }

        private void GenerateDeferredFactoryMethodBodyPropertyInitialization(
            PropertyDefinition property,
            string previousNodeRealizationResult,
            Namespace rules,
            Namespace inners,
            StringBuilder builder)
        {
            builder.Append($"var {property.Name} = Future.Create(() => ");

            TranslateType(property.Type, out var translatedType, out var elementType);

            var isDiscriminatedUnion = (elementType != null && IsDiscriminatedUnion(elementType, rules, inners)) || IsDiscriminatedUnion(translatedType, rules, inners);

            string deferredType;
            string realizedType;

            if (isDiscriminatedUnion)
            {
                deferredType = $"{elementType}Deferred";
                realizedType = $"{elementType}Realized";
            }
            else
            {
                deferredType = $"{elementType}<ParseMode.Deferred>";
                realizedType = $"{elementType}<ParseMode.Realized>";
            }

            var atleastone = "CombinatorParsingV3.AtLeastOne<";
            var many = "CombinatorParsingV3.Many<";
            if (translatedType.StartsWith(atleastone)) //// TODO what about other ranges?
            {
                builder.Append($"CombinatorParsingV3.AtLeastOne.Create<{deferredType}, {realizedType}>({previousNodeRealizationResult}, input => {elementType}.Create(input)));");
            }
            else if (translatedType.StartsWith(many))
            {
                builder.Append($"CombinatorParsingV3.Many.Create<{deferredType}, {realizedType}>(input => {elementType}.Create(input), {previousNodeRealizationResult}));");
            }
            else
            {
                builder.Append($"{translatedType}.Create({previousNodeRealizationResult}));");
            }
        }

        private bool IsDiscriminatedUnion(
            string namespaceQualifiedType,
            Namespace rules,
            Namespace inners)
        {
            var lastPeriodIndex = namespaceQualifiedType.LastIndexOf('.');
            if (lastPeriodIndex == -1)
            {
                throw new Exception("TODO");
            }

            var @namespace = namespaceQualifiedType.Substring(0, lastPeriodIndex);
            var type = namespaceQualifiedType.Substring(lastPeriodIndex + 1);

            Func<Class, bool> predicate = @class => string.Equals(type, @class.Name, StringComparison.Ordinal) && @class.NestedClasses.Where(nestedClass => string.Equals(nestedClass.Name, "Visitor", StringComparison.Ordinal)).Any();
            if (string.Equals(@namespace, rules.Name, StringComparison.Ordinal))
            {
                if (rules.Classes.Where(predicate).Any())
                {
                    return true;
                }
            }

            if (string.Equals(@namespace, inners.Name, StringComparison.Ordinal))
            {
                if (inners.Classes.Where(predicate).Any())
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<Class> TranslateInner(Class toTranslate, Namespace rules, Namespace inners)
        {
            // the factory methods for the cst node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Static,
                toTranslate.Name,
                Enumerable.Empty<string>(),
                null,
                Enumerable.Empty<ConstructorDefinition>(),
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Create",
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
$$"""
return {{toTranslate.Name}}<ParseMode.Deferred>.Create(previousNodeRealizationResult);
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                Enumerable.Empty<PropertyDefinition>());

            // the cst node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Sealed,
                toTranslate.Name,
                new[]
                {
                    "TMode",
                },
                $"IAstNode<char, {toTranslate.Name}<ParseMode.Realized>>, IFromRealizedable<{toTranslate.Name}<ParseMode.Deferred>> where TMode : ParseMode", //// TODO generic type constraints should be built into `class`
                new[]
                {
                    new ConstructorDefinition(
                        AccessModifier.Private,
                        toTranslate
                            .Properties
                            .Select(
                                property =>
                                    new MethodParameter(
                                        $"IFuture<{TranslateType(property.Type)}>",
                                        property.Name)),
                        toTranslate
                            .Properties
                            .Select(
                                property =>
                                    $"this._{property.Name} = {property.Name};")
                            .Append(
                                $"this.realizationResult = new Future<IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>>(this.RealizeImpl);")),
                    new ConstructorDefinition(
                        AccessModifier.Private,
                        toTranslate
                            .Properties
                            .Select(
                                property =>
                                    new MethodParameter(
                                        TranslateType(property.Type),
                                        property.Name))
                            .Append(
                                new MethodParameter(
                                    $"IFuture<IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>>",
                                    "realizationResult")),
                        toTranslate
                            .Properties
                            .Select(
                                property =>
                                    $"this._{property.Name} = Future.Create(() => {property.Name});")
                            .Append(
                                "this.realizationResult = realizationResult;")),
                },
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Create",
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
                        GenerateDeferredFactoryMethodBody(toTranslate, rules, inners)),
                    new MethodDefinition(
                        AccessModifier.Public, 
                        ClassModifier.None, 
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Convert",
                        Enumerable.Empty<MethodParameter>(),
$$"""
if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new {{toTranslate.Name}}<ParseMode.Deferred>(
        {{
            string.Join(
                "," + Environment.NewLine, 
                toTranslate
                    .Properties
                    .Select(
                        property => $"this._{property.Name}.Select(_ => _.Convert())"))
        }});
}
else
{
    return new {{toTranslate.Name}}<ParseMode.Deferred>(
        {{
            string.Join(
                "," + Environment.NewLine, 
                toTranslate
                    .Properties
                    .Select(
                        property => $"this._{property.Name}.Value.Convert()"))
        }},
        this.realizationResult);
}
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>",
                        Enumerable.Empty<string>(),
                        "Realize",
                        Enumerable.Empty<MethodParameter>(),
"""
return this.realizationResult.Value;
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Private,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>",
                        Enumerable.Empty<string>(),
                        "RealizeImpl",
                        Enumerable.Empty<MethodParameter>(),
$$"""
var output = this.{{toTranslate.Properties.Last().Name}}.Realize();
if (output.Success)
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(
        true,
        new {{toTranslate.Name}}<ParseMode.Realized>(
            {{
                string.Join(
                    "," + Environment.NewLine,
                    toTranslate
                        .Properties
                        .Select(
                            property =>
                                $"this.{property.Name}.Realize().RealizedValue"))
            }},
            this.realizationResult), 
        output.RemainingTokens);
}
else
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(false, default, output.RemainingTokens);
}
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                toTranslate
                    .Properties
                    .Select(
                        //// TODO these should be fields
                        property =>
                            new PropertyDefinition(
                                AccessModifier.Private,
                                false,
                                $"IFuture<{TranslateType(property.Type)}>",
                                $"_{property.Name}",
                                true,
                                false,
                                null))
                    .Append(
                        //// TODO this should be a field
                        new PropertyDefinition(
                            AccessModifier.Private,
                            false,
                            $"IFuture<IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>>",
                            "realizationResult",
                            true,
                            false,
                            null))
                    .Concat(
                        toTranslate
                            .Properties
                            .Select(
                                property =>
                                    new PropertyDefinition(
                                        AccessModifier.Public,
                                        false,
                                        TranslateType(property.Type),
                                        property.Name,
                                        true, //// TODO need a way to define this getter body
                                        false,
                                        null)))
                );
        }

        private void TranslateType(string toTranslate, out string translated, out string? elementType)
        {
            var ienumerable = "System.Collections.Generic.IEnumerable<";
            if (toTranslate.StartsWith(ienumerable))
            {
                elementType = toTranslate.Substring(ienumerable.Length);
                elementType = elementType.Substring(0, elementType.Length - 1);
                translated = $"CombinatorParsingV3.Many<{elementType}<ParseMode.Deferred>, {elementType}<ParseMode.Realized>, TMode>";
                return;
            }

            var atleastone = "__GeneratedPartialV1.Deferred.CstNodes.Inners.HelperRangedAtLeast1<"; //// TODO what about handling other range sizes?
            if (toTranslate.StartsWith(atleastone))
            {
                elementType = toTranslate.Substring(atleastone.Length);
                elementType = elementType.Substring(0, elementType.Length - 1);
                translated = $"CombinatorParsingV3.AtLeastOne<{elementType}<ParseMode.Deferred>, {elementType}<ParseMode.Realized>, TMode>";
                return;
            }

            translated = $"{toTranslate}<TMode>";
            elementType = null;
            return;
        }

        private string TranslateType(string toTranslate)
        {
            TranslateType(toTranslate, out var translated, out _);

            return translated;
        }

        private IEnumerable<Class> TranslateDiscriminatedUnion(Class toTranslate)
        {
            //// TODO actually implement this
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Sealed,
                toTranslate.Name,
                new[]
                {
                    "TMode",
                },
                $"IAstNode<char, {toTranslate.Name}<ParseMode.Realized>>, IFromRealizedable<{toTranslate.Name}<ParseMode.Deferred>> where TMode : ParseMode", //// TODO generic type constraints should be built into `class`
                Enumerable.Empty<ConstructorDefinition>(),
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Convert",
                        Enumerable.Empty<MethodParameter>(),
$$"""
throw new System.Exception("TODO");
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>",
                        Enumerable.Empty<string>(),
                        "Realize",
                        Enumerable.Empty<MethodParameter>(),
$$"""
throw new System.Exception("TODO");
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                Enumerable.Empty<PropertyDefinition>());


            var realizedTypeName = $"{toTranslate.Name}Realized";
            var deferredTypeName = $"{toTranslate.Name}Deferred";

            // the factory methods for the cst node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Static,
                toTranslate.Name,
                Enumerable.Empty<string>(),
                null,
                Enumerable.Empty<ConstructorDefinition>(),
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        false,
                        deferredTypeName,
                        Enumerable.Empty<string>(),
                        "Create",
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
$$"""
return new {{deferredTypeName}}(previousNodeRealizationResult);
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                Enumerable.Empty<PropertyDefinition>());

            // the deferred node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Sealed,
                deferredTypeName,
                Enumerable.Empty<string>(),
                $"IAstNode<char, {realizedTypeName}>",
                new[]
                {
                    new ConstructorDefinition(
                        AccessModifier.Public,
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
"""
//// TODO get constructor accessibility correct
this.previousNodeRealizationResult = previousNodeRealizationResult;

this.realizationResult = Future.Create(() => this.RealizeImpl());
""".Split(Environment.NewLine)
                        ),
                    new ConstructorDefinition(
                        AccessModifier.Public,
                        new[]
                        {
                            new MethodParameter(
                                $"IFuture<IRealizationResult<char, {realizedTypeName}>>",
                                "realizationResult"),
                        },
"""
this.realizationResult = realizationResult;
""".Split(Environment.NewLine)
                        ),
                },
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {realizedTypeName}>",
                        Enumerable.Empty<string>(),
                        "Realize",
                        Enumerable.Empty<MethodParameter>(),
"""
return this.realizationResult.Value;
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Private,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {realizedTypeName}>",
                        Enumerable.Empty<string>(),
                        "RealizeImpl",
                        Enumerable.Empty<MethodParameter>(),
                        string.Concat(
$$"""
if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, {{realizedTypeName}}>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}

""",
                            string.Join(
                                Environment.NewLine,
                                toTranslate
                                    .NestedClasses
                                    .Where(nestedClass => !string.Equals(nestedClass.Name, "Visitor"))
                                    .Select(
                                        nestedClass =>
$$"""
var {{nestedClass.Name}} = {{realizedTypeName}}.{{nestedClass.Name}}.Create(this.previousNodeRealizationResult);
if ({{nestedClass.Name}}.Success)
{
    return {{nestedClass.Name}};
}

"""
                                    )),
$$"""
return new RealizationResult<char, {{realizedTypeName}}>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
"""
                            )),
                },
                Enumerable.Empty<Class>(),
                new[]
                {
                    new PropertyDefinition(
                        AccessModifier.Private,
                        false,
                        "IFuture<IRealizationResult<char>>",
                        "previousNodeRealizationResult",
                        true,
                        false,
                        null),
                    new PropertyDefinition(
                        AccessModifier.Private,
                        false,
                        $"IFuture<IRealizationResult<char, {realizedTypeName}>>",
                        "realizationResult",
                        true,
                        false,
                        null),
                });

            // the realized node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Abstract,
                realizedTypeName,
                Enumerable.Empty<string>(),
                $"IFromRealizedable<{deferredTypeName}>",
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
                        ClassModifier.Abstract, 
                        false,
                        deferredTypeName,
                        Enumerable.Empty<string>(),
                        "Convert",
                        Enumerable.Empty<MethodParameter>(),
                        null),
                    new MethodDefinition(
                        AccessModifier.Protected,
                        ClassModifier.Abstract,
                        false,
                        "TResult",
                        new[]
                        {
                            "TResult",
                            "TContext",
                        },
                        "Dispatch",
                        new[]
                        {
                            new MethodParameter(
                                "Visitor<TResult, TContext>",
                                "visitor"),
                            new MethodParameter(
                                "TContext",
                                "context"),
                        },
                        null),
                },
                toTranslate
                    .NestedClasses
                    .Where(nestedClass => !string.Equals(nestedClass.Name, "Visitor"))
                    .Select(
                        nestedClass =>
                            new Class(
                                AccessModifier.Public,
                                ClassModifier.Sealed,
                                nestedClass.Name,
                                Enumerable.Empty<string>(),
                                realizedTypeName,
                                new[]
                                {
                                    new ConstructorDefinition(
                                        AccessModifier.Private,
                                        new[]
                                        {
                                            new MethodParameter(
                                                "ITokenStream<char>?",
                                                "nextTokens"),
                                        },
                                        new[]
                                        {
                                            $"this.RealizationResult = new RealizationResult<char, {realizedTypeName}.{nestedClass.Name}>(true, this, nextTokens);",
                                        }),
                                },
                                new[]
                                {
                                    new MethodDefinition(
                                        AccessModifier.Public,
                                        ClassModifier.Static, 
                                        false,
                                        $"IRealizationResult<char, {realizedTypeName}.{nestedClass.Name}>",
                                        Enumerable.Empty<string>(),
                                        "Create",
                                        new[]
                                        {
                                            new MethodParameter(
                                                "IFuture<IRealizationResult<char>>",
                                                "previousNodeRealizationResult"),
                                        },
$$"""
var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, {{realizedTypeName}}.{{nestedClass.Name}}>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, {{realizedTypeName}}.{{nestedClass.Name}}>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new {{realizedTypeName}}.{{nestedClass.Name}}(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, {{realizedTypeName}}.{{nestedClass.Name}}>(false, default, input);
}
"""
                                        ),
                                    new MethodDefinition(
                                        AccessModifier.Public, 
                                        ClassModifier.None,
                                        true,
                                        deferredTypeName,
                                        Enumerable.Empty<string>(),
                                        "Convert",
                                        Enumerable.Empty<MethodParameter>(),
$$"""
return new {{deferredTypeName}}(Future.Create(() => this.RealizationResult));
"""
                                        ),
                                    new MethodDefinition(
                                        AccessModifier.Protected,
                                        ClassModifier.None,
                                        true,
                                        "TResult",
                                        new[]
                                        {
                                            "TResult",
                                            "TContext",
                                        },
                                        "Dispatch",
                                        new[]
                                        {
                                            new MethodParameter(
                                                "Visitor<TResult, TContext>",
                                                "visitor"),
                                            new MethodParameter(
                                                "TContext",
                                                "context"),
                                        },
                                        "return visitor.Accept(this, context);"),
                                },
                                Enumerable.Empty<Class>(),
                                nestedClass
                                    .Properties
                                    .Select(
                                        property =>
                                            new PropertyDefinition(
                                                AccessModifier.Public,
                                                false,
                                                $"{property.Type}<ParseMode.Realized>",
                                                property.Name,
                                                true,
                                                false,
                                                null))
                                    .Prepend(
                                        new PropertyDefinition(
                                            AccessModifier.Private,
                                            false,
                                            $"IRealizationResult<char, {realizedTypeName}.{nestedClass.Name}>",
                                            "RealizationResult",
                                            true,
                                            false,
                                            null))))
                    .Prepend(
                        new Class(
                            AccessModifier.Public,
                            ClassModifier.Abstract,
                            "Visitor",
                            new[]
                            {
                                "TResult",
                                "TContext",
                            },
                            null,
                            Enumerable.Empty<ConstructorDefinition>(),
                            toTranslate
                                .NestedClasses
                                .Where(nestedClass => !string.Equals(nestedClass.Name, "Visitor"))
                                .Select(
                                    nestedClass =>
                                        new MethodDefinition(
                                            AccessModifier.Protected | AccessModifier.Internal,
                                            ClassModifier.Abstract,
                                            false,
                                            "TResult",
                                            Enumerable.Empty<string>(),
                                            "Accept",
                                            new[]
                                            {
                                                new MethodParameter(
                                                    nestedClass.Name,
                                                    "node"),
                                                new MethodParameter(
                                                    "TContext",
                                                    "context"),
                                            },
                                            null))
                                .Prepend(
                                    new MethodDefinition(
                                        AccessModifier.Public, 
                                        ClassModifier.None, 
                                        false, 
                                        "TResult",
                                        Enumerable.Empty<string>(),
                                        "Visit",
                                        new[]
                                        {
                                            new MethodParameter(
                                                realizedTypeName,
                                                "node"),
                                            new MethodParameter(
                                                "TContext",
                                                "context"),
                                        },
                                        "return node.Dispatch(this, context);")),
                            Enumerable.Empty<Class>(),
                            Enumerable.Empty<PropertyDefinition>())),
                Enumerable.Empty<PropertyDefinition>());
        }

        private IEnumerable<Class> TranslateTerminal(Class toTranslate)
        {
            char parsedCharacter;
            if (toTranslate.Name.Length == 4)
            {
                var ascii = Convert.ToInt32(toTranslate.Name.Substring(2, 2), 16); //// TODO this throws

                parsedCharacter = (char)ascii;
            }
            else
            {
                //// TODO what to do in these cases?
                yield break;
            }

            // the factory methods for the cst node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Static,
                toTranslate.Name,
                Enumerable.Empty<string>(),
                null,
                Enumerable.Empty<ConstructorDefinition>(),
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Create",
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
$$"""
return {{toTranslate.Name}}<ParseMode.Deferred>.Create(previousNodeRealizationResult);
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                Enumerable.Empty<PropertyDefinition>());

            // the cst node
            yield return new Class(
                AccessModifier.Public,
                ClassModifier.Sealed,
                toTranslate.Name,
                new[]
                {
                    "TMode",
                },
                $"IAstNode<char, {toTranslate.Name}<ParseMode.Realized>>, IFromRealizedable<{toTranslate.Name}<ParseMode.Deferred>> where TMode : ParseMode", //// TODO generic type constraints should be built into `class`
                new[]
                {
                    new ConstructorDefinition(
                        AccessModifier.Private,
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
$$"""
if (typeof(TMode) != typeof(ParseMode.Deferred))
{
    throw new ArgumentException("TODO");
}

this.previousNodeRealizationResult = previousNodeRealizationResult;

this.realizationResult = new Future<IRealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>>(() => this.RealizeImpl());
""".Split(Environment.NewLine)),
                    new ConstructorDefinition(
                        AccessModifier.Private,
                        new[]
                        {
                            new MethodParameter(
                                $"IFuture<IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>>",
                                "realizationResult"),
                        },
"""
if (typeof(TMode) != typeof(ParseMode.Realized))
{
    throw new ArgumentException("TODO");
}

this.realizationResult = realizationResult;
""".Split(Environment.NewLine)),
                },
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Internal,
                        ClassModifier.Static,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Create",
                        new[]
                        {
                            new MethodParameter(
                                "IFuture<IRealizationResult<char>>",
                                "previousNodeRealizationResult"),
                        },
$"""         
return new {toTranslate.Name}<ParseMode.Deferred>(previousNodeRealizationResult);
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"{toTranslate.Name}<ParseMode.Deferred>",
                        Enumerable.Empty<string>(),
                        "Convert",
                        Enumerable.Empty<MethodParameter>(),
$$"""
if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new {{toTranslate.Name}}<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new {{toTranslate.Name}}<ParseMode.Deferred>(this.realizationResult);
}
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Public,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>",
                        Enumerable.Empty<string>(),
                        "Realize",
                        Enumerable.Empty<MethodParameter>(),
"""
return realizationResult.Value;
"""
                        ),
                    new MethodDefinition(
                        AccessModifier.Private,
                        ClassModifier.None,
                        false,
                        $"IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>",
                        Enumerable.Empty<string>(),
                        "RealizeImpl",
                        Enumerable.Empty<MethodParameter>(),
$$"""
var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '{{parsedCharacter}}')
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(
        true,
        new {{toTranslate.Name}}<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, {{toTranslate.Name}}<ParseMode.Realized>>(false, default, input);
}
"""
                        ),
                },
                Enumerable.Empty<Class>(),
                new[]
                {
                    new PropertyDefinition( //// TODO this should be a field, not a property
                        AccessModifier.Private,
                        false,
                        "IFuture<IRealizationResult<char>>",
                        "previousNodeRealizationResult",
                        true,
                        false,
                        null),
                    new PropertyDefinition( //// TODO this should be a field, not a property
                        AccessModifier.Private,
                        false,
                        $"IFuture<IRealizationResult<char, {toTranslate.Name}<ParseMode.Realized>>>",
                        "realizationResult",
                        true,
                        false,
                        null),
                });
        }
    }
}
