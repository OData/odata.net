namespace GeneratorV3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using AbnfParser.CstNodes;
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator;
    using Root;

    public static class NotNullExtension
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
        {
            foreach (var element in source)
            {
                if (element != null)
                {
                    yield return element;
                }
            }
        }
    }

    public sealed class Generator
    {
        private Generator()
        {
        }

        public static Generator Intance { get; } = new Generator();

        private static string Namespace = "GeneratorV3"; //// TODO parameterize this

        private static string InnersClassName = "Inners"; //// TODO parameterize this

        private static class CharacterSubstituions
        {
            public static char Dash { get; } = 'ⲻ'; //// TODO parameterize these

            public static string OpenParenthesis { get; } = "open"; //// TODO 'Ⲥ'; 

            public static char CloseParenthesis { get; } = 'Ↄ';

            public static char OpenBracket { get; } = '꘡';

            public static char CloseBracket { get; } = '꘡';

            public static string Asterisk { get; } = "asterisk"; //// TODO 'ж';

            public static char Slash { get; } = 'Ⳇ';

            public static char Space { get; } = '_';

            public static string DoubleQuote { get; } = "doublequote"; //// TODO

            public static string Period { get; } = "period"; //// TODO

            public static string Percent { get; } = "percent"; //// TODO
        }

        public IEnumerable<Class> Generate(RuleList ruleList, Root.Void context)
        {
            var innerClasses = new Dictionary<string, Class>();
            return ruleList
                .Inners
                .Select(
                    inner => RuleListInnerGenerator
                        .Instance
                        .Visit(inner, (innerClasses, context)))
                .NotNull()
                .Append(
                    new Class(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        InnersClassName, //// TODO how to make sure this doesn't conflict?
                        Enumerable.Empty<string>(),
                        null,
                        Enumerable.Empty<ConstructorDefinition>(),
                        Enumerable.Empty<MethodDefinition>(),
                        innerClasses.Values,
                        Enumerable.Empty<PropertyDefinition>()));
        }

        private sealed class RuleListInnerGenerator : RuleList.Inner.Visitor<Class?, (Dictionary<string, Class> InnerClasses, Root.Void @void)>
        {
            private RuleListInnerGenerator()
            {
            }

            public static RuleListInnerGenerator Instance { get; } = new RuleListInnerGenerator();

            protected internal override Class? Accept(RuleList.Inner.RuleInner node, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
            {
                return RuleGenerator.Instance.Generate(node.Rule, context);
            }

            private sealed class RuleGenerator
            {
                private RuleGenerator()
                {
                }

                public static RuleGenerator Instance { get; } = new RuleGenerator();

                public Class Generate(Rule rule, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
                {
                    var className = RuleNameToClassName.Instance.Generate(rule.RuleName, context.@void);
                    return ElementsGenerator.Instance.Generate(rule.Elements, (className, context.InnerClasses));
                }

                private sealed class ElementsGenerator
                {
                    private ElementsGenerator()
                    {
                    }

                    public static ElementsGenerator Instance { get; } = new ElementsGenerator();

                    public Class Generate(Elements elements, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                    {
                        return AlternationGenerator.Instance.Generate(elements.Alternation, context);
                    }

                    private sealed class AlternationGenerator
                    {
                        private AlternationGenerator()
                        {
                        }

                        public static AlternationGenerator Instance { get; } = new AlternationGenerator();

                        public Class Generate(Alternation alternation, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                        {
                            if (alternation.Inners.Any())
                            {
                                return ConcatenationsToDiscriminatedUnion
                                    .Instance
                                    .Generate(
                                        alternation
                                            .Inners
                                            .Select(
                                                inner => inner.Concatenation)
                                            .Prepend(alternation.Concatenation),
                                        context);
                            }
                            else
                            {
                                return ConcatenationToClass.Instance.Generate(alternation.Concatenation, (context.ClassName, null, Enumerable.Empty<MethodDefinition>(), context.InnerClasses));
                            }
                        }

                        private sealed class ConcatenationToClass
                        {
                            private ConcatenationToClass()
                            {
                            }

                            public static ConcatenationToClass Instance { get; } = new ConcatenationToClass();

                            public Class Generate(
                                Concatenation concatenation, 
                                (string ClassName, string? BaseType, IEnumerable<MethodDefinition> MethodDefinitions, Dictionary<string, Class> InnerClasses) context)
                            {
                                var propertyTypeToCount = new Dictionary<string, int>();
                                var properties = concatenation
                                    .Inners
                                    .Select(
                                        inner => inner.Repetition)
                                    .Prepend(
                                        concatenation.Repetition)
                                    .Select(repetition => RepetitonToPropertyDefinition
                                        .Instance
                                        .Visit(
                                            repetition, 
                                            (propertyTypeToCount, context.InnerClasses)))
                                    .ToList();
                                return new Class(
                                    AccessModifier.Public,
                                    ClassModifier.Sealed,
                                    context.ClassName,
                                    Enumerable.Empty<string>(),
                                    context.BaseType,
                                    new[]
                                    {
                                        new ConstructorDefinition(
                                            AccessModifier.Public,
                                            properties
                                                .Select(property =>
                                                    new MethodParameter(property.Type, property.Name)),
                                            properties
                                                .Select(property =>
                                                    $"this.{property.Name} = {property.Name};")),
                                    },
                                    context.MethodDefinitions,
                                    Enumerable.Empty<Class>(),
                                    properties);
                            }

                            private sealed class RepetitonToPropertyDefinition : Repetition.Visitor<PropertyDefinition, (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses)>
                            {
                                private RepetitonToPropertyDefinition()
                                {
                                }

                                public static RepetitonToPropertyDefinition Instance { get; } = new RepetitonToPropertyDefinition();

                                protected internal override PropertyDefinition Accept(
                                    Repetition.ElementOnly node, 
                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                {
                                    return ElementToPropertyDefinition
                                        .Instance
                                        .Visit(
                                            node.Element, 
                                            (false, context.PropertyTypeToCount, context.InnerClasses));
                                }

                                protected internal override PropertyDefinition Accept(
                                    Repetition.RepeatAndElement node,
                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                {
                                    return ElementToPropertyDefinition
                                        .Instance
                                        .Visit(
                                            node.Element,
                                            (true, context.PropertyTypeToCount, context.InnerClasses));
                                }

                                private sealed class ElementToPropertyDefinition : Element.Visitor<PropertyDefinition, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses)>
                                {
                                    private ElementToPropertyDefinition()
                                    {
                                    }

                                    public static ElementToPropertyDefinition Instance { get; } = new ElementToPropertyDefinition();

                                    protected internal override PropertyDefinition Accept(
                                        Element.RuleName node, 
                                        (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var ruleName = RuleNameToClassName
                                            .Instance
                                            .Generate(
                                                node.Value,
                                                default);
                                        var propertyType = $"{Namespace}.{ruleName}";
                                        if (context.IsCollection)
                                        {
                                            propertyType = $"IEnumerable<{propertyType}>";
                                        }

                                        if (!context.PropertyTypeToCount.TryGetValue(ruleName, out var count))
                                        {
                                            count = 0;
                                        }

                                        ++count;
                                        context.PropertyTypeToCount[ruleName] = count;

                                        var propertyName = $"{ruleName}_{count}";

                                        return new PropertyDefinition(
                                            AccessModifier.Public,
                                            propertyType,
                                            propertyName,
                                            true,
                                            false);
                                    }

                                    private static bool IsOnlyRuleName(Alternation alternation)
                                    {
                                        return !alternation.Inners.Any() &&
                                            !alternation.Concatenation.Inners.Any() &&
                                            alternation.Concatenation.Repetition is Repetition.ElementOnly elementOnly &&
                                            elementOnly.Element is Element.RuleName;
                                    }

                                    protected internal override PropertyDefinition Accept(
                                        Element.Group node, 
                                        (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var isOnlyRuleName = IsOnlyRuleName(node.Value.Alternation);
                                        
                                        var groupInnerClassName = AlternationToClassName.Instance.Generate(node.Value.Alternation);
                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(groupInnerClassName))
                                        {
                                            context.InnerClasses[groupInnerClassName] = AlternationGenerator.Instance.Generate(node.Value.Alternation, (groupInnerClassName, context.InnerClasses));
                                        }

                                        var groupClassName = GroupToClassName.Instance.Generate(node.Value);

                                        if (!context.InnerClasses.ContainsKey(groupClassName))
                                        {
                                            var groupClass = new Class(
                                                AccessModifier.Public,
                                                ClassModifier.Sealed,
                                                groupClassName,
                                                Enumerable.Empty<string>(),
                                                null,
                                                new[]
                                                {
                                                    new ConstructorDefinition(
                                                        AccessModifier.Public,
                                                        new[]
                                                        {
                                                            new MethodParameter($"{(isOnlyRuleName ? Namespace : InnersClassName)}.{groupInnerClassName}", $"{groupInnerClassName}_1"),
                                                        },
                                                        new[]
                                                        {
                                                            $"this.{groupInnerClassName}_1 = {groupInnerClassName}_1;",
                                                        }),
                                                },
                                                Enumerable.Empty<MethodDefinition>(),
                                                Enumerable.Empty<Class>(),
                                                new[]
                                                {
                                                    new PropertyDefinition(
                                                        AccessModifier.Public,
                                                        $"{(isOnlyRuleName ? Namespace : InnersClassName)}.{groupInnerClassName}",
                                                        $"{groupInnerClassName}_1",
                                                        true,
                                                        false),
                                                });

                                            context.InnerClasses[groupClassName] = groupClass;
                                        }

                                        var propertyType = $"{InnersClassName}.{groupClassName}";
                                        if (context.IsCollection)
                                        {
                                            propertyType = $"IEnumerable<{propertyType}>";
                                        }

                                        if (!context.PropertyTypeToCount.TryGetValue(groupClassName, out var count))
                                        {
                                            count = 0;
                                        }

                                        ++count;
                                        context.PropertyTypeToCount[groupClassName] = count;

                                        var propertyName = $"{groupClassName}_{count}";

                                        return new PropertyDefinition(
                                            AccessModifier.Public,
                                            propertyType,
                                            propertyName,
                                            true,
                                            false);
                                    }

                                    protected internal override PropertyDefinition Accept(Element.Option node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var isOnlyRuleName = IsOnlyRuleName(node.Value.Alternation);
                                        var innerClassName = AlternationToClassName.Instance.Generate(node.Value.Alternation);

                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            context.InnerClasses[innerClassName] = AlternationGenerator
                                                .Instance
                                                .Generate(
                                                    node.Value.Alternation,
                                                    (innerClassName, context.InnerClasses));
                                        }

                                        var propertyType = $"{(isOnlyRuleName ? Namespace : InnersClassName)}.{innerClassName}?";
                                        if (context.IsCollection)
                                        {
                                            propertyType = $"IEnumerable<{propertyType}>";
                                        }

                                        if (!context.PropertyTypeToCount.TryGetValue(innerClassName, out var count))
                                        {
                                            count = 0;
                                        }

                                        ++count;
                                        context.PropertyTypeToCount[innerClassName] = count;

                                        var propertyName = $"{innerClassName}_{count}";

                                        return new PropertyDefinition(
                                            AccessModifier.Public,
                                            propertyType,
                                            propertyName,
                                            true,
                                            false);
                                    }

                                    protected internal override PropertyDefinition Accept(Element.CharVal node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        throw new NotImplementedException("TODO");
                                    }

                                    protected internal override PropertyDefinition Accept(Element.NumVal node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = NumValToClassName.Instance.Visit(node.Value, default);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            var innerClass = NumValToClass.Instance.Visit(node.Value, (innerClassName, context.InnerClasses));

                                            context.InnerClasses[innerClassName] = innerClass;
                                        }

                                        var propertyType = $"{InnersClassName}.{innerClassName}";
                                        if (context.IsCollection)
                                        {
                                            propertyType = $"IEnumerable<{propertyType}>";
                                        }

                                        if (!context.PropertyTypeToCount.TryGetValue(innerClassName, out var count))
                                        {
                                            count = 0;
                                        }

                                        ++count;
                                        context.PropertyTypeToCount[innerClassName] = count;

                                        var propertyName = $"{innerClassName}_{count}";

                                        return new PropertyDefinition(
                                            AccessModifier.Public,
                                            propertyType,
                                            propertyName,
                                            true,
                                            false);
                                    }

                                    private sealed class NumValToClass : NumVal.Visitor<Class, (string ClassName, Dictionary<string, Class> InnerClasses)>
                                    {
                                        private NumValToClass()
                                        {
                                        }

                                        public static NumValToClass Instance { get; } = new NumValToClass();

                                        protected internal override Class Accept(NumVal.BinVal node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            throw new NotImplementedException("TODO");
                                        }

                                        protected internal override Class Accept(NumVal.DecVal node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            throw new NotImplementedException("TODO");
                                        }

                                        protected internal override Class Accept(NumVal.HexVal node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            return HexValToClass.Instance.Visit(node.Value, context);
                                        }

                                        private sealed class HexValToClass : HexVal.Visitor<Class, (string ClassName, Dictionary<string, Class> InnerClasses)>
                                        {
                                            private HexValToClass()
                                            {
                                            }

                                            public static HexValToClass Instance { get; } = new HexValToClass();

                                            protected internal override Class Accept(
                                                HexVal.HexOnly node, 
                                                (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                return HexDigsToClass.Instance.Generate(node.HexDigs, (context.ClassName, null, context.InnerClasses));
                                            }

                                            protected internal override Class Accept(
                                                HexVal.ConcatenatedHex node, 
                                                (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                var segments = node
                                                    .Inners
                                                    .Select(inner => inner.HexDigs)
                                                    .Prepend(node.HexDigs);

                                                var propertyTypeToCount = new Dictionary<string, int>();
                                                var properties = SegmentsToProperties
                                                    .Instance
                                                    .Generate(
                                                        segments,
                                                        (propertyTypeToCount, context.InnerClasses))
                                                    .ToList();

                                                return new Class(
                                                    AccessModifier.Public,
                                                    ClassModifier.Sealed,
                                                    context.ClassName,
                                                    Enumerable.Empty<string>(),
                                                    null,
                                                    new[]
                                                    {
                                                        new ConstructorDefinition(
                                                            AccessModifier.Public,
                                                            properties
                                                                .Select(property =>
                                                                    new MethodParameter(
                                                                        property.Type,
                                                                        property.Name)),
                                                            properties
                                                                .Select(property =>
                                                                    $"this.{property.Name} = {property.Name};")),
                                                    },
                                                    Enumerable.Empty<MethodDefinition>(),
                                                    Enumerable.Empty<Class>(),
                                                    properties);
                                            }

                                            private sealed class SegmentsToProperties
                                            {
                                                private SegmentsToProperties()
                                                {
                                                }

                                                public static SegmentsToProperties Instance { get; } = new SegmentsToProperties();

                                                public IEnumerable<PropertyDefinition> Generate(
                                                    IEnumerable<IEnumerable<HexDig>> segments, 
                                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                {
                                                    foreach (var segment in segments)
                                                    {
                                                        var className = HexDigsToClassName.Instance.Generate(segment, default);
                                                        if (!context.InnerClasses.ContainsKey(className))
                                                        {
                                                            var @class = HexDigsToClass.Instance.Generate(segment, (className, null, context.InnerClasses));
                                                            context.InnerClasses[className] = @class;
                                                        }

                                                        if (!context.PropertyTypeToCount.TryGetValue(className, out var count))
                                                        {
                                                            count = 0;
                                                        }

                                                        ++count;
                                                        context.PropertyTypeToCount[className] = count;

                                                        yield return new PropertyDefinition(
                                                            AccessModifier.Public,
                                                            $"{InnersClassName}.{className}",
                                                            $"{className}_{count}",
                                                            true,
                                                            false);
                                                    }
                                                }
                                            }

                                            protected internal override Class Accept(
                                                HexVal.Range node, 
                                                (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                var range = HexDigsRange(
                                                    node.HexDigs.ToList(),
                                                    node.Inners.First().HexDigs.ToList());
                                                var duElements = range
                                                        .Select(hexDigs => HexDigsToClass
                                                            .Instance
                                                            .Generate(
                                                                hexDigs,
                                                                (HexDigsToClassName.Instance.Generate(hexDigs, default), context.ClassName, context.InnerClasses)))
                                                        .ToList();

                                                return new Class(
                                                    AccessModifier.Public,
                                                    ClassModifier.Abstract,
                                                    context.ClassName,
                                                    Enumerable.Empty<string>(),
                                                    null,
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
                                                                new MethodParameter("Visitor<TResult, TContext>", "visitor"),
                                                                new MethodParameter("TContext", "context"),
                                                            },
                                                            null),
                                                    },
                                                    duElements
                                                        .Prepend(new Class(
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
                                                            duElements
                                                                .Select(element =>
                                                                    new MethodDefinition(
                                                                        AccessModifier.Protected | AccessModifier.Internal,
                                                                        ClassModifier.Abstract,
                                                                        false,
                                                                        "TResult",
                                                                        Enumerable.Empty<string>(),
                                                                        "Accept",
                                                                        new[]
                                                                        {
                                                                            new MethodParameter($"{context.ClassName}.{element.Name}", "node"),
                                                                            new MethodParameter("TContext", "context"),
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
                                                                            new MethodParameter(context.ClassName, "node"),
                                                                            new MethodParameter("TContext", "context"),
                                                                        },
                                                                        "return node.Dispatch(this, context);")),
                                                            Enumerable.Empty<Class>(),
                                                            Enumerable.Empty<PropertyDefinition>())),
                                                    Enumerable.Empty<PropertyDefinition>());

                                            }

                                            private static IEnumerable<IEnumerable<HexDig>> HexDigsRange(
                                                IReadOnlyList<HexDig> low,
                                                IReadOnlyList<HexDig> high)
                                            {
                                                yield return low;
                                                var next = Next(low);
                                                while (!HexDigsEqual(next, high))
                                                {
                                                    yield return next;
                                                }

                                                yield return high;
                                            }

                                            private static bool HexDigsEqual(IReadOnlyList<HexDig> first, IReadOnlyList<HexDig> second)
                                            {
                                                if (first.Count != second.Count)
                                                {
                                                    return false;
                                                }

                                                for (int i = 0; i < first.Count; ++i)
                                                {
                                                    if (!HexDigEqual.Instance.Visit(first[i], second[i]))
                                                    {
                                                        return false;
                                                    }
                                                }

                                                return true;
                                            }

                                            private sealed class HexDigEqual : HexDig.Visitor<bool, HexDig>
                                            {
                                                private HexDigEqual()
                                                {
                                                }

                                                public static HexDigEqual Instance { get; } = new HexDigEqual();

                                                protected internal override bool Accept(HexDig.Digit node, HexDig context)
                                                {
                                                    if (!(context is HexDig.Digit digit))
                                                    {
                                                        return false;
                                                    }

                                                    return DigitEqual.Instance.Visit(node.Value, digit.Value);
                                                }

                                                private sealed class DigitEqual : Digit.Visitor<bool, Digit>
                                                {
                                                    private DigitEqual()
                                                    {
                                                    }

                                                    public static DigitEqual Instance { get; } = new DigitEqual();

                                                    protected internal override bool Accept(Digit.x30 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x31 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x32 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x33 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x34 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x35 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x36 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x37 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x38 node, Digit context)
                                                    {
                                                    }

                                                    protected internal override bool Accept(Digit.x39 node, Digit context)
                                                    {
                                                    }
                                                }

                                                protected internal override bool Accept(HexDig.A node, HexDig context)
                                                {
                                                }

                                                protected internal override bool Accept(HexDig.B node, HexDig context)
                                                {
                                                }

                                                protected internal override bool Accept(HexDig.C node, HexDig context)
                                                {
                                                }

                                                protected internal override bool Accept(HexDig.D node, HexDig context)
                                                {
                                                }

                                                protected internal override bool Accept(HexDig.E node, HexDig context)
                                                {
                                                }

                                                protected internal override bool Accept(HexDig.F node, HexDig context)
                                                {
                                                }
                                            }

                                            private static IReadOnlyList<HexDig> Next(IReadOnlyList<HexDig> previous)
                                            {
                                                var list = new HexDig[previous.Count];

                                                var overflow = false;
                                                for (int i = previous.Count - 1; i >= 0; --i)
                                                {
                                                    var result = HexDigPlusOne.Instance.Visit(previous[i], default);
                                                    overflow = result.Overflow;
                                                    list[i] = result.HexDig;
                                                }

                                                return list;
                                            }

                                            private sealed class HexDigPlusOne : HexDig.Visitor<(HexDig HexDig, bool Overflow), Root.Void>
                                            {
                                                private HexDigPlusOne()
                                                {
                                                }

                                                public static HexDigPlusOne Instance { get; } = new HexDigPlusOne();

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.Digit node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.A node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.B node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.C node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.D node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.E node, Root.Void context)
                                                {
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.F node, Root.Void context)
                                                {
                                                }
                                            }

                                            private sealed class HexDigsToClass
                                            {
                                                private HexDigsToClass()
                                                {
                                                }

                                                public static HexDigsToClass Instance { get; } = new HexDigsToClass();

                                                public Class Generate(
                                                    IEnumerable<HexDig> hexDigs, 
                                                    (string ClassName, string? BaseClass, Dictionary<string, Class> InnerClasses) context)
                                                {
                                                    var propertyTypeToCount = new Dictionary<string, int>();
                                                    var properties = HexDigsToProperties
                                                        .Instance
                                                        .Generate(
                                                            hexDigs,
                                                            (propertyTypeToCount, context.InnerClasses))
                                                        .ToList();

                                                    return new Class(
                                                        AccessModifier.Public,
                                                        ClassModifier.Sealed,
                                                        context.ClassName,
                                                        Enumerable.Empty<string>(),
                                                        context.BaseClass,
                                                        new[]
                                                        {
                                                            new ConstructorDefinition(
                                                                AccessModifier.Public,
                                                                properties
                                                                    .Select(property =>
                                                                        new MethodParameter(property.Type, property.Name)),
                                                                properties
                                                                    .Select(property =>
                                                                        $"this.{property.Name} = {property.Name};")),
                                                        },
                                                        Enumerable.Empty<MethodDefinition>(),
                                                        Enumerable.Empty<Class>(),
                                                        properties);
                                                }

                                                private sealed class HexDigsToProperties
                                                {
                                                    private HexDigsToProperties()
                                                    {
                                                    }

                                                    public static HexDigsToProperties Instance { get; } = new HexDigsToProperties();

                                                    public IEnumerable<PropertyDefinition> Generate(
                                                        IEnumerable<HexDig> hexDigs,
                                                        (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                    {
                                                        foreach (var hexDig in hexDigs)
                                                        {
                                                            var className = HexDigToClassName.Instance.Visit(hexDig, context.@void);
                                                            if (!context.InnerClasses.ContainsKey(className))
                                                            {
                                                                var @class = new Class(
                                                                    AccessModifier.Public,
                                                                    ClassModifier.Sealed,
                                                                    className,
                                                                    Enumerable.Empty<string>(),
                                                                    null,
                                                                    new[]
                                                                    {
                                                                        new ConstructorDefinition(
                                                                            AccessModifier.Private,
                                                                            Enumerable.Empty<MethodParameter>(),
                                                                            Enumerable.Empty<string>()),
                                                                    },
                                                                    Enumerable.Empty<MethodDefinition>(),
                                                                    Enumerable.Empty<Class>(),
                                                                    new[]
                                                                    {
                                                                        new PropertyDefinition(
                                                                            AccessModifier.Public,
                                                                            true,
                                                                            className,
                                                                            "Instance",
                                                                            true,
                                                                            false), //// TODO needs an initiatilizer
                                                                    });

                                                                context.InnerClasses[className] = @class;
                                                            }

                                                            if (!context.PropertyTypeToCount.TryGetValue(className, out var count))
                                                            {
                                                                count = 0;
                                                            }

                                                            ++count;
                                                            context.PropertyTypeToCount[className] = count;

                                                            yield return new PropertyDefinition(
                                                                AccessModifier.Public,
                                                                $"{InnersClassName}.{className}",
                                                                $"{className}_{count}",
                                                                true,
                                                                false);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    protected internal override PropertyDefinition Accept(Element.ProseVal node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        throw new NotImplementedException("TODO");
                                    }
                                }
                            }
                        }

                        private sealed class ConcatenationsToDiscriminatedUnion
                        {
                            private ConcatenationsToDiscriminatedUnion()
                            {
                            }

                            public static ConcatenationsToDiscriminatedUnion Instance { get; } = new ConcatenationsToDiscriminatedUnion();

                            public Class Generate(IEnumerable<Concatenation> concatenations, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                            {
                                var dispatchMethod = new MethodDefinition(
                                    AccessModifier.Protected,
                                    false,
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
                                        new MethodParameter("Visitor<TResult, TContext>", "visitor"),
                                        new MethodParameter("TContext", "context"),
                                    },
                                    "return visitor.Accept(this, context);");
                                var discriminatedUnionElements = concatenations
                                    .Select(concatenation => ConcatenationToClass
                                        .Instance
                                        .Generate(
                                            concatenation, 
                                            (ConcatenationToClassName.Instance.Generate(concatenation), context.ClassName, new[] { dispatchMethod }, context.InnerClasses)));

                                var visitor = new Class(AccessModifier.Public,
                                    ClassModifier.Abstract,
                                    "Visitor",
                                    new[]
                                    {
                                        "TResult",
                                        "TContext",
                                    },
                                    null,
                                    Enumerable.Empty<ConstructorDefinition>(),
                                    discriminatedUnionElements
                                        .Select(element =>
                                            new MethodDefinition(
                                                AccessModifier.Protected | AccessModifier.Internal,
                                                true,
                                                false,
                                                "TResult",
                                                Enumerable.Empty<string>(),
                                                "Accept",
                                                new[]
                                                {
                                                    new MethodParameter($"{context.ClassName}.{element.Name}", "node"),
                                                    new MethodParameter("TContext", "context"),
                                                },
                                                null))
                                        .Prepend(
                                            new MethodDefinition(
                                                AccessModifier.Public,
                                                null,
                                                false,
                                                "TResult",
                                                Enumerable.Empty<string>(),
                                                "Visit",
                                                new[]
                                                {
                                                    new MethodParameter($"{context.ClassName}", "node"), //// TODO it'd be really nice if you could fully qualify the type
                                                    new MethodParameter("TContext", "context"),
                                                },
                                                "return node.Dispatch(this, context);")),
                                    Enumerable.Empty<Class>(),
                                    Enumerable.Empty<PropertyDefinition>());

                                return new Class(
                                    AccessModifier.Public,
                                    ClassModifier.Abstract,
                                    context.ClassName,
                                    Enumerable.Empty<string>(),
                                    null,
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
                                            AccessModifier.Protected, 
                                            true,
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
                                                new MethodParameter("Visitor<TResult, TContext>", "visitor"),
                                                new MethodParameter("TContext", "context"),
                                            },
                                            null),
                                    },
                                    discriminatedUnionElements.Prepend(visitor),
                                    Enumerable.Empty<PropertyDefinition>());
                            }
                        }
                    }
                }
            }

            protected internal override Class? Accept(RuleList.Inner.CommentInner node, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
            {
                return null;
            }
        }

        private sealed class GroupToClassName
        {
            private GroupToClassName()
            {
            }

            public static GroupToClassName Instance { get; } = new GroupToClassName();

            public string Generate(Group group)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(CharacterSubstituions.OpenParenthesis);
                stringBuilder.Append(AlternationToClassName.Instance.Generate(group.Alternation));
                stringBuilder.Append(CharacterSubstituions.CloseParenthesis);

                return stringBuilder.ToString();
            }
        }

        private sealed class AlternationToClassName
        {
            private AlternationToClassName()
            {
            }

            public static AlternationToClassName Instance { get; } = new AlternationToClassName();

            public string Generate(Alternation alternation)
            {
                var className = ConcatenationToClassName.Instance.Generate(alternation.Concatenation);
                foreach (var inner in alternation.Inners)
                {
                    className += $"{CharacterSubstituions.Slash}{ConcatenationToClassName.Instance.Generate(inner.Concatenation)}";
                }

                return className;
            }
        }

        private sealed class ConcatenationToClassName
        {
            private ConcatenationToClassName()
            {
            }

            public static ConcatenationToClassName Instance { get; } = new ConcatenationToClassName();

            public string Generate(Concatenation concatenation)
            {
                var className = RepetitionToClassName.Instance.Visit(concatenation.Repetition, default);
                foreach (var inner in concatenation.Inners)
                {
                    className += $"{CharacterSubstituions.Space}{RepetitionToClassName.Instance.Visit(inner.Repetition, default)}";
                }

                return className;
            }
        }

        private sealed class RepetitionToClassName : Repetition.Visitor<string, Root.Void>
        {
            private RepetitionToClassName()
            {
            }

            public static RepetitionToClassName Instance { get; } = new RepetitionToClassName();

            protected internal override string Accept(Repetition.ElementOnly node, Root.Void context)
            {
                return ElementToClassName.Instance.Visit(node.Element, context);
            }

            protected internal override string Accept(Repetition.RepeatAndElement node, Root.Void context)
            {
                return $"{RepeatToClassName.Instance.Visit(node.Repeat, context)}{ElementToClassName.Instance.Visit(node.Element, context)}";
            }
        }

        private sealed class RepeatToClassName : Repeat.Visitor<string, Root.Void>
        {
            private RepeatToClassName()
            {
            }

            public static RepeatToClassName Instance { get; } = new RepeatToClassName();

            protected internal override string Accept(Repeat.Count node, Root.Void context)
            {
                return DigitsToInt.Instance.Generate(node.Digits, context).ToString();
            }

            protected internal override string Accept(Repeat.Range node, Root.Void context)
            {
                var stringBuilder = new StringBuilder();
                if (node.PrefixDigits.Any())
                {
                    stringBuilder.Append(DigitsToInt.Instance.Generate(node.PrefixDigits, context).ToString());
                }

                stringBuilder.Append(CharacterSubstituions.Asterisk);

                if (node.SuffixDigits.Any())
                {
                    stringBuilder.Append(DigitsToInt.Instance.Generate(node.SuffixDigits, context).ToString());
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class ElementToClassName : Element.Visitor<string, Root.Void>
        {
            private ElementToClassName()
            {
            }

            public static ElementToClassName Instance { get; } = new ElementToClassName();

            protected internal override string Accept(Element.RuleName node, Root.Void context)
            {
                return RuleNameToClassName.Instance.Generate(node.Value, context);
            }

            protected internal override string Accept(Element.Group node, Root.Void context)
            {
                return GroupToClassName.Instance.Generate(node.Value);
            }

            protected internal override string Accept(Element.Option node, Root.Void context)
            {
                return OptionToClassName.Instance.Generate(node.Value);
            }

            protected internal override string Accept(Element.CharVal node, Root.Void context)
            {
                return CharValToClassName.Instance.Generate(node.Value);
            }

            protected internal override string Accept(Element.NumVal node, Root.Void context)
            {
                return NumValToClassName.Instance.Visit(node.Value, context);
            }

            protected internal override string Accept(Element.ProseVal node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class NumValToClassName : NumVal.Visitor<string, Root.Void>
        {
            private NumValToClassName()
            {
            }

            public static NumValToClassName Instance { get; } = new NumValToClassName();

            protected internal override string Accept(NumVal.BinVal node, Root.Void context)
            {
                return $"{CharacterSubstituions.Percent}{BinValToClassName.Instance.Visit(node.Value, context)}";
            }

            protected internal override string Accept(NumVal.DecVal node, Root.Void context)
            {
                return $"{CharacterSubstituions.Percent}{DecValToClassName.Instance.Visit(node.Value, context)}";
            }

            protected internal override string Accept(NumVal.HexVal node, Root.Void context)
            {
                return $"{CharacterSubstituions.Percent}{HexValToClassName.Instance.Visit(node.Value, context)}";
            }
        }

        private sealed class BinValToClassName : BinVal.Visitor<string, Root.Void>
        {
            private BinValToClassName()
            {
            }

            public static BinValToClassName Instance { get; } = new BinValToClassName();

            protected internal override string Accept(BinVal.BitsOnly node, Root.Void context)
            {
            }

            protected internal override string Accept(BinVal.ConcatenatedBits node, Root.Void context)
            {
            }

            protected internal override string Accept(BinVal.Range node, Root.Void context)
            {
            }
        }

        private sealed class DecValToClassName : DecVal.Visitor<string, Root.Void>
        {
            private DecValToClassName()
            {
            }

            public static DecValToClassName Instance { get; } = new DecValToClassName();

            protected internal override string Accept(DecVal.DecsOnly node, Root.Void context)
            {
            }

            protected internal override string Accept(DecVal.ConcatenatedDecs node, Root.Void context)
            {
            }

            protected internal override string Accept(DecVal.Range node, Root.Void context)
            {
            }
        }

        private sealed class HexValToClassName : HexVal.Visitor<string, Root.Void>
        {
            private HexValToClassName()
            {
            }

            public static HexValToClassName Instance { get; } = new HexValToClassName();

            protected internal override string Accept(HexVal.HexOnly node, Root.Void context)
            {
            }

            protected internal override string Accept(HexVal.ConcatenatedHex node, Root.Void context)
            {
            }

            protected internal override string Accept(HexVal.Range node, Root.Void context)
            {
            }
        }

        private sealed class HexDigsToClassName
        {
            private HexDigsToClassName()
            {
            }

            public static HexDigsToClassName Instance { get; } = new HexDigsToClassName();

            public string Generate(IEnumerable<HexDig> hexDigs, Root.Void context)
            {
            }
        }

        private sealed class HexDigToClassName : HexDig.Visitor<string, Root.Void>
        {
            private HexDigToClassName()
            {
            }

            public static HexDigToClassName Instance { get; } = new HexDigToClassName();

            protected internal override string Accept(HexDig.Digit node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.A node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.B node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.C node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.D node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.E node, Root.Void context)
            {
            }

            protected internal override string Accept(HexDig.F node, Root.Void context)
            {
            }
        }

        private sealed class CharValToClassName
        {
            private CharValToClassName()
            {
            }

            public static CharValToClassName Instance { get; } = new CharValToClassName();

            public string Generate(CharVal charVal)
            {
                return $"{CharacterSubstituions.DoubleQuote}{charVal.Inners.Select(inner => InnerToClassName.Instance.Visit(inner, default))}{CharacterSubstituions.DoubleQuote}";
            }

            private sealed class InnerToClassName : CharVal.Inner.Visitor<string, Root.Void>
            {
                private InnerToClassName()
                {
                }

                public static InnerToClassName Instance { get; } = new InnerToClassName();

                protected internal override string Accept(CharVal.Inner.x20 node, Root.Void context)
                {
                    //// TODO do you like this pattern?
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x21 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x23 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x24 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x25 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x26 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x27 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x28 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x29 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2E node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x2F node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x30 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x31 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x32 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x33 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x34 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x35 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x36 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x37 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x38 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x39 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3E node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x3F node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x40 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x41 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x42 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x43 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x44 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x45 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x46 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x47 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x48 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x49 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4E node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x4F node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x50 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x51 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x52 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x53 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x54 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x55 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x56 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x57 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x58 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x59 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5E node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x5F node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x60 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x61 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x62 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x63 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x64 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x65 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x66 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x67 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x68 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x69 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6E node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x6F node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x70 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x71 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x72 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x73 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x74 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x75 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x76 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x77 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x78 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x79 node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x7A node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x7B node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x7C node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x7D node, Root.Void context)
                {
                    return node.GetType().Name;
                }

                protected internal override string Accept(CharVal.Inner.x7E node, Root.Void context)
                {
                    return node.GetType().Name;
                }
            }
        }

        private sealed class OptionToClassName
        {
            private OptionToClassName()
            {
            }

            public static OptionToClassName Instance { get; } = new OptionToClassName();

            public string Generate(Option option)
            {
                return $"{CharacterSubstituions.OpenBracket}{AlternationToClassName.Instance.Generate(option.Alternation)}{CharacterSubstituions.CloseBracket}";
            }
        }

        private sealed class RuleNameToClassName
        {
            private RuleNameToClassName()
            {
            }

            public static RuleNameToClassName Instance { get; } = new RuleNameToClassName();

            public string Generate(RuleName ruleName, Root.Void context)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(AlphaToChar.Instance.Visit(ruleName.Alpha, default));
                foreach (var inner in ruleName.Inners)
                {
                    InnerToString.Instance.Visit(inner, stringBuilder);
                }

                return stringBuilder.ToString();
            }

            private sealed class InnerToString : RuleName.Inner.Visitor<Root.Void, StringBuilder>
            {
                private InnerToString()
                {
                }

                public static InnerToString Instance { get; } = new InnerToString();

                protected internal override Root.Void Accept(RuleName.Inner.AlphaInner node, StringBuilder context)
                {
                    context.Append(AlphaToChar.Instance.Visit(node.Alpha, default));
                    return default;
                }

                protected internal override Root.Void Accept(RuleName.Inner.DigitInner node, StringBuilder context)
                {
                    context.Append(DigitToInt.Instance.Visit(node.Digit, default).ToString());
                    return default;
                }

                protected internal override Root.Void Accept(RuleName.Inner.DashInner node, StringBuilder context)
                {
                    //// TODO create classes to traverse the individual CST nodes
                    context.Append(CharacterSubstituions.Dash);
                    return default;
                }
            }
        }
    }

    public sealed class DigitsToInt
    {
        private DigitsToInt()
        {
        }

        public static DigitsToInt Instance { get; } = new DigitsToInt();

        public int Generate(IEnumerable<Digit> digits, Root.Void context)
        {
            var value = 0;
            foreach (var digit in digits)
            {
                value *= 10;
                value += DigitToInt.Instance.Visit(digit, default);
            }

            return value;
        }
    }

    public sealed class AlphaToChar : Alpha.Visitor<char, Root.Void>
    {
        private AlphaToChar()
        {
        }

        public static AlphaToChar Instance { get; } = new AlphaToChar();

        protected internal override char Accept(Alpha.x41 node, Root.Void context)
        {
            //// TODO add visitors for each CST node, and do it for each method in this class
            return (char)0x41;
        }

        protected internal override char Accept(Alpha.x42 node, Root.Void context)
        {
            return (char)0x42;
        }

        protected internal override char Accept(Alpha.x43 node, Root.Void context)
        {
            return (char)0x43;
        }

        protected internal override char Accept(Alpha.x44 node, Root.Void context)
        {
            return (char)0x44;
        }

        protected internal override char Accept(Alpha.x45 node, Root.Void context)
        {
            return (char)0x45;
        }

        protected internal override char Accept(Alpha.x46 node, Root.Void context)
        {
            return (char)0x46;
        }

        protected internal override char Accept(Alpha.x47 node, Root.Void context)
        {
            return (char)0x47;
        }

        protected internal override char Accept(Alpha.x48 node, Root.Void context)
        {
            return (char)0x48;
        }

        protected internal override char Accept(Alpha.x49 node, Root.Void context)
        {
            return (char)0x49;
        }

        protected internal override char Accept(Alpha.x4A node, Root.Void context)
        {
            return (char)0x4A;
        }

        protected internal override char Accept(Alpha.x4B node, Root.Void context)
        {
            return (char)0x4B;
        }

        protected internal override char Accept(Alpha.x4C node, Root.Void context)
        {
            return (char)0x4C;
        }

        protected internal override char Accept(Alpha.x4D node, Root.Void context)
        {
            return (char)0x4D;
        }

        protected internal override char Accept(Alpha.x4E node, Root.Void context)
        {
            return (char)0x4E;
        }

        protected internal override char Accept(Alpha.x4F node, Root.Void context)
        {
            return (char)0x4F;
        }

        protected internal override char Accept(Alpha.x50 node, Root.Void context)
        {
            return (char)0x50;
        }

        protected internal override char Accept(Alpha.x51 node, Root.Void context)
        {
            return (char)0x51;
        }

        protected internal override char Accept(Alpha.x52 node, Root.Void context)
        {
            return (char)0x52;
        }

        protected internal override char Accept(Alpha.x53 node, Root.Void context)
        {
            return (char)0x53;
        }

        protected internal override char Accept(Alpha.x54 node, Root.Void context)
        {
            return (char)0x54;
        }

        protected internal override char Accept(Alpha.x55 node, Root.Void context)
        {
            return (char)0x55;
        }

        protected internal override char Accept(Alpha.x56 node, Root.Void context)
        {
            return (char)0x56;
        }

        protected internal override char Accept(Alpha.x57 node, Root.Void context)
        {
            return (char)0x57;
        }

        protected internal override char Accept(Alpha.x58 node, Root.Void context)
        {
            return (char)0x58;
        }

        protected internal override char Accept(Alpha.x59 node, Root.Void context)
        {
            return (char)0x59;
        }

        protected internal override char Accept(Alpha.x5A node, Root.Void context)
        {
            return (char)0x5A;
        }

        protected internal override char Accept(Alpha.x61 node, Root.Void context)
        {
            return (char)0x61;
        }

        protected internal override char Accept(Alpha.x62 node, Root.Void context)
        {
            return (char)0x62;
        }

        protected internal override char Accept(Alpha.x63 node, Root.Void context)
        {
            return (char)0x63;
        }

        protected internal override char Accept(Alpha.x64 node, Root.Void context)
        {
            return (char)0x64;
        }

        protected internal override char Accept(Alpha.x65 node, Root.Void context)
        {
            return (char)0x65;
        }

        protected internal override char Accept(Alpha.x66 node, Root.Void context)
        {
            return (char)0x66;
        }

        protected internal override char Accept(Alpha.x67 node, Root.Void context)
        {
            return (char)0x67;
        }

        protected internal override char Accept(Alpha.x68 node, Root.Void context)
        {
            return (char)0x68;
        }

        protected internal override char Accept(Alpha.x69 node, Root.Void context)
        {
            return (char)0x69;
        }

        protected internal override char Accept(Alpha.x6A node, Root.Void context)
        {
            return (char)0x6A;
        }

        protected internal override char Accept(Alpha.x6B node, Root.Void context)
        {
            return (char)0x6B;
        }

        protected internal override char Accept(Alpha.x6C node, Root.Void context)
        {
            return (char)0x6C;
        }

        protected internal override char Accept(Alpha.x6D node, Root.Void context)
        {
            return (char)0x6D;
        }

        protected internal override char Accept(Alpha.x6E node, Root.Void context)
        {
            return (char)0x6E;
        }

        protected internal override char Accept(Alpha.x6F node, Root.Void context)
        {
            return (char)0x6F;
        }

        protected internal override char Accept(Alpha.x70 node, Root.Void context)
        {
            return (char)0x70;
        }

        protected internal override char Accept(Alpha.x71 node, Root.Void context)
        {
            return (char)0x71;
        }

        protected internal override char Accept(Alpha.x72 node, Root.Void context)
        {
            return (char)0x72;
        }

        protected internal override char Accept(Alpha.x73 node, Root.Void context)
        {
            return (char)0x73;
        }

        protected internal override char Accept(Alpha.x74 node, Root.Void context)
        {
            return (char)0x74;
        }

        protected internal override char Accept(Alpha.x75 node, Root.Void context)
        {
            return (char)0x75;
        }

        protected internal override char Accept(Alpha.x76 node, Root.Void context)
        {
            return (char)0x76;
        }

        protected internal override char Accept(Alpha.x77 node, Root.Void context)
        {
            return (char)0x77;
        }

        protected internal override char Accept(Alpha.x78 node, Root.Void context)
        {
            return (char)0x78;
        }

        protected internal override char Accept(Alpha.x79 node, Root.Void context)
        {
            return (char)0x79;
        }

        protected internal override char Accept(Alpha.x7A node, Root.Void context)
        {
            return (char)0x7A;
        }
    }

    public sealed class DigitToInt : Digit.Visitor<int, Root.Void>
    {
        private DigitToInt()
        {
        }

        public static DigitToInt Instance { get; } = new DigitToInt();

        protected internal override int Accept(Digit.x30 node, Root.Void context)
        {
            //// TODO add visitors foreach CST node and do this for each method in this class
            return 0x30 - '0';
        }

        protected internal override int Accept(Digit.x31 node, Root.Void context)
        {
            return 0x31 - '0';
        }

        protected internal override int Accept(Digit.x32 node, Root.Void context)
        {
            return 0x32 - '0';
        }

        protected internal override int Accept(Digit.x33 node, Root.Void context)
        {
            return 0x33 - '0';
        }

        protected internal override int Accept(Digit.x34 node, Root.Void context)
        {
            return 0x34 - '0';
        }

        protected internal override int Accept(Digit.x35 node, Root.Void context)
        {
            return 0x35 - '0';
        }

        protected internal override int Accept(Digit.x36 node, Root.Void context)
        {
            return 0x36 - '0';
        }

        protected internal override int Accept(Digit.x37 node, Root.Void context)
        {
            return 0x37 - '0';
        }

        protected internal override int Accept(Digit.x38 node, Root.Void context)
        {
            return 0x38 - '0';
        }

        protected internal override int Accept(Digit.x39 node, Root.Void context)
        {
            return 0x39 - '0';
        }
    }
}
