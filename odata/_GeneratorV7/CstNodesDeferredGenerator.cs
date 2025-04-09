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
            if (toTranslate.Properties.Where(property => property.Name == "Instance" && property.IsStatic).Any()) //// TODO should be single
            {
                return TranslateTerminal(toTranslate);
            }
            else if (toTranslate.NestedClasses.Where(nestedClass => nestedClass.Name == "Visitor").Any()) //// TODO should be single
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
                return $"CombinatorParsingV3.AtLeastOne<{elementType}<ParseMode.Deferred>, {elementType}<ParseMode.Realized>, TMode";
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
