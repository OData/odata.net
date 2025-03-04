namespace CombinatorParsingV3
{
    using System.Runtime.ExceptionServices;
    using System.Text;

    using GeneratorV3;

    public static partial class V3ParserPlayground
    {
        public sealed class OdataUriTranscriber : ITranscriber<OdataUri<ParseMode.Realized>>
        {
            private OdataUriTranscriber()
            {
            }

            public static OdataUriTranscriber Instance { get; } = new OdataUriTranscriber();

            private static AtLeastOneTranscriber<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>> SegmentsTranscriber { get; } = new AtLeastOneTranscriber<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(SegmentTranscriber.Instance);

            public void Transcribe(OdataUri<ParseMode.Realized> value, StringBuilder builder)
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class SegmentTranscriber : ITranscriber<Segment<ParseMode.Realized>>
        {
            private SegmentTranscriber()
            {
            }

            public static SegmentTranscriber Instance { get; } = new SegmentTranscriber();

            private static AtLeastOneTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new AtLeastOneTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

            public void Transcribe(Segment<ParseMode.Realized> value, StringBuilder builder)
            {
                SlashTranscriber.Instance.Transcribe(value.Slash, builder);

                throw new System.NotImplementedException();
            }
        }

        public sealed class SlashTranscriber : ITranscriber<Slash<ParseMode.Realized>>
        {
            private SlashTranscriber()
            {
            }

            public static SlashTranscriber Instance { get; } = new SlashTranscriber();

            public void Transcribe(Slash<ParseMode.Realized> value, StringBuilder builder)
            {
                builder.Append('/');
            }
        }

        public sealed class AlphaNumericTranscriber : ITranscriber<AlphaNumeric<ParseMode.Realized>>
        {
            private AlphaNumericTranscriber()
            {
            }

            public static AlphaNumericTranscriber Instance { get; } = new AlphaNumericTranscriber();

            public void Transcribe(AlphaNumeric<ParseMode.Realized> value, StringBuilder builder)
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class AtLeastOneTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
        {
            private readonly ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber;

            private readonly ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode> manyNodeTranscriber;

            public AtLeastOneTranscriber(ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber)
            {
                this.realizedAstNodeTranscriber = realizedAstNodeTranscriber;

                this.manyNodeTranscriber = new ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode>(this.realizedAstNodeTranscriber);
            }

            public void Transcribe(AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                this.realizedAstNodeTranscriber.Transcribe(value._1.Realize().Parsed, builder);
                this.manyNodeTranscriber.Transcribe(value.Node, builder);
            }
        }

        public sealed class ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
        {
            private readonly ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber;

            public ManyNodeTranscriber(ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber)
            {
                this.realizedAstNodeTranscriber = realizedAstNodeTranscriber;
            }

            public void Transcribe(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                if (value.Element.Value.TryGetValue(out var element))
                {
                    this.realizedAstNodeTranscriber.Transcribe(element, builder);
                    this.Transcribe(value.Next, builder);
                }
            }
        }
    }
}
