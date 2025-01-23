namespace CombinatorParsingV2
{
    public static class Parse
    {
        public static IParser<char, char> Char(char @char)
        {

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
                    //// TODO is this the right output? also, you need a concrete `input` implementation now
                    return Output.Create(true, this.@char, )
                }
            }
        }
    }
}
