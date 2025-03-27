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
                    SegmentsRewriter.Transcribe(value.Segments, builder),
                    QuestionMarkRewriter.Instance.Transcribe(value.QuestionMark, builder).Realize().Parsed,
                    QueryOptionsRewriter.Transcribe(value.QueryOptions, builder),
                    null);
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
                    OptionNameRewriter.Instance.Transcribe(value.Name, builder),
                    EqualsSignRewriter.Instance.Transcribe(value.EqualsSign, builder),
                    OptionValueRewriter.Instance.Transcribe(value.OptionValue, builder),
                    null);
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
                    CharactersRewriter.Transcribe(value.Characters, builder),
                    null);
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
                    CharactersRewriter.Transcribe(value.Characters, builder),
                    null);
            }
        }

        public sealed class QuestionMarkRewriter : IRewriter<QuestionMark<ParseMode.Realized>, QuestionMark<ParseMode.Deferred>>
        {
            private QuestionMarkRewriter()
            {
            }

            public static QuestionMarkRewriter Instance { get; } = new QuestionMarkRewriter();

            public QuestionMark<ParseMode.Deferred> Transcribe(QuestionMark<ParseMode.Realized> value, StringBuilder builder)
            {
                return new QuestionMark<ParseMode.Deferred>(
                    new Future<IOutput<char, QuestionMark<ParseMode.Realized>>>(
                        () => new Output<char, QuestionMark<ParseMode.Realized>>(true, value, null)));
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
                return Visitor.Instance.Visit(value, builder);
            }

            private sealed class Visitor : AlphaNumeric<ParseMode.Realized>.Visitor<AlphaNumeric<ParseMode.Realized>, StringBuilder>
            {
                private Visitor()
                {
                }

                public static Visitor Instance { get; } = new Visitor();

                protected internal override AlphaNumeric<ParseMode.Realized> Accept(AlphaNumeric<ParseMode.Realized>.A node, StringBuilder context)
                {
                    return GetC(node);
                }

                private static AlphaNumeric<ParseMode.Realized>.C GetC(AlphaNumeric<ParseMode.Realized>.A node)
				{
					return new AlphaNumeric<ParseMode.Realized>.C(
						new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.C>>(
							() => new Output<char, AlphaNumeric<ParseMode.Realized>.C>(true, GetC(node), null)));
				}

				protected internal override AlphaNumeric<ParseMode.Realized> Accept(AlphaNumeric<ParseMode.Realized>.C node, StringBuilder context)
				{
                    return GetA(node);
				}

                private static AlphaNumeric<ParseMode.Realized>.A GetA(AlphaNumeric<ParseMode.Realized>.C node)
				{
					return new AlphaNumeric<ParseMode.Realized>.A(
						new Future<IOutput<char, AlphaNumeric<ParseMode.Realized>.A>>(
							() => new Output<char, AlphaNumeric<ParseMode.Realized>.A>(true, GetA(node), null)));
				}
            }
        }

        public sealed class SlashRewriter : IRewriter<Slash<ParseMode.Realized>, Slash<ParseMode.Deferred>>
        {
            private SlashRewriter()
            {
            }

            public static SlashRewriter Instance { get; } = new SlashRewriter();

            public Slash<ParseMode.Deferred> Transcribe(Slash<ParseMode.Realized> value, StringBuilder builder)
            {
                return new Slash<ParseMode.Deferred>(
                    new Future<IOutput<char, Slash<ParseMode.Realized>>>(
                        () => new Output<char, Slash<ParseMode.Realized>>(true, value, null)));
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
                return new Segment<ParseMode.Realized>(
                    SlashRewriter.Instance.Transcribe(value.Slash, builder).Realize().Parsed,
                    CharactersRewriter.Transcribe(value.Characters, builder),
                    new Future<IOutput<char, Segment<ParseMode.Realized>>>(
                        () => new Output<char, Segment<ParseMode.Realized>>(true, this.Transcribe(value, builder), null)));
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
                return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    this.realizedAstNodeRewriter.Transcribe(value._1.Realize().Parsed, builder),
                    this.manyNodeRewriter.Transcribe(value.Node, builder),
                    null);
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
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                        value.Element.Value.TryGetValue(out var element) ? this.realizedAstNodeRewriter.Transcribe(element, builder) : new RealNullable<TRealizedAstNode>(),
                        null),
                    () => value.Element.Value.TryGetValue(out var element) ? this.Transcribe(value.Next, builder) : ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>.GetTerminalRealizedNode(value.cachedOutput, value.element.Value.Realize().Parsed),
                    null);
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
                return new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    this.manyNodeRewriter.Transcribe(value.Node, builder),
                    null);
            }
        }
    }
}
