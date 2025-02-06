namespace CombinatorParsingV3
{
    using System.Collections.Generic;

    public static class Parse
    {
        /*public static IParser<TToken, TParsed> Token<TToken, TParsed>(TParsed token, IEqualityComparer<TToken> comparer) where TParsed : TToken
        {
            return new TokenParser<TToken, TParsed>(token, comparer);
        }

        private sealed class TokenParser<TToken, TParsed> : IParser<TToken, TParsed> where TParsed : TToken
        {
            private readonly TParsed parsed;
            private readonly IEqualityComparer<TToken> comparer;

            public TokenParser(TParsed parsed, IEqualityComparer<TToken> comparer)
            {
                this.parsed = parsed;
                this.comparer = comparer;
            }

            public IOutput<TToken, TParsed> Parse(IInput<TToken>? input)
            {
                if (input == null)
                {
                    return Output.Create(false, default(TParsed)!, input);
                }

                if (this.comparer.Equals(input.Current, this.parsed))
                {
                    return Output.Create(true, this.parsed, input.Next());
                }

                return Output.Create(false, default(TParsed)!, input);
            }
        }

        public static IParser<char, char> Char(char @char)
        {
            ////return Parse.Token(@char, EqualityComparer<char>.Default);
            //// PERF
            return new CharParser(@char);
        }

        private sealed class CharParser : IParser<char, char>
        {
            private readonly char @char;

            public CharParser(char @char)
            {
                this.@char = @char;
            }

            public IOutput<char, char> Parse(IInput<char>? input)
            {
                if (input == null)
                {
                    return Output.Create(false, default(char), input);
                }

                if (input.Current == this.@char)
                {
                    return Output.Create(true, this.@char, input.Next());
                }

                return Output.Create(false, default(char), input);
            }
        }*/
    }
}
