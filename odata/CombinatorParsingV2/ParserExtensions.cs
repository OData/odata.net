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

                    //// TODO is lazy evaluation a consideration here
                    parsed.Add(output.Parsed);
                    input = output.Remainder;
                }
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
                var output = this.parser.AtMost(this.count).Parse(input);
                if (!output.Success)
                {
                    //// TODO `atmost` can't really fail...
                    //// TODO add errors
                    return Output.Create(false, Enumerable.Empty<TParsed>(), input);
                }

                if (output.Parsed.Count() != count)
                {
                    return Output.Create(false, Enumerable.Empty<TParsed>(), input);
                }

                return output;
            }
        }

        public static IParser<TToken, IEnumerable<TParsed>> AtMost<TToken, TParsed>(this IParser<TToken, TParsed> parser, int maximum)
        {
            return new AtMostParser<TToken, TParsed>(parser, maximum);
        }

        private sealed class AtMostParser<TToken, TParsed> : IParser<TToken, IEnumerable<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;
            private readonly int maximum;

            public AtMostParser(IParser<TToken, TParsed> parser, int maximum)
            {
                this.parser = parser;
                this.maximum = maximum;
            }

            public IOutput<TToken, IEnumerable<TParsed>> Parse(IInput<TToken> input)
            {
                var parsed = new List<TParsed>();
                for (int i = 0; i < this.maximum; ++i)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return Output.Create(true, parsed, input);
                    }

                    parsed.Add(output.Parsed);
                    input = output.Remainder;
                }

                //// TODO do you want to look to see if another `TParsed` is available in input? or is it the job of the caller to check the remainder?
                return Output.Create(true, parsed, input);
            }
        }

        public static IParser<TToken, IEnumerable<TParsed>> AtLeast<TToken, TParsed>(
            this IParser<TToken, TParsed> parser, 
            int minimum)
        {
            return new AtLeastParser<TToken, TParsed>(parser, minimum);
        }

        private sealed class AtLeastParser<TToken, TParsed> : IParser<TToken, IEnumerable<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;
            private readonly int minimum;

            public AtLeastParser(IParser<TToken, TParsed> parser, int minimum)
            {
                this.parser = parser;
                this.minimum = minimum;
            }

            public IOutput<TToken, IEnumerable<TParsed>> Parse(IInput<TToken> input)
            {
                var output = this.parser.Exactly(this.minimum).Parse(input);
                if (!output.Success)
                {
                    return output;
                }

                output = this.parser.Many().Parse(output.Remainder);

                return output;
            }
        }

        public static IParser<TToken, IEnumerable<TParsed>> Range<TToken, TParsed>(
            this IParser<TToken, TParsed> parser,
            int minimum, 
            int maximum)
        {
            return new RangeParser<TToken, TParsed>(parser, minimum, maximum);
        }

        private sealed class RangeParser<TToken, TParsed> : IParser<TToken, IEnumerable<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;
            private readonly int minimum;
            private readonly int maximum;

            public RangeParser(IParser<TToken, TParsed> parser, int minimum, int maximum)
            {
                //// TODO nail down if `maximum` is inclusive or exclusive, and then propogate that to the `helperranged` classes in the cst nodess
                this.parser = parser;
                this.minimum = minimum;
                this.maximum = maximum;
            }

            public IOutput<TToken, IEnumerable<TParsed>> Parse(IInput<TToken> input)
            {
                var output = this.parser.Exactly(this.minimum).Parse(input);
                if (!output.Success)
                {
                    return output;
                }

                output = this.parser.AtMost(this.maximum - this.minimum).Parse(output.Remainder);

                return output;
            }
        }
    }
}
