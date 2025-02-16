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
        }

        /*public static IParser2<char, char> Char2(char @char)
        {
            return new CharParser2(@char);
        }

        private sealed class CharParser2 : IParser2<char, char>
        {
            private readonly char @char;

            public CharParser2(char @char)
            {
                this.@char = @char;
            }

            public Future<Output2<char, char>> Parse2(IInput<char>? input)
            {
                //// TODO i think you probably want continuewith and to take the output of the previous as the input of the current...
                return new Future<Output2<char, char>>(() => this.Parse(input), Future.Empty);
            }

            private Output2<char, char> Parse(IInput<char>? input)
            {
                if (input == null)
                {
                    return new Output2<char, char>(false, default, input);
                }

                if (input.Current == this.@char)
                {
                    return new Output2<char, char>(true, this.@char, input.Next());
                }

                return new Output2<char, char>(false, default, input);
            }
        }*/
    }
}
