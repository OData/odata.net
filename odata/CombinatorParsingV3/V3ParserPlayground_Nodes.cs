namespace CombinatorParsingV3
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections;

    public static partial class V3ParserPlayground
    {
        public sealed class Slash : IDeferredAstNode<char, Slash>
        {
            private readonly IInput<char> input;

            public Slash(IInput<char> input)
            {
                this.input = input;
            }

            public IOutput<char, Slash> Realize()
            {
                if (this.input.Current == '/')
                {
                    return new Output<char, Slash>(true, this, this.input.Next());
                }
                else
                {
                    return new Output<char, Slash>(false, default, this.input);
                }
            }
        }

        public abstract class AlphaNumeric : IDeferredAstNode<char, AlphaNumeric>
        {
            private AlphaNumeric()
            {
            }

            public IOutput<char, AlphaNumeric> Realize()
            {
                return this.RealizeImpl();
            }

            protected abstract IOutput<char, AlphaNumeric> RealizeImpl();

            public sealed class A : AlphaNumeric, IDeferredAstNode<char, A>
            {
                private readonly IInput<char> input;

                public A(IInput<char> input)
                {
                    this.input = input;
                }

                public new IOutput<char, A> Realize()
                {
                    if (this.input.Current == 'A')
                    {
                        return new Output<char, A>(true, this, this.input.Next());
                    }
                    else
                    {
                        return new Output<char, A>(false, default, this.input);
                    }
                }

                protected override IOutput<char, AlphaNumeric> RealizeImpl()
                {
                    return this.Realize();
                }
            }
        }

        /*public sealed class AlphaNumeric : IDeferredAstNode<char, AlphaNumeric>
        {
            private readonly IParser<char, AlphaNumeric> parser;
            private readonly IInput<char> input;

            //// private char @char;

            //// private bool deferred;

            public AlphaNumeric(IParser<char, AlphaNumeric> parser, IInput<char> input)
            {
                this.parser = parser;
                this.input = input;

                //// this.deferred = true;
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

                }
            }

            public IOutput<char, AlphaNumeric> Realize()
            {
                if (this.deferred)
                {
                    var _char_1 = this.parser.Parse(input);
                    if (_char_1.Success)
                    {
                        this.@char = _char_1.Parsed.@char;
                        this.deferred = false;
                        return new Output<char, AlphaNumeric>(true, this, _char_1.Remainder);
                    }
                    else
                    {
                        return new Output<char, AlphaNumeric>(false, default, this.input);
                    }
                }
                else
                {
                    //// TODO where do you get the remainder from for the case where this instance was constructed with the char already? //// TODO maybe that constructor overload shouldn't exist?
                    return new Output<char, AlphaNumeric>(true, this, null);
                }
            }
        }*/

        public sealed class Many<T> : IDeferredAstNode<char, IEnumerable<T>> where T : IDeferredAstNode<char, T>
        {
            ////private IInput<char> input;
            private readonly Func<IInput<char>, T> nodeFactory;

            private readonly Func<IDeferredOutput2<char>> future;

            /*public Many(IInput<char> input, Func<IInput<char>, T> nodeFactory)
            {
                this.input = input;
                this.nodeFactory = nodeFactory;

                this.future = null;
            }*/

            public Many(Func<IDeferredOutput2<char>> future, Func<IInput<char>, T> nodeFactory)
            {
                this.future = future; //// TODO this should be of type `future`
                this.nodeFactory = nodeFactory;
            }

            public IOutput<char, IEnumerable<T>> Realize() //// TODO the other ones all have the second type as themselves...maybe you need an ienumerable property instead?
            {
                var output2 = this.future();
                if (!output2.Success)
                {
                    return new Output<char, IEnumerable<T>>(false, default, output2.Remainder);
                }

                var input = output2.Remainder;

                var sequence = new List<T>();
                var node = this.nodeFactory(input);
                var output = node.Realize();
                while (output.Success)
                {
                    sequence.Add(output.Parsed);
                    node = this.nodeFactory(output.Remainder);
                }

                return new Output<char, IEnumerable<T>>(true, sequence, output.Remainder);
            }
        }

        public sealed class Segment : IDeferredAstNode<char, Segment>
        {
            ////private readonly IParser<char, Segment> parser;
            private readonly IInput<char> input;
            ////private Slash slash;
            ////private IEnumerable<AlphaNumeric> characters;

            ////private bool deferred;

            public Segment(IInput<char> input)
               //// : this(SegmentParser.Instance, input)
            {
                this.input = input;
            }

            /*public Segment(IParser<char, Segment> parser, IInput<char> input)
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
            }*/

            public Slash Slash
            {
                get
                {
                    return new Slash(this.input);
                }
            }

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(() =>
                    {
                        var output = this.Slash.Realize();
                        return new DeferredOutput2<char>(output.Success, output.Remainder);
                    },
                    input => new AlphaNumeric.A(input)); //// TODO what would a discriminated union actually look like here?

                    /*if (this.deferred)
                    {
                        throw new System.Exception("TODO not parsed yet");
                        // TODO ideferredoutput should probably just be ioutput so that you can get this remainder
                        // return new Characters(CharactersParser.Instance, this.Slash.Realize().Remainder)
                    }

                    return this.characters;*/
                }
            }

            public IOutput<char, Segment> Realize()
            {
                var slashOutput = this.Slash.Realize();
                if (!slashOutput.Success)
                {
                    return new Output<char, Segment>(false, default, this.input);
                }

                var charactersOutput = this.Characters.Realize();
                if (!charactersOutput.Success)
                {
                    return new Output<char, Segment>(false, default, this.input);
                }

                return new Output<char, Segment>(true, this, charactersOutput.Remainder);

                //// TODO you should cache the deferredoutput instance
                /*if (this.deferred)
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

                    //// TODO this method basically just mimics the parser implementations; should you actually just put all of the parsing here?

                    return new DeferredOutput<Segment>(true, this);
                }*/
            }
        }

        public sealed class EqualsSign : IDeferredAstNode<char, EqualsSign>
        {
            private readonly IInput<char> input;

            public EqualsSign(IInput<char> input)
            {
                this.input = input;
            }

            public IOutput<char, EqualsSign> Realize()
            {
                if (this.input.Current == '=')
                {
                    return new Output<char, EqualsSign>(true, this, this.input.Next());
                }
                else
                {
                    return new Output<char, EqualsSign>(false, default, this.input);
                }
            }
        }

        public sealed class OptionName : IDeferredAstNode<char, OptionName>
        {
            private readonly IInput<char> input;

            /*public OptionName(IEnumerable<AlphaNumeric> characters)
{
Characters = characters;
}*/

            public OptionName(IInput<char> input)
            {
                this.input = input;
            }

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(() => DeferredOutput2.FromValue(this.input), input => new AlphaNumeric.A(input));
                }
            }

            public IOutput<char, OptionName> Realize()
            {
                var charactersOutput = this.Characters.Realize();
                if (!charactersOutput.Success)
                {
                    return new Output<char, OptionName>(false, default, this.input);
                }

                return new Output<char, OptionName>(true, this, charactersOutput.Remainder);
            }
        }

        public sealed class OptionValue : IDeferredAstNode<char, OptionValue>
        {
            private readonly IInput<char> input;

            public OptionValue(IInput<char> input)
            {
                this.input = input;
            }

            /*public OptionValue(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }*/

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(() => DeferredOutput2.FromValue(this.input), input => new AlphaNumeric.A(input));
                }
            }

            public IOutput<char, OptionValue> Realize()
            {
                var charactersOutput = this.Characters.Realize();
                if (!charactersOutput.Success)
                {
                    return new Output<char, OptionValue>(false, default, this.input);
                }

                return new Output<char, OptionValue>(true, this, charactersOutput.Remainder);
            }
        }

        public sealed class QueryOption : IDeferredAstNode<char, QueryOption>
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

            public IOutput<char, QueryOption> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class QuestionMark : IDeferredAstNode<char, QuestionMark>
        {
            private QuestionMark()
            {
            }

            public static QuestionMark Instance { get; } = new QuestionMark();

            public IOutput<char, QuestionMark> Realize()
            {
                throw new System.NotImplementedException();
            }
        }

        public class OdataUri : DeferredOdataUri
        {
            public OdataUri(IEnumerable<Segment> segments, QuestionMark questionMark, IEnumerable<QueryOption> queryOptions)
                :base(new Lazy<IEnumerable<Segment>>(segments), new Lazy<QuestionMark>(questionMark), new Lazy<IEnumerable<QueryOption>>(queryOptions))
            {
            }
        }

        public class DeferredOdataUri : IDeferredAstNode<char, OdataUri>
        {
            private readonly Lazy<CombinatorParsingV3.IOutput<char, IEnumerable<Segment>>> segments;

            protected DeferredOdataUri(Lazy<IOutput<char, IEnumerable<Segment>>> segments, Lazy<QuestionMark> questionMark, Lazy<IEnumerable<QueryOption>> queryOptions)
            {
                this.segments = segments;
            }

            public DeferredOdataUri(IInput<char> input)
                : this(input, SegmentParser.Instance)
            {
            }

            public DeferredOdataUri(IInput<char> input, IParser<char, IEnumerable<Segment>> segmentParser, /**/)
                : this(new Lazy<IEnumerable<Segment>>(() => segmentParser.Parse(input)))
            {
            }

            public IEnumerable<Segment> Segments
            {
                get
                {
                    if (segments.Value.Success)
                    {
                        return segments.Value.Parsed;
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }
                }
            }

            public QuestionMark QuestionMark { get; }
            public IEnumerable<QueryOption> QueryOptions { get; }

            public IOutput<char, OdataUri> Realize()
            {
                var finalSegments = segments.Value.Parsed;
                if (!segments.Value.Success)
                {
                    return new DeferredOutput<OdataUri>(false, default);
                }

                // 

                return new DeferredOutput<OdataUri>(true, new OdataUri(finalSegments, ));
            }
        }

        /*public class UriParserV2 : IDeferredParser<char, OdataUri, DeferredOdataUri>
        {
            public DeferredOdataUri Parse(IInput<char> input)
            {
                return new DeferredOdataUri(
                    );
            }
        }*/
    }
}
