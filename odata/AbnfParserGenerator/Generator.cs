namespace AbnfParserGenerator
{
    using System.Collections.Generic;

    public sealed class Generator
    {
        private Generator()
        {
        }

        public static Generator Instance { get; } = new Generator();

        public IEnumerable<Class> Generate(AbnfParser.CstNodes.RuleList ruleList, Root.Void context)
        {
            yield break;
        }
    }
}
