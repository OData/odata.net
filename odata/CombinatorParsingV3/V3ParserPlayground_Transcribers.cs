namespace CombinatorParsingV3
{
    using System.Text;

    using GeneratorV3;

    public static partial class V3ParserPlayground
    {
        public sealed class OdataUriTranscriber : ITranscriber<OdataUri<ParseMode.Realized>>
        {
            public void Transcribe(OdataUri<ParseMode.Realized> value, StringBuilder builder)
            {
                
            }
        }

        public sealed class AtLeastOneTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
        {
            private readonly ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber;

            public AtLeastOneTranscriber(ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber)
            {
                this.realizedAstNodeTranscriber = realizedAstNodeTranscriber;
            }

            public void Transcribe(AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                
            }
        }
    }
}
