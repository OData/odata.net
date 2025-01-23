namespace CombinatorParsingV2
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ParserExtensions
    {
        public static IParser<TToken, TParsed> Or<TToken, TParsed>(
            this IParser<TToken, TParsed> first, 
            IParser<TToken, TParsed> second)
        {
            return new OrParser<TToken, TParsed>(first, second);
        }

        private sealed class OrParser<TToken, TParsed> : IParser<TToken, TParsed>
        {
            private readonly IParser<TToken, TParsed> first;
            private readonly IParser<TToken, TParsed> second;

            public OrParser(IParser<TToken, TParsed> first, IParser<TToken, TParsed> second)
            {
                this.first = first;
                this.second = second;
            }

            public IOutput<TToken, TParsed> Parse(IInput<TToken> input)
            {
                var firstOutput = this.first.Parse(input);
                if (firstOutput.Success)
                {
                    return firstOutput;
                }

                var secondOutput = this.second.Parse(input);
                if (secondOutput.Success)
                {
                    return secondOutput;
                }

                //// TODO differentiate success and failure
                //// TODO have a way to surface errors
                //// TODO for this `orparser`, you need to somehow combine the errors surface by each parser
                return Output.Create(false, default(TParsed)!, input);
            }
        }

        public static IParser<TToken, IEnumerable<TParsed>> Exactly<TToken, TParsed>(
            this IParser<TToken, TParsed> parser,
            int count)
        {
            return new ExactlyParser<TToken, TParsed>(parser, count);
        }

        private sealed class ExactlyParser<TToken, TParsed> : IParser<TToken, IEnumerable<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;
            private readonly int count;

            public ExactlyParser(IParser<TToken, TParsed> parser, int count)
            {
                this.parser = parser;
                this.count = count;
            }

            public IOutput<TToken, IEnumerable<TParsed>> Parse(IInput<TToken> input)
            {
                //// TODO is lazy evaluation a consideration here
                var parsed = new List<TParsed>();
                for (int i = 0; i < count; ++i)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        //// TODO add errors
                        return Output.Create(false, Enumerable.Empty<TParsed>(), input);
                    }

                    parsed.Add(output.Parsed);
                    input = output.Remainder;
                }

                return Output.Create(true, parsed, input);
            }
        }

        public static IParser<TToken, IEnumerable<TParsed>> Many<TToken, TParsed>(this IParser<TToken, TParsed> parser)
        {
            return new ManyParser<TToken, TParsed>(parser);
        }

        private sealed class ManyParser<TToken, TParsed> : IParser<TToken, IEnumerable<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;

            public ManyParser(IParser<TToken, TParsed> parser)
            {
                this.parser = parser;
            }

            public IOutput<TToken, IEnumerable<TParsed>> Parse(IInput<TToken> input)
            {
                var parsed = new List<TParsed>();
                while (true)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return Output.Create(true, parsed, input);
                    }

                    parsed.Add(output.Parsed);
                    input = output.Remainder;
                }
            }
        }
    }
}
