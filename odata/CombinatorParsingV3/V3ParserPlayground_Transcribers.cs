namespace CombinatorParsingV3
{
    using System.Runtime.ExceptionServices;
    using System.Text;
    using CombinatorParsingV2;
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

            private static ManyTranscriber<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>> QueryOptionsTranscriber { get; } = new ManyTranscriber<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(QueryOptionTranscriber.Instance);

            public void Transcribe(OdataUri<ParseMode.Realized> value, StringBuilder builder)
            {
                SegmentsTranscriber.Transcribe(value.Segments, builder);
                QuestionMarkTranscriber.Instance.Transcribe(value.QuestionMark, builder);
                QueryOptionsTranscriber.Transcribe(value.QueryOptions, builder);
            }
        }

        public sealed class QueryOptionTranscriber : ITranscriber<QueryOption<ParseMode.Realized>>
        {
            private QueryOptionTranscriber()
            {
            }

            public static QueryOptionTranscriber Instance { get; } = new QueryOptionTranscriber();

            public void Transcribe(QueryOption<ParseMode.Realized> value, StringBuilder builder)
            {
                OptionNameTranscriber.Instance.Transcribe(value.Name, builder);
                EqualsSignTranscriber.Instance.Transcribe(value.EqualsSign, builder);
                OptionValueTranscriber.Instance.Transcribe(value.OptionValue, builder);
            }
        }

        public sealed class OptionValueTranscriber : ITranscriber<OptionValue<ParseMode.Realized>>
        {
            private OptionValueTranscriber()
            {
            }

            public static OptionValueTranscriber Instance { get; } = new OptionValueTranscriber();

            private static ManyTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new ManyTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

            public void Transcribe(OptionValue<ParseMode.Realized> value, StringBuilder builder)
            {
                CharactersTranscriber.Transcribe(value.Characters, builder);
            }
        }

        public sealed class EqualsSignTranscriber : ITranscriber<EqualsSign<ParseMode.Realized>>
        {
            private EqualsSignTranscriber()
            {
            }

            public static EqualsSignTranscriber Instance { get; } = new EqualsSignTranscriber();

            public void Transcribe(EqualsSign<ParseMode.Realized> value, StringBuilder builder)
            {
                builder.Append('=');
            }
        }

        public sealed class OptionNameTranscriber : ITranscriber<OptionName<ParseMode.Realized>>
        {
            private OptionNameTranscriber()
            {
            }

            public static OptionNameTranscriber Instance { get; } = new OptionNameTranscriber();

            private static AtLeastOneTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new AtLeastOneTranscriber<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

            public void Transcribe(OptionName<ParseMode.Realized> value, StringBuilder builder)
            {
                CharactersTranscriber.Transcribe(value.Characters, builder);
            }
        }

        public sealed class QuestionMarkTranscriber : ITranscriber<QuestionMark<ParseMode.Realized>>
        {
            private QuestionMarkTranscriber()
            {
            }

            public static QuestionMarkTranscriber Instance { get; } = new QuestionMarkTranscriber();

            public void Transcribe(QuestionMark<ParseMode.Realized> value, StringBuilder builder)
            {
                builder.Append('?');
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
                Visitor.Instance.Visit(value, builder);
            }

            private sealed class Visitor : AlphaNumeric<ParseMode.Realized>.Visitor<Nothing, StringBuilder>
            {
                private Visitor()
                {
                }

                public static Visitor Instance { get; } = new Visitor();

                protected internal override Nothing Accept(AlphaNumeric<ParseMode.Realized>.A node, StringBuilder context)
                {
                    context.Append('A');

                    return default;
                }

                protected internal override Nothing Accept(AlphaNumeric<ParseMode.Realized>.C node, StringBuilder context)
                {
                    context.Append('C');

                    return default;
                }
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
                CharactersTranscriber.Transcribe(value.Characters, builder);
            }
        }

        public sealed class AtLeastOneTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
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
                this.realizedAstNodeTranscriber.Transcribe(value._1.Realize().RealizedValue, builder);
                this.manyNodeTranscriber.Transcribe(value.Node, builder);
            }
        }

        public sealed class ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
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

        public sealed class ManyTranscriber<TDeferredAstNode, TRealizedAstNode> : ITranscriber<Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
        {
            private readonly ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode> manyNodeTranscriber;

            public ManyTranscriber(ITranscriber<TRealizedAstNode> realizedAstNodeTranscriber)
            {
                this.manyNodeTranscriber = new ManyNodeTranscriber<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeTranscriber);
            }

            public void Transcribe(Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                this.manyNodeTranscriber.Transcribe(value.Node, builder);
            }
        }
    }
}
