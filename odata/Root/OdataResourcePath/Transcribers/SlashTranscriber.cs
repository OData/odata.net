namespace Root.OdataResourcePath.Transcribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class SlashTranscriber
    {
        private SlashTranscriber()
        {
        }

        public static SlashTranscriber Default { get; } = new SlashTranscriber();

        public void Transcribe(Slash node, StringBuilder context)
        {
            context.Append("/");
        }
    }
}
