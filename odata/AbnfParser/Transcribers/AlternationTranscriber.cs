using AbnfParser.CstNodes;
using Root;
using System.Text;

namespace AbnfParser.Transcribers
{
    public sealed class AlternationTranscriber
    {
        private AlternationTranscriber()
        {
        }

        public static AlternationTranscriber Instance { get; } = new AlternationTranscriber();

        public Void Transcribe(Alternation node, StringBuilder context)
        {

        }
    }
}
