namespace _GeneratorV4
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using AbnfParserGenerator;
    using System.Collections.Generic;

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

    /*public sealed class Generator
    {
        private readonly string innersClassName;
        private readonly RuleListInnerGenerator ruleListInnerGenerator;

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
            this.ruleListInnerGenerator = new RuleListInnerGenerator(
                settings.ClassNamePrefix,
                @namespace,
                this.innersClassName,
                toClassNames);
        }

        public IEnumerable<Class> Generate(RuleList ruleList)
        {
            //// !!!!!IMPORTANT WHENEVER YOU REMOVE OLD CODE, CHECK FOR TODOS!!!!!
            //// TODO implement this generator that uses the new cst nodes
            //// TODO remove v3 generator
            //// TODO automatically generate the transcribers
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
    }*/
}
