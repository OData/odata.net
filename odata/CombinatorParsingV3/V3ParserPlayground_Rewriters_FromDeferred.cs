namespace CombinatorParsingV3
{
    using System.Runtime.ExceptionServices;
    using System.Text;
    using CombinatorParsingV2;
    using GeneratorV3;

    public static partial class V3ParserPlayground
    {
        public static class Rewriter2
        {
            public interface IRewriter<TSource, TResult>
            {
                TResult Transcribe(TSource value, StringBuilder builder);
            }

            public interface IRewriter<T> : IRewriter<T, T>
            {
            }

            public sealed class OdataUriRewriter : IRewriter<OdataUri<ParseMode.Deferred>, OdataUri<ParseMode.Deferred>>
            {
                private OdataUriRewriter()
                {
                }

                public static OdataUriRewriter Instance { get; } = new OdataUriRewriter();

                private static AtLeastOneRewriter<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>> SegmentsRewriter { get; } = new AtLeastOneRewriter<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>>(SegmentRewriter.Instance);

                private static ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>> QueryOptionsRewriter { get; } = new ManyRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>>(QueryOptionRewriter.Instance);

                public OdataUri<ParseMode.Deferred> Transcribe(OdataUri<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new OdataUri<ParseMode.Deferred>(
                        new Future<AtLeastOne<Segment<ParseMode.Deferred>, Segment<ParseMode.Realized>, ParseMode.Deferred>>(
                            () => SegmentsRewriter.Transcribe(value.Segments, builder)),
                        new Future<QuestionMark<ParseMode.Deferred>>(
                            () => QuestionMarkRewriter.Instance.Transcribe(value.QuestionMark, builder)),
                        new Future<Many<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Realized>, ParseMode.Deferred>>(
                            () => QueryOptionsRewriter.Transcribe(value.QueryOptions, builder)));
                }
            }

            public sealed class QueryOptionRewriter : IRewriter<QueryOption<ParseMode.Deferred>, QueryOption<ParseMode.Deferred>>
            {
                private QueryOptionRewriter()
                {
                }

                public static QueryOptionRewriter Instance { get; } = new QueryOptionRewriter();

                public QueryOption<ParseMode.Deferred> Transcribe(QueryOption<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new QueryOption<ParseMode.Deferred>(
                        new Future<OptionName<ParseMode.Deferred>>(
                            () => OptionNameRewriter.Instance.Transcribe(value.Name, builder)),
                        new Future<EqualsSign<ParseMode.Deferred>>(
                            () => EqualsSignRewriter.Instance.Transcribe(value.EqualsSign, builder)),
                        new Future<OptionValue<ParseMode.Deferred>>(
                            () => OptionValueRewriter.Instance.Transcribe(value.OptionValue, builder)));
                }
            }

            public sealed class OptionValueRewriter : IRewriter<OptionValue<ParseMode.Deferred>, OptionValue<ParseMode.Deferred>>
            {
                private OptionValueRewriter()
                {
                }

                public static OptionValueRewriter Instance { get; } = new OptionValueRewriter();

                private static ManyRewriter<AlphaNumericDeferred, AlphaNumericRealized> CharactersRewriter { get; } = new ManyRewriter<AlphaNumericDeferred, AlphaNumericRealized>(AlphaNumericRewriter.Instance);

                public OptionValue<ParseMode.Deferred> Transcribe(OptionValue<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new OptionValue<ParseMode.Deferred>(
                        new Future<Many<AlphaNumericDeferred, AlphaNumericRealized, ParseMode.Deferred>>(
                            () => CharactersRewriter.Transcribe(value.Characters, builder)));
                }
            }

            public sealed class EqualsSignRewriter : IRewriter<EqualsSign<ParseMode.Deferred>, EqualsSign<ParseMode.Deferred>>
            {
                private EqualsSignRewriter()
                {
                }

                public static EqualsSignRewriter Instance { get; } = new EqualsSignRewriter();

                public EqualsSign<ParseMode.Deferred> Transcribe(EqualsSign<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return value;
                }
            }

            public sealed class OptionNameRewriter : IRewriter<OptionName<ParseMode.Deferred>, OptionName<ParseMode.Deferred>>
            {
                private OptionNameRewriter()
                {
                }

                public static OptionNameRewriter Instance { get; } = new OptionNameRewriter();

                private static AtLeastOneRewriter<AlphaNumericDeferred, AlphaNumericRealized> CharactersRewriter { get; } = new AtLeastOneRewriter<AlphaNumericDeferred, AlphaNumericRealized>(AlphaNumericRewriter.Instance);

                public OptionName<ParseMode.Deferred> Transcribe(OptionName<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new OptionName<ParseMode.Deferred>(
                        new Future<AtLeastOne<AlphaNumericDeferred, AlphaNumericRealized, ParseMode.Deferred>>(
                            () => CharactersRewriter.Transcribe(value.Characters, builder)));
                }
            }

            public sealed class QuestionMarkRewriter : IRewriter<QuestionMark<ParseMode.Deferred>, QuestionMark<ParseMode.Deferred>>
            {
                private QuestionMarkRewriter()
                {
                }

                public static QuestionMarkRewriter Instance { get; } = new QuestionMarkRewriter();

                public QuestionMark<ParseMode.Deferred> Transcribe(QuestionMark<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return value;
                }
            }

            public sealed class AlphaNumericRewriter : IRewriter<AlphaNumericDeferred, AlphaNumericDeferred>
            {
                private AlphaNumericRewriter()
                {
                }

                public static AlphaNumericRewriter Instance { get; } = new AlphaNumericRewriter();

                public AlphaNumericDeferred Transcribe(AlphaNumericDeferred value, StringBuilder builder)
                {
                    //// TODO once you get to something that you actually need to make a decision on, you have to realize; is that correct?
                    var realized = value.Realize();
                    if (!realized.Success)
                    {
                        throw new System.Exception("tODO");
                    }

                    var parsed = realized.RealizedValue;

                    return Visitor.Instance.Visit(parsed, builder);
                }

                private sealed class Visitor : AlphaNumericRealized.Visitor<AlphaNumericDeferred, StringBuilder>
                {
                    private Visitor()
                    {
                    }

                    public static Visitor Instance { get; } = new Visitor();

                    protected internal override AlphaNumericDeferred Accept(AlphaNumericRealized.A node, StringBuilder context)
                    {
                        return new AlphaNumericDeferred(
                            new Future<IRealizationResult<char>>(
                                () => new RealizationResult<char>(true, new CharacterTokenStream("C"))));
                    }

                    protected internal override AlphaNumericDeferred Accept(AlphaNumericRealized.C node, StringBuilder context)
                    {
                        return new AlphaNumericDeferred(
                            new Future<IRealizationResult<char>>(
                                () => new RealizationResult<char>(true, new CharacterTokenStream("A"))));
                    }
                }
            }

            public sealed class SlashRewriter : IRewriter<Slash<ParseMode.Deferred>, Slash<ParseMode.Deferred>>
            {
                private SlashRewriter()
                {
                }

                public static SlashRewriter Instance { get; } = new SlashRewriter();

                public Slash<ParseMode.Deferred> Transcribe(Slash<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return value;
                }
            }

            public sealed class SegmentRewriter : IRewriter<Segment<ParseMode.Deferred>, Segment<ParseMode.Deferred>>
            {
                private SegmentRewriter()
                {
                }

                public static SegmentRewriter Instance { get; } = new SegmentRewriter();

                private static AtLeastOneRewriter<AlphaNumericDeferred, AlphaNumericRealized> CharactersRewriter { get; } = new AtLeastOneRewriter<AlphaNumericDeferred, AlphaNumericRealized>(AlphaNumericRewriter.Instance);

                public Segment<ParseMode.Deferred> Transcribe(Segment<ParseMode.Deferred> value, StringBuilder builder)
                {
                    return Segment.Create(
                        new Future<Slash<ParseMode.Deferred>>(
                            () => SlashRewriter.Instance.Transcribe(value.Slash, builder)),
                        new Future<AtLeastOne<AlphaNumericDeferred, AlphaNumericRealized, ParseMode.Deferred>>(
                            () => CharactersRewriter.Transcribe(value.Characters, builder)));
                }
            }

            public sealed class AtLeastOneRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>, AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                private readonly IRewriter<TDeferredAstNode, TDeferredAstNode> realizedAstNodeRewriter;

                private readonly ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> manyNodeRewriter;

                public AtLeastOneRewriter(IRewriter<TDeferredAstNode, TDeferredAstNode> realizedAstNodeRewriter)
                {
                    this.realizedAstNodeRewriter = realizedAstNodeRewriter;

                    this.manyNodeRewriter = new ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode>(this.realizedAstNodeRewriter);
                }

                public AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new AtLeastOne<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        new Future<TDeferredAstNode>(
                            () => this.realizedAstNodeRewriter.Transcribe(value._1, builder)),
                        new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                            () => this.manyNodeRewriter.Transcribe(value.Node, builder)));
                }
            }

            public sealed class ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>, ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                private readonly OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode> optionalNodeRewriter;

                public ManyNodeRewriter(IRewriter<TDeferredAstNode, TDeferredAstNode> realizedAstNodeRewriter)
                {
                    this.optionalNodeRewriter = new OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeRewriter);
                }

                public ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        new Future<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                            () => this.optionalNodeRewriter.Transcribe(value.Element, builder)),
                        new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                            () => this.Transcribe(value.Next, builder)));
                }
            }

            public sealed class OptionalNodeRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                private readonly IRewriter<TDeferredAstNode, TDeferredAstNode> deferredAstNodeRewriter;

                public OptionalNodeRewriter(IRewriter<TDeferredAstNode, TDeferredAstNode> realizedAstNodeRewriter)
                {
                    this.deferredAstNodeRewriter = realizedAstNodeRewriter;
                }

                public OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> value, StringBuilder builder)
                {
                    var realized = value.Realize();
                    if (!realized.Success)
                    {
                        throw new System.Exception("TODO");
                    }

                    var parsed = realized.RealizedValue;

                    if (parsed.Value.TryGetValue(out var element))
                    {
                        return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                            _ => this.deferredAstNodeRewriter.Transcribe(element.Convert(), builder),
                            new Future<IRealizationResult<char>>(
                                () => new RealizationResult<char>(true, null)),
                            new Optional<Optional<TRealizedAstNode>>());
                    }
                    else
                    {
                        return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                            new Optional<TRealizedAstNode>(),
                            new Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                                () => new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(true, GetEmpty(), null)));

                    }
                }

                private static OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized> GetEmpty()
                {
                    return new OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>(
                        new Optional<TRealizedAstNode>(),
                        new Future<IRealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>>(
                            () => new RealizationResult<char, OptionalNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Realized>>(true, GetEmpty(), null)));
                }
            }

            public sealed class ManyRewriter<TDeferredAstNode, TRealizedAstNode> : IRewriter<Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>, Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>
                where TDeferredAstNode : IAstNode<char, TRealizedAstNode>
                where TRealizedAstNode : IFromRealizedable<TDeferredAstNode>
            {
                private readonly ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode> manyNodeRewriter;

                public ManyRewriter(IRewriter<TDeferredAstNode, TDeferredAstNode> realizedAstNodeRewriter)
                {
                    this.manyNodeRewriter = new ManyNodeRewriter<TDeferredAstNode, TRealizedAstNode>(realizedAstNodeRewriter);
                }

                public Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> Transcribe(Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred> value, StringBuilder builder)
                {
                    return new Many<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>(
                        new Future<ManyNode<TDeferredAstNode, TRealizedAstNode, ParseMode.Deferred>>(
                            () => this.manyNodeRewriter.Transcribe(value.Node, builder)));
                }
            }
        }
    }
}
