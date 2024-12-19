namespace AbnfParserGenerator
{
    using AbnfParser.CstNodes;
    using AbnfParser.CstNodes.Core;
    using Root;
    using System;
    using System.Collections.Generic;
    using System.Data;
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
                return RuleToClass.Instance.Generate(node.Rule, context);
            }

            private sealed class RuleToClass
            {
                private RuleToClass()
                {
                }

                public static RuleToClass Instance { get; } = new RuleToClass();

                public Class Generate(AbnfParser.CstNodes.Rule rule, Root.Void context)
                {
                    var ruleNameBuilder = new StringBuilder();
                    RuleNameToClassName.Instance.Generate(rule.RuleName, ruleNameBuilder);
                    return ElementsToClass.Instance.Generate(rule.Elements, (ruleNameBuilder.ToString(), context));
                }

                private sealed class ElementsToClass
                {
                    private ElementsToClass()
                    {
                    }

                    public static ElementsToClass Instance { get; } = new ElementsToClass();

                    public Class Generate(AbnfParser.CstNodes.Elements elements, (string ClassName, Root.Void) context)
                    {
                        return AlternationToClass.Instance.Generate(elements.Alternation, context);
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
                var nestedGroupingClasses = AlternationToNestedGroupingClasses
                    .Instance
                    .Generate(alternation, context.@void)
                    .NotNull();
                if (alternation.Inners.Any())
                {
                    // if there are multiple concatenations, then we are going to need a discriminated union to distinguish them
                    var discriminatedUnionMembers = AlternationToDiscriminatedUnionMembers.Instance.Generate(alternation, context);
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
                        nestedGroupingClasses
                            .Concat(
                                discriminatedUnionMembers)
                            .Prepend(
                                new Class(
                                    AccessModifier.Public,
                                    true,
                                    "Visitor",
                                    new[]
                                    {
                                        "TResult",
                                        "TContext",
                                    },
                                    null,
                                    Enumerable.Empty<ConstructorDefinition>(),
                                    discriminatedUnionMembers
                                        .Select(discriminatedUnionMember =>
                                            new MethodDefinition(
                                                AccessModifier.Protected | AccessModifier.Internal,
                                                true,
                                                false,
                                                "TResult",
                                                Enumerable.Empty<string>(),
                                                "Accept",
                                                new[]
                                                {
                                                    new MethodParameter(discriminatedUnionMember.Name, "node"),
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
                                                    new MethodParameter(context.ClassName, "node"),
                                                    new MethodParameter("TContext", "context"),
                                                },
                                                "return node.Dispatch(this, context);")),
                                    Enumerable.Empty<Class>(),
                                    Enumerable.Empty<PropertyDefinition>())),
                        Enumerable.Empty<PropertyDefinition>());
                }

                if (AlternationToIsThereAnOptionPresent.Instance.Generate(alternation, context.@void))
                {
                    // if there are options present, then we are going to need a discriminated union to distinguish whether the option was taken or not
                    //// TODO
                    return new Class(
                        AccessModifier.Public,
                        true,
                        context.ClassName,
                        Enumerable.Empty<string>(), //// TODO do this
                        null, //// TODO add this
                        Enumerable.Empty<ConstructorDefinition>(), //// TODO add these
                        Enumerable.Empty<MethodDefinition>(), //// TODO add these
                        Enumerable.Empty<Class>(), //// TODO add these
                        Enumerable.Empty<PropertyDefinition>()); //// TODO add these
                }

                // there is no need for a discriminated union, so let's just create the class
                var propertyDefinitions = ConcatenationToPropertyDefinitions
                    .Instance
                    .Generate(alternation.Concatenation, context.@void);
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
                            propertyDefinitions.Select(propertyDefinition =>
                                new MethodParameter(propertyDefinition.Type, propertyDefinition.Name)),
                                    propertyDefinitions.Select(propertyDefinition =>
                                        $"this.{propertyDefinition.Name} = {propertyDefinition.Name};")),
                    },
                    Enumerable.Empty<MethodDefinition>(),
                    nestedGroupingClasses,
                    propertyDefinitions);
            }

            private sealed class AlternationToNestedGroupingClasses
            {
                private AlternationToNestedGroupingClasses()
                {
                }

                public static AlternationToNestedGroupingClasses Instance { get; } = new AlternationToNestedGroupingClasses();

                public IEnumerable<Class?> Generate(Alternation alternation, Root.Void context)
                {
                    yield return ConcatenationToGroupingClass.Instance.Generate(alternation.Concatenation, context);
                    foreach (var inner in alternation.Inners)
                    {
                        yield return InnerToNestedGroupingClasses.Instance.Generate(inner, context);
                    }
                }

                private sealed class InnerToNestedGroupingClasses
                {
                    private InnerToNestedGroupingClasses()
                    {
                    }

                    public static InnerToNestedGroupingClasses Instance { get; } = new InnerToNestedGroupingClasses();

                    public Class? Generate(Alternation.Inner inner, Root.Void context)
                    {
                        return ConcatenationToGroupingClass.Instance.Generate(inner.Concatenation, context);
                    }
                }
            }

            private sealed class AlternationToDiscriminatedUnionMembers
            {
                private AlternationToDiscriminatedUnionMembers()
                {
                }

                public static AlternationToDiscriminatedUnionMembers Instance { get; } = new AlternationToDiscriminatedUnionMembers();

                public IEnumerable<Class> Generate(Alternation alternation, (string BaseType, Root.Void @void) context)
                {
                    yield return ConcatenationToDisciminatedUnionMember.Instance.Generate(alternation.Concatenation, context);
                    foreach (var inner in alternation.Inners)
                    {
                        yield return InnerToDiscriminatedUnionMember.Instance.Generate(inner, context);
                    }
                }

                private sealed class InnerToDiscriminatedUnionMember
                {
                    private InnerToDiscriminatedUnionMember()
                    {
                    }

                    public static InnerToDiscriminatedUnionMember Instance { get; } = new InnerToDiscriminatedUnionMember();

                    public Class Generate(Alternation.Inner inner, (string BaseType, Root.Void @void) context)
                    {
                        return ConcatenationToDisciminatedUnionMember.Instance.Generate(inner.Concatenation, context);
                    }
                }

                private sealed class ConcatenationToDisciminatedUnionMember
                {
                    private ConcatenationToDisciminatedUnionMember()
                    {
                    }

                    public static ConcatenationToDisciminatedUnionMember Instance { get; } = new ConcatenationToDisciminatedUnionMember();

                    public Class Generate(Concatenation concatenation, (string BaseType, Root.Void @void) context)
                    {
                        var classNameBuilder = new StringBuilder();
                        ConcatenationToClassName.Instance.Generate(concatenation, classNameBuilder);
                        var className = classNameBuilder.ToString();

                        IEnumerable<Class> nestedGroupingClasses;
                        IEnumerable<PropertyDefinition> propertyDefinitions;
                        //// TODO this is very hacky and you should do better
                        var groupingOfLiteral = "groupingofᴖ";
                        if (className.StartsWith(groupingOfLiteral))
                        {
                            nestedGroupingClasses = Enumerable.Empty<Class>();
                            propertyDefinitions = new[]
                            {
                                new PropertyDefinition(
                                    AccessModifier.Public,
                                    className,
                                    $"{className}1",
                                    true,
                                    false),
                            };
                            className = className.Substring(groupingOfLiteral.Length, className.Length - groupingOfLiteral.Length - 1);
                        }
                        else
                        {
                            //// TODO it's possible that you'll never have nested grouping classes because the two possible callers of this method are either going to be single rules or things that are grouping because of an alternation
                            //// TODO i don't think this is right
                            nestedGroupingClasses = new[]
                            {
                                ConcatenationToGroupingClass
                                    .Instance
                                    .Generate(concatenation, context.@void)
                            }.NotNull();
                            propertyDefinitions = ConcatenationToPropertyDefinitions
                                .Instance
                                .Generate(concatenation, context.@void);
                        }

                        return new Class(
                            AccessModifier.Public,
                            false,
                            className,
                            Enumerable.Empty<string>(),
                            context.BaseType,
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Public,
                                    propertyDefinitions
                                        .Select(propertyDefinition =>
                                            new MethodParameter(propertyDefinition.Type, propertyDefinition.Name)),
                                    propertyDefinitions
                                        .Select(propertyDefinition =>
                                            $"this.{propertyDefinition.Name} = {propertyDefinition.Name};")),
                            },
                            new[]
                            {
                                new MethodDefinition(
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
                                    "return visitor.Accept(this, context);"),
                            },
                            nestedGroupingClasses,
                            propertyDefinitions);
                    }
                }
            }

            private sealed class ConcatenationToPropertyDefinitions
            {
                private ConcatenationToPropertyDefinitions()
                {
                }

                public static ConcatenationToPropertyDefinitions Instance { get; } = new ConcatenationToPropertyDefinitions();

                public IEnumerable<PropertyDefinition> Generate(Concatenation concatenation, Root.Void context)
                {
                    var propertyTypeCounts = new Dictionary<string, int>();

                    yield return RepetitionToPropertyDefinition.Instance.Visit(concatenation.Repetition, (propertyTypeCounts, context));
                    foreach (var inner in concatenation.Inners)
                    {
                        yield return InnerToPropertyDefinition.Instance.Generate(inner, (propertyTypeCounts, context));
                    }
                }

                private sealed class InnerToPropertyDefinition
                {
                    private InnerToPropertyDefinition()
                    {
                    }

                    public static InnerToPropertyDefinition Instance { get; } = new InnerToPropertyDefinition();

                    public PropertyDefinition Generate(Concatenation.Inner inner, (Dictionary<string, int> PropertyTypeCounts, Root.Void) context)
                    {
                        return RepetitionToPropertyDefinition.Instance.Visit(inner.Repetition, context);
                    }
                }

                private sealed class RepetitionToPropertyDefinition : Repetition.Visitor<PropertyDefinition, (Dictionary<string, int> PropertyTypeCounts, Root.Void)>
                {
                    private RepetitionToPropertyDefinition()
                    {
                    }

                    public static RepetitionToPropertyDefinition Instance { get; } = new RepetitionToPropertyDefinition();

                    protected internal override PropertyDefinition Accept(
                        Repetition.ElementOnly node,
                        (Dictionary<string, int> PropertyTypeCounts, Root.Void) context)
                    {
                        return ElementToPropertyDefinition.Instance.Visit(node.Element, (context.PropertyTypeCounts, false));
                    }

                    protected internal override PropertyDefinition Accept(
                        Repetition.RepeatAndElement node,
                        (Dictionary<string, int> PropertyTypeCounts, Root.Void) context)
                    {
                        return ElementToPropertyDefinition.Instance.Visit(node.Element, (context.PropertyTypeCounts, true));
                    }

                    private sealed class ElementToPropertyDefinition : Element.Visitor<PropertyDefinition, (Dictionary<string, int> PropertyTypeCounts, bool IsCollection)>
                    {
                        private ElementToPropertyDefinition()
                        {
                        }

                        public static ElementToPropertyDefinition Instance { get; } = new ElementToPropertyDefinition();

                        protected internal override PropertyDefinition Accept(
                            Element.RuleName node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            var propertyTypeBuilder = new StringBuilder();
                            RuleNameToClassName.Instance.Generate(node.Value, propertyTypeBuilder);
                            var propertyType = propertyTypeBuilder.ToString();

                            if (!context.PropertyTypeCounts.TryGetValue(propertyType, out var propertyTypeCount))
                            {
                                propertyTypeCount = 0;
                            }

                            ++propertyTypeCount;
                            context.PropertyTypeCounts[propertyType] = propertyTypeCount;

                            var propertyName = $"{propertyType}{propertyTypeCount}";
                            if (context.IsCollection)
                            {
                                propertyType = $"IEnumerable<{propertyType}>";
                            }

                            return new PropertyDefinition(
                                AccessModifier.Public,
                                propertyType,
                                propertyName,
                                true,
                                false);
                        }

                        protected internal override PropertyDefinition Accept(
                            Element.Group node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            var propertyTypeBuilder = new StringBuilder();
                            GroupToClassName.Instance.Generate(node.Value, propertyTypeBuilder);
                            var propertyType = propertyTypeBuilder.ToString();

                            if (!context.PropertyTypeCounts.TryGetValue(propertyType, out var propertyTypeCount))
                            {
                                propertyTypeCount = 0;
                            }

                            ++propertyTypeCount;
                            context.PropertyTypeCounts[propertyType] = propertyTypeCount;

                            var propertyName = $"{propertyType}{propertyTypeCount}";
                            if (context.IsCollection)
                            {
                                propertyType = $"IEnumerable<{propertyType}>";
                            }

                            return new PropertyDefinition(
                                AccessModifier.Public,
                                propertyType,
                                propertyName,
                                true,
                                false);
                        }

                        protected internal override PropertyDefinition Accept(
                            Element.Option node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            var propertyTypeBuilder = new StringBuilder();
                            AlternationToClassName.Instance.Generate(node.Value.Alternation, propertyTypeBuilder);
                            var propertyType = propertyTypeBuilder.ToString();

                            if (!context.PropertyTypeCounts.TryGetValue(propertyType, out var propertyTypeCount))
                            {
                                propertyTypeCount = 0;
                            }

                            ++propertyTypeCount;
                            context.PropertyTypeCounts[propertyType] = propertyTypeCount;

                            var propertyName = $"{propertyType}{propertyTypeCount}";
                            if (context.IsCollection)
                            {
                                propertyType = $"IEnumerable<{propertyType}>";
                            }

                            return new PropertyDefinition(
                                AccessModifier.Public,
                                propertyType,
                                propertyName,
                                true,
                                false);
                        }

                        protected internal override PropertyDefinition Accept(
                            Element.CharVal node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            //// TODO
                            return new PropertyDefinition(
                                AccessModifier.Public,
                                "int",
                                "TODO",
                                true,
                                false);
                        }

                        protected internal override PropertyDefinition Accept(
                            Element.NumVal node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            //// TODO
                            return new PropertyDefinition(
                                AccessModifier.Public,
                                "int",
                                "TODO",
                                true,
                                false);
                        }

                        protected internal override PropertyDefinition Accept(
                            Element.ProseVal node,
                            (Dictionary<string, int> PropertyTypeCounts, bool IsCollection) context)
                        {
                            //// TODO
                            return new PropertyDefinition(
                                AccessModifier.Public,
                                "int",
                                "TODO",
                                true,
                                false);
                        }
                    }
                }
            }

            private sealed class ConcatenationToGroupingClass
            {
                private ConcatenationToGroupingClass()
                {
                }

                public static ConcatenationToGroupingClass Instance { get; } = new ConcatenationToGroupingClass();

                public Class? Generate(Concatenation concatenation, Root.Void context)
                {
                    //// TODO if this is a single repetiton and that repetition doesn't isn't a group or option, we won't have a nested class
                    if (concatenation.Inners.Any())
                    {
                        var classNameBuilder = new StringBuilder();
                        ConcatenationToClassName.Instance.Generate(concatenation, classNameBuilder);
                        var className = classNameBuilder.ToString();

                        var propertyDefinitions = ConcatenationToPropertyDefinitions
                            .Instance
                            .Generate(concatenation, context);
                        var nestedGroupingClasses = ConcatenationToNestedGroupingClasses
                            .Instance
                            .Generate(concatenation, context)
                            .NotNull();

                        return new Class(
                            AccessModifier.Public,
                            false,
                            className,
                            Enumerable.Empty<string>(),
                            null,
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Public,
                                    propertyDefinitions
                                        .Select(propertyDefinition =>
                                            new MethodParameter(propertyDefinition.Type, propertyDefinition.Name)),
                                    propertyDefinitions
                                        .Select(propertyDefinition =>
                                            $"this.{propertyDefinition.Name} = {propertyDefinition.Name};")),
                            },
                            Enumerable.Empty<MethodDefinition>(),
                            nestedGroupingClasses,
                            propertyDefinitions);
                    }

                    //// TODO this call might not be working correctly: //// TODO or maybe it's just that you haven't fully implemented the callers that are getting recursively called into...
                    return RepetitionToNestedGroupingClass.Instance.Visit(concatenation.Repetition, context);
                }

                private sealed class ConcatenationToNestedGroupingClasses
                {
                    private ConcatenationToNestedGroupingClasses()
                    {
                    }

                    public static ConcatenationToNestedGroupingClasses Instance { get; } = new ConcatenationToNestedGroupingClasses();

                    public IEnumerable<Class?> Generate(Concatenation concatenation, Root.Void context)
                    {
                        yield return RepetitionToNestedGroupingClass.Instance.Visit(concatenation.Repetition, context);
                        foreach (var inner in concatenation.Inners)
                        {
                            yield return InnerToNestedGroupingClass.Instance.Generate(inner, context);
                        }
                    }

                    private sealed class InnerToNestedGroupingClass
                    {
                        private InnerToNestedGroupingClass()
                        {
                        }

                        public static InnerToNestedGroupingClass Instance { get; } = new InnerToNestedGroupingClass();

                        public Class? Generate(Concatenation.Inner inner, Root.Void context)
                        {
                            return RepetitionToNestedGroupingClass.Instance.Visit(inner.Repetition, context);
                        }
                    }
                }

                private sealed class RepetitionToNestedGroupingClass : Repetition.Visitor<Class?, Root.Void>
                {
                    private RepetitionToNestedGroupingClass()
                    {
                    }

                    public static RepetitionToNestedGroupingClass Instance { get; } = new RepetitionToNestedGroupingClass();

                    protected internal override Class? Accept(Repetition.ElementOnly node, Root.Void context)
                    {
                        return ElementToNestedGroupingClass.Instance.Visit(node.Element, context);
                    }

                    protected internal override Class? Accept(Repetition.RepeatAndElement node, Root.Void context)
                    {
                        return ElementToNestedGroupingClass.Instance.Visit(node.Element, context);
                    }

                    private sealed class ElementToNestedGroupingClass : Element.Visitor<Class?, Root.Void>
                    {
                        private ElementToNestedGroupingClass()
                        {
                        }

                        public static ElementToNestedGroupingClass Instance { get; } = new ElementToNestedGroupingClass();

                        protected internal override Class? Accept(Element.RuleName node, Root.Void context)
                        {
                            return null;
                        }

                        protected internal override Class? Accept(Element.Group node, Root.Void context)
                        {
                            return GroupToNestedGroupingClass.Instance.Generate(node.Value, context);
                        }

                        private sealed class GroupToNestedGroupingClass
                        {
                            private GroupToNestedGroupingClass()
                            {
                            }

                            public static GroupToNestedGroupingClass Instance { get; } = new GroupToNestedGroupingClass();

                            public Class Generate(AbnfParser.CstNodes.Group group, Root.Void context)
                            {
                                var classNameBuilder = new StringBuilder();
                                GroupToClassName.Instance.Generate(group, classNameBuilder);
                                return AlternationToClass.Instance.Generate(group.Alternation, (classNameBuilder.ToString(), context));
                            }
                        }

                        protected internal override Class? Accept(Element.Option node, Root.Void context)
                        {
                            return OptionToNestedGroupingClass.Instance.Generate(node.Value, context);
                        }

                        private sealed class OptionToNestedGroupingClass
                        {
                            private OptionToNestedGroupingClass()
                            {
                            }

                            public static OptionToNestedGroupingClass Instance { get; } = new OptionToNestedGroupingClass();

                            public Class? Generate(AbnfParser.CstNodes.Option option, Root.Void context)
                            {
                                //// TODO create classes to traverse these cst nodes
                                if (!option.Alternation.Inners.Any())
                                {
                                    // the option only has one element inside, so we don't need a discriminated union
                                    return null;
                                }

                                var classNameBuilder = new StringBuilder();
                                OptionToClassName.Instance.Generate(option, classNameBuilder);
                                return AlternationToClass.Instance.Generate(option.Alternation, (classNameBuilder.ToString(), context));
                            }
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

            public Root.Void Generate(AbnfParser.CstNodes.Concatenation concatenation, StringBuilder ClassNameBuilder)
            {
                var implicitGrouping = concatenation.Inners.Any();

                if (implicitGrouping)
                {
                    //// TODO call this "concatenationof" instead? also use a different symbol?
                    ClassNameBuilder.Append("groupingofᴖ");
                }

                RepetitionToClassName.Instance.Visit(concatenation.Repetition, ClassNameBuilder);
                foreach (var inner in concatenation.Inners)
                {
                    ClassNameBuilder.Append("followedby");
                    InnerToClassName.Instance.Generate(inner, ClassNameBuilder);
                }

                if (implicitGrouping)
                {
                    ClassNameBuilder.Append("ᴖ");
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
                var ruleNameBuilder = new StringBuilder();
                RuleNameToString.Instance.Generate(node.Value, ruleNameBuilder);
                ruleNameBuilder.Replace('-', '_');
                context.Append(ruleNameBuilder);
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
                        context.Append("atmost");
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
                var needsGrouping = group.Alternation.Inners.Any() || !group.Alternation.Concatenation.Inners.Any();
                if (needsGrouping)
                {
                    context.Append("groupingofᴖ");
                }

                AlternationToClassName.Instance.Generate(group.Alternation, context);

                if (needsGrouping)
                {
                    context.Append("ᴖ");
                }
                return default;
            }
        }

        private sealed class RuleNameToClassName
        {
            private RuleNameToClassName()
            {
            }

            public static RuleNameToClassName Instance { get; } = new RuleNameToClassName();

            public Root.Void Generate(RuleName ruleName, StringBuilder context)
            {
                context.Append("rulewithname");
                RuleNameToString.Instance.Generate(ruleName, context);
                context.Replace('-', '_');

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
            context.Append(char.ToUpperInvariant(AlphaToChar.Instance.Visit(ruleName.Alpha, default)));
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
                //// TODO add a visitor for the dash CST node
                context.Append("-");
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
