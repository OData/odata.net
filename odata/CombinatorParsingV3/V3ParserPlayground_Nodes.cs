namespace CombinatorParsingV3
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections;
    using System.Threading;

    public static partial class V3ParserPlayground
    {
        public sealed class Slash : IDeferredAstNode<char, Slash>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            public Slash(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            public IOutput<char, Slash> Realize()
            {
                var output = this.future();
                if (!output.Success)
                {
                    return new Output<char, Slash>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '/')
                {
                    return new Output<char, Slash>(true, this, input.Next());
                }
                else
                {
                    return new Output<char, Slash>(false, default, input);
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
                private readonly Func<IDeferredOutput2<char>> future;

                public A(Func<IDeferredOutput2<char>> future)
                {
                    this.future = future;
                }

                public new IOutput<char, A> Realize()
                {
                    var output = this.future();
                    if (!output.Success)
                    {
                        return new Output<char, A>(false, default, output.Remainder);
                    }

                    var input = output.Remainder;

                    if (input.Current == 'A')
                    {
                        return new Output<char, A>(true, this, input.Next());
                    }
                    else
                    {
                        return new Output<char, A>(false, default, input);
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
            private readonly Func<Func<IDeferredOutput2<char>>, T> nodeFactory;

            private readonly Func<IDeferredOutput2<char>> future;

            /*public Many(IInput<char> input, Func<IInput<char>, T> nodeFactory)
            {
                this.input = input;
                this.nodeFactory = nodeFactory;

                this.future = null;
            }*/

            public Many(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, T> nodeFactory)
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
                var node = this.nodeFactory(DeferredOutput2.FromValue(input));
                var output = node.Realize();
                while (output.Success)
                {
                    sequence.Add(output.Parsed);
                    node = this.nodeFactory(DeferredOutput2.FromValue(output.Remainder));
                }

                return new Output<char, IEnumerable<T>>(true, sequence, input);
            }
        }

        public sealed class OptionalNode<T> : IDeferredAstNode<char, OptionalNode<T>> where T : IDeferredAstNode<char, T>
        {
            private readonly Func<IDeferredOutput2<char>> future;
            private readonly Func<Func<IDeferredOutput2<char>>, T?> nodeFactory;

            public OptionalNode(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, T?> nodeFactory)
            {
                this.future = future;
                this.nodeFactory = nodeFactory;
            }

            public T? Value
            {
                get
                {
                    return Get().Parsed;
                }
            }

            private IOutput<char, T?> Get()
            {
                IDeferredOutput2<char> output = null;
                var node = this.nodeFactory(() => output = this.future());
                if (node == null)
                {
                    return new Output<char, T?>(true, node, output.Remainder);
                }
                else
                {
                    return new Output<char, T?>(true, node, output.Remainder);
                }
            }

            public IOutput<char, OptionalNode<T>> Realize()
            {
                var output = Get();
                return new Output<char, OptionalNode<T>>(true, this, output.Remainder);
            }
        }

        public sealed class SequenceNode<T> : IDeferredAstNode<char, SequenceNode<T>> where T : IDeferredAstNode<char, T>
        {
            private readonly Func<IDeferredOutput2<char>> future;
            private readonly Func<Func<IDeferredOutput2<char>>, T> nodeFactory;

            public SequenceNode(Func<IDeferredOutput2<char>> future, Func<Func<IDeferredOutput2<char>>, T> nodeFactory) //// TODO use a DU for the terminal node?
            {
                this.future = future;
                this.nodeFactory = nodeFactory;
            }

            public T Value
            {
                get
                {
                    return this.nodeFactory(this.future);
                }
            }

            public OptionalNode<SequenceNode<T>> Next
            {
                get
                {
                    return new OptionalNode<SequenceNode<T>>(DeferredOutput2.ToPromise(this.Value.Realize), input => new SequenceNode<T>(input, this.nodeFactory));
                }
            }

            public IOutput<char, SequenceNode<T>> Realize()
            {
                var output = this.Value.Realize();
                if (output.Success)
                {
                    return new Output<char, SequenceNode<T>>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, SequenceNode<T>>(false, default, output.Remainder);
                }
            }
        }

        public sealed class Segment : IDeferredAstNode<char, Segment>
        {
            ////private readonly IParser<char, Segment> parser;
            private readonly Func<IDeferredOutput2<char>> future;

            ////private Slash slash;
            ////private IEnumerable<AlphaNumeric> characters;

            ////private bool deferred;

            public Segment(Func<IDeferredOutput2<char>> future)
            //// : this(SegmentParser.Instance, input)
            {
                this.future = future;
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
                    return new Slash(this.future);
                }
            }

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(
                        DeferredOutput2.ToPromise(this.Slash.Realize),
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
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, Segment>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, Segment>(false, default, output.Remainder);
                }

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
            ////private readonly IInput<char> input;
            private readonly Func<IDeferredOutput2<char>> future;

            /*public EqualsSign(IInput<char> input)
            {
                this.input = input;
            }*/

            public EqualsSign(Func<IDeferredOutput2<char>> future)
            {
                //// TODO create the `future` type and then follow this single constructor pattern everywhere
                this.future = future;
            }

            public IOutput<char, EqualsSign> Realize()
            {
                var output = this.future();
                if (!output.Success)
                {
                    return new Output<char, EqualsSign>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '=')
                {
                    return new Output<char, EqualsSign>(true, this, input.Next());
                }
                else
                {
                    return new Output<char, EqualsSign>(false, default, input);
                }
            }
        }

        public sealed class OptionName : IDeferredAstNode<char, OptionName>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            /*public OptionName(IEnumerable<AlphaNumeric> characters)
{
Characters = characters;
}*/

            public OptionName(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(future, input => new AlphaNumeric.A(input));
                }
            }

            public IOutput<char, OptionName> Realize()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, OptionName>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, OptionName>(false, default, output.Remainder);
                }
            }
        }

        public sealed class OptionValue : IDeferredAstNode<char, OptionValue>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            /*private readonly IInput<char> input;

public OptionValue(IInput<char> input)
{
this.input = input;
}*/

            /*public OptionValue(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }*/

            public OptionValue(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            public Many<AlphaNumeric> Characters
            {
                get
                {
                    return new Many<AlphaNumeric>(this.future, input => new AlphaNumeric.A(input));
                }
            }

            public IOutput<char, OptionValue> Realize()
            {
                var output = this.Characters.Realize();
                if (output.Success)
                {
                    return new Output<char, OptionValue>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, OptionValue>(false, default, output.Remainder);
                }

            }
        }

        public sealed class QueryOption : IDeferredAstNode<char, QueryOption>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            public QueryOption(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            /*public QueryOption(OptionName name, EqualsSign equalsSign, OptionValue optionValue)
            {
                Name = name;
                EqualsSign = equalsSign;
                OptionValue = optionValue;
            }*/

            public OptionName Name
            {
                get
                {
                    return new OptionName(this.future);
                }
            }

            public EqualsSign EqualsSign
            {
                get
                {
                    return new EqualsSign(DeferredOutput2.ToPromise(this.Name.Realize));
                }
            }

            public OptionValue OptionValue
            {
                get
                {
                    return new OptionValue(DeferredOutput2.ToPromise(this.EqualsSign.Realize));
                }
            }

            public IOutput<char, QueryOption> Realize()
            {
                var output = this.OptionValue.Realize();
                if (output.Success)
                {
                    return new Output<char, QueryOption>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, QueryOption>(false, default, output.Remainder);
                }
            }
        }

        public sealed class QuestionMark : IDeferredAstNode<char, QuestionMark>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            /*private QuestionMark()
{
}

public static QuestionMark Instance { get; } = new QuestionMark();*/

            public QuestionMark(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            public IOutput<char, QuestionMark> Realize()
            {
                var output = this.future();
                if (!output.Success)
                {
                    return new Output<char, QuestionMark>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '?')
                {
                    return new Output<char, QuestionMark>(true, this, input.Next());
                }
                else
                {
                    return new Output<char, QuestionMark>(false, default, input);
                }
            }
        }

        public sealed class OdataUri : IDeferredAstNode<char, OdataUri>
        {
            private readonly Func<IDeferredOutput2<char>> future;

            public OdataUri(Func<IDeferredOutput2<char>> future)
            {
                this.future = future;
            }

            public Many<Segment> Segments //// TODO implement "at least one"
            {
                get
                {
                    return new Many<Segment>(this.future, input => new Segment(input));
                }
            }

            public QuestionMark QuestionMark
            {
                get
                {
                    return new QuestionMark(DeferredOutput2.ToPromise(this.Segments.Realize));
                }
            }

            public Many<QueryOption> QueryOptions
            {
                get
                {
                    return new Many<QueryOption>(DeferredOutput2.ToPromise(this.QueryOptions.Realize), input => new QueryOption(input));
                }
            }

            public IOutput<char, OdataUri> Realize()
            {
                var output = this.QueryOptions.Realize();
                if (output.Success)
                {
                    return new Output<char, OdataUri>(true, this, output.Remainder);
                }
                else
                {
                    return new Output<char, OdataUri>(false, default, output.Remainder);
                }
            }
        }

        /*public class OdataUri : DeferredOdataUri
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

            public DeferredOdataUri(IInput<char> input, IParser<char, IEnumerable<Segment>> segmentParser, )
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
        }*/

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
