namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;

    public sealed class RuleTranscriber
    {
        private RuleTranscriber()
        {
        }

        public static RuleTranscriber Instance { get; } = new RuleTranscriber();

        public Void Transcribe(Rule node, StringBuilder context)
        {
            RuleNameTranscriber.Instance.Transcribe(node.RuleName, context);
            DefinedAsTranscriber.Instance.Visit(node.DefinedAs, context);
            ElementsTranscriber.Instance.Transcribe(node.Elements, context);
            CnlTranscriber.Instance.Visit(node.Cnl, context);
            return default;
        }
    }
}
