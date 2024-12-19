namespace AbnfParserGenerator
{
    using AbnfParser.CstNodes;
    using AbnfParser.CstNodes.Core;
    using Root;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class Generator
    {
        private Generator()
        {
        }

        public static Generator Instance { get; } = new Generator();

        public IEnumerable<Class> Generate(AbnfParser.CstNodes.RuleList ruleList, Root.Void context)
        {
            return ruleList
                .Inners
                .Select(rule => RuleListInnerToClass.Instance.Visit(rule, context))
                .NotNull();
        }

        private sealed class RuleListInnerToClass : RuleList.Inner.Visitor<Class?, Root.Void>
        {
            private RuleListInnerToClass()
            {
            }

            public static RuleListInnerToClass Instance { get; } = new RuleListInnerToClass();

            protected internal override Class? Accept(RuleList.Inner.RuleInner node, Root.Void context)
            {
            }

            private sealed class RuleToClass
            {
                private RuleToClass()
                {
                }

                public static RuleToClass Instance { get; } = new RuleToClass();

                public Class Generate(AbnfParser.CstNodes.Rule rule, Root.Void context)
                {
                }

                private sealed class ElementsToClass
                {
                    private ElementsToClass()
                    {
                    }

                    public static ElementsToClass Instance { get; } = new ElementsToClass();

                    public Class Generate(AbnfParser.CstNodes.Elements elements, Root.Void context)
                    {
                    }
                }
            }

            protected internal override Class? Accept(RuleList.Inner.CommentInner node, Root.Void context)
            {
                return null;
            }
        }

        private sealed class AlternationToClass
        {
            private AlternationToClass()
            {
            }

            public static AlternationToClass Instance { get; } = new AlternationToClass();

            public Class Generate(Alternation alternation, (string ClassName, Root.Void @void) context)
            {
                if (alternation.Inners.Any())
                {
                    // if there are multiple alternations, then we are going to need a discriminated union to distinguish them
                    //// TODO
                    return null;
                }

                if (AlternationToIsThereAnOptionPresent.Instance.Generate(alternation, context.@void))
                {
                    // if there are options present, then we are going to need a discriminated union to distinguish whether the option was taken or not
                    //// TODO
                    return null;
                }

                // there is no need for a discriminated union, so let's just create the class

                return new Class(
                    AccessModifier.Public,
                    false,
                    context.ClassName,
                    Enumerable.Empty<string>(),
                    null,
                    Enumerable.Empty<ConstructorDefinition>(), //// TODO add these
                    Enumerable.Empty<MethodDefinition>(),
                    Enumerable.Empty<Class>(), //// TODO add these
                    Enumerable.Empty<PropertyDefinition>()); //// TODO add these
            }

            private sealed class ConcatenationToNestedGroupingClasses
            {
                private ConcatenationToNestedGroupingClasses()
                {
                }

                public static ConcatenationToNestedGroupingClasses Instance { get; } = new ConcatenationToNestedGroupingClasses();

                public IEnumerable<Class> Generate(Concatenation concatenation, Root.Void context)
                {
                    
                }

                private sealed class RepetitionToNestedGroupingClasses : Repetition.Visitor<Class?, Root.Void>
                {
                    private RepetitionToNestedGroupingClasses()
                    {
                    }

                    public static RepetitionToNestedGroupingClasses Instance { get; } = new RepetitionToNestedGroupingClasses();

                    protected internal override Class? Accept(Repetition.ElementOnly node, Root.Void context)
                    {
                    }

                    protected internal override Class? Accept(Repetition.RepeatAndElement node, Root.Void context)
                    {
                    }

                    private sealed class ElementToNestedGroupingClasses : Element.Visitor<Class?, Root.Void>
                    {
                        private ElementToNestedGroupingClasses()
                        {
                        }

                        public static ElementToNestedGroupingClasses Instance { get; } = new ElementToNestedGroupingClasses();

                        protected internal override Class? Accept(Element.RuleName node, Root.Void context)
                        {
                            return null;
                        }

                        protected internal override Class? Accept(Element.Group node, Root.Void context)
                        {
                            return GroupToNestedGroupingClasses.Instance.Generate(node.Value, context);
                        }

                        private sealed class GroupToNestedGroupingClasses
                        {
                            private GroupToNestedGroupingClasses()
                            {
                            }

                            public static GroupToNestedGroupingClasses Instance { get; } = new GroupToNestedGroupingClasses();

                            public Class Generate(AbnfParser.CstNodes.Group group, Root.Void context)
                            {
                                var classNameBuilder = new StringBuilder();
                                GroupToClassName.Instance.Generate(group, classNameBuilder);
                                return AlternationToClass.Instance.Generate(group.Alternation, (classNameBuilder.ToString(), context));
                            }
                        }

                        protected internal override Class? Accept(Element.Option node, Root.Void context)
                        {
                            //// TODO if the option has concatenations, then return something, otherwise null
                        }

                        protected internal override Class? Accept(Element.CharVal node, Root.Void context)
                        {
                            return null;
                        }

                        protected internal override Class? Accept(Element.NumVal node, Root.Void context)
                        {
                            return null;
                        }

                        protected internal override Class? Accept(Element.ProseVal node, Root.Void context)
                        {
                            return null;
                        }
                    }
                }
            }

            private sealed class AlternationToIsThereAnOptionPresent
            {
                private AlternationToIsThereAnOptionPresent()
                {
                }

                public static AlternationToIsThereAnOptionPresent Instance { get; } = new AlternationToIsThereAnOptionPresent();

                public bool Generate(Alternation alternation, Root.Void context)
                {
                    //// TODO create classes to traverse the individual CST nodes
                    if (RepetitionToIsThereAnOptionPresent
                        .Instance
                        .Visit(alternation.Concatenation.Repetition, context))
                    {
                        return true;
                    }
                        
                    return alternation
                        .Concatenation
                        .Inners
                        .Any(inner => RepetitionToIsThereAnOptionPresent.Instance.Visit(inner.Repetition, context));
                }

                private sealed class RepetitionToIsThereAnOptionPresent : Repetition.Visitor<bool, Root.Void>
                {
                    private RepetitionToIsThereAnOptionPresent()
                    {
                    }

                    public static RepetitionToIsThereAnOptionPresent Instance { get; } = new RepetitionToIsThereAnOptionPresent();

                    protected internal override bool Accept(Repetition.ElementOnly node, Root.Void context)
                    {
                        //// TODO create classes to traverse the individual CST nodes
                        return node.Element is Element.Option;
                    }

                    protected internal override bool Accept(Repetition.RepeatAndElement node, Root.Void context)
                    {
                        //// TODO create classes to traverse the individual CST nodes
                        return node.Element is Element.Option;
                    }
                }
            }
        }

        private sealed class AlternationToClassName
        {
            private AlternationToClassName()
            {
            }

            public static AlternationToClassName Instance { get; } = new AlternationToClassName();

            public Root.Void Generate(AbnfParser.CstNodes.Alternation alternation, StringBuilder context)
            {
                ConcatenationToClassName.Instance.Generate(alternation.Concatenation, context);
                foreach (var inner in alternation.Inners)
                {
                    context.Append("or");
                    ConcatenationToClassName.Instance.Generate(inner.Concatenation, context);
                }

                return default;
            }
        }

        private sealed class ConcatenationToClassName
        {
            private ConcatenationToClassName()
            {
            }

            public static ConcatenationToClassName Instance { get; } = new ConcatenationToClassName();

            public Root.Void Generate(AbnfParser.CstNodes.Concatenation concatenation, StringBuilder context)
            {
                var grouping = concatenation.Inners.Any();

                if (grouping)
                {
                    //// TODO call this "concatenationof" instead? also use a different symbol?
                    context.Append("groupingofᴖ");
                }

                //// TODO do the repetitions now
                RepetitionToClassName.Instance.Visit(concatenation.Repetition, context);
                foreach (var inner in concatenation.Inners)
                {
                    context.Append("followedby");
                    InnerToClassName.Instance.Generate(inner, context);
                }

                if (grouping)
                {
                    context.Append("ᴖ");
                }

                return default;
            }

            private sealed class InnerToClassName
            {
                private InnerToClassName()
                {
                }

                public static InnerToClassName Instance { get; } = new InnerToClassName();

                public Root.Void Generate(AbnfParser.CstNodes.Concatenation.Inner inner, StringBuilder context)
                {
                    RepetitionToClassName.Instance.Visit(inner.Repetition, context);
                    return default;
                }
            }
        }

        private sealed class RepetitionToClassName : Repetition.Visitor<Root.Void, StringBuilder>
        {
            private RepetitionToClassName()
            {
            }

            public static RepetitionToClassName Instance { get; } = new RepetitionToClassName();

            protected internal override Root.Void Accept(Repetition.ElementOnly node, StringBuilder context)
            {
                ElementToClassName.Instance.Visit(node.Element, context);
                return default;
            }

            protected internal override Root.Void Accept(Repetition.RepeatAndElement node, StringBuilder context)
            {
                RepeatToClassName.Instance.Visit(node.Repeat, context);
                ElementToClassName.Instance.Visit(node.Element, context);
                return default;
            }
        }

        private sealed class ElementToClassName : Element.Visitor<Root.Void, StringBuilder>
        {
            private ElementToClassName()
            {
            }

            public static ElementToClassName Instance { get; } = new ElementToClassName();

            protected internal override Root.Void Accept(Element.RuleName node, StringBuilder context)
            {
                RuleNameToString.Instance.Generate(node.Value, context);
                return default;
            }

            protected internal override Root.Void Accept(Element.Group node, StringBuilder context)
            {
                GroupToClassName.Instance.Generate(node.Value, context);
                return default;
            }

            protected internal override Root.Void Accept(Element.Option node, StringBuilder context)
            {
                OptionToClassName.Instance.Generate(node.Value, context);
                return default;
            }

            protected internal override Root.Void Accept(Element.CharVal node, StringBuilder context)
            {
                //// TODO do this;
                return default;
            }

            protected internal override Root.Void Accept(Element.NumVal node, StringBuilder context)
            {
                //// TODO do this;
                return default;
            }

            protected internal override Root.Void Accept(Element.ProseVal node, StringBuilder context)
            {
                //// TODO do this;
                return default;
            }
        }

        private sealed class OptionToClassName
        {
            private OptionToClassName()
            {
            }

            public static OptionToClassName Instance { get; } = new OptionToClassName();

            public Root.Void Generate(Option option, StringBuilder context)
            {
                context.Append("anoptional");
                var needsGrouping = OptionToHasMultipleConcatenations.Instance.Generate(option, default);
                if (needsGrouping)
                {
                    //// in the classes, this will still be a discriminated union, but do you want name it something specific to options? and use a different symbol?
                    context.Append("groupingofᴖ");
                }

                AlternationToClassName.Instance.Generate(option.Alternation, context);

                if (needsGrouping)
                {
                    context.Append("ᴖ");
                }

                return default;
            }

            private sealed class OptionToHasMultipleConcatenations
            {
                private OptionToHasMultipleConcatenations()
                {
                }

                public static OptionToHasMultipleConcatenations Instance { get; } = new OptionToHasMultipleConcatenations();

                public bool Generate(Option option, Root.Void context)
                {
                    //// TODO traverse the CST nodes instead of this shortcut
                    return option.Alternation.Inners.Any();
                }
            }
        }

        private sealed class RepeatToClassName : Repeat.Visitor<Root.Void, StringBuilder>
        {
            private RepeatToClassName()
            {
            }

            public static RepeatToClassName Instance { get; } = new RepeatToClassName();

            protected internal override Root.Void Accept(Repeat.Count node, StringBuilder context)
            {
                var count = DigitsToInt.Instance.Generate(node.Digits, default);
                IntToNumberWord(count, context);
                return default;
            }

            protected internal override Root.Void Accept(Repeat.Range node, StringBuilder context)
            {
                if (!node.PrefixDigits.Any())
                {
                    if (!node.SuffixDigits.Any())
                    {
                        context.Append("anynumberof");
                        return default;
                    }
                    else
                    {
                        context.Append("betweenZEROand");
                        var count = DigitsToInt.Instance.Generate(node.SuffixDigits, default);
                        IntToNumberWord(count, context);
                        return default;
                    }
                }
                else
                {
                    if (!node.SuffixDigits.Any())
                    {
                        context.Append("atleast");
                        var count = DigitsToInt.Instance.Generate(node.PrefixDigits, default);
                        IntToNumberWord(count, context);
                        return default;
                    }
                    else
                    {
                        context.Append("between");
                        var prefixCount = DigitsToInt.Instance.Generate(node.PrefixDigits, default);
                        IntToNumberWord(prefixCount, context);
                        context.Append("and");

                        var suffixCount = DigitsToInt.Instance.Generate(node.SuffixDigits, default);
                        IntToNumberWord(suffixCount, context);
                        return default;
                    }
                }
            }

            private static Root.Void IntToNumberWord(int value, StringBuilder context)
            {
                //// TODO use a standard implementation for this
                if (value == 0)
                {
                    context.Append("ZERO");
                }
                else if (value == 1)
                {
                    context.Append("ONE");
                }
                else if (value == 2)
                {
                    context.Append("TWO");
                }
                else if (value == 3)
                {
                    context.Append("THREE");
                }
                else if (value == 4)
                {
                    context.Append("FOUR");
                }
                else if (value == 5)
                {
                    context.Append("FIVE");
                }
                else if (value == 6)
                {
                    context.Append("SIX");
                }
                else if (value == 7)
                {
                    context.Append("SEVEN");
                }
                else if (value == 8)
                {
                    context.Append("EIGHT");
                }
                else if (value == 9)
                {
                    context.Append("NINE");
                }
                else
                {
                    throw new Exception("TODO use a standard implementation");
                }

                return default;
            }
        }

        private sealed class GroupToClassName
        {
            private GroupToClassName()
            {
            }

            public static GroupToClassName Instance { get; } = new GroupToClassName();

            public Root.Void Generate(AbnfParser.CstNodes.Group group, StringBuilder context)
            {
                context.Append("groupingofᴖ");
                AlternationToClassName.Instance.Generate(group.Alternation, context);
                context.Append("ᴖ");
                return default;
            }
        }
    }

    public sealed class RuleNameToString
    {
        private RuleNameToString()
        {
        }

        public static RuleNameToString Instance { get; } = new RuleNameToString();

        public Root.Void Generate(RuleName ruleName, StringBuilder context)
        {
            context.Append(AlphaToChar.Instance.Visit(ruleName.Alpha, default));
            foreach (var inner in ruleName.Inners)
            {
                InnerToString.Instance.Visit(inner, context);
            }

            return default;
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
                //// TODO add a visitor for the dash CST node
                //// TODO using underscore here instead of dash means that this class isn't really "rulename to string", but "rulename to classname"
                context.Append("_");
                return default;
            }
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
}
