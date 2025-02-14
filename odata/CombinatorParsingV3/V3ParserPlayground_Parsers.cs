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

        public sealed class 
    }
}
