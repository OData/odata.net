namespace _GeneratorV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator;

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
        /// a result, we will nest the "inner" rules in another class. This property specifies the name of the class that
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
            string dash,
            string openParenthesis,
            string closeParenthesis,
            string openBracket,
            string closeBracket,
            string asterisk,
            string slash,
            string space,
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
            "ⲻ",
            "Ⲥ",
            "Ↄ",
            "꘡",
            "꘡",
            "Ж",
            "Ⳇ",
            "_",
            "ʺ",
            "٠",
            "Ⰳ");

        public string Dash { get; }

        public string OpenParenthesis { get; }

        public string CloseParenthesis { get; }

        public string OpenBracket { get; }

        public string CloseBracket { get; }

        public string Asterisk { get; }

        public string Slash { get; }

        public string Space { get; }

        public string DoubleQuote { get; }

        public string Period { get; }

        public string Percent { get; }
    }

    public sealed class Generator
    {
        private readonly string innersClassName;
        private readonly _ruleⳆⲤЖcⲻwsp_cⲻnlↃToClassGenerator _ruleⳆⲤЖcⲻwsp_cⲻnlↃToClass;

        public Generator(string @namespace)
            : this(@namespace, CstNodesGeneratorSettings.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace">The namespace that all of the generated class should go in</param>
        public Generator(string @namespace, CstNodesGeneratorSettings settings)
        {
            this.innersClassName = settings.InnersClassName;
            var toClassNames = new ToClassNames(settings.CharacterSubstitutions);
            this._ruleⳆⲤЖcⲻwsp_cⲻnlↃToClass = new _ruleⳆⲤЖcⲻwsp_cⲻnlↃToClassGenerator(
                settings.ClassNamePrefix,
                @namespace,
                this.innersClassName,
                toClassNames);

            //// !!!!!IMPORTANT WHENEVER YOU REMOVE OLD CODE, CHECK FOR TODOS!!!!!
            //// TODO automatically generate the transcribers
            //// TODO remove the v3 converters
            //// TODO remove the manually written transcribers
            //// TODO manually write the new parsers
            //// TODO remove the old parsers
            //// TODO remove the old cst nodes
            //// TODO automatically generate parsers

            //// TODO should you compose transcribers like you do parsers and this would be InnerTranscriber.Instance.Many().Transcribe()?
            //// TODO preserve ABNF comments as xmldoc?
            //// TODO it would be nice if collection properties on cst nodes were plural
            //// TODO if you let the "innersclassname" actually be a namespace, then the caller could decide if they want all the cases together, or if they want them separate, or whatever
            //// TODO make "optionals" not be nullable
            //// TODO i don't really like using _ for spaces *and* for the property name conflict resolution
            //// TODO you are entirely skipping out on incremental definitions, by the way
            //// TODO make sure to flesh out the code quality checks for the generated code
            //// TODO it could happen that someojne has first-rule = first-rule / second-rule in which case the du name first-rule with conflict with one of its elements
        }

        public IEnumerable<Class> Generate(_rulelist ruleList)
        {
            //// TODO doing this as the context means that you have to `tolist` all over the place to make sure that the nested classes of `inners` doesn't get modified while it is being enumerated
            var innerClasses = new Dictionary<string, Class>();
            return ruleList
                ._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1
                .Select(
                    _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ => this
                        ._ruleⳆⲤЖcⲻwsp_cⲻnlↃToClass
                        .Visit(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ._ruleⳆⲤЖcⲻwsp_cⲻnlↃ_1, (innerClasses, default)))
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

        private sealed class _ruleⳆⲤЖcⲻwsp_cⲻnlↃToClassGenerator : Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ.Visitor<Class?, (Dictionary<string, Class> InnerClasses, Root.Void @void)>
        {
            private readonly _ruleToClassGenerator ruleToClass;

            public _ruleⳆⲤЖcⲻwsp_cⲻnlↃToClassGenerator(
                string classNamePrefix,
                string @namespace,
                string innersClassName,
                ToClassNames toClassNames)
            {
                this.ruleToClass = new _ruleToClassGenerator(
                    classNamePrefix,
                    @namespace,
                    innersClassName,
                    toClassNames);
            }

            protected internal override Class? Accept(Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule node, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
            {
                return this.ruleToClass.Generate(node._rule_1, context);
            }

            private sealed class _ruleToClassGenerator
            {
                private readonly string classNamePrefix;
                private readonly ToClassNames toClassNames;
                private readonly _elementsToClassGenerator elementsToClass;

                public _ruleToClassGenerator(
                    string classNamePrefix,
                    string @namespace,
                    string innersClassName,
                    ToClassNames toClassNames)
                {
                    this.classNamePrefix = classNamePrefix;
                    this.toClassNames = toClassNames;
                    this.elementsToClass = new _elementsToClassGenerator(
                        this.classNamePrefix,
                        @namespace,
                        innersClassName,
                        toClassNames);
                }

                public Class Generate(_rule rule, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
                {
                    var className = this.classNamePrefix + this.toClassNames.RuleNameToClassName.Generate(rule._rulename_1);
                    return this.elementsToClass.Generate(rule._elements_1, (className, context.InnerClasses));
                }

                private sealed class _elementsToClassGenerator
                {
                    private readonly _alternationToClassGenerator alternationToClass;

                    public _elementsToClassGenerator(
                        string classNamePrefix,
                        string @namespace,
                        string innersClassName,
                        ToClassNames toClassNames)
                    {
                        this.alternationToClass = new _alternationToClassGenerator(
                            classNamePrefix,
                            @namespace,
                            innersClassName,
                            toClassNames);
                    }

                    public Class Generate(_elements elements, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                    {
                        return this.alternationToClass.Generate(elements._alternation_1, context);
                    }

                    private sealed class _alternationToClassGenerator
                    {
                        private readonly _concatenationToClassGenerator concatenationToClass;
                        private readonly _concatenationsToDiscriminatedUnion concatenationsToDiscriminatedUnion;

                        public _alternationToClassGenerator(
                            string classNamePrefix,
                            string @namespace,
                            string innersClassName,
                            ToClassNames toClassNames)
                        {
                            this.concatenationToClass = new _concatenationToClassGenerator(
                                classNamePrefix,
                                @namespace,
                                innersClassName,
                                this,
                                toClassNames);
                            this.concatenationsToDiscriminatedUnion = new _concatenationsToDiscriminatedUnion(
                                classNamePrefix,
                                this.concatenationToClass,
                                toClassNames);
                        }

                        public Class Generate(_alternation alternation, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                        {
                            if (alternation._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1.Any())
                            {
                                return this
                                    .concatenationsToDiscriminatedUnion
                                    .Generate(
                                        alternation
                                            ._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1
                                            .Select(
                                                _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ => _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation_1._concatenation_1)
                                            .Prepend(alternation._concatenation_1),
                                        context);
                            }
                            else
                            {
                                return this.concatenationToClass.Generate(alternation._concatenation_1, (context.ClassName, null, Enumerable.Empty<MethodDefinition>(), context.InnerClasses));
                            }
                        }

                        private sealed class _concatenationToClassGenerator
                        {
                            private readonly _repetitonToPropertyDefinitionGenerator repetitonToPropertyDefinition;

                            public _concatenationToClassGenerator(
                                string classNamePrefix,
                                string @namespace,
                                string innersClassName,
                                _alternationToClassGenerator alternationToClass,
                                ToClassNames toClassNames)
                            {
                                this.repetitonToPropertyDefinition = new _repetitonToPropertyDefinitionGenerator(
                                    classNamePrefix,
                                    @namespace,
                                    innersClassName,
                                    alternationToClass,
                                    toClassNames);
                            }

                            public Class Generate(
                                _concatenation concatenation,
                                (string ClassName, string? BaseType, IEnumerable<MethodDefinition> MethodDefinitions, Dictionary<string, Class> InnerClasses) context)
                            {
                                var propertyTypeToCount = new Dictionary<string, int>();
                                var properties = concatenation
                                    ._Ⲥ1Жcⲻwsp_repetitionↃ_1
                                    .Select(
                                        _Ⲥ1Жcⲻwsp_repetitionↃ => _Ⲥ1Жcⲻwsp_repetitionↃ._1Жcⲻwsp_repetition_1._repetition_1)
                                    .Prepend(
                                        concatenation._repetition_1)
                                    .Select(repetition => this.
                                        repetitonToPropertyDefinition
                                        .Generate(
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

                            private sealed class _repetitonToPropertyDefinitionGenerator
                            {
                                private readonly _elementToPropertyDefinitionGenerator elementToPropertyDefinition;

                                public _repetitonToPropertyDefinitionGenerator(
                                    string classNamePrefix,
                                    string @namespace,
                                    string innersClassName,
                                    _alternationToClassGenerator alternationToClass,
                                    ToClassNames toClassNames)
                                {
                                    this.elementToPropertyDefinition = new _elementToPropertyDefinitionGenerator(
                                        classNamePrefix,
                                        @namespace,
                                        innersClassName,
                                        alternationToClass,
                                        toClassNames);
                                }

                                public PropertyDefinition Generate(_repetition repetition, (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                {
                                    return this
                                        .elementToPropertyDefinition
                                        .Visit(
                                            repetition._element_1,
                                            (repetition._repeat_1 != null, context.PropertyTypeToCount, context.InnerClasses));
                                }

                                private sealed class _elementToPropertyDefinitionGenerator : _element.Visitor<PropertyDefinition, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses)>
                                {
                                    private readonly string classNamePrefix;
                                    private readonly string @namespace;
                                    private readonly string innersClassName;

                                    private readonly _alternationToClassGenerator alternationToClass;
                                    private readonly ToClassNames toClassNames;
                                    private readonly _charⲻvalToClassGenerator _charⲻvalToClass;
                                    private readonly _numⲻvalToClassGenerator _numⲻvalToClass;

                                    public _elementToPropertyDefinitionGenerator(
                                        string classNamePrefix,
                                        string @namespace,
                                        string innersClassName,
                                        _alternationToClassGenerator alternationToClass,
                                        ToClassNames toClassNames)
                                    {
                                        this.classNamePrefix = classNamePrefix;
                                        this.@namespace = @namespace;
                                        this.innersClassName = innersClassName;

                                        this.alternationToClass = alternationToClass;
                                        this.toClassNames = toClassNames;

                                        this._charⲻvalToClass = new _charⲻvalToClassGenerator(classNamePrefix, innersClassName, this.toClassNames);
                                        this._numⲻvalToClass = new _numⲻvalToClassGenerator(classNamePrefix, innersClassName, this.toClassNames);
                                    }

                                    protected internal override PropertyDefinition Accept(_element._rulename node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var ruleName = this.classNamePrefix + this
                                            .toClassNames
                                            .RuleNameToClassName
                                            .Generate(
                                                node._rulename_1);
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

                                    private static bool IsOnlyRuleName(_alternation alternation)
                                    {
                                        return !alternation._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1.Any() &&
                                            !alternation._concatenation_1._Ⲥ1Жcⲻwsp_repetitionↃ_1.Any() &&
                                            alternation._concatenation_1._repetition_1._repeat_1 == null &&
                                            alternation._concatenation_1._repetition_1._element_1 is _element._rulename;
                                    }

                                    protected internal override PropertyDefinition Accept(_element._group node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var isOnlyRuleName = IsOnlyRuleName(node._group_1._alternation_1);

                                        var groupInnerClassName = this.classNamePrefix + this.toClassNames.AlternationToClassName.Generate(node._group_1._alternation_1);
                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(groupInnerClassName))
                                        {
                                            context.InnerClasses[groupInnerClassName] = this.alternationToClass.Generate(node._group_1._alternation_1, (groupInnerClassName, context.InnerClasses));
                                        }

                                        var groupClassName = this.classNamePrefix + this.toClassNames.GroupToClassName.Generate(node._group_1);

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

                                    protected internal override PropertyDefinition Accept(_element._option node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var isOnlyRuleName = IsOnlyRuleName(node._option_1._alternation_1);
                                        var innerClassName = this.classNamePrefix + this.toClassNames.AlternationToClassName.Generate(node._option_1._alternation_1);

                                        if (!isOnlyRuleName && !context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            context.InnerClasses[innerClassName] = this.
                                                alternationToClass
                                                .Generate(
                                                    node._option_1._alternation_1,
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

                                    protected internal override PropertyDefinition Accept(_element._charⲻval node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = this.classNamePrefix + this.toClassNames.CharValToClassName.Generate(node._charⲻval_1);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            var innerClass = this._charⲻvalToClass.Generate(node._charⲻval_1, (innerClassName, context.InnerClasses));

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

                                    protected internal override PropertyDefinition Accept(_element._numⲻval node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        var innerClassName = this.classNamePrefix + this.toClassNames.NumValToClassName.Generate(node._numⲻval_1);

                                        if (!context.InnerClasses.ContainsKey(innerClassName))
                                        {
                                            var innerClass = this._numⲻvalToClass.Visit(
                                                node._numⲻval_1._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1._binⲻvalⳆdecⲻvalⳆhexⲻval_1,
                                                (innerClassName, context.InnerClasses));

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

                                    protected internal override PropertyDefinition Accept(_element._proseⲻval node, (bool IsCollection, Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                    {
                                        throw new NotImplementedException("TODO");
                                    }

                                    private sealed class _charⲻvalToClassGenerator
                                    {
                                        private readonly _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinitionGenerator _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinition;

                                        public _charⲻvalToClassGenerator(string classNamePrefix, string innersClassName, ToClassNames toClassNames)
                                        {
                                            this._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinition = new _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinitionGenerator(classNamePrefix, innersClassName, toClassNames);
                                        }

                                        public Class Generate(_charⲻval charVal, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            var propertyTypeToCount = new Dictionary<string, int>();
                                            var properties = charVal
                                                ._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1
                                                .Select(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ => this
                                                    ._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinition
                                                    .Generate(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ, (propertyTypeToCount, context.InnerClasses)))
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

                                        private sealed class _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinitionGenerator
                                        {
                                            private readonly string classNamePrefix;
                                            private readonly string innersClassName;
                                            private readonly ToClassNames toClassNames;

                                            public _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToPropertyDefinitionGenerator(string classNamePrefix, string innersClassName, ToClassNames toClassNames)
                                            {
                                                this.classNamePrefix = classNamePrefix;
                                                this.innersClassName = innersClassName;
                                                this.toClassNames = toClassNames;
                                            }

                                            public PropertyDefinition Generate(
                                                Inners._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ,
                                                (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                var stringBuilder = new StringBuilder();
                                                this.toClassNames._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName.Visit(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E_1, stringBuilder);
                                                var className = this.classNamePrefix + stringBuilder.ToString();

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

                                    private sealed class _numⲻvalToClassGenerator : Inners._binⲻvalⳆdecⲻvalⳆhexⲻval.Visitor<Class, (string ClassName, Dictionary<string, Class> InnerClasses)>
                                    {
                                        private readonly _hexⲻvalToClassGenerator _hexⲻvalToClass;

                                        public _numⲻvalToClassGenerator(
                                            string classNamePrefix,
                                            string innersClassName,
                                            ToClassNames toClassNames)
                                        {
                                            this._hexⲻvalToClass = new _hexⲻvalToClassGenerator(classNamePrefix, innersClassName, toClassNames);
                                        }

                                        protected internal override Class Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            throw new NotImplementedException();
                                        }

                                        protected internal override Class Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            throw new NotImplementedException();
                                        }

                                        protected internal override Class Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval node, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                        {
                                            return this._hexⲻvalToClass.Generate(node._hexⲻval_1, context);
                                        }

                                        private sealed class _hexⲻvalToClassGenerator
                                        {
                                            private readonly string classNamePrefix;
                                            private readonly ToClassNames toClassNames;

                                            private readonly HexDigsToClassGenerator hexDigsToClass;
                                            private readonly _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClassGenerator _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClass;

                                            public _hexⲻvalToClassGenerator(string classNamePrefix, string innersClassName, ToClassNames toClassNames)
                                            {
                                                this.classNamePrefix = classNamePrefix;
                                                this.toClassNames = toClassNames;
                                                this.hexDigsToClass = new HexDigsToClassGenerator(
                                                    this.classNamePrefix,
                                                    innersClassName,
                                                    this.toClassNames);
                                                this._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClass = new _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClassGenerator(
                                                    this.classNamePrefix,
                                                    innersClassName,
                                                    this.toClassNames,
                                                    this.hexDigsToClass);
                                            }

                                            public Class Generate(_hexⲻval hexⲻval, (string ClassName, Dictionary<string, Class> InnerClasses) context)
                                            {
                                                if (hexⲻval._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 == null)
                                                {
                                                    return this.hexDigsToClass.Generate(hexⲻval._HEXDIG_1, (context.ClassName, null, context.InnerClasses));
                                                }
                                                else
                                                {
                                                    return this._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClass.Visit(hexⲻval._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1, (context.ClassName, context.InnerClasses, hexⲻval));
                                                }
                                            }

                                            private sealed class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClassGenerator : Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ.Visitor<Class, (string ClassName, Dictionary<string, Class> InnerClasses, _hexⲻval hexⲻval)>
                                            {
                                                private readonly string classNamePrefix;
                                                private readonly ToClassNames toClassNames;
                                                private readonly HexDigsToClassGenerator hexDigsToClass;

                                                private readonly SegmentsToPropertyDefinitionsGenerator segmentsToPropertyDefinitions;

                                                public _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃToClassGenerator(
                                                    string classNamePrefix, 
                                                    string innersClassName, 
                                                    ToClassNames toClassNames, 
                                                    HexDigsToClassGenerator hexDigsToClass)
                                                {
                                                    this.classNamePrefix = classNamePrefix;
                                                    this.toClassNames = toClassNames;
                                                    this.hexDigsToClass = hexDigsToClass;

                                                    this.segmentsToPropertyDefinitions = new SegmentsToPropertyDefinitionsGenerator(
                                                        this.classNamePrefix,
                                                        innersClassName,
                                                        this.hexDigsToClass,
                                                        this.toClassNames);
                                                }

                                                protected internal override Class Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ node, (string ClassName, Dictionary<string, Class> InnerClasses, _hexⲻval hexⲻval) context)
                                                {
                                                    var segments = node
                                                        ._Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1
                                                        .Select(_Ⲥʺx2Eʺ_1ЖHEXDIGↃ => _Ⲥʺx2Eʺ_1ЖHEXDIGↃ._ʺx2Eʺ_1ЖHEXDIG_1._HEXDIG_1)
                                                        .Prepend(context.hexⲻval._HEXDIG_1);

                                                    var propertyTypeToCount = new Dictionary<string, int>();
                                                    var properties = this
                                                        .segmentsToPropertyDefinitions
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
                                                                        property.Type, //// TODO should this and other calls lower in the stack use the inners class qualifier?
                                                                        property.Name)),
                                                            properties
                                                                .Select(property =>
                                                                    $"this.{property.Name} = {property.Name};")),
                                                        },
                                                        Enumerable.Empty<MethodDefinition>(),
                                                        Enumerable.Empty<Class>(),
                                                        properties);
                                                }

                                                protected internal override Class Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ node, (string ClassName, Dictionary<string, Class> InnerClasses, _hexⲻval hexⲻval) context)
                                                {
                                                    var range = HexDigsRange(
                                                        context.hexⲻval._HEXDIG_1.ToList(),
                                                        node._Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1._ʺx2Dʺ_1ЖHEXDIG_1._HEXDIG_1.ToList());
                                                    var duElements = range
                                                            .Select(hexDigs => this
                                                                .hexDigsToClass
                                                                .Generate(
                                                                    hexDigs,
                                                                    (this.classNamePrefix + this.toClassNames.HexDigsToClassName.Generate(hexDigs), context.ClassName, context.InnerClasses)))
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

                                                private static IEnumerable<IEnumerable<_HEXDIG>> HexDigsRange(
                                                    IReadOnlyList<_HEXDIG> low,
                                                    IReadOnlyList<_HEXDIG> high)
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

                                                private static IReadOnlyList<_HEXDIG> Next(IReadOnlyList<_HEXDIG> previous)
                                                {
                                                    var list = new _HEXDIG[previous.Count];

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

                                                private static bool HexDigsEqual(IReadOnlyList<_HEXDIG> first, IReadOnlyList<_HEXDIG> second)
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

                                                private sealed class HexDigPlusOne : _HEXDIG.Visitor<(_HEXDIG HexDig, bool Overflow), Root.Void>
                                                {
                                                    private HexDigPlusOne()
                                                    {
                                                    }

                                                    public static HexDigPlusOne Instance { get; } = new HexDigPlusOne();

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._DIGIT node, Root.Void context)
                                                    {
                                                        return (DigitPlusOne.Instance.Visit(node._DIGIT_1._Ⰳx30ⲻ39_1, default), false);
                                                    }

                                                    private sealed class DigitPlusOne : Inners._Ⰳx30ⲻ39.Visitor<_HEXDIG, Root.Void>
                                                    {
                                                        private DigitPlusOne()
                                                        {
                                                        }

                                                        public static DigitPlusOne Instance { get; } = new DigitPlusOne();

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._30 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._31(
                                                                        Inners._3.Instance,
                                                                        Inners._1.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._31 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._32(
                                                                        Inners._3.Instance,
                                                                        Inners._2.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._32 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._33(
                                                                        Inners._3.Instance,
                                                                        Inners._3.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._33 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._34(
                                                                        Inners._3.Instance,
                                                                        Inners._4.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._34 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._35(
                                                                        Inners._3.Instance,
                                                                        Inners._5.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._35 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._36(
                                                                        Inners._3.Instance,
                                                                        Inners._6.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._36 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._37(
                                                                        Inners._3.Instance,
                                                                        Inners._7.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._37 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._38(
                                                                        Inners._3.Instance,
                                                                        Inners._8.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._38 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._DIGIT(
                                                                new _DIGIT(
                                                                    new Inners._Ⰳx30ⲻ39._39(
                                                                        Inners._3.Instance,
                                                                        Inners._9.Instance)));
                                                        }

                                                        protected internal override _HEXDIG Accept(Inners._Ⰳx30ⲻ39._39 node, Root.Void context)
                                                        {
                                                            return new _HEXDIG._ʺx41ʺ(
                                                                new Inners._ʺx41ʺ(
                                                                    Inners._x41.Instance));
                                                        }
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx41ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._ʺx42ʺ(new Inners._ʺx42ʺ(Inners._x42.Instance)), false);
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx42ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._ʺx43ʺ(new Inners._ʺx43ʺ(Inners._x43.Instance)), false);
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx43ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._ʺx44ʺ(new Inners._ʺx44ʺ(Inners._x44.Instance)), false);
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx44ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._ʺx45ʺ(new Inners._ʺx45ʺ(Inners._x45.Instance)), false);
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx45ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._ʺx46ʺ(new Inners._ʺx46ʺ(Inners._x46.Instance)), false);
                                                    }

                                                    protected internal override (_HEXDIG HexDig, bool Overflow) Accept(_HEXDIG._ʺx46ʺ node, Root.Void context)
                                                    {
                                                        return (new _HEXDIG._DIGIT(new _DIGIT(new Inners._Ⰳx30ⲻ39._30(Inners._3.Instance, Inners._0.Instance))), true);
                                                    }
                                                }

                                                private sealed class HexDigEqual : _HEXDIG.Visitor<bool, _HEXDIG>
                                                {
                                                    private HexDigEqual()
                                                    {
                                                    }

                                                    public static HexDigEqual Instance { get; } = new HexDigEqual();

                                                    protected internal override bool Accept(_HEXDIG._DIGIT node, _HEXDIG context)
                                                    {
                                                        if (!(context is _HEXDIG._DIGIT digit))
                                                        {
                                                            return false;
                                                        }

                                                        return DigitEqual.Instance.Visit(node._DIGIT_1._Ⰳx30ⲻ39_1, digit._DIGIT_1._Ⰳx30ⲻ39_1);
                                                    }

                                                    private sealed class DigitEqual : Inners._Ⰳx30ⲻ39.Visitor<bool, Inners._Ⰳx30ⲻ39>
                                                    {
                                                        private DigitEqual()
                                                        {
                                                        }

                                                        public static DigitEqual Instance { get; } = new DigitEqual();

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._30 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._30;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._31 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._31;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._32 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._32;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._33 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._33;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._34 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._34;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._35 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._35;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._36 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._36;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._37 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._37;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._38 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._38;
                                                        }

                                                        protected internal override bool Accept(Inners._Ⰳx30ⲻ39._39 node, Inners._Ⰳx30ⲻ39 context)
                                                        {
                                                            return context is Inners._Ⰳx30ⲻ39._39;
                                                        }
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx41ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx41ʺ;
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx42ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx42ʺ;
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx43ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx43ʺ;
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx44ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx44ʺ;
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx45ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx45ʺ;
                                                    }

                                                    protected internal override bool Accept(_HEXDIG._ʺx46ʺ node, _HEXDIG context)
                                                    {
                                                        return context is _HEXDIG._ʺx46ʺ;
                                                    }
                                                }

                                                private sealed class SegmentsToPropertyDefinitionsGenerator
                                                {
                                                    private readonly string classNamePrefix;
                                                    private readonly string innersClassName;
                                                    private readonly HexDigsToClassGenerator hexDigsToClass;
                                                    private readonly ToClassNames toClassNames;

                                                    public SegmentsToPropertyDefinitionsGenerator(
                                                        string classNamePrefix,
                                                        string innersClassName,
                                                        HexDigsToClassGenerator hexDigsToClass,
                                                        ToClassNames toClassNames)
                                                    {
                                                        this.classNamePrefix = classNamePrefix;
                                                        this.innersClassName = innersClassName;
                                                        this.hexDigsToClass = hexDigsToClass;
                                                        this.toClassNames = toClassNames;
                                                    }

                                                    public IEnumerable<PropertyDefinition> Generate(
                                                        IEnumerable<IEnumerable<_HEXDIG>> segments,
                                                        (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                    {
                                                        foreach (var segment in segments)
                                                        {
                                                            var className = this.classNamePrefix + this.toClassNames.HexDigsToClassName.Generate(segment);
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
                                            }

                                            private sealed class HexDigsToClassGenerator
                                            {
                                                private readonly HexDigsToPropertyDefinitionsGenerator HexDigsToPropertyDefinitions;

                                                public HexDigsToClassGenerator(
                                                    string classNamePrefix,
                                                    string innersClassName,
                                                    ToClassNames toClassNames)
                                                {
                                                    this.HexDigsToPropertyDefinitions = new HexDigsToPropertyDefinitionsGenerator(
                                                        classNamePrefix,
                                                        innersClassName,
                                                        toClassNames);
                                                }

                                                public Class Generate(
                                                    IEnumerable<_HEXDIG> hexDigs,
                                                    (string ClassName, string? BaseClass, Dictionary<string, Class> InnerClasses) context)
                                                {
                                                    var propertyTypeToCount = new Dictionary<string, int>();
                                                    var properties = this
                                                        .HexDigsToPropertyDefinitions
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

                                                private sealed class HexDigsToPropertyDefinitionsGenerator
                                                {
                                                    private readonly string classNamePrefix;
                                                    private readonly string innersClassName;
                                                    private readonly ToClassNames toClassNames;

                                                    public HexDigsToPropertyDefinitionsGenerator(
                                                        string classNamePrefix,
                                                        string innersClassName,
                                                        ToClassNames toClassNames)
                                                    {
                                                        this.classNamePrefix = classNamePrefix;
                                                        this.innersClassName = innersClassName;
                                                        this.toClassNames = toClassNames;
                                                    }

                                                    public IEnumerable<PropertyDefinition> Generate(
                                                        IEnumerable<_HEXDIG> hexDigs,
                                                        (Dictionary<string, int> PropertyTypeToCount, Dictionary<string, Class> InnerClasses) context)
                                                    {
                                                        foreach (var hexDig in hexDigs)
                                                        {
                                                            var className = this.classNamePrefix + this.toClassNames.HexDigToClassName.Visit(hexDig, default);
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
                                }
                            }
                        }

                        private sealed class _concatenationsToDiscriminatedUnion
                        {
                            private readonly string classNamePrefix;
                            private readonly _concatenationToClassGenerator concatenationToClass;
                            private readonly ToClassNames toClassNames;

                            public _concatenationsToDiscriminatedUnion(
                                string classNamePrefix,
                                _concatenationToClassGenerator concatenationToClass,
                                ToClassNames toClassNames)
                            {
                                this.classNamePrefix = classNamePrefix;
                                this.concatenationToClass = concatenationToClass;
                                this.toClassNames = toClassNames;
                            }

                            public Class Generate(IEnumerable<_concatenation> concatenations, (string ClassName, Dictionary<string, Class> InnerClasses) context)
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
                                            (this.classNamePrefix + this.toClassNames.ConcatenationToClassName.Generate(concatenation), context.ClassName, new[] { dispatchMethod }, context.InnerClasses)))
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

            protected internal override Class? Accept(Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ node, (Dictionary<string, Class> InnerClasses, Root.Void @void) context)
            {
                return null;
            }
        }

        /// <summary>
        /// TODO i'm using this class because i don't remember the "right" way to do mutual recursion //// TODO instead of types you can just have methods for the non-visitors
        /// </summary>
        private sealed class ToClassNames
        {
            public ToClassNames(CharacterSubstitutions characterSubstitutions)
            {
                //// TODO use builders and transcribers
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
                this._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName = _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName.Instance;
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

            public _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName { get; }

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

            public string Generate(_group group)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(characterSubstitutions.OpenParenthesis);
                stringBuilder.Append(this.toClassNames.AlternationToClassName.Generate(group._alternation_1));
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

            public string Generate(_alternation alternation)
            {
                var className = this.toClassNames.ConcatenationToClassName.Generate(alternation._concatenation_1);
                foreach (var _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ in alternation._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1)
                {
                    className += $"{characterSubstitutions.Slash}{this.toClassNames.ConcatenationToClassName.Generate(_ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation_1._concatenation_1)}";
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

            public string Generate(_concatenation concatenation)
            {
                var className = this.toClassNames.RepetitionToClassName.Generate(concatenation._repetition_1);
                foreach (var _Ⲥ1Жcⲻwsp_repetitionↃ_1 in concatenation._Ⲥ1Жcⲻwsp_repetitionↃ_1)
                {
                    className += $"{characterSubstitutions.Space}{this.toClassNames.RepetitionToClassName.Generate(_Ⲥ1Жcⲻwsp_repetitionↃ_1._1Жcⲻwsp_repetition_1._repetition_1)}";
                }

                return className;
            }
        }

        private sealed class RepetitionToClassName
        {
            private readonly ToClassNames toClassNames;

            public RepetitionToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(_repetition repetition)
            {
                var result = string.Empty;
                if (repetition._repeat_1 != null)
                {
                    result = this.toClassNames.RepeatToClassName.Visit(repetition._repeat_1, default);
                }

                return result + this.toClassNames.ElementToClassName.Visit(repetition._element_1, default);
            }
        }

        private sealed class RepeatToClassName : _repeat.Visitor<string, Root.Void>
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;

            public RepeatToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(_repeat._1ЖDIGIT node, Root.Void context)
            {
                return this.toClassNames.DigitsToClassName.Generate(node._DIGIT_1);
            }

            protected internal override string Accept(_repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, Root.Void context)
            {
                var stringBuilder = new StringBuilder();
                if (node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1._ЖDIGIT_ʺx2Aʺ_ЖDIGIT_1._DIGIT_1.Any()) //// TODO do you need the `any` check?
                {
                    stringBuilder.Append(this.toClassNames.DigitsToClassName.Generate(node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1._ЖDIGIT_ʺx2Aʺ_ЖDIGIT_1._DIGIT_1));
                }

                stringBuilder.Append(characterSubstitutions.Asterisk);

                if (node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1._ЖDIGIT_ʺx2Aʺ_ЖDIGIT_1._DIGIT_2.Any()) //// TODO do you need the `any` check?
                {
                    stringBuilder.Append(this.toClassNames.DigitsToClassName.Generate(node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1._ЖDIGIT_ʺx2Aʺ_ЖDIGIT_1._DIGIT_2));
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

            public string Generate(IEnumerable<_DIGIT> digits)
            {
                var stringBuilder = new StringBuilder();
                foreach (var digit in digits)
                {
                    stringBuilder.Append(this.toClassNames.DigitToClassName.Generate(digit));
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class ElementToClassName : _element.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public ElementToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(_element._rulename node, Root.Void context)
            {
                return this.toClassNames.RuleNameToClassName.Generate(node._rulename_1);
            }

            protected internal override string Accept(_element._group node, Root.Void context)
            {
                return this.toClassNames.GroupToClassName.Generate(node._group_1);
            }

            protected internal override string Accept(_element._option node, Root.Void context)
            {
                return this.toClassNames.OptionToClassName.Generate(node._option_1);
            }

            protected internal override string Accept(_element._charⲻval node, Root.Void context)
            {
                return this.toClassNames.CharValToClassName.Generate(node._charⲻval_1);
            }

            protected internal override string Accept(_element._numⲻval node, Root.Void context)
            {
                return this.toClassNames.NumValToClassName.Generate(node._numⲻval_1);
            }

            protected internal override string Accept(_element._proseⲻval node, Root.Void context)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class NumValToClassName
        {
            private readonly Visitor visitor;

            public NumValToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.visitor = new Visitor(characterSubstitutions, toClassNames);
            }

            public string Generate(_numⲻval numⲻval)
            {
                return this.visitor.Visit(numⲻval._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1._binⲻvalⳆdecⲻvalⳆhexⲻval_1, default);
            }

            private sealed class Visitor : Inners._binⲻvalⳆdecⲻvalⳆhexⲻval.Visitor<string, Root.Void>
            {
                private readonly CharacterSubstitutions characterSubstitutions;
                private readonly ToClassNames toClassNames;

                public Visitor(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
                {
                    this.characterSubstitutions = characterSubstitutions;
                    this.toClassNames = toClassNames;
                }

                protected internal override string Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval node, Root.Void context)
                {
                    return $"{characterSubstitutions.Percent}{this.toClassNames.BinValToClassName.Generate(node._binⲻval_1)}";
                }

                protected internal override string Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval node, Root.Void context)
                {
                    return $"{characterSubstitutions.Percent}{this.toClassNames.DecValToClassName.Generate(node._decⲻval_1)}";
                }

                protected internal override string Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval node, Root.Void context)
                {
                    return $"{characterSubstitutions.Percent}{this.toClassNames.HexValToClassName.Generate(node._hexⲻval_1)}";
                }
            }
        }

        private sealed class BinValToClassName
        {
            private readonly ToClassNames toClassNames;

            public BinValToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(_binⲻval binⲻval)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class DecValToClassName
        {
            private readonly ToClassNames toClassNames;

            public DecValToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(_decⲻval decⲻval)
            {
                throw new NotImplementedException("TODO");
            }
        }

        private sealed class HexValToClassName
        {
            private readonly CharacterSubstitutions characterSubstitutions;
            private readonly ToClassNames toClassNames;
            private readonly Visitor visitor;

            public HexValToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
            {
                this.characterSubstitutions = characterSubstitutions;
                this.toClassNames = toClassNames;

                this.visitor = new Visitor(this.characterSubstitutions, this.toClassNames);
            }

            public string Generate(_hexⲻval hexⲻval)
            {
                var builder = new StringBuilder();
                builder.Append("x");

                builder.Append(this.toClassNames.HexDigsToClassName.Generate(hexⲻval._HEXDIG_1));

                if (hexⲻval._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 != null)
                {
                    this.visitor.Visit(hexⲻval._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1, builder);
                }

                return builder.ToString();
            }

            private sealed class Visitor : Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ.Visitor<Root.Void, StringBuilder>
            {
                private readonly CharacterSubstitutions characterSubstitutions;
                private readonly ToClassNames toClassNames;

                public Visitor(CharacterSubstitutions characterSubstitutions, ToClassNames toClassNames)
                {
                    this.characterSubstitutions = characterSubstitutions;
                    this.toClassNames = toClassNames;
                }

                protected internal override Root.Void Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ node, StringBuilder context)
                {
                    foreach (var _Ⲥʺx2Eʺ_1ЖHEXDIGↃ in node._Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1)
                    {
                        context.Append(this.characterSubstitutions.Period);
                        context.Append(this.toClassNames.HexDigsToClassName.Generate(_Ⲥʺx2Eʺ_1ЖHEXDIGↃ._ʺx2Eʺ_1ЖHEXDIG_1._HEXDIG_1));
                    }

                    return default;
                }

                protected internal override Root.Void Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ node, StringBuilder context)
                {
                    context.Append(this.characterSubstitutions.Dash);
                    context.Append(this.toClassNames.HexDigsToClassName.Generate(node._Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1._ʺx2Dʺ_1ЖHEXDIG_1._HEXDIG_1));

                    return default;
                }
            }
        }

        private sealed class HexDigsToClassName
        {
            private readonly ToClassNames toClassNames;

            public HexDigsToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(IEnumerable<_HEXDIG> hexDigs)
            {
                var stringBuilder = new StringBuilder();
                foreach (var hexDig in hexDigs)
                {
                    stringBuilder.Append(this.toClassNames.HexDigToClassName.Visit(hexDig, default));
                }

                return stringBuilder.ToString();
            }
        }

        private sealed class HexDigToClassName : _HEXDIG.Visitor<string, Root.Void>
        {
            private readonly ToClassNames toClassNames;

            public HexDigToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            protected internal override string Accept(_HEXDIG._DIGIT node, Root.Void context)
            {
                return this.toClassNames.DigitToClassName.Generate(node._DIGIT_1);
            }

            protected internal override string Accept(_HEXDIG._ʺx41ʺ node, Root.Void context)
            {
                return "A";
            }

            protected internal override string Accept(_HEXDIG._ʺx42ʺ node, Root.Void context)
            {
                return "B";
            }

            protected internal override string Accept(_HEXDIG._ʺx43ʺ node, Root.Void context)
            {
                return "C";
            }

            protected internal override string Accept(_HEXDIG._ʺx44ʺ node, Root.Void context)
            {
                return "D";
            }

            protected internal override string Accept(_HEXDIG._ʺx45ʺ node, Root.Void context)
            {
                return "E";
            }

            protected internal override string Accept(_HEXDIG._ʺx46ʺ node, Root.Void context)
            {
                return "F";
            }
        }

        private sealed class DigitToClassName
        {
            private readonly ToClassNames toClassNames;

            public DigitToClassName(ToClassNames toClassNames)
            {
                this.toClassNames = toClassNames;
            }

            public string Generate(_DIGIT digit)
            {
                return Visitor.Instance.Visit(digit._Ⰳx30ⲻ39_1, default);
            }

            private sealed class Visitor : Inners._Ⰳx30ⲻ39.Visitor<string, Root.Void>
            {
                private Visitor()
                {
                }

                public static Visitor Instance { get; } = new Visitor();

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._30 node, Root.Void context)
                {
                    return "0";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._31 node, Root.Void context)
                {
                    return "1";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._32 node, Root.Void context)
                {
                    return "2";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._33 node, Root.Void context)
                {
                    return "3";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._34 node, Root.Void context)
                {
                    return "4";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._35 node, Root.Void context)
                {
                    return "5";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._36 node, Root.Void context)
                {
                    return "6";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._37 node, Root.Void context)
                {
                    return "7";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._38 node, Root.Void context)
                {
                    return "8";
                }

                protected internal override string Accept(Inners._Ⰳx30ⲻ39._39 node, Root.Void context)
                {
                    return "9";
                }
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

            public string Generate(_charⲻval charⲻval)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(this.characterSubstitutions.DoubleQuote);
                foreach (var _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ in charⲻval._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1)
                {
                    this.toClassNames._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName.Visit(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E_1, stringBuilder);
                }

                stringBuilder.Append(this.characterSubstitutions.DoubleQuote);

                return stringBuilder.ToString();
            }
        }

        private sealed class _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName : Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E.Visitor<Root.Void, StringBuilder>
        {
            private _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName()
            {
            }

            public static _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName Instance { get; } = new _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃToClassName();

            protected internal override Root.Void Accept(Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx20ⲻ21 node, StringBuilder context)
            {
                _Ⰳx20ⲻ21Visitor.Instance.Visit(node._Ⰳx20ⲻ21_1, context);

                return default;
            }

            protected internal override Root.Void Accept(Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx23ⲻ7E node, StringBuilder context)
            {
                _Ⰳx23ⲻ7EVisitor.Instance.Visit(node._Ⰳx23ⲻ7E_1, context);

                return default;
            }

            private sealed class _Ⰳx20ⲻ21Visitor : Inners._Ⰳx20ⲻ21.Visitor<Root.Void, StringBuilder>
            {
                private _Ⰳx20ⲻ21Visitor()
                {
                }

                public static _Ⰳx20ⲻ21Visitor Instance { get; } = new _Ⰳx20ⲻ21Visitor();

                protected internal override Root.Void Accept(Inners._Ⰳx20ⲻ21._20 node, StringBuilder context)
                {
                    //// TODO what you could do here is `context.Append((char)0x20)`; you're doing what's here currently because you are being compatbile with something you did previously
                    context.Append("x20");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx20ⲻ21._21 node, StringBuilder context)
                {
                    context.Append("x21");

                    return default;
                }
            }

            public sealed class _Ⰳx23ⲻ7EVisitor : Inners._Ⰳx23ⲻ7E.Visitor<Root.Void, StringBuilder>
            {
                private _Ⰳx23ⲻ7EVisitor()
                {
                }

                public static _Ⰳx23ⲻ7EVisitor Instance { get; } = new _Ⰳx23ⲻ7EVisitor();

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._23 node, StringBuilder context)
                {
                    //// TODO what you could do here is `context.Append((char)0x20)`; you're doing what's here currently because you are being compatbile with something you did previously
                    context.Append("x23");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._24 node, StringBuilder context)
                {
                    context.Append("x24");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._25 node, StringBuilder context)
                {
                    context.Append("x25");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._26 node, StringBuilder context)
                {
                    context.Append("x26");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._27 node, StringBuilder context)
                {
                    context.Append("x27");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._28 node, StringBuilder context)
                {
                    context.Append("x28");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._29 node, StringBuilder context)
                {
                    context.Append("x29");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2A node, StringBuilder context)
                {
                    context.Append("x2A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2B node, StringBuilder context)
                {
                    context.Append("x2B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2C node, StringBuilder context)
                {
                    context.Append("x2C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2D node, StringBuilder context)
                {
                    context.Append("x2D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2E node, StringBuilder context)
                {
                    context.Append("x2E");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._2F node, StringBuilder context)
                {
                    context.Append("x2F");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._30 node, StringBuilder context)
                {
                    context.Append("x30");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._31 node, StringBuilder context)
                {
                    context.Append("x31");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._32 node, StringBuilder context)
                {
                    context.Append("x32");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._33 node, StringBuilder context)
                {
                    context.Append("x33");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._34 node, StringBuilder context)
                {
                    context.Append("x34");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._35 node, StringBuilder context)
                {
                    context.Append("x35");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._36 node, StringBuilder context)
                {
                    context.Append("x36");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._37 node, StringBuilder context)
                {
                    context.Append("x37");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._38 node, StringBuilder context)
                {
                    context.Append("x38");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._39 node, StringBuilder context)
                {
                    context.Append("x39");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3A node, StringBuilder context)
                {
                    context.Append("x3A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3B node, StringBuilder context)
                {
                    context.Append("x3B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3C node, StringBuilder context)
                {
                    context.Append("x3C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3D node, StringBuilder context)
                {
                    context.Append("x3D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3E node, StringBuilder context)
                {
                    context.Append("x3E");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._3F node, StringBuilder context)
                {
                    context.Append("x3F");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._40 node, StringBuilder context)
                {
                    context.Append("x40");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._41 node, StringBuilder context)
                {
                    context.Append("x41");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._42 node, StringBuilder context)
                {
                    context.Append("x42");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._43 node, StringBuilder context)
                {
                    context.Append("x43");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._44 node, StringBuilder context)
                {
                    context.Append("x44");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._45 node, StringBuilder context)
                {
                    context.Append("x45");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._46 node, StringBuilder context)
                {
                    context.Append("x46");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._47 node, StringBuilder context)
                {
                    context.Append("x47");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._48 node, StringBuilder context)
                {
                    context.Append("x48");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._49 node, StringBuilder context)
                {
                    context.Append("x49");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4A node, StringBuilder context)
                {
                    context.Append("x4A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4B node, StringBuilder context)
                {
                    context.Append("x4B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4C node, StringBuilder context)
                {
                    context.Append("x4C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4D node, StringBuilder context)
                {
                    context.Append("x4D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4E node, StringBuilder context)
                {
                    context.Append("x4E");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._4F node, StringBuilder context)
                {
                    context.Append("x4F");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._50 node, StringBuilder context)
                {
                    context.Append("x50");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._51 node, StringBuilder context)
                {
                    context.Append("x51");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._52 node, StringBuilder context)
                {
                    context.Append("x52");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._53 node, StringBuilder context)
                {
                    context.Append("x53");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._54 node, StringBuilder context)
                {
                    context.Append("x54");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._55 node, StringBuilder context)
                {
                    context.Append("x55");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._56 node, StringBuilder context)
                {
                    context.Append("x56");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._57 node, StringBuilder context)
                {
                    context.Append("x57");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._58 node, StringBuilder context)
                {
                    context.Append("x58");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._59 node, StringBuilder context)
                {
                    context.Append("x59");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5A node, StringBuilder context)
                {
                    context.Append("x5A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5B node, StringBuilder context)
                {
                    context.Append("x5B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5C node, StringBuilder context)
                {
                    context.Append("x5C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5D node, StringBuilder context)
                {
                    context.Append("x5D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5E node, StringBuilder context)
                {
                    context.Append("x5E");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._5F node, StringBuilder context)
                {
                    context.Append("x5F");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._60 node, StringBuilder context)
                {
                    context.Append("x60");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._61 node, StringBuilder context)
                {
                    context.Append("x61");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._62 node, StringBuilder context)
                {
                    context.Append("x62");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._63 node, StringBuilder context)
                {
                    context.Append("x63");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._64 node, StringBuilder context)
                {
                    context.Append("x64");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._65 node, StringBuilder context)
                {
                    context.Append("x65");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._66 node, StringBuilder context)
                {
                    context.Append("x66");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._67 node, StringBuilder context)
                {
                    context.Append("x67");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._68 node, StringBuilder context)
                {
                    context.Append("x68");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._69 node, StringBuilder context)
                {
                    context.Append("x69");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6A node, StringBuilder context)
                {
                    context.Append("x6A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6B node, StringBuilder context)
                {
                    context.Append("x6B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6C node, StringBuilder context)
                {
                    context.Append("x6C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6D node, StringBuilder context)
                {
                    context.Append("x6D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6E node, StringBuilder context)
                {
                    context.Append("x6E");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._6F node, StringBuilder context)
                {
                    context.Append("x6F");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._70 node, StringBuilder context)
                {
                    context.Append("x70");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._71 node, StringBuilder context)
                {
                    context.Append("x71");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._72 node, StringBuilder context)
                {
                    context.Append("x72");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._73 node, StringBuilder context)
                {
                    context.Append("x73");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._74 node, StringBuilder context)
                {
                    context.Append("x74");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._75 node, StringBuilder context)
                {
                    context.Append("x75");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._76 node, StringBuilder context)
                {
                    context.Append("x76");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._77 node, StringBuilder context)
                {
                    context.Append("x77");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._78 node, StringBuilder context)
                {
                    context.Append("x78");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._79 node, StringBuilder context)
                {
                    context.Append("x79");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._7A node, StringBuilder context)
                {
                    context.Append("x7A");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._7B node, StringBuilder context)
                {
                    context.Append("x7B");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._7C node, StringBuilder context)
                {
                    context.Append("x7C");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._7D node, StringBuilder context)
                {
                    context.Append("x7D");

                    return default;
                }

                protected internal override Root.Void Accept(Inners._Ⰳx23ⲻ7E._7E node, StringBuilder context)
                {
                    context.Append("x7E");

                    return default;
                }
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

            public string Generate(_option option)
            {
                return $"{this.characterSubstitutions.OpenBracket}{this.toClassNames.AlternationToClassName.Generate(option._alternation_1)}{this.characterSubstitutions.CloseBracket}";
            }
        }

        private sealed class RuleNameToClassName
        {
            private readonly Visitor visitor;

            public RuleNameToClassName(CharacterSubstitutions characterSubstitutions, ToClassNames toClassName)
            {
                this.visitor = new Visitor(characterSubstitutions);
            }

            public string Generate(_rulename ruleName)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(_ALPHAToChar.Instance.Visit(ruleName._ALPHA_1, default));
                foreach (var _ⲤALPHAⳆDIGITⳆʺx2DʺↃ in ruleName._ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1)
                {
                    this.visitor.Visit(_ⲤALPHAⳆDIGITⳆʺx2DʺↃ._ALPHAⳆDIGITⳆʺx2Dʺ_1, stringBuilder);
                }

                return stringBuilder.ToString();
            }

            private sealed class Visitor : Inners._ALPHAⳆDIGITⳆʺx2Dʺ.Visitor<Root.Void, StringBuilder>
            {
                private readonly CharacterSubstitutions characterSubstitutions;

                public Visitor(CharacterSubstitutions characterSubstitutions)
                {
                    this.characterSubstitutions = characterSubstitutions;
                }

                protected internal override Root.Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA node, StringBuilder context)
                {
                    context.Append(_ALPHAToChar.Instance.Visit(node._ALPHA_1, default));

                    return default;
                }

                protected internal override Root.Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT node, StringBuilder context)
                {
                    context.Append(DigitToInt.Instance.Generate(node._DIGIT_1));

                    return default;
                }

                protected internal override Root.Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ node, StringBuilder context)
                {
                    context.Append(this.characterSubstitutions.Dash);

                    return default;
                }
            }
        }
    }

    public sealed class _ALPHAToChar : _ALPHA.Visitor<char, Root.Void>
    {
        private _ALPHAToChar()
        {
        }

        public static _ALPHAToChar Instance { get; } = new _ALPHAToChar();

        protected internal override char Accept(_ALPHA._Ⰳx41ⲻ5A node, Root.Void context)
        {
            return _Ⰳx41ⲻ5AVisitor.Instance.Visit(node._Ⰳx41ⲻ5A_1, default);
        }

        protected internal override char Accept(_ALPHA._Ⰳx61ⲻ7A node, Root.Void context)
        {
            return _Ⰳx61ⲻ7AVisitor.Instance.Visit(node._Ⰳx61ⲻ7A_1, default);
        }

        private sealed class _Ⰳx41ⲻ5AVisitor : Inners._Ⰳx41ⲻ5A.Visitor<char, Root.Void>
        {
            private _Ⰳx41ⲻ5AVisitor()
            {
            }

            public static _Ⰳx41ⲻ5AVisitor Instance { get; } = new _Ⰳx41ⲻ5AVisitor();

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._41 node, Root.Void context)
            {
                return (char)0x41;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._42 node, Root.Void context)
            {
                return (char)0x42;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._43 node, Root.Void context)
            {
                return (char)0x43;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._44 node, Root.Void context)
            {
                return (char)0x44;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._45 node, Root.Void context)
            {
                return (char)0x45;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._46 node, Root.Void context)
            {
                return (char)0x46;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._47 node, Root.Void context)
            {
                return (char)0x47;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._48 node, Root.Void context)
            {
                return (char)0x48;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._49 node, Root.Void context)
            {
                return (char)0x49;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4A node, Root.Void context)
            {
                return (char)0x4A;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4B node, Root.Void context)
            {
                return (char)0x4B;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4C node, Root.Void context)
            {
                return (char)0x4C;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4D node, Root.Void context)
            {
                return (char)0x4D;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4E node, Root.Void context)
            {
                return (char)0x4E;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._4F node, Root.Void context)
            {
                return (char)0x4F;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._50 node, Root.Void context)
            {
                return (char)0x50;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._51 node, Root.Void context)
            {
                return (char)0x51;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._52 node, Root.Void context)
            {
                return (char)0x52;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._53 node, Root.Void context)
            {
                return (char)0x53;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._54 node, Root.Void context)
            {
                return (char)0x54;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._55 node, Root.Void context)
            {
                return (char)0x55;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._56 node, Root.Void context)
            {
                return (char)0x56;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._57 node, Root.Void context)
            {
                return (char)0x57;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._58 node, Root.Void context)
            {
                return (char)0x58;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._59 node, Root.Void context)
            {
                return (char)0x59;
            }

            protected internal override char Accept(Inners._Ⰳx41ⲻ5A._5A node, Root.Void context)
            {
                return (char)0x5A;
            }
        }

        private sealed class _Ⰳx61ⲻ7AVisitor : Inners._Ⰳx61ⲻ7A.Visitor<char, Root.Void>
        {
            private _Ⰳx61ⲻ7AVisitor()
            {
            }

            public static _Ⰳx61ⲻ7AVisitor Instance { get; } = new _Ⰳx61ⲻ7AVisitor();

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._61 node, Root.Void context)
            {
                return (char)0x61;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._62 node, Root.Void context)
            {
                return (char)0x62;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._63 node, Root.Void context)
            {
                return (char)0x63;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._64 node, Root.Void context)
            {
                return (char)0x64;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._65 node, Root.Void context)
            {
                return (char)0x65;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._66 node, Root.Void context)
            {
                return (char)0x66;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._67 node, Root.Void context)
            {
                return (char)0x67;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._68 node, Root.Void context)
            {
                return (char)0x68;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._69 node, Root.Void context)
            {
                return (char)0x69;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6A node, Root.Void context)
            {
                return (char)0x6A;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6B node, Root.Void context)
            {
                return (char)0x6B;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6C node, Root.Void context)
            {
                return (char)0x6C;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6D node, Root.Void context)
            {
                return (char)0x6D;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6E node, Root.Void context)
            {
                return (char)0x6E;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._6F node, Root.Void context)
            {
                return (char)0x6F;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._70 node, Root.Void context)
            {
                return (char)0x70;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._71 node, Root.Void context)
            {
                return (char)0x71;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._72 node, Root.Void context)
            {
                return (char)0x72;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._73 node, Root.Void context)
            {
                return (char)0x73;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._74 node, Root.Void context)
            {
                return (char)0x74;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._75 node, Root.Void context)
            {
                return (char)0x75;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._76 node, Root.Void context)
            {
                return (char)0x76;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._77 node, Root.Void context)
            {
                return (char)0x77;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._78 node, Root.Void context)
            {
                return (char)0x78;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._79 node, Root.Void context)
            {
                return (char)0x79;
            }

            protected internal override char Accept(Inners._Ⰳx61ⲻ7A._7A node, Root.Void context)
            {
                return (char)0x7A;
            }
        }
    }

    public sealed class DigitToInt
    {
        private DigitToInt()
        {
        }

        public static DigitToInt Instance { get; } = new DigitToInt();

        public int Generate(_DIGIT digit)
        {
            return Visitor.Instance.Visit(digit._Ⰳx30ⲻ39_1, default);
        }

        private sealed class Visitor : Inners._Ⰳx30ⲻ39.Visitor<int, Root.Void>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._30 node, Root.Void context)
            {
                return 0;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._31 node, Root.Void context)
            {
                return 1;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._32 node, Root.Void context)
            {
                return 2;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._33 node, Root.Void context)
            {
                return 3;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._34 node, Root.Void context)
            {
                return 4;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._35 node, Root.Void context)
            {
                return 5;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._36 node, Root.Void context)
            {
                return 6;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._37 node, Root.Void context)
            {
                return 7;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._38 node, Root.Void context)
            {
                return 8;
            }

            protected internal override int Accept(Inners._Ⰳx30ⲻ39._39 node, Root.Void context)
            {
                return 9;
            }
        }
    }
}
