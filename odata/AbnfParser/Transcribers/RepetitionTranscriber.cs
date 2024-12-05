using AbnfParser.CstNodes;
using Root;
using System.Text;

namespace AbnfParser.Transcribers
{
    public sealed class RepetitionTranscriber : Repetition.Visitor<Void, StringBuilder>
    {
        private RepetitionTranscriber()
        {
        }

        public static RepetitionTranscriber Instance { get; } = new RepetitionTranscriber();

        protected internal override Void Accept(Repetition.ElementOnly node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }

        protected internal override Void Accept(Repetition.RepeatAndElement node, StringBuilder context)
        {
            throw new System.NotImplementedException();
        }
    }
}
