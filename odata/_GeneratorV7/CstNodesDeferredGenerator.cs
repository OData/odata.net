namespace _GeneratorV7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using AbnfParserGenerator;

    public sealed class CstNodesDeferredGenerator
    {
        public CstNodesDeferredGenerator()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) CreateDeferred(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var translatedRules = Translate(cstNodes.RuleCstNodes);
            var translatedInners = Translate(cstNodes.InnerCstNodes);

            return
                (
                    translatedRules,
                    translatedInners
                );
        }

        private Namespace Translate(Namespace toTranslate)
        {
            var translatedClasses = Translate(toTranslate.Classes).ToList();
            return new Namespace(
                toTranslate.Name,
                translatedClasses,
                new[]
                {
                    "System",
                    "CombinatorParsingV3",
                });
        }

        private IEnumerable<Class> Translate(IEnumerable<Class> toTranslate)
        {
            foreach (var @class in toTranslate)
            {
                var translatedClasses = Translate(@class);
                foreach (var translatedClass in translatedClasses)
                {
                    yield return translatedClass;
                }
            }
        }

        private IEnumerable<Class> Translate(Class toTranslate)
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
                return TranslateInner(toTranslate);
            }
        }

        private IEnumerable<Class> TranslateInner(Class toTranslate)
        {
            //// TODO factory class

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
                        string.Join(
                            Environment.NewLine,
                            toTranslate
                                .Properties
                                .Select(
                                    property =>
                                    {
                                        return string.Empty; //// TODO implement this
                                    })
                                .Append(
                                    string.Concat(
                                        $"return new {toTranslate.Name}<ParseMode.Deferred>(",
                                        string.Join(
                                            ", ",
                                            toTranslate
                                                .Properties
                                                .Select(property => property.Name)),
                                        ");")))),
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

        private string TranslateType(string toTranslate)
        {
            var ienumerable = "System.Collections.Generic.IEnumerable<";
            if (toTranslate.StartsWith(ienumerable))
            {
                var elementType = toTranslate.Substring(ienumerable.Length);
                elementType = elementType.Substring(0, elementType.Length - 1);
                return $"CombinatorParsingV3.Many<{elementType}<ParseMode.Deferred>, {elementType}<ParseMode.Realized>, TMode>";
            }

            var atleastone = "__GeneratedPartialV1.Deferred.CstNodes.Inners.HelperRangedAtLeast1<"; //// TODO what about handling other range sizes?
            if (toTranslate.StartsWith(atleastone))
            {
                var elementType = toTranslate.Substring(atleastone.Length);
                elementType = elementType.Substring(0, elementType.Length - 1);
                return $"CombinatorParsingV3.AtLeastOne<{elementType}<ParseMode.Deferred>, {elementType}<ParseMode.Realized>, TMode>";
            }

            return $"{toTranslate}<TMode>";
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
