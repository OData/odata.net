namespace CombinatorParsingV3
{
    using System.Collections.Generic;

    public static partial class V3ParserPlayground
    {
        public sealed class Slash
        {
            private Slash()
            {
            }

            public static Slash Instance { get; } = new Slash();
        }

        public sealed class AlphaNumeric
        {
            public AlphaNumeric(char @char)
            {
                Char = @char;
            }

            public char Char { get; }
        }

        public sealed class Segment
        {
            public Segment(Slash slash, IEnumerable<AlphaNumeric> characters)
            {
                Slash = slash;
                Characters = characters;
            }

            public Slash Slash { get; }
            public IEnumerable<AlphaNumeric> Characters { get; }
        }

        public sealed class EqualsSign
        {
            private EqualsSign()
            {
            }

            public static EqualsSign Instance { get; } = new EqualsSign();
        }

        public sealed class OptionName
        {
            public OptionName(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }

            public IEnumerable<AlphaNumeric> Characters { get; }
        }

        public sealed class OptionValue
        {
            public OptionValue(IEnumerable<AlphaNumeric> characters)
            {
                Characters = characters;
            }

            public IEnumerable<AlphaNumeric> Characters { get; }
        }

        public sealed class QueryOption
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
        }

        public sealed class OdataUri
        {
            public OdataUri(IEnumerable<Segment> segments, IEnumerable<QueryOption> queryOptions)
            {
                Segments = segments;
                QueryOptions = queryOptions;
            }

            public IEnumerable<Segment> Segments { get; }
            public IEnumerable<QueryOption> QueryOptions { get; }
        }
    }
}
