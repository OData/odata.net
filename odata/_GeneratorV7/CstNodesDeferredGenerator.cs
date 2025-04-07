using AbnfParserGenerator;
using System;
using System.Linq;

namespace _GeneratorV7
{
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

        private void Translate(Namespace toTranslate)
        {
            for (int i = 0; i < toTranslate.Classes.Count; ++i)
            {
                var cstNode = toTranslate.Classes[i];
                var translated = Translate(cstNode);
                if (translated == null)
                {
                    toTranslate.Classes.RemoveAt(i);
                    --i;
                    continue;
                    //// throw new System.Exception("TODO");
                }

                if (translated != null)
                {
                    toTranslate.Classes[i] = translated;
                }
            }
        }

        private Class? Translate(Class @class)
        {
            if (@class.Properties.Where(property => property.Name == "Instance" && property.IsStatic).Any())
            {
                return new Class(
                    AccessModifier.Public,
                    ClassModifier.Sealed,
                    @class.Name,
                    new[]
                    {
                        "TMode",
                    },
                    $"IAstNode<char, {@class.Name}<ParseMode.Realized>>, IFromRealizedable<{@class.Name}<ParseMode.Deferred>> where TMode : ParseMode", //// TODO generic type constraints should be built into `class`
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

this.realizationResult = new Future<IRealizationResult<char, {{@class.Name}}<ParseMode.Realized>>>(() => this.RealizeImpl());
""".Split(Environment.NewLine)),
                        new ConstructorDefinition(
                            AccessModifier.Private,
                            new[]
                            {
                                new MethodParameter(
                                    $"IFuture<IRealizationResult<char, {@class.Name}<ParseMode.Realized>>>",
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
                            $"{@class.Name}<ParseMode.Deferred>",
                            Enumerable.Empty<string>(),
                            "Create",
                            new[]
                            {
                                new MethodParameter(
                                    "IFuture<IRealizationResult<char>>",
                                    "previousNodeRealizationResult"),
                            },
$"""         
return new {@class.Name}<ParseMode.Deferred>(previousNodeRealizationResult);
"""
                        ),
                        new MethodDefinition(
                            AccessModifier.Public,
                            ClassModifier.None,
                            false,
                            $"{@class.Name}<ParseMode.Deferred>",
                            Enumerable.Empty<string>(),
                            "Convert",
                            Enumerable.Empty<MethodParameter>(),
$$"""
if (typeof(TMode) == typeof(ParseMode.Deferred))
{
    return new {{@class.Name}}<ParseMode.Deferred>(this.previousNodeRealizationResult);
}
else
{
    return new {{@class.Name}}<ParseMode.Deferred>(this.realizationResult);
}
"""
                            ),
                        new MethodDefinition(
                            AccessModifier.Public,
                            ClassModifier.None,
                            false,
                            $"IRealizationResult<char, {@class.Name}<ParseMode.Realized>>",
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
                            $"IRealizationResult<char, {@class.Name}<ParseMode.Realized>>",
                            Enumerable.Empty<string>(),
                            "RealizeImpl",
                            Enumerable.Empty<MethodParameter>(),
$$"""
var output = this.previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, {{@class.Name}}<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    return new RealizationResult<char, {{@class.Name}}<ParseMode.Realized>>(false, default, output.RemainingTokens);
}

if (input.Current == '/') //// TODO
{
    return new RealizationResult<char, {{@class.Name}}<ParseMode.Realized>>(
        true,
        new {{@class.Name}}<ParseMode.Realized>(this.realizationResult),
        input.Next());
}
else
{
    return new RealizationResult<char, {{@class.Name}}<ParseMode.Realized>>(false, default, input);
}
"""
                            ),
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
                            $"IFuture<IRealizationResult<char, {@class.Name}<ParseMode.Realized>>>",
                            "realizationResult",
                            true,
                            false,
                            null),
                    });
            }

            return null;
        }
    }
}
