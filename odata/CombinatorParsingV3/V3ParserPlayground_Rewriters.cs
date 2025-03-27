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

            private static AtLeastOneRewriter2<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>> SegmentsRewriter { get; } = new AtLeastOneRewriter2<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(SegmentRewriter.Instance);

            private static ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>> QueryOptionsRewriter { get; } = new ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(QueryOptionRewriter.Instance);

            public OdataUri<ParseMode.Realized> Transcribe(OdataUri<ParseMode.Realized> value, StringBuilder builder)
            {
                return new OdataUri<ParseMode.Realized>(
                    SegmentsRewriter.Transcribe(value.Segments, builder).Realize().Parsed,
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

        public sealed class AlphaNumericRewriter2 : IRewriter<AlphaNumeric<ParseMode.Realized>, AlphaNumericHolder>
        {
            private AlphaNumericRewriter2()
            {
            }

            public static AlphaNumericRewriter2 Instance { get; } = new AlphaNumericRewriter2();

            public AlphaNumericHolder Transcribe(AlphaNumeric<ParseMode.Realized> value, StringBuilder builder)
            {
                return Visitor.Instance.Visit(value, builder);
            }

            private sealed class Visitor : AlphaNumeric<ParseMode.Realized>.Visitor<AlphaNumericHolder, StringBuilder>
            {
                private Visitor()
                {
                }

                public static Visitor Instance { get; } = new Visitor();

                protected internal override AlphaNumericHolder Accept(AlphaNumeric<ParseMode.Realized>.A node, StringBuilder context)
                {
                    return new AlphaNumericHolder(
                        new Future<IDeferredOutput<char>>(
                            () => new DeferredOutput<char>(true, new StringInput("C"))));
                }

                protected internal override AlphaNumericHolder Accept(AlphaNumeric<ParseMode.Realized>.C node, StringBuilder context)
                {
                    return new AlphaNumericHolder(
                        new Future<IDeferredOutput<char>>(
                            () => new DeferredOutput<char>(true, new StringInput("A"))));
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

        public sealed class SegmentRewriter : IRewriter<Segment<ParseMode.Realized>, Segment<ParseMode.Deferred>>
        {
            private SegmentRewriter()
            {
            }

            public static SegmentRewriter Instance { get; } = new SegmentRewriter();

            private static AtLeastOneRewriter2<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>> CharactersRewriter { get; } = new AtLeastOneRewriter2<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>>(AlphaNumericRewriter2.Instance);

            public Segment<ParseMode.Deferred> Transcribe(Segment<ParseMode.Realized> value, StringBuilder builder)
            {
                return new Segment<ParseMode.Deferred>(
                    new Future<Slash<ParseMode.Deferred>>(
                        () => SlashRewriter.Instance.Transcribe(value.Slash, builder)),
                    new Future<AtLeastOne<AlphaNumericHolder, AlphaNumeric<ParseMode.Realized>, ParseMode.Deferred>>(
                        () => CharactersRewriter.Transcribe(value.Characters, builder)));
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

		public sealed class AtLeastOneRewriter2<TDeferredAstNode, TRealizedAstNode> : IRewriter<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
			where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
			where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
		{
			private readonly IRewriter<TRealizedAstNode, TDeferredAstNode> realizedAstNodeRewriter;

			private readonly ManyNodeRewriter2<TDeferredAstNode, TRealizedAstNode> manyNodeRewriter;

			public AtLeastOneRewriter2(IRewriter<TRealizedAstNode, TDeferredAstNode> realizedAstNodeRewriter)
			{
				this.realizedAstNodeRewriter = realizedAstNodeRewriter;

				this.manyNodeRewriter = new ManyNodeRewriter2<TDeferredAstNode, TRealizedAstNode>(this.realizedAstNodeRewriter);
			}

			public AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
			{
                return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                    new Future<TDeferredAstNode>(
                        () => this.realizedAstNodeRewriter.Transcribe(value._1.Realize().Parsed, builder)),
                    new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                        () => this.manyNodeRewriter.Transcribe(value.Node, builder)));
			}
		}

		public sealed class ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
            where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
        {
            private readonly OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode> optionalNodeRewriter;

            public ManyNodeRewriter(IRewriter<TRealizedAstNode> realizedAstNodeRewriter)
            {
                this.optionalNodeRewriter = new OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeRewriter);
            }

            public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> Transcribe(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    this.optionalNodeRewriter.Transcribe(value.Element, builder),
                    () => value.Element.Value.TryGetValue(out var element) ? this.Transcribe(value.Next, builder) : ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>.GetTerminalRealizedNode(value.cachedOutput, value.element.Value.Realize().Parsed),
                    null);
            }
		}

		public sealed class ManyNodeRewriter2<TDeferredAstNode, TRealizedAstNode> : IRewriter<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
			where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
		{
			private readonly OptionalNodeRewriter2<TDeferredAstNode, TRealizedAstNode> optionalNodeRewriter;

			public ManyNodeRewriter2(IRewriter<TRealizedAstNode, TDeferredAstNode> realizedAstNodeRewriter)
			{
				this.optionalNodeRewriter = new OptionalNodeRewriter2<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeRewriter);
			}

			public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
			{
                return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                    new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                        () => this.optionalNodeRewriter.Transcribe(value.Element, builder)),
                    new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                        () => this.Transcribe(value.Next, builder)));
			}
		}

		public sealed class OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>
			where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
		{
            private readonly IRewriter<TRealizedAstNode> realizedAstNodeRewriter;

            public OptionalNodeRewriter(IRewriter<TRealizedAstNode> realizedAstNodeRewriter)
            {
                this.realizedAstNodeRewriter = realizedAstNodeRewriter;
            }

            public OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> Transcribe(OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
            {
                if (value.Value.TryGetValue(out var element))
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                        new RealNullable<TRealizedAstNode>(this.realizedAstNodeRewriter.Transcribe(element, builder)),
                        null);
                }
                else
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                        new RealNullable<TRealizedAstNode>(),
                        null);
                }
            }
        }

		public sealed class OptionalNodeRewriter2<TDeferredAstNode, TRealizedAstNode> : IRewriter<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
			where TDeferredAstNode : IDeferredAstNode<char, TRealizedAstNode>
		{
			private readonly IRewriter<TRealizedAstNode, TDeferredAstNode> realizedAstNodeRewriter;

			public OptionalNodeRewriter2(IRewriter<TRealizedAstNode, TDeferredAstNode> realizedAstNodeRewriter)
			{
				this.realizedAstNodeRewriter = realizedAstNodeRewriter;
			}

			public OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> value, StringBuilder builder)
			{
				if (value.Value.TryGetValue(out var element))
				{
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        _ => this.realizedAstNodeRewriter.Transcribe(element, builder),
                        new Future<IDeferredOutput<char>>(
                            () => new DeferredOutput<char>(true, null)),
                        new RealNullable<RealNullable<TRealizedAstNode>>());
				}
				else
				{
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        new RealNullable<TRealizedAstNode>(),
                        new Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                            () => new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(true, GetEmpty(), null)));

                }
			}

            private static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetEmpty()
            {
                return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                    new RealNullable<TRealizedAstNode>(),
                    new Future<IOutput<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                        () => new Output<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(true, GetEmpty(), null)));
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
