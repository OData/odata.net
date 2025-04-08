namespace _GeneratorV7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AbnfParserGenerator;

    public sealed class CstNodesDeferredGenerator
    {
        public CstNodesDeferredGenerator()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) CreateDeferred(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var ruleCstNodes =
                new Namespace(
                    cstNodes.RuleCstNodes.Name,
                    cstNodes.RuleCstNodes.Classes.ToList(),
                    new[]
                    {
                        "System",
                        "CombinatorParsingV3",
                    });
            var innerCstNodes =
                new Namespace(
                    cstNodes.InnerCstNodes.Name,
                    cstNodes.InnerCstNodes.Classes.ToList(),
                    new[]
                    {
                        "System",
                        "CombinatorParsingV3",
                    });

            Translate(ruleCstNodes);
            Translate(innerCstNodes);

            return
                (
                    ruleCstNodes,
                    innerCstNodes
                );
        }

        private Namespace Translate(Namespace toTranslate)
        {
            return new Namespace(
                toTranslate.Name,
                Translate(toTranslate.Classes).ToList(),
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
                var translated = Translate(@class);
                foreach (var element in translated)
                {
                    yield return element;
                }
            }
        }

        private IEnumerable<Class> Translate(Class toTranslate)
        {
            //// TODO you're not generating any of the factory classes
            if (toTranslate.Properties.Where(property => property.Name == "Instance" && property.IsStatic).Any())
            {
                char parsedCharacter;
                if (toTranslate.Name.Length == 4)
                {
                    var ascii = Convert.ToInt32(toTranslate.Name.Substring(2, 2), 16); //// TODO this throws

                    parsedCharacter = (char)ascii;
                }
                else
                {
                    yield break;
                }

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
if (typeof(TMode) != typeof(ParseMode.Deferred))
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
}
