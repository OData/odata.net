namespace CombinatorParsingV3
{
    public static class Parse
    {
        public static IParser<char, char> Char(char @char)
        {
            return new CharParser(@char);
        }

        private sealed class CharParser : IParser<char, char>
        {
            private readonly char @char;

            public CharParser(char @char)
            {
                this.@char = @char;
            }

            public IOutput<char, char> Parse(IInput<char> input)
            {
                if (input.Current == this.@char)
                {
                    return new Output<char, char>(true, this.@char, input.Next());
                }

                return new Output<char, char>(false, default, input);
            }
        }
    }
}
