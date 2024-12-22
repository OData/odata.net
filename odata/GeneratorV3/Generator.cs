namespace GeneratorV3
{
    using System.Collections.Generic;
    using AbnfParser.CstNodes;
    using AbnfParserGenerator;

    public sealed class Generator
    {
        private Generator()
        {
        }

        public static Generator Intance { get; } = new Generator();

        public IEnumerable<Class> Generate(RuleList ruleList, Root.Void context)
        {
        }
    }
}
