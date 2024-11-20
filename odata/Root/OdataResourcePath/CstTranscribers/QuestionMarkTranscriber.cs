namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTree;

    public sealed class QuestionMarkTranscriber
    {
        private QuestionMarkTranscriber()
        {
        }

        public static QuestionMarkTranscriber Default { get; } = new QuestionMarkTranscriber();

        public void Transcribe(QuestionMark node, StringBuilder context)
        {
            context.Append('?');
        }
    }
}
