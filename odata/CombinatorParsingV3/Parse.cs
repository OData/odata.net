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
        }*/

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public static IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, char> Char(char @char)
        {
            return new CharParser(@char);
        }

        private sealed class CharParser : IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, char>
        {
            private readonly char @char;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public CharParser(char @char)
            {
                this.@char = @char;
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public char Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                if (start >= input.Count)
                {
                    newStart = start;
                    return default;
                }

                if (input[start] == this.@char)
                {
                    newStart = start + 1;
                    return this.@char;
                }

                newStart = start;
                return default;
            }
        }
    }
}
