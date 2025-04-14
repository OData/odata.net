namespace CombinatorParsingV3.TranscribersUsingGeneratedNodes
{
    using System.Text;

    using GeneratorV3;
    using __GeneratedPartialV1.Deferred.CstNodes.Inners;
    using __GeneratedPartialV1.Deferred.CstNodes.Rules;
    using CombinatorParsingV2;
    using System.Diagnostics.Contracts;

    public sealed class OdataUriTranscriber : ITranscriber<_odataUri<ParseMode.Realized>>
    {
        private OdataUriTranscriber()
        {
        }

        public static OdataUriTranscriber Instance { get; } = new OdataUriTranscriber();

        private static V3ParserPlayground.AtLeastOneTranscriber<_segment<ParseMode.Deferred>, _segment<ParseMode.Realized>> SegmentsTranscriber = new V3ParserPlayground.AtLeastOneTranscriber<_segment<ParseMode.Deferred>, _segment<ParseMode.Realized>>(SegmentTranscriber.Instance);

        private static V3ParserPlayground.ManyTranscriber<_queryOption<ParseMode.Deferred>, _queryOption<ParseMode.Realized>> QueryOptionsTranscriber { get; } = new V3ParserPlayground.ManyTranscriber<_queryOption<ParseMode.Deferred>, _queryOption<ParseMode.Realized>>(QueryOptionTranscriber.Instance);

        public void Transcribe(_odataUri<ParseMode.Realized> value, StringBuilder builder)
        {
            SegmentsTranscriber.Transcribe(value._segment_1, builder);
            QuestionMarkTranscriber.Instance.Transcribe(value._questionMark_1, builder);
            QueryOptionsTranscriber.Transcribe(value._queryOption_1, builder);
        }
    }

    public sealed class QueryOptionTranscriber : ITranscriber<_queryOption<ParseMode.Realized>>
    {
        private QueryOptionTranscriber()
        {
        }

        public static QueryOptionTranscriber Instance { get; } = new QueryOptionTranscriber();

        public void Transcribe(_queryOption<ParseMode.Realized> value, StringBuilder builder)
        {
            OptionNameTranscriber.Instance.Transcribe(value._optionName_1, builder);
            EqualsSignTranscriber.Instance.Transcribe(value._equalsSign_1, builder);
            OptionValueTranscriber.Instance.Transcribe(value._optionValue_1, builder);
        }
    }

    public sealed class OptionValueTranscriber : ITranscriber<_optionValue<ParseMode.Realized>>
    {
        private OptionValueTranscriber()
        {
        }

        public static OptionValueTranscriber Instance { get; } = new OptionValueTranscriber();

        private static V3ParserPlayground.ManyTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new V3ParserPlayground.ManyTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

        public void Transcribe(_optionValue<ParseMode.Realized> value, StringBuilder builder)
        {
            CharactersTranscriber.Transcribe(value._alphaNumeric_1, builder);
        }
    }

    public sealed class EqualsSignTranscriber : ITranscriber<_equalsSign<ParseMode.Realized>>
    {
        private EqualsSignTranscriber()
        {
        }

        public static EqualsSignTranscriber Instance { get; } = new EqualsSignTranscriber();

        public void Transcribe(_equalsSign<ParseMode.Realized> value, StringBuilder builder)
        {
            builder.Append('=');
        }
    }

    public sealed class OptionNameTranscriber : ITranscriber<_optionName<ParseMode.Realized>>
    {
        private OptionNameTranscriber()
        {
        }

        public static OptionNameTranscriber Instance { get; } = new OptionNameTranscriber();

        private static V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

        public void Transcribe(_optionName<ParseMode.Realized> value, StringBuilder builder)
        {
            CharactersTranscriber.Transcribe(value._alphaNumeric_1, builder);
        }
    }

    public sealed class QuestionMarkTranscriber : ITranscriber<_questionMark<ParseMode.Realized>>
    {
        private QuestionMarkTranscriber()
        {
        }

        public static QuestionMarkTranscriber Instance { get; } = new QuestionMarkTranscriber();

        public void Transcribe(_questionMark<ParseMode.Realized> value, StringBuilder builder)
        {
            builder.Append('?');
        }
    }

    public sealed class SegmentTranscriber : ITranscriber<_segment<ParseMode.Realized>>
    {
        private SegmentTranscriber()
        {
        }

        public static SegmentTranscriber Instance { get; } = new SegmentTranscriber();

        private static V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>>(AlphaNumericTranscriber.Instance);

        public void Transcribe(_segment<ParseMode.Realized> value, StringBuilder builder)
        {
            SlashTranscriber.Instance.Transcribe(value._slash_1, builder);
            CharactersTranscriber.Transcribe(value._alphaNumeric_1, builder);
        }
    }

    public sealed class SlashTranscriber : ITranscriber<_slash<ParseMode.Realized>>
    {
        private SlashTranscriber()
        {
        }

        public static SlashTranscriber Instance { get; } = new SlashTranscriber();

        public void Transcribe(_slash<ParseMode.Realized> value, StringBuilder builder)
        {
            builder.Append('/');
        }
    }

    public sealed class AlphaNumericTranscriber : ITranscriber<_alphaNumeric<ParseMode.Realized>>
    {
        private AlphaNumericTranscriber()
        {
        }

        public static AlphaNumericTranscriber Instance { get; } = new AlphaNumericTranscriber();

        public void Transcribe(_alphaNumeric<ParseMode.Realized> value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _alphaNumeric<ParseMode.Realized>.Realized.Visitor<Nothing, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Nothing Accept(_alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ node, StringBuilder context)
            {
                context.Append('A');
                return default;
            }

            protected internal override Nothing Accept(_alphaNumeric<ParseMode.Realized>.Realized._ʺx43ʺ node, StringBuilder context)
            {
                context.Append('C');
                return default;
            }
        }
    }
}
