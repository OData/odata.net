namespace CombinatorParsingV3
{
    using System.Collections.Generic;

    public static partial class V3ParserPlayground
    {
        public sealed class SlashParser : IParser<char, Slash>
        {
            private SlashParser()
            {
            }

            public static SlashParser Instance { get; } = new SlashParser();

            public IOutput<char, Slash> Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('/').Parse(input);
                if (output.Success)
                {
                    return new Output<char, Slash>(true, Slash.Instance, output.Remainder);
                }

                return new Output<char, Slash>(false, default, input);
            }
        }

        public sealed class AlphaNumericParser : IParser<char, AlphaNumeric>
        {
            private AlphaNumericParser()
            {
            }

            public static AlphaNumericParser Instance { get; } = new AlphaNumericParser();

            public IOutput<char, AlphaNumeric> Parse(IInput<char> input)
            {
                var current = input.Current;
                if (
                    (current >= '0' && current <= '9') || 
                    (current >= 'A' && current <= 'Z') || 
                    (current >= 'a' && current <= 'z'))
                {
                    return new Output<char, AlphaNumeric>(true, new AlphaNumeric(input.Current), input.Next());
                }

                return new Output<char, AlphaNumeric>(false, default, input);
            }
        }

        public sealed class SegmentParser : IParser<char, Segment>
        {
            private SegmentParser()
            {
            }

            public static SegmentParser Instance { get; } = new SegmentParser();

            public IOutput<char, Segment> Parse(IInput<char> input)
            {
                var slashOutput = SlashParser.Instance.Parse(input);
                if (!slashOutput.Success)
                {
                    return new Output<char, Segment>(false, default, input);
                }

                var characters = new List<AlphaNumeric>();
                var alphaNumericOutput = AlphaNumericParser.Instance.Parse(slashOutput.Remainder);
                while (alphaNumericOutput.Success)
                {
                    characters.Add(alphaNumericOutput.Parsed);
                    alphaNumericOutput = AlphaNumericParser.Instance.Parse(alphaNumericOutput.Remainder);
                }

                if (characters.Count == 0)
                {
                    return new Output<char, Segment>(false, default, input);
                }

                return new Output<char, Segment>(true, new Segment(slashOutput.Parsed, characters), alphaNumericOutput.Remainder);
            }
        }

        public sealed class EqualsSignParser : IParser<char, EqualsSign>
        {
            private EqualsSignParser()
            {
            }

            public static EqualsSignParser Instance { get; } = new EqualsSignParser();

            public IOutput<char, EqualsSign> Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('=').Parse(input);
                if (output.Success)
                {
                    return new Output<char, EqualsSign>(true, EqualsSign.Instance, output.Remainder);
                }

                return new Output<char, EqualsSign>(false, default, input);
            }
        }

        public sealed class OptionNameParser : IParser<char, OptionName>
        {
            private OptionNameParser()
            {
            }

            public static OptionNameParser Instance { get; } = new OptionNameParser();

            public IOutput<char, OptionName> Parse(IInput<char> input)
            {
                var characters = new List<AlphaNumeric>();
                var alphaNumericOutput = AlphaNumericParser.Instance.Parse(input);
                while (alphaNumericOutput.Success)
                {
                    characters.Add(alphaNumericOutput.Parsed);
                    alphaNumericOutput = AlphaNumericParser.Instance.Parse(alphaNumericOutput.Remainder);
                }

                if (characters.Count == 0)
                {
                    return new Output<char, OptionName>(false, default, input);
                }

                return new Output<char, OptionName>(true, new OptionName(characters), alphaNumericOutput.Remainder);
            }
        }

        public sealed class OptionValueParser : IParser<char, OptionValue>
        {
            private OptionValueParser()
            {
            }

            public static OptionValueParser Instance { get; } = new OptionValueParser();

            public IOutput<char, OptionValue> Parse(IInput<char> input)
            {
                var characters = new List<AlphaNumeric>();
                var alphaNumericOutput = AlphaNumericParser.Instance.Parse(input);
                while (alphaNumericOutput.Success)
                {
                    characters.Add(alphaNumericOutput.Parsed);
                    alphaNumericOutput = AlphaNumericParser.Instance.Parse(alphaNumericOutput.Remainder);
                }

                if (characters.Count == 0)
                {
                    return new Output<char, OptionValue>(false, default, input);
                }

                return new Output<char, OptionValue>(true, new OptionValue(characters), alphaNumericOutput.Remainder);
            }
        }

        public sealed class QueryOptionParser : IParser<char, QueryOption>
        {
            private QueryOptionParser()
            {
            }

            public static QueryOptionParser Instance { get; } = new QueryOptionParser();

            public IOutput<char, QueryOption> Parse(IInput<char> input)
            {
                var optionName = OptionNameParser.Instance.Parse(input);
                if (!optionName.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                var equalsSign = EqualsSignParser.Instance.Parse(optionName.Remainder);
                if (!equalsSign.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                var optionValue = OptionValueParser.Instance.Parse(equalsSign.Remainder);
                if (!optionValue.Success)
                {
                    return new Output<char, QueryOption>(false, default, input);
                }

                return new Output<char, QueryOption>(true, new QueryOption(optionName.Parsed, equalsSign.Parsed, optionValue.Parsed), optionValue.Remainder);
            }
        }

        public sealed class QuestionMarkParser : IParser<char, QuestionMark>
        {
            private QuestionMarkParser()
            {
            }

            public static QuestionMarkParser Instance { get; } = new QuestionMarkParser();

            public IOutput<char, QuestionMark> Parse(IInput<char> input)
            {
                var output = CombinatorParsingV3.Parse.Char('?').Parse(input);
                if (!output.Success)
                {
                    return new Output<char, QuestionMark>(false, default, input);
                }

                return new Output<char, QuestionMark>(true, QuestionMark.Instance, output.Remainder);
            }
        }

        public sealed class OdataUriParser : IParser<char, OdataUri>
        {
            private OdataUriParser()
            {
            }

            public static OdataUriParser Instance { get; } = new OdataUriParser();

            public IOutput<char, OdataUri> Parse(IInput<char> input)
            {
                var segments = new List<Segment>();
                var segmentOutput = SegmentParser.Instance.Parse(input);
                while (segmentOutput.Success)
                {
                    segments.Add(segmentOutput.Parsed);
                    segmentOutput = SegmentParser.Instance.Parse(segmentOutput.Remainder);
                }

                if (segments.Count == 0)
                {
                    return new Output<char, OdataUri>(false, default, input);
                }

                var questionMark = QuestionMarkParser.Instance.Parse(segmentOutput.Remainder);
                if (!questionMark.Success)
                {
                    return new Output<char, OdataUri>(false, default, input);
                }

                var queryOptions = new List<QueryOption>();
                var queryOptionOutput = QueryOptionParser.Instance.Parse(questionMark.Remainder);
                while (queryOptionOutput.Success)
                {
                    queryOptions.Add(queryOptionOutput.Parsed);
                    queryOptionOutput = QueryOptionParser.Instance.Parse(queryOptionOutput.Remainder);
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
