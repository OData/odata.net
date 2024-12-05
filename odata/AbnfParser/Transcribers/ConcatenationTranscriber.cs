using AbnfParser.CstNodes;
using Root;
using System.Text;

namespace AbnfParser.Transcribers
{
    public sealed class ConcatenationTranscriber
    {
        private ConcatenationTranscriber()
        {
        }

        public static ConcatenationTranscriber Instance { get; } = new ConcatenationTranscriber();

        public Void Transcribe(Concatenation node, StringBuilder context)
        {

        }
    }
}
