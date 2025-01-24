namespace CombinatorParsingV2
{
    using System.Collections.Generic;

    public static class Parse
    {
        public static IParser<TToken, TParsed> Token<TToken, TParsed>(TParsed token, IEqualityComparer<TToken> comparer) where TParsed : TToken
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

            public IOutput<TToken, TParsed> Parse(IInput<TToken> input)
            {
                if (this.comparer.Equals(input.Current, this.parsed))
                {
                    return Output.Create(true, this.parsed, input.Next());
                }

                return Output.Create(false, default(TParsed)!, input);
            }
        }

        public static IParser<char, char> Char(char @char)
        {
            return Parse.Token(@char, EqualityComparer<char>.Default);
        }
    }
}
