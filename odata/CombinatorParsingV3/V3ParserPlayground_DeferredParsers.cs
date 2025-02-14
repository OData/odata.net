using System.Collections.Generic;

namespace CombinatorParsingV3
{
    public static partial class V3ParserPlayground
    {
        public sealed class SlashDeferredParser : IDeferredParser<char, Slash, Slash>
        {
            private SlashDeferredParser()
            {
            }

            public static SlashDeferredParser Instance { get; } = new SlashDeferredParser();

            public Slash Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('/').Parse(input);
                if (output.Success)
                {
                    return new Output<char, Slash>(true, Slash.Instance, output.Remainder);
                }

                return new Output<char, Slash>(false, default, input);
            }
        }

        public sealed class AlphaNumericDeferredParser : IDeferredParser<char, AlphaNumeric, AlphaNumeric>
        {
            private AlphaNumericDeferredParser()
            {
            }

            public static AlphaNumericDeferredParser Instance { get; } = new AlphaNumericDeferredParser();

            public AlphaNumeric Parse(IInput<char> input)
            {
                return new AlphaNumeric(AlphaNumericParser.Instance, input);
            }
        }

        public sealed class SegmentDeferredParser : IDeferredParser<char, Segment, Segment>
        {
            private SegmentDeferredParser()
            {
            }

            public static SegmentDeferredParser Instance { get; } = new SegmentDeferredParser();

            public Segment Parse(IInput<char> input)
            {
                return new Segment(SegmentParser.Instance, input);
            }
        }

        public sealed class EqualsSignDeferredParser : IDeferredParser<char, EqualsSign, EqualsSign>
        {
            private EqualsSignDeferredParser()
            {
            }

            public static EqualsSignDeferredParser Instance { get; } = new EqualsSignDeferredParser();

            public EqualsSign Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('=').Parse(input);
                if (output.Success)
                {
                    return new Output<char, EqualsSign>(true, EqualsSign.Instance, output.Remainder);
                }

                return new Output<char, EqualsSign>(false, default, input);
            }
        }

        public sealed class OptionNameDeferredParser : IDeferredParser<char, OptionName, OptionName>
        {
            private OptionNameDeferredParser()
            {
            }

            public static OptionNameDeferredParser Instance { get; } = new OptionNameDeferredParser();

            public OptionName Parse(IInput<char> input)
            {
                var characters = new List<AlphaNumeric>();
                var alphaNumericOutput = AlphaNumericDeferredParser.Instance.Parse(input);
                while (alphaNumericOutput.Success)
                {
                    characters.Add(alphaNumericOutput.Parsed);
                    alphaNumericOutput = AlphaNumericDeferredParser.Instance.Parse(alphaNumericOutput.Remainder);
                }

                if (characters.Count == 0)
                {
                    return new Output<char, OptionName>(false, default, input);
                }

                return new Output<char, OptionName>(true, new OptionName(characters), alphaNumericOutput.Remainder);
            }
        }

        public sealed class OptionValueDeferredParser : IDeferredParser<char, OptionValue, OptionValue>
        {
            private OptionValueDeferredParser()
            {
            }

            public static OptionValueDeferredParser Instance { get; } = new OptionValueDeferredParser();

            public OptionValue Parse(IInput<char> input)
            {
                var characters = new List<AlphaNumeric>();
                var alphaNumericOutput = AlphaNumericDeferredParser.Instance.Parse(input);
                while (alphaNumericOutput.Success)
                {
                    characters.Add(alphaNumericOutput.Parsed);
                    alphaNumericOutput = AlphaNumericDeferredParser.Instance.Parse(alphaNumericOutput.Remainder);
                }

                if (characters.Count == 0)
                {
                    return new Output<char, OptionValue>(false, default, input);
                }

                return new Output<char, OptionValue>(true, new OptionValue(characters), alphaNumericOutput.Remainder);
            }
        }

        public sealed class QueryOptionDeferredParser : IDeferredParser<char, QueryOption, QueryOption>
        {
            private QueryOptionDeferredParser()
            {
            }

            public static QueryOptionDeferredParser Instance { get; } = new QueryOptionDeferredParser();

            public QueryOption Parse(IInput<char> input)
            {
                var optionName = OptionNameDeferredParser.Instance.Parse(input);
                if (!optionName.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                var equalsSign = EqualsSignDeferredParser.Instance.Parse(optionName.Remainder);
                if (!equalsSign.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                var optionValue = OptionValueDeferredParser.Instance.Parse(equalsSign.Remainder);
                if (!optionValue.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                return new Output<char, QueryOption>(true, new QueryOption(optionName.Parsed, equalsSign.Parsed, optionValue.Parsed), optionValue.Remainder);
            }
        }

        public sealed class QuestionMarkDeferredParser : IDeferredParser<char, QuestionMark>
        {
            private QuestionMarkDeferredParser()
            {
            }

            public static QuestionMarkDeferredParser Instance { get; } = new QuestionMarkDeferredParser();

            public QuestionMark Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('?').Parse(input);
                if (!output.Success)
                {
                    return new Output<char, QuestionMark>(false, default, input);
                }

                return new Output<char, QuestionMark>(true, QuestionMark.Instance, output.Remainder);
            }
        }

        public sealed class OdataUriDeferredParser : IDeferredParser<char, OdataUri, OdataUri>
        {
            private OdataUriDeferredParser()
            {
            }

            public static OdataUriDeferredParser Instance { get; } = new OdataUriDeferredParser();

            public OdataUri Parse(IInput<char> input)
            {
                var segments = new List<Segment>();
                var segmentOutput = SegmentDeferredParser.Instance.Parse(input);
                while (segmentOutput.Success)
                {
                    segments.Add(segmentOutput.Parsed);
                    segmentOutput = SegmentDeferredParser.Instance.Parse(segmentOutput.Remainder);
                }

                if (segments.Count == 0)
                {
                    return new Output<char, OdataUri>(false, default, input);
                }

                var questionMark = QuestionMarkDeferredParser.Instance.Parse(segmentOutput.Remainder);
                if (!questionMark.Success)
                {
                    return new Output<char, OdataUri>(false, default, input);
                }

                var queryOptions = new List<QueryOption>();
                var queryOptionOutput = QueryOptionDeferredParser.Instance.Parse(questionMark.Remainder);
                while (queryOptionOutput.Success)
                {
                    queryOptions.Add(queryOptionOutput.Parsed);
                    queryOptionOutput = QueryOptionDeferredParser.Instance.Parse(queryOptionOutput.Remainder);
                }

                if (queryOptions.Count == 0)
                {
                    return new Output<char, OdataUri>(false, default, input);
                }

                return new Output<char, OdataUri>(true, new OdataUri(segments, questionMark.Parsed, queryOptions), queryOptionOutput.Remainder);
            }
        }
    }
}
