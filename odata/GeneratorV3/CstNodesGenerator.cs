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
    using GeneratorV3.Odata;
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

    public sealed class CstNodesGeneratorSettings
    {
        private CstNodesGeneratorSettings(
            string innersClassName, 
            string classNamePrefix, 
            CharacterSubstitutions characterSubstitutions)
        {
            this.InnersClassName = innersClassName;
            this.ClassNamePrefix = classNamePrefix;
            this.CharacterSubstitutions = characterSubstitutions;
        }

        public static CstNodesGeneratorSettings Default { get; } = new CstNodesGeneratorSettings(
            "Inners",
            "_",
            CharacterSubstitutions.Default);

        /// <summary>
        /// Rules often compose several other elements at a time; when this happens, these end up being something akin to
        /// an "implied" rules, which is being referred to here as "inner" rules. When these are encountered, classes
        /// still need to be generated, but it is useful to separate them from the classes for the rules themselves. As
        /// a result, we will nest the "inner" rules in another class. This property species the name of the class that
        /// the "inner" rules should be nested under. It is critical that the name selected for this class does not
        /// conflict with the name of any rules in the ABNF.
        /// </summary>
        public string InnersClassName { get; }

        /// <summary>
        /// Because rule names can sometimes conflict with c# reserved words and because literals can sometimes start
        /// with characters that result in c# identifiers that are not legal, all names of generated class are given
        /// a prefix. This property speficies what that prefix should be.
        /// </summary>
        public string ClassNamePrefix { get; }

        /// <summary>
        /// When generating the "inner" classes, ABNF reserved characters are translated into c# friendly class name
        /// characters. This property defines what those translations should be.
        /// </summary>
        public CharacterSubstitutions CharacterSubstitutions { get; }
    }

    public sealed class CharacterSubstitutions
    {
        private CharacterSubstitutions(
            char dash, 
            string openParenthesis, 
            char closeParenthesis, 
            char openBracket, 
            char closeBracket, 
            string asterisk, 
            char slash, 
            char space, 
            string doubleQuote, 
            string period,
            string percent)
        {
            this.Dash = dash;
            this.OpenParenthesis = openParenthesis;
            this.CloseParenthesis = closeParenthesis;
            this.OpenBracket = openBracket;
            this.CloseBracket = closeBracket;
            this.Asterisk = asterisk;
            this.Slash = slash;
            this.Space = space;
            this.DoubleQuote = doubleQuote;
            this.Period = period;
            this.Percent = percent;
        }

        public static CharacterSubstitutions Default { get; } = new CharacterSubstitutions(
            'ⲻ',
            "open",
            'Ↄ',
            '꘡',
            '꘡',
            "asterisk",
            'Ⳇ',
            '_',
            "doublequote",
            "period",
            "percent");

        //// TODO use string for all

        public char Dash { get; } //// TODO parameterize these

        public string OpenParenthesis { get; } //// TODO 'Ⲥ'; 

        public char CloseParenthesis { get; }

        public char OpenBracket { get; }

        public char CloseBracket { get; }

        public string Asterisk { get; } //// TODO 'ж';

        public char Slash { get; }

        public char Space { get; }

        public string DoubleQuote { get; } //// TODO

        public string Period { get; } //// TODO

        public string Percent { get; } //// TODO
    }

    public sealed class CstNodesGenerator
    {
        private readonly string innersClassName;
        private readonly RuleListInnerGenerator ruleListInnerGenerator;

        public CstNodesGenerator(string @namespace)
            : this(@namespace, CstNodesGeneratorSettings.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace">The namespace that all of the generated class should go in</param>
        public CstNodesGenerator(string @namespace, CstNodesGeneratorSettings settings)
        {
            this.innersClassName = settings.InnersClassName;
            this.ruleListInnerGenerator = new RuleListInnerGenerator(
                settings.ClassNamePrefix, 
                @namespace, 
                this.innersClassName,
                settings.CharacterSubstitutions);
        }

        public IEnumerable<Class> Generate(RuleList ruleList, Root.Void context)
        {
            //// TODO doing this as the context means that you have to `tolist` all over the place to make sure that the nested classes of `inners` doesn't get modified while it is being enumerated
            var innerClasses = new Dictionary<string, Class>();
            return ruleList
                .Inners
                .Select(
                    inner => this
                        .ruleListInnerGenerator
                        .Visit(inner, (innerClasses, context)))
                .NotNull()
                .Append(
                    new Class(
                        AccessModifier.Public,
                        ClassModifier.Static,
                        this.innersClassName,
                        Enumerable.Empty<string>(),
                        null,
                        Enumerable.Empty<ConstructorDefinition>(),
                        Enumerable.Empty<MethodDefinition>(),
                        innerClasses.Values,
                        Enumerable.Empty<PropertyDefinition>()));
        }

        private sealed class RuleListInnerGenerator : RuleList.Inner.Visitor<Class?, (Dictionary<string, Class> InnerClasses, Root.Void @void)>
        {
            private readonly RuleGenerator ruleGenerator;

            public RuleListInnerGenerator(
                string classNamePrefix, 
                string @namespace, 
                string innersClassName,
                CharacterSubstitutions characterSubstitutions)
            {
                this.ruleGenerator = new RuleGenerator(
                    classNamePrefix, 
                    @namespace, 
                    innersClassName, 
                    characterSubstitutions);
            }

            protected internal override Class? Accept(RuleList.Inner.RuleInner node, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
            {
                return this.ruleGenerator.Generate(node.Rule, context);
            }

            private sealed class RuleGenerator
            {
                private readonly string classNamePrefix;
                private readonly ElementsGenerator elementsGenerator;
                private readonly RuleNameToClassName ruleNameToClassName;

                public RuleGenerator(
                    string classNamePrefix, 
                    string @namespace, 
                    string innersClassName,
                    CharacterSubstitutions characterSubstitutions)
                {
                    this.classNamePrefix = classNamePrefix;
                    this.elementsGenerator = new ElementsGenerator(
                        this.classNamePrefix, 
                        @namespace, 
                        innersClassName,
                        characterSubstitutions);
                    this.ruleNameToClassName = new RuleNameToClassName(characterSubstitutions);
                }

                public Class Generate(Rule rule, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
                {
                    var className = this.classNamePrefix + this.ruleNameToClassName.Generate(rule.RuleName, context.@void);
                    return this.elementsGenerator.Generate(rule.Elements, (className, context.InnerClasses));
                }

                private sealed class ElementsGenerator
                {
                    private readonly AlternationGenerator alternationGenerator;

                    public ElementsGenerator(
                        string classNamePrefix,
                        string @namespace, 
                        string innersClassName,
                        CharacterSubstitutions characterSubstitutions)
                    {
                        this.alternationGenerator = new AlternationGenerator(
                            classNamePrefix, 
                            @namespace, 
                            innersClassName,
                            characterSubstitutions);
                    }

                    public Class Generate(Elements elements, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                    {
                        return this.alternationGenerator.Generate(elements.Alternation, context);
                    }

                    private sealed class AlternationGenerator
                    {
                        private readonly ConcatenationToClass concatenationToClass;
                        private readonly ConcatenationsToDiscriminatedUnion concatenationsToDiscriminatedUnion;

                        public AlternationGenerator(
                            string classNamePrefix,
                            string @namespace, 
                            string innersClassName,
                            CharacterSubstitutions characterSubstitutions)
                        {
                            this.concatenationToClass = new ConcatenationToClass(
                                classNamePrefix, 
                                @namespace, 
                                innersClassName,
                                characterSubstitutions,
                                this);
                            this.concatenationsToDiscriminatedUnion = new ConcatenationsToDiscriminatedUnion(
                                classNamePrefix,
                                this.concatenationToClass,
                                characterSubstitutions);
                        }

                        public Class Generate(Alternation alternation, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                        {
                            if (alternation.Inners.Any())
                            {
                                return this
                                    .concatenationsToDiscriminatedUnion
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
                                return this.concatenationToClass.Generate(alternation.Concatenation, (context.ClassName, null, Enumerable.Empty<MethodDefinition>(), context.InnerClasses));
                            }
                        }

                        private sealed class ConcatenationToClass
                        {
                            private readonly RepetitonToPropertyDefinition repetitonToPropertyDefinition;

                            public ConcatenationToClass(
                                string classNamePrefix,
                                string @namespace, 
                                string innersClassName,
                                CharacterSubstitutions characterSubstitutions,
                                AlternationGenerator alternationGenerator)
                            {
                                this.repetitonToPropertyDefinition = new RepetitonToPropertyDefinition(
                                    classNamePrefix,
                                    @namespace, 
                                    innersClassName,
                                    characterSubstitutions,
                                    alternationGenerator);
                            }

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
                                    .Select(repetition => this.
                                        repetitonToPropertyDefinition
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
                                private readonly ElementToPropertyDefinition elementToPropertyDefinition;

                                public RepetitonToPropertyDefinition(
                                    string classNamePrefix,
                                    string @namespace,
                                    string innersClassName,
                                    CharacterSubstitutions characterSubstitutions,
                                    AlternationGenerator alternationGenerator)
                                {
                                    this.elementToPropertyDefinition = new ElementToPropertyDefinition(
                                        classNamePrefix,
                                        @namespace,
                                        innersClassName,
                                        characterSubstitutions,
                                        alternationGenerator);
                                }

                                protected internal override PropertyDefinition Accept(
                                    Repetition.ElementOnly node, 
                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                {
                                    return this
                                        .elementToPropertyDefinition
                                        .Visit(
                                            node.Element, 
                                            (false, context.PropertyTypeToCount, context.InnerClasses));
                                }

                                protected internal override PropertyDefinition Accept(
                                    Repetition.RepeatAndElement node,
                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                {
                                    return this.
                                        elementToPropertyDefinition
                                        .Visit(
                                            node.Element,
                                            (true, context.PropertyTypeToCount, context.InnerClasses));
                                }

                                private sealed class ElementToPropertyDefinition : Element.Visitor<PropertyDefinition, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses)>
                                {
                                    private readonly string classNamePrefix;
                                    private readonly string @namespace;
                                    private readonly string innersClassName;

                                    private readonly AlternationGenerator alternationGenerator;
                                    private readonly CharValToClass charValToClass;
                                    private readonly NumValToClass numValToClass;
                                    private readonly AlternationToClassName alternationToClassName;
                                    private readonly RuleNameToClassName ruleNameToClassName;
                                    private readonly GroupToClassName groupToClassName;
                                    private readonly CharValToClassName charValToClassName;
                                    private readonly NumValToClassName numValToClassName;

                                    public ElementToPropertyDefinition(
                                        string classNamePrefix,
                                        string @namespace,
                                        string innersClassName,
                                        CharacterSubstitutions characterSubstitutions,
                                        AlternationGenerator alternationGenerator)
                                    {
                                        this.classNamePrefix = classNamePrefix;
                                        this.@namespace = @namespace;
                                        this.innersClassName = innersClassName;

                                        this.alternationGenerator = alternationGenerator;

                                        this.charValToClass = new CharValToClass(classNamePrefix, innersClassName);
                                        this.numValToClass = new NumValToClass(classNamePrefix, innersClassName);
                                        this.alternationToClassName = new AlternationToClassName(characterSubstitutions);
                                        this.ruleNameToClassName = new RuleNameToClassName(characterSubstitutions);
                                        this.groupToClassName = new GroupToClassName(characterSubstitutions);
                                        this.charValToClassName = new CharValToClassName(characterSubstitutions);
                                        this.numValToClassName = new NumValToClassName(characterSubstitutions);
                                    }

                                    protected internal override PropertyDefinition Accept(
                                        Element.RuleName node, 
                                        (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var ruleName = this.classNamePrefix + this
                                            .ruleNameToClassName
                                            .Generate(
                                                node.Value,
                                                default);
                                        var propertyType = $"{this.@namespace}.{ruleName}";
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
                                        
                                        var groupInnerClassName = this.classNamePrefix + this.alternationToClassName.Generate(node.Value.Alternation);
                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(groupInnerClassName))
                                        {
                                            context.InnerClasses[groupInnerClassName] = this.alternationGenerator.Generate(node.Value.Alternation, (groupInnerClassName, context.InnerClasses));
                                        }

                                        var groupClassName = this.classNamePrefix + this.groupToClassName.Generate(node.Value);

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
                                                            new MethodParameter($"{(isOnlyRuleName ? this.@namespace : this.innersClassName)}.{groupInnerClassName}", $"{groupInnerClassName}_1"),
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
                                                        $"{(isOnlyRuleName ? this.@namespace : this.innersClassName)}.{groupInnerClassName}",
                                                        $"{groupInnerClassName}_1",
                                                        true,
                                                        false),
                                                });

                                            context.InnerClasses[groupClassName] = groupClass;
                                        }

                                        var propertyType = $"{this.innersClassName}.{groupClassName}";
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
                                        var innerClassName = this.classNamePrefix + this.alternationToClassName.Generate(node.Value.Alternation);

                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            context.InnerClasses[innerClassName] = this.
                                                alternationGenerator
                                                .Generate(
                                                    node.Value.Alternation,
                                                    (innerClassName, context.InnerClasses));
                                        }

                                        var propertyType = $"{(isOnlyRuleName ? this.@namespace : this.innersClassName)}.{innerClassName}?";
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
                                        var innerClassName = this.classNamePrefix + this.charValToClassName.Generate(node.Value);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            var innerClass = this.charValToClass.Generate(node.Value, (innerClassName, context.InnerClasses));

                                            context.InnerClasses[innerClassName] = innerClass;
                                        }

                                        var propertyType = $"{this.innersClassName}.{innerClassName}";
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

                                    private sealed class CharValToClass
                                    {
                                        private readonly InnerToProperty innerToProperty;

                                        public CharValToClass(string classNamePrefix, string innersClassName)
                                        {
                                            this.innerToProperty = new InnerToProperty(classNamePrefix, innersClassName);
                                        }

                                        public Class Generate(CharVal charVal, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            var propertyTypeToCount = new Dictionary<string, int>();
                                            var properties = charVal
                                                .Inners
                                                .Select(inner => this.
                                                    innerToProperty
                                                    .Generate(inner, (propertyTypeToCount, context.InnerClasses)))
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
                                                                new MethodParameter(property.Type, property.Name)), //// TODO should this an other calls lower in the stack use the inners class qualifier?
                                                        properties
                                                            .Select(property =>
                                                                $"this.{property.Name} = {property.Name};")),
                                                },
                                                Enumerable.Empty<MethodDefinition>(),
                                                Enumerable.Empty<Class>(),
                                                properties);
                                        }

                                        private sealed class InnerToProperty
                                        {
                                            private readonly string classNamePrefix;
                                            private readonly string innersClassName;

                                            public InnerToProperty(string classNamePrefix, string innersClassName)
                                            {
                                                this.classNamePrefix = classNamePrefix;
                                                this.innersClassName = innersClassName;
                                            }

                                            public PropertyDefinition Generate(
                                                CharVal.Inner inner, 
                                                (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                var className = this.classNamePrefix + CharValInnerToClassName.Instance.Visit(inner, default);

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
                                                                false,
                                                                $"new {className}();"),
                                                        });
                                                    context.InnerClasses[className] = @class;
                                                }

                                                if (!context.PropertyTypeToCount.TryGetValue(className, out var count))
                                                {
                                                    count = 0;
                                                }

                                                ++count;
                                                context.PropertyTypeToCount[className] = count;

                                                return new PropertyDefinition(
                                                    AccessModifier.Public,
                                                    $"{this.innersClassName}.{className}",
                                                    $"{className}_{count}",
                                                    true,
                                                    false);
                                            }
                                        }
                                    }

                                    protected internal override PropertyDefinition Accept(Element.NumVal node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = this.classNamePrefix + this.numValToClassName.Visit(node.Value, default);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            var innerClass = this.numValToClass.Visit(node.Value, (innerClassName, context.InnerClasses));

                                            context.InnerClasses[innerClassName] = innerClass;
                                        }

                                        var propertyType = $"{this.innersClassName}.{innerClassName}";
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
                                        private readonly HexValToClass hexValToClass;

                                        public NumValToClass(string classNamePrefix, string innersClassName)
                                        {
                                            this.hexValToClass = new HexValToClass(classNamePrefix, innersClassName);
                                        }

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
                                            return this.hexValToClass.Visit(node.Value, context);
                                        }

                                        private sealed class HexValToClass : HexVal.Visitor<Class, (string ClassName, Dictionary<string, Class> InnerClasses)>
                                        {
                                            private readonly string classNamePrefix;

                                            private readonly HexDigsToClass hexDigsToClass;
                                            private readonly SegmentsToProperties segmentsToProperties;

                                            public HexValToClass(string classNamePrefix, string innersClassName)
                                            {
                                                this.classNamePrefix = classNamePrefix;

                                                this.hexDigsToClass = new HexDigsToClass(
                                                    this.classNamePrefix, 
                                                    innersClassName);
                                                this.segmentsToProperties = new SegmentsToProperties(
                                                    this.classNamePrefix,
                                                    innersClassName, 
                                                    this.hexDigsToClass);
                                            }

                                            protected internal override Class Accept(
                                                HexVal.HexOnly node, 
                                                (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                return this.hexDigsToClass.Generate(node.HexDigs, (context.ClassName, null, context.InnerClasses));
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
                                                var properties = this
                                                    .segmentsToProperties
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
                                                                        property.Type, //// TODO should this an other calls lower in the stack use the inners class qualifier?
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
                                                private readonly string classNamePrefix;
                                                private readonly string innersClassName;
                                                private readonly HexDigsToClass hexDigsToClass;

                                                public SegmentsToProperties(
                                                    string classNamePrefix,
                                                    string innersClassName,
                                                    HexDigsToClass hexDigsToClass)
                                                {
                                                    this.classNamePrefix = classNamePrefix;
                                                    this.innersClassName = innersClassName;
                                                    this.hexDigsToClass = hexDigsToClass;
                                                }

                                                public IEnumerable<PropertyDefinition> Generate(
                                                    IEnumerable<IEnumerable<HexDig>> segments, 
                                                    (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                {
                                                    foreach (var segment in segments)
                                                    {
                                                        var className = this.classNamePrefix + HexDigsToClassName.Instance.Generate(segment, default);
                                                        if (!context.InnerClasses.ContainsKey(className))
                                                        {
                                                            var @class = this.hexDigsToClass.Generate(segment, (className, null, context.InnerClasses));
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
                                                            $"{this.innersClassName}.{className}",
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
                                                        .Select(hexDigs => this
                                                            .hexDigsToClass
                                                            .Generate(
                                                                hexDigs,
                                                                (this.classNamePrefix + HexDigsToClassName.Instance.Generate(hexDigs, default), context.ClassName, context.InnerClasses)))
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
                                                    next = Next(next);
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
                                                        return context is Digit.x30;
                                                    }

                                                    protected internal override bool Accept(Digit.x31 node, Digit context)
                                                    {
                                                        return context is Digit.x31;
                                                    }

                                                    protected internal override bool Accept(Digit.x32 node, Digit context)
                                                    {
                                                        return context is Digit.x32;
                                                    }

                                                    protected internal override bool Accept(Digit.x33 node, Digit context)
                                                    {
                                                        return context is Digit.x33;
                                                    }

                                                    protected internal override bool Accept(Digit.x34 node, Digit context)
                                                    {
                                                        return context is Digit.x34;
                                                    }

                                                    protected internal override bool Accept(Digit.x35 node, Digit context)
                                                    {
                                                        return context is Digit.x35;
                                                    }

                                                    protected internal override bool Accept(Digit.x36 node, Digit context)
                                                    {
                                                        return context is Digit.x36;
                                                    }

                                                    protected internal override bool Accept(Digit.x37 node, Digit context)
                                                    {
                                                        return context is Digit.x37;
                                                    }

                                                    protected internal override bool Accept(Digit.x38 node, Digit context)
                                                    {
                                                        return context is Digit.x38;
                                                    }

                                                    protected internal override bool Accept(Digit.x39 node, Digit context)
                                                    {
                                                        return context is Digit.x39;
                                                    }
                                                }

                                                protected internal override bool Accept(HexDig.A node, HexDig context)
                                                {
                                                    return context is HexDig.A;
                                                }

                                                protected internal override bool Accept(HexDig.B node, HexDig context)
                                                {
                                                    return context is HexDig.B;
                                                }

                                                protected internal override bool Accept(HexDig.C node, HexDig context)
                                                {
                                                    return context is HexDig.C;
                                                }

                                                protected internal override bool Accept(HexDig.D node, HexDig context)
                                                {
                                                    return context is HexDig.D;
                                                }

                                                protected internal override bool Accept(HexDig.E node, HexDig context)
                                                {
                                                    return context is HexDig.E;
                                                }

                                                protected internal override bool Accept(HexDig.F node, HexDig context)
                                                {
                                                    return context is HexDig.F;
                                                }
                                            }

                                            private static IReadOnlyList<HexDig> Next(IReadOnlyList<HexDig> previous)
                                            {
                                                var list = new HexDig[previous.Count];

                                                var overflow = true;
                                                for (int i = previous.Count - 1; i >= 0; --i)
                                                {
                                                    if (overflow)
                                                    {
                                                        var result = HexDigPlusOne.Instance.Visit(previous[i], default);
                                                        overflow = result.Overflow;
                                                        list[i] = result.HexDig;
                                                    }
                                                    else
                                                    {
                                                        list[i] = previous[i];
                                                    }
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
                                                    return (DigitPlusOne.Instance.Visit(node.Value, default), false);
                                                }

                                                private sealed class DigitPlusOne : Digit.Visitor<HexDig, Root.Void>
                                                {
                                                    private DigitPlusOne()
                                                    {
                                                    }

                                                    public static DigitPlusOne Instance { get; } = new DigitPlusOne();

                                                    protected internal override HexDig Accept(Digit.x30 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x31(x31.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x31 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x32(x32.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x32 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x33(x33.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x33 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x34(x34.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x34 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x35(x35.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x35 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x36(x36.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x36 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x37(x37.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x37 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x38(x38.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x38 node, Root.Void context)
                                                    {
                                                        return new HexDig.Digit(new Digit.x39(x39.Instance));
                                                    }

                                                    protected internal override HexDig Accept(Digit.x39 node, Root.Void context)
                                                    {
                                                        return new HexDig.A(x41.Instance);
                                                    }
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.A node, Root.Void context)
                                                {
                                                    return (new HexDig.B(x42.Instance), false);
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.B node, Root.Void context)
                                                {
                                                    return (new HexDig.C(x43.Instance), false);
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.C node, Root.Void context)
                                                {
                                                    return (new HexDig.D(x44.Instance), false);
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.D node, Root.Void context)
                                                {
                                                    return (new HexDig.E(x45.Instance), false);
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.E node, Root.Void context)
                                                {
                                                    return (new HexDig.F(x46.Instance), false);
                                                }

                                                protected internal override (HexDig HexDig, bool Overflow) Accept(HexDig.F node, Root.Void context)
                                                {
                                                    return (new HexDig.Digit(new Digit.x30(x30.Instance)), true);
                                                }
                                            }

                                            private sealed class HexDigsToClass
                                            {
                                                private readonly HexDigsToProperties hexDigsToProperties;

                                                public HexDigsToClass(string classNamePrefix, string innersClassName)
                                                {
                                                    this.hexDigsToProperties = new HexDigsToProperties(
                                                        classNamePrefix, 
                                                        innersClassName);
                                                }

                                                public Class Generate(
                                                    IEnumerable<HexDig> hexDigs, 
                                                    (string ClassName, string? BaseClass, Dictionary<string, Class> InnerClasses) context)
                                                {
                                                    var propertyTypeToCount = new Dictionary<string, int>();
                                                    var properties = this
                                                        .hexDigsToProperties
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
                                                        context.BaseClass == null ?
                                                            Enumerable.Empty<MethodDefinition>() :
                                                            new[]
                                                            {
                                                                new MethodDefinition(
                                                                    AccessModifier.Protected,
                                                                    ClassModifier.Sealed,
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
                                                            }, //// TODO this is a really hacky way to add the dispatch method here; i think you had to do somethign similar elsewhere and did a better pattern there
                                                        Enumerable.Empty<Class>(),
                                                        properties);
                                                }

                                                private sealed class HexDigsToProperties
                                                {
                                                    private readonly string classNamePrefix;
                                                    private readonly string innersClassName;

                                                    public HexDigsToProperties(string classNamePrefix, string innersClassName)
                                                    {
                                                        this.classNamePrefix = classNamePrefix;
                                                        this.innersClassName = innersClassName;
                                                    }

                                                    public IEnumerable<PropertyDefinition> Generate(
                                                        IEnumerable<HexDig> hexDigs,
                                                        (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                    {
                                                        foreach (var hexDig in hexDigs)
                                                        {
                                                            var className = this.classNamePrefix + HexDigToClassName.Instance.Visit(hexDig, default);
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
                                                                            false,
                                                                            $"new {className}();"),
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
                                                                $"{this.innersClassName}.{className}",
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
                            private readonly string classNamePrefix;
                            private readonly ConcatenationToClass concatenationToClass;
                            private readonly ConcatenationToClassName concatenationToClassName;

                            public ConcatenationsToDiscriminatedUnion(
                                string classNamePrefix,
                                ConcatenationToClass concatenationToClass,
                                CharacterSubstitutions characterSubstitutions)
                            {
                                this.classNamePrefix = classNamePrefix;
                                this.concatenationToClass = concatenationToClass;

                                this.concatenationToClassName = new ConcatenationToClassName(characterSubstitutions);
                            }

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
                                    .Select(concatenation => this.
                                        concatenationToClass
                                        .Generate(
                                            concatenation,
                                            (this.classNamePrefix + this.concatenationToClassName.Generate(concatenation), context.ClassName, new[] { dispatchMethod }, context.InnerClasses)))
                                    .ToList();

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

        private sealed class ToClassNames
        {
            public ToClassNames(CharacterSubstitutions characterSubstitutions)
            {
                this.GroupToClassName = new GroupToClassName(characterSubstitutions, this);
                this.AlternationToClassName = new AlternationToClassName(characterSubstitutions, this);
                this.ConcatenationToClassName = new ConcatenationToClassName(characterSubstitutions, this);
                this.RepetitionToClassName = new RepetitionToClassName(characterSubstitutions, this);
                this.RepeatToClassName = new RepeatToClassName(characterSubstitutions, this);
                this.DigitsToClassName = new DigitsToClassName(this);
                this.ElementToClassName = new ElementToClassName(this);
                this.NumValToClassName = new NumValToClassName(characterSubstitutions, this);
                this.BinValToClassName = new BinValToClassName(this);
                this.DecValToClassName = new DecValToClassName(this);
                this.HexValToClassName = new HexValToClassName(characterSubstitutions, this);
                this.HexDigsToClassName = new HexDigsToClassName(this);
                this.HexDigToClassName = new HexDigToClassName(this);
                this.DigitToClassName = new DigitToClassName(this);
                this.CharValToClassName = new CharValToClassName(characterSubstitutions, this);
                this.CharValInnerToClassName = new CharValInnerToClassName(this);
                this.OptionToClassName = new OptionToClassName(characterSubstitutions, this);
                this.RuleNameToClassName = new RuleNameToClassName(characterSubstitutions, this);
            }

            public GroupToClassName GroupToClassName { get; }

            public AlternationToClassName AlternationToClassName { get; }

            public ConcatenationToClassName ConcatenationToClassName { get; }
            
            public RepetitionToClassName RepetitionToClassName { get; }

            public RepeatToClassName RepeatToClassName { get; }

            public DigitsToClassName DigitsToClassName { get; }

            public ElementToClassName ElementToClassName { get; }

            public NumValToClassName NumValToClassName { get; }

            public BinValToClassName BinValToClassName { get; }

            public DecValToClassName DecValToClassName { get; }

            public HexValToClassName HexValToClassName { get; }

            public HexDigsToClassName HexDigsToClassName { get; }

            public HexDigToClassName HexDigToClassName { get; }
            
            public DigitToClassName DigitToClassName { get; }

            public CharValToClassName CharValToClassName { get; }

            public CharValInnerToClassName CharValInnerToClassName { get; }

            public OptionToClassName OptionToClassName { get; }

            public RuleNameToClassName RuleNameToClassName { get; }
        }

        private sealed class GroupToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public GroupToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            public string Generate(Group group)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(characterSubstitutions.OpenParenthesis);
                stringBuilder.Append(this.toClassNames.AlternationToClassName.Generate(group.Alternation));
                stringBuilder.Append(characterSubstitutions.CloseParenthesis);

                return stringBuilder.ToString();
            }
        }

        private sealed class AlternationToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public AlternationToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            public string Generate(Alternation alternation)
            {
                var className = this.toClassNames.ConcatenationToClassName.Generate(alternation.Concatenation);
                foreach (var inner in alternation.Inners)
                {
                    className += $"{characterSubstitutions.Slash}{this.toClassNames.ConcatenationToClassName.Generate(inner.Concatenation)}";
                }

                return className;
            }
        }

        private sealed class ConcatenationToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public ConcatenationToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            public string Generate(Concatenation concatenation)
            {
                var className = this.toClassNames.RepetitionToClassName.Visit(concatenation.Repetition, default);
                foreach (var inner in concatenation.Inners)
                {
                    className += $"{characterSubstitutions.Space}{this.toClassNames.RepetitionToClassName.Visit(inner.Repetition, default)}";
                }

                return className;
            }
        }

        private sealed class RepetitionToClassName : Repetition.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public RepetitionToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(Repetition.ElementOnly node, Root.Void context)
            {
                return this.toClassNames.ElementToClassName.Visit(node.Element, context);
            }

            protected internal override string Accept(Repetition.RepeatAndElement node, Root.Void context)
            {
                return $"{this.toClassNames.RepeatToClassName.Visit(node.Repeat, context)}{this.toClassNames.ElementToClassName.Visit(node.Element, context)}";
            }
        }

        private sealed class RepeatToClassName : Repeat.Visitor<string, Root.Void>
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public RepeatToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(Repeat.Count node, Root.Void context)
            {
                return this.toClassNames.DigitsToClassName.Generate(node.Digits);
            }

            protected internal override string Accept(Repeat.Range node, Root.Void context)
            {
                var stringBuilder = new StringBuilder();
                if (node.PrefixDigits.Any())
                {
                    stringBuilder.Append(this.toClassNames.DigitsToClassName.Generate(node.PrefixDigits));
                }

                stringBuilder.Append(characterSubstitutions.Asterisk);

                if (node.SuffixDigits.Any())
                {
                    stringBuilder.Append(this.toClassNames.DigitsToClassName.Generate(node.SuffixDigits));
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class DigitsToClassName
        {
            private readonly ToClassNames toClassNames;

            public DigitsToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(IEnumerable<Digit> digits)
            {
                var stringBuilder = new StringBuilder();
                foreach (var digit in digits)
                {
                    stringBuilder.Append(DigitToClassName.Instance.Visit(digit, default));
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class ElementToClassName : Element.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public ElementToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(Element.RuleName node, Root.Void context)
            {
                return this.toClassNames.RuleNameToClassName.Generate(node.Value, context);
            }

            protected internal override string Accept(Element.Group node, Root.Void context)
            {
                return this.toClassNames.GroupToClassName.Generate(node.Value);
            }

            protected internal override string Accept(Element.Option node, Root.Void context)
            {
                return this.toClassNames.OptionToClassName.Generate(node.Value);
            }

            protected internal override string Accept(Element.CharVal node, Root.Void context)
            {
                return this.toClassNames.CharValToClassName.Generate(node.Value);
            }

            protected internal override string Accept(Element.NumVal node, Root.Void context)
            {
                return this.toClassNames.NumValToClassName.Visit(node.Value, context);
            }

            protected internal override string Accept(Element.ProseVal node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class NumValToClassName : NumVal.Visitor<string, Root.Void>
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public NumValToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(NumVal.BinVal node, Root.Void context)
            {
                return $"{characterSubstitutions.Percent}{this.toClassNames.BinValToClassName.Visit(node.Value, context)}";
            }

            protected internal override string Accept(NumVal.DecVal node, Root.Void context)
            {
                return $"{characterSubstitutions.Percent}{this.toClassNames.DecValToClassName.Visit(node.Value, context)}";
            }

            protected internal override string Accept(NumVal.HexVal node, Root.Void context)
            {
                return $"{characterSubstitutions.Percent}{this.toClassNames.HexValToClassName.Visit(node.Value, context)}";
            }
        }

        private sealed class BinValToClassName : BinVal.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public BinValToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(BinVal.BitsOnly node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(BinVal.ConcatenatedBits node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(BinVal.Range node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class DecValToClassName : DecVal.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public DecValToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(DecVal.DecsOnly node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(DecVal.ConcatenatedDecs node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }

            protected internal override string Accept(DecVal.Range node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class HexValToClassName : HexVal.Visitor<string, Root.Void>
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public HexValToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(HexVal.HexOnly node, Root.Void context)
            {
                return $"x{this.toClassNames.HexDigsToClassName.Generate(node.HexDigs, context)}";
            }

            protected internal override string Accept(HexVal.ConcatenatedHex node, Root.Void context)
            {
                var builder = new StringBuilder();
                builder.Append($"x{this.toClassNames.HexDigsToClassName.Generate(node.HexDigs, context)}");
                foreach (var inner in node.Inners)
                {
                    builder.Append(characterSubstitutions.Period);
                    builder.Append(this.toClassNames.HexDigsToClassName.Generate(inner.HexDigs, context));
                }

                return builder.ToString();
            }

            protected internal override string Accept(HexVal.Range node, Root.Void context)
            {
                return $"x{this.toClassNames.HexDigsToClassName.Generate(node.HexDigs, context)}{characterSubstitutions.Dash}{HexDigsToClassName.Instance.Generate(node.Inners.First().HexDigs, context)}";
            }
        }

        private sealed class HexDigsToClassName
        {
            private readonly ToClassNames toClassNames;

            public HexDigsToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(IEnumerable<HexDig> hexDigs, Root.Void context)
            {
                var stringBuilder = new StringBuilder();
                foreach (var hexDig in hexDigs)
                {
                    stringBuilder.Append(this.toClassNames.HexDigToClassName.Visit(hexDig, context));
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class HexDigToClassName : HexDig.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public HexDigToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(HexDig.Digit node, Root.Void context)
            {
                return this.toClassNames.DigitToClassName.Visit(node.Value, context);
            }

            protected internal override string Accept(HexDig.A node, Root.Void context)
            {
                return "A";
            }

            protected internal override string Accept(HexDig.B node, Root.Void context)
            {
                return "B";
            }

            protected internal override string Accept(HexDig.C node, Root.Void context)
            {
                return "C";
            }

            protected internal override string Accept(HexDig.D node, Root.Void context)
            {
                return "D";
            }

            protected internal override string Accept(HexDig.E node, Root.Void context)
            {
                return "E";
            }

            protected internal override string Accept(HexDig.F node, Root.Void context)
            {
                return "F";
            }
        }

        private sealed class DigitToClassName : Digit.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public DigitToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(Digit.x30 node, Root.Void context)
            {
                return "0";
            }

            protected internal override string Accept(Digit.x31 node, Root.Void context)
            {
                return "1";
            }

            protected internal override string Accept(Digit.x32 node, Root.Void context)
            {
                return "2";
            }

            protected internal override string Accept(Digit.x33 node, Root.Void context)
            {
                return "3";
            }

            protected internal override string Accept(Digit.x34 node, Root.Void context)
            {
                return "4";
            }

            protected internal override string Accept(Digit.x35 node, Root.Void context)
            {
                return "5";
            }

            protected internal override string Accept(Digit.x36 node, Root.Void context)
            {
                return "6";
            }

            protected internal override string Accept(Digit.x37 node, Root.Void context)
            {
                return "7";
            }

            protected internal override string Accept(Digit.x38 node, Root.Void context)
            {
                return "8";
            }

            protected internal override string Accept(Digit.x39 node, Root.Void context)
            {
                return "9";
            }
        }

        private sealed class CharValToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public CharValToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            public string Generate(CharVal charVal)
            {
                return $"{characterSubstitutions.DoubleQuote}{string.Join(string.Empty, charVal.Inners.Select(inner => this.CharValInnerToClassName.Visit(inner, default)))}{characterSubstitutions.DoubleQuote}";
            }

        }

        private sealed class CharValInnerToClassName : CharVal.Inner.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public CharValInnerToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

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

        private sealed class OptionToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public OptionToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            public string Generate(Option option)
            {
                return $"{characterSubstitutions.OpenBracket}{this.toClassNames.AlternationToClassName.Generate(option.Alternation)}{characterSubstitutions.CloseBracket}";
            }
        }

        private sealed class RuleNameToClassName
        {
            private readonly InnerToString innerToString;

            public RuleNameToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassName)
            {
                this.innerToString = new InnerToString(characterSubstitutions);
            }

            public string Generate(RuleName ruleName, Root.Void context)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(AlphaToChar.Instance.Visit(ruleName.Alpha, default));
                foreach (var inner in ruleName.Inners)
                {
                    this.innerToString.Visit(inner, stringBuilder);
                }

                return stringBuilder.ToString();
            }

            private sealed class InnerToString : RuleName.Inner.Visitor<Root.Void, StringBuilder>
            {
                private readonly CharacterSubstitutions characterSubstitutions;

                public InnerToString(CharacterSubstitutions characterSubstitutions)
                {
                    this.characterSubstitutions = characterSubstitutions;
                }

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
                    context.Append(characterSubstitutions.Dash);
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
