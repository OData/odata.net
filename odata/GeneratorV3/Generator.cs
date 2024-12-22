namespace GeneratorV3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

            public static char OpenParenthesis { get; } = 'Ⲥ';

            public static char CloseParenthesis { get; } = 'Ↄ';

            public static char OpenBracket { get; } = '꘡';

            public static char CloseBracket { get; } = '꘡';

            public static char Asterisk { get; } = 'ж';

            public static char Slash { get; } = 'Ⳇ';

            public static char Space { get; } = '_';
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
                        false, //// TODO this should be static
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
                                return ConcatenationToSealed.Instance.Generate(alternation.Concatenation, context);
                            }
                        }

                        private sealed class ConcatenationToSealed
                        {
                            private ConcatenationToSealed()
                            {
                            }

                            public static ConcatenationToSealed Instance { get; } = new ConcatenationToSealed();

                            public Class Generate(
                                Concatenation concatenation, 
                                (string ClassName, Dictionary<string, Class> InnerClasses) context)
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
                                    false,
                                    context.ClassName,
                                    Enumerable.Empty<string>(),
                                    null,
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

                                    protected internal override PropertyDefinition Accept(
                                        Element.Group node, 
                                        (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = GroupToClassName.Instance.Generate(node.Value);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            context.InnerClasses[innerClassName] = AlternationGenerator
                                                .Instance
                                                .Generate(
                                                    node.Value.Alternation, 
                                                    (innerClassName, context.InnerClasses));
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
                                            $"{InnersClassName}.{innerClassName}",
                                            propertyName,
                                            true,
                                            false);
                                    }

                                    protected internal override PropertyDefinition Accept(Element.Option node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = OptionToClassName.Instance.Generate(node.Value);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            context.InnerClasses[innerClassName] = AlternationGenerator
                                                .Instance
                                                .Generate(
                                                    node.Value.Alternation,
                                                    (innerClassName, context.InnerClasses));
                                        }

                                        var propertyType = $"{InnersClassName}.{innerClassName}?";
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
                                            $"{InnersClassName}.{innerClassName}",
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
                                        throw new NotImplementedException("TODO");
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
                                //// TODO this should have the base type...
                                var discriminatedUnionElements = concatenations
                                    .Select(concatenation => ConcatenationToSealed
                                        .Instance
                                        .Generate(
                                            concatenation, 
                                            (ConcatenationToClassName.Instance.Generate(concatenation), context.InnerClasses)));
                                return new Class(
                                    AccessModifier.Public,
                                    true,
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
                                    Enumerable.Empty<MethodDefinition>(),
                                    discriminatedUnionElements, //// TODO add visitor
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
                return $"{CharacterSubstituions.OpenParenthesis}{AlternationToClassName.Instance.Generate(group.Alternation)}{CharacterSubstituions.CloseParenthesis}";
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
                var className = string.Empty;
                if (node.PrefixDigits.Any())
                {
                    className += DigitsToInt.Instance.Generate(node.PrefixDigits, context).ToString();
                }

                className += CharacterSubstituions.Asterisk;

                if (node.SuffixDigits.Any())
                {
                    className += DigitsToInt.Instance.Generate(node.SuffixDigits, context).ToString();
                }

                return className;
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
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(Element.NumVal node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(Element.ProseVal node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
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

                stringBuilder.Append(char.ToUpperInvariant(AlphaToChar.Instance.Visit(ruleName.Alpha, default)));
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
                    context.Append(char.ToUpperInvariant(AlphaToChar.Instance.Visit(node.Alpha, default)));
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
