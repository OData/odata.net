namespace CombinatorParsingV3
{
    using System.Runtime.ExceptionServices;
    using System.Text;
    using CombinatorParsingV2;
    using GeneratorV3;

    public static partial class V3ParserPlayground
    {
        public interface IRewriter<TSource, TResult>
        {
            TResult Transcribe(TSource value, StringBuilder builder);
        }

        public interface IRewriter<T> : IRewriter<T, T>
        {
        }

        public sealed class OdataUriRewriter : IRewriter<OdataUri<ParseMode.Realized>>
        {
            private OdataUriRewriter()
            {
            }

            public static OdataUriRewriter Instance { get; } = new OdataUriRewriter();

            private static AtLeastOneRewriter<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>> SegmentsRewriter { get; } = new AtLeastOneRewriter<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(SegmentRewriter.Instance);

            private static ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>> QueryOptionsRewriter { get; } = new ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(QueryOptionRewriter.Instance);

            public OdataUri<ParseMode.Realized> Transcribe(OdataUri<ParseMode.Realized> value, StringBuilder builder)
            {
                return new OdataUri<ParseMode.Realized>(
                    new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Realized>>(
                        () => SegmentsRewriter.Transcribe(value.Segments, builder)),
                    new Future<QuestionMark<ParseMode.Realized>>(
                        () => QuestionMarkRewriter.Instance.Transcribe(value.QuestionMark, builder)),
                    new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Realized>>(
                        () => QueryOptionsRewriter.Transcribe(value.QueryOptions, builder)));
            }
        }

        public sealed class QueryOptionRewriter : IRewriter<QueryOption<ParseMode.Realized>>
        {
            private QueryOptionRewriter()
            {
            }

            public static QueryOptionRewriter Instance { get; } = new QueryOptionRewriter();

            public QueryOption<ParseMode.Realized> Transcribe(QueryOption<ParseMode.Realized> value, StringBuilder builder)
            {
                return new QueryOption<ParseMode.Realized>(
                    new Future<OptionName<ParseMode.Realized>>(
                        () => OptionNameRewriter.Instance.Transcribe(value.Name, builder)),
                    new Future<EqualsSign<ParseMode.Realized>>(
                        () => EqualsSignRewriter.Instance.Transcribe(value.EqualsSign, builder)),
                    new Future<OptionValue<ParseMode.Realized>>(
                        () => OptionValueRewriter.Instance.Transcribe(value.OptionValue, builder)));
            }
        }

        public sealed class OptionValueRewriter : IRewriter<OptionValue<ParseMode.Realized>>
        {
            private OptionValueRewriter()
            {
            }

            public static OptionValueRewriter Instance { get; } = new OptionValueRewriter();

            private static ManyRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersRewriter { get; } = new ManyRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericRewriter.Instance);

            public OptionValue<ParseMode.Realized> Transcribe(OptionValue<ParseMode.Realized> value, StringBuilder builder)
            {
                return new OptionValue<ParseMode.Realized>(
                    new Future<Many<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized>>(
                        () => CharactersRewriter.Transcribe(value.Characters, builder)));
            }
        }

        public sealed class EqualsSignRewriter : IRewriter<EqualsSign<ParseMode.Realized>>
        {
            private EqualsSignRewriter()
            {
            }

            public static EqualsSignRewriter Instance { get; } = new EqualsSignRewriter();

            public EqualsSign<ParseMode.Realized> Transcribe(EqualsSign<ParseMode.Realized> value, StringBuilder builder)
            {
                return value;
            }
        }

        public sealed class OptionNameRewriter : IRewriter<OptionName<ParseMode.Realized>>
        {
            private OptionNameRewriter()
            {
            }

            public static OptionNameRewriter Instance { get; } = new OptionNameRewriter();

            private static AtLeastOneRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersRewriter { get; } = new AtLeastOneRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericRewriter.Instance);

            public OptionName<ParseMode.Realized> Transcribe(OptionName<ParseMode.Realized> value, StringBuilder builder)
            {
                return new OptionName<ParseMode.Realized>(
                    new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Realized>>(
                        () => CharactersRewriter.Transcribe(value.Characters, builder)));
            }
        }

        public sealed class QuestionMarkRewriter : IRewriter<QuestionMark<ParseMode.Realized>>
        {
            private QuestionMarkRewriter()
            {
            }

            public static QuestionMarkRewriter Instance { get; } = new QuestionMarkRewriter();

            public QuestionMark<ParseMode.Realized> Transcribe(QuestionMark<ParseMode.Realized> value, StringBuilder builder)
            {
                builder.Append('?');
            }
        }

        public sealed class AlphaNumericRewriter : IRewriter<AlphaNumeric<ParseMode.Realized>>
        {
            private AlphaNumericRewriter()
            {
            }

            public static AlphaNumericRewriter Instance { get; } = new AlphaNumericRewriter();

            public AlphaNumeric<ParseMode.Realized> Transcribe(AlphaNumeric<ParseMode.Realized> value, StringBuilder builder)
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

        public sealed class SlashRewriter : IRewriter<Slash<ParseMode.Realized>>
        {
            private SlashRewriter()
            {
            }

            public static SlashRewriter Instance { get; } = new SlashRewriter();

            public Slash<ParseMode.Realized> Transcribe(Slash<ParseMode.Realized> value, StringBuilder builder)
            {
                builder.Append('/');
            }
        }

        public sealed class SegmentRewriter : IRewriter<Segment<ParseMode.Realized>>
        {
            private SegmentRewriter()
            {
            }

            public static SegmentRewriter Instance { get; } = new SegmentRewriter();

            private static AtLeastOneRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersRewriter { get; } = new AtLeastOneRewriter<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericRewriter.Instance);

            public Segment<ParseMode.Realized> Transcribe(Segment<ParseMode.Realized> value, StringBuilder builder)
            {
                SlashRewriter.Instance.Transcribe(value.Slash, builder);
                CharactersRewriter.Transcribe(value.Characters, builder);
            }
        }

        public sealed class AtLeastOneRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
            where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
        {
            private readonly IRewriter<TRealizedAstNode> realizedAstNodeRewriter;

            private readonly ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> manyNodeRewriter;

            public AtLeastOneRewriter(IRewriter<TRealizedAstNode> realizedAstNodeRewriter)
            {
                this.realizedAstNodeRewriter = realizedAstNodeRewriter;

                this.manyNodeRewriter = new ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode>(this.realizedAstNodeRewriter);
            }

            public AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> Transcribe(AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                this.realizedAstNodeRewriter.Transcribe(value._1.Realize().Parsed, builder);
                this.manyNodeRewriter.Transcribe(value.Node, builder);
            }
        }

        public sealed class ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
        {
            private readonly IRewriter<TRealizedAstNode> realizedAstNodeRewriter;

            public ManyNodeRewriter(IRewriter<TRealizedAstNode> realizedAstNodeRewriter)
            {
                this.realizedAstNodeRewriter = realizedAstNodeRewriter;
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> Transcribe(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                if (value.Element.Value.TryGetValue(out var element))
                {
                    this.realizedAstNodeRewriter.Transcribe(element, builder);
                    this.Transcribe(value.Next, builder);
                }
            }
        }

        public sealed class ManyRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
        {
            private readonly ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> manyNodeRewriter;

            public ManyRewriter(IRewriter<TRealizedAstNode> realizedAstNodeRewriter)
            {
                this.manyNodeRewriter = new ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeRewriter);
            }

            public Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> Transcribe(Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                this.manyNodeRewriter.Transcribe(value.Node, builder);
            }
        }
    }
}
