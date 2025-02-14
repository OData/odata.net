namespace CombinatorParsingV3
{
    public static class DeferredParse
    {
        public sealed class PossibleChar : IDeferredAstNode<char>
        {
            private readonly IParser<char, char> parser;
            private readonly IInput<char> input;

            public PossibleChar(IParser<char, char> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;
            }

            public IDeferredOutput<char> Realize()
            {
                var output = this.parser.Parse(this.input);

                return new DeferredOutput<char>(output.Success, output.Parsed);
            }
        }

        public static IDeferredParser<char, char, PossibleChar> Char(char @char)
        {
            return new CharParser(@char);
        }

        private sealed class CharParser : IDeferredParser<char, char, PossibleChar>
        {
            private readonly char @char;

            public CharParser(char @char)
            {
                this.@char = @char;
            }

            public PossibleChar Parse(IInput<char> input)
            {
                return new PossibleChar(CombinatorParsingV3.Parse.Char(this.@char), input);
            }
        }
    }
}
