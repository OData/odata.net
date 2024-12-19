namespace AbnfParserGenerator
{
    using AbnfParser.CstNodes;
    using Root;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                throw new System.NotImplementedException();
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

            public Class Generate(Alternation alternation, Root.Void context)
            {
                if (alternation.Inners.Any())
                {
                    // if there are multiple alternations, then we are going to need a discriminated union to distinguish them
                    //// TODO
                    return null;
                }
                

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
