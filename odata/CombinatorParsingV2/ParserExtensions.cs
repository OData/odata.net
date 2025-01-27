namespace CombinatorParsingV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ParserExtensions
    {
        public static bool TryParse<TParsed>(this IParser<char, TParsed> parser, string input, out TParsed parsed)
        {
            return parser.TryParse(new StringInput(input), out parsed);
        }

        public static bool TryParse<TToken, TParsed>(this IParser<TToken, TParsed> parser, IInput<TToken> input, out TParsed parsed)
        {
            var output = parser.Parse(input);
            if (!output.Success)
            {
                parsed = default(TParsed)!;
                return false;
            }

            parsed = output.Parsed;
            return true;
        }

        public static IParser<TToken, IEnumerable<TParsed>> Repeat<TToken, TParsed>(
            this IParser<TToken, TParsed> parser,
            int? minimum,
            int? maximum)
        {
            //// TODO remove this; you only did it so you didn't have to make massive changes to the generator when moving away from sprach

            if (minimum == null && maximum == null)
            {
                return parser.Many();
            }

            if (minimum == null)
            {
                return parser.AtMost(maximum.Value);
            }

            if (maximum == null)
            {
                return parser.AtLeast(minimum.Value);
            }

            return parser.Range(minimum.Value, maximum.Value);
        }

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
                    if (output.Remainder == null)
                    {
                        return Output.Create(true, parsed, input);
                    }

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
                    if (output.Remainder == null)
                    {
                        return Output.Create(true, parsed, input);
                    }

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
                var parsed = new List<TParsed>();
                for (int i = 0; i < this.minimum; ++i)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return Output.Create(false, Enumerable.Empty<TParsed>(), input);
                    }

                    parsed.Add(output.Parsed);

                    if (output.Remainder == null)
                    {
                        return Output.Create(false, Enumerable.Empty<TParsed>(), input);
                    }

                    input = output.Remainder;
                }

                while (true)
                {
                    var output = this.parser.Parse(input);
                    if (!output.Success)
                    {
                        return Output.Create(true, parsed, output.Remainder);
                    }

                    parsed.Add(output.Parsed);

                    if (output.Remainder == null)
                    {
                        return Output.Create(true, parsed, output.Remainder);
                    }

                    input = output.Remainder;
                }

                /*var output = this.parser.Exactly(this.minimum).Parse(input);
                if (!output.Success)
                {
                    return output;
                }

                if (output.Remainder == null)
                {
                    return output;
                }

                output = this.parser.Many().Parse(output.Remainder);

                return output;*/
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
                //// TODO it's implemented as inclusive
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

                if (output.Remainder == null)
                {
                    return output;
                }

                output = this.parser.AtMost(this.maximum - this.minimum).Parse(output.Remainder);

                return output;
            }
        }

        public interface IOption<TParsed>
        {
            TParsed GetOrElse(TParsed value);
        }

        private sealed class Option<TParsed> : IOption<TParsed>
        {
            private readonly TParsed parsed;

            private readonly bool hasValue;

            public Option(TParsed parsed)
            {
                this.parsed = parsed;
                this.hasValue = true;
            }

            public Option()
            {
                this.hasValue = false;
            }

            public TParsed GetOrElse(TParsed value)
            {
                if (this.hasValue)
                {
                    return this.parsed;
                }
                else
                {
                    return value;
                }
            }
        }

        public static IParser<TToken, IOption<TParsed>> Optional<TToken, TParsed>(this IParser<TToken, TParsed> parser)
        {
            var asdf = 
                from foo in parser
                from bar in parser
                select new object();

            return new OptionalParser<TToken, TParsed>(parser);
        }

        private sealed class OptionalParser<TToken, TParsed> : IParser<TToken, IOption<TParsed>>
        {
            private readonly IParser<TToken, TParsed> parser;

            public OptionalParser(IParser<TToken, TParsed> parser)
            {
                this.parser = parser;
            }

            public IOutput<TToken, IOption<TParsed>> Parse(IInput<TToken> input)
            {
                var output = this.parser.Parse(input);
                if (!output.Success)
                {
                    return Output.Create(true, new Option<TParsed>(), output.Remainder);
                }

                return Output.Create(true, new Option<TParsed>(output.Parsed), output.Remainder);
            }
        }

        public static IParser<TToken, TResult> SelectMany<TToken, TSource, TParser, TResult>(
            this IParser<TToken, TSource> parser,
            Func<TSource, IParser<TToken, TParser>> parserSelector,
            Func<TSource, TParser, TResult> resultSelector)
        {
            return new SelectManyParser<TToken, TSource, TParser, TResult>(parser, parserSelector, resultSelector);
        }

        private sealed class SelectManyParser<TToken, TSource, TParser, TResult> : IParser<TToken, TResult>
        {
            private readonly IParser<TToken, TSource> parser;
            private readonly Func<TSource, IParser<TToken, TParser>> parserSelector;
            private readonly Func<TSource, TParser, TResult> resultSelector;

            public SelectManyParser(
                IParser<TToken, TSource> parser,
                Func<TSource, IParser<TToken, TParser>> parserSelector,
                Func<TSource, TParser, TResult> resultSelector)
            {
                this.parser = parser;
                this.parserSelector = parserSelector;
                this.resultSelector = resultSelector;
            }

            public IOutput<TToken, TResult> Parse(IInput<TToken> input)
            {
                var output = this.parser.Parse(input);
                if (!output.Success)
                {
                    return Output.Create(false, default(TResult)!, input);
                }

                if (output.Remainder == null)
                {
                    return Output.Create(false, default(TResult)!, input);
                }

                var subParser = this.parserSelector(output.Parsed);

                var subOutput = subParser.Parse(output.Remainder);
                if (!subOutput.Success)
                {
                    return Output.Create(false, default(TResult)!, input);
                }

                var parsed = this.resultSelector(output.Parsed, subOutput.Parsed);

                return Output.Create(true, parsed, subOutput.Remainder);
            }
        }

        public static IParser<TToken, TResult> Select<TToken, TSource, TResult>(
            this IParser<TToken, TSource> parser,
            Func<TSource, TResult> selector)
        {
            return new SelectParser<TToken, TSource, TResult>(parser, selector);
        }

        private sealed class SelectParser<TToken, TSource, TResult> : IParser<TToken, TResult>
        {
            private readonly IParser<TToken, TSource> parser;
            private readonly Func<TSource, TResult> selector;

            public SelectParser(IParser<TToken, TSource> parser, Func<TSource, TResult> selector)
            {
                this.parser = parser;
                this.selector = selector;
            }

            public IOutput<TToken, TResult> Parse(IInput<TToken> input)
            {
                var output = this.parser.Parse(input);
                if (!output.Success)
                {
                    return Output.Create(false, default(TResult)!, input);
                }

                return Output.Create(true, this.selector(output.Parsed), output.Remainder);
            }
        }
    }
}
