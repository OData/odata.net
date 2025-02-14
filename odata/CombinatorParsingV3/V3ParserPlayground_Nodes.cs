namespace CombinatorParsingV3
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public static partial class V3ParserPlayground
    {
        public sealed class Slash : IDeferredAstNode<Slash>
        {
            private Slash()
            {
            }

            public static Slash Instance { get; } = new Slash();

            public IDeferredOutput<Slash> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class AlphaNumeric : IDeferredAstNode<AlphaNumeric>
        {
            private readonly IParser<char, AlphaNumeric> parser;
            private readonly IInput<char> input;

            private char @char;

            private bool deferred;

            public AlphaNumeric(IParser<char, AlphaNumeric> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;

                this.deferred = true;
            }

            public AlphaNumeric(char @char)
            {
                this.@char = @char;

                this.deferred = false;
            }

            public char Char
            {
                get
                {
                    if (this.deferred)
                    {
                        throw new System.Exception("TODO not parsed yet");
                    }

                    return this.@char;
                }
            }

            public IDeferredOutput<AlphaNumeric> Realize()
            {
                if (this.deferred)
                {
                    var output = this.parser.Parse(input);
                    if (output.Success)
                    {
                        this.@char = output.Parsed.@char;
                        this.deferred = false;
                        return new DeferredOutput<AlphaNumeric>(true, this);
                    }
                    else
                    {
                        return new DeferredOutput<AlphaNumeric>(false, default);
                    }
                }
                else
                {
                    return new DeferredOutput<AlphaNumeric>(true, this);
                }
            }
        }

        public sealed class Segment : IDeferredAstNode<Segment>
        {
            private readonly IParser<char, Segment> parser;
            private readonly IInput<char> input;
            private Slash slash;
            private IEnumerable<AlphaNumeric> characters;

            private bool deferred;

            public Segment(IParser<char, Segment> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;

                this.deferred = true;
            }

            public Segment(Slash slash, IEnumerable<AlphaNumeric> characters)
            {
                this.slash = slash;
                this.characters = characters;

                this.deferred = false;
            }

            public Slash Slash
            {
                get
                {
                    if (this.deferred)
                    {
                        throw new System.Exception("TODO not parsed yet");
                    }

                    return this.slash;
                }
            }

            public IEnumerable<AlphaNumeric> Characters
            {
                get
                {
                    if (this.deferred)
                    {
                        throw new System.Exception("TODO not parsed yet");
                    }

                    return this.characters;
                }
            }

            public IDeferredOutput<Segment> Realize()
            {
                if (this.deferred)
                {
                    var output = this.parser.Parse(this.input);
                    if (output.Success)
                    {
                        this.slash = output.Parsed.slash;
                        this.characters = output.Parsed.characters;
                        this.deferred = false;
                        return new DeferredOutput<Segment>(true, this);
                    }
                    else
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }
                }
                else
                {
                    // we are not a deferred instance, but maybe our members are
                    var slashOutput = this.slash.Realize();
                    if (!slashOutput.Success)
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }

                    var charactersOutput = this.characters.Select(character => character.Realize());
                    if (!charactersOutput.All(_ => _.Success))
                    {
                        return new DeferredOutput<Segment>(false, default);
                    }

                    this.slash = slashOutput.Parsed;
                    this.characters = charactersOutput.Select(_ => _.Parsed);

                    return new DeferredOutput<Segment>(true, this);
                }
            }
        }

        public sealed class EqualsSign : IDeferredAstNode<EqualsSign>
        {
            private EqualsSign()
            {
            }

            public static EqualsSign Instance { get; } = new EqualsSign();

            public IDeferredOutput<EqualsSign> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class OptionName : IDeferredAstNode<OptionName>
        {
            public OptionName(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }

            public IEnumerable<AlphaNumeric> Characters { get; }

            public IDeferredOutput<OptionName> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class OptionValue : IDeferredAstNode<OptionValue>
        {
            public OptionValue(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }

            public IEnumerable<AlphaNumeric> Characters { get; }

            public IDeferredOutput<OptionValue> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class QueryOption : IDeferredAstNode<QueryOption>
        {
            public QueryOption(OptionName name, EqualsSign equalsSign, OptionValue optionValue)
            {
                Name = name;
                EqualsSign = equalsSign;
                OptionValue = optionValue;
            }

            public OptionName Name { get; }
            public EqualsSign EqualsSign { get; }
            public OptionValue OptionValue { get; }

            public IDeferredOutput<QueryOption> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class QuestionMark : IDeferredAstNode<QuestionMark>
        {
            private QuestionMark()
            {
            }

            public static QuestionMark Instance { get; } = new QuestionMark();

            public IDeferredOutput<QuestionMark> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class OdataUri : IDeferredAstNode<OdataUri>
        {
            public OdataUri(IEnumerable<Segment> segments, QuestionMark questionMark, IEnumerable<QueryOption> queryOptions)
            {
                Segments = segments;
                QuestionMark = questionMark;
                QueryOptions = queryOptions;
            }

            public IEnumerable<Segment> Segments { get; }
            public QuestionMark QuestionMark { get; }
            public IEnumerable<QueryOption> QueryOptions { get; }

            public IDeferredOutput<OdataUri> Realize()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
