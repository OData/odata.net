namespace AbnfParserGenerator
{
    using AbnfParser.CstNodes;
    using Root;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                        }

                        protected internal override Class? Accept(Element.Group node, Root.Void context)
                        {
                            
                        }

                        private sealed class GroupToNestedGroupingClasses
                        {
                            private GroupToNestedGroupingClasses()
                            {
                            }

                            public static GroupToNestedGroupingClasses Instance { get; } = new GroupToNestedGroupingClasses();

                            public IEnumerable<Class> Generate(AbnfParser.CstNodes.Group group, Root.Void context)
                            {

                                return AlternationToClass.Instance.Generate(group.Alternation, )
                            }
                        }

                        protected internal override Class? Accept(Element.Option node, Root.Void context)
                        {
                        }

                        protected internal override Class? Accept(Element.CharVal node, Root.Void context)
                        {
                        }

                        protected internal override Class? Accept(Element.NumVal node, Root.Void context)
                        {
                        }

                        protected internal override Class? Accept(Element.ProseVal node, Root.Void context)
                        {
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

                if (grouping)
                {
                    context.Append("ᴖ");
                }
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
