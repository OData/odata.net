namespace NewStuff.Odata.Headers
{
    using NewStuff.Odata.Headers.Inners;
    using System.Collections.Generic;

    /// <summary>
    /// https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_HeaderFieldExtensibility
    /// 
    /// The OData standard calls any header not used by OData a "custom" header, even headers defined by HTTP that are simply not directly leveraged by OData.
    /// </summary>
    public abstract class CustomHeader
    {
        private CustomHeader()
        {
        }

        public sealed class First : CustomHeader //// TODO better name
        {
            public First(NotOHeaderCharacter firstCharacter, IEnumerable<HeaderCharacter> remainingCharacters)
            {
                FirstCharacter = firstCharacter;
                RemainingCharacters = remainingCharacters;
            }

            public NotOHeaderCharacter FirstCharacter { get; }

            public IEnumerable<HeaderCharacter> RemainingCharacters { get; }
        }

        public sealed class Second : CustomHeader //// TODO better name
        {
            public Second(HeaderCharacter.O o, NotDHeaderCharacter secondCharacter, IEnumerable<HeaderCharacter> remainingCharacters)
            {
                O = o;
                SecondCharacter = secondCharacter;
                RemainingCharacters = remainingCharacters;
            }

            public HeaderCharacter.O O { get; }

            public NotDHeaderCharacter SecondCharacter { get; }

            public IEnumerable<HeaderCharacter> RemainingCharacters { get; }
        }

        public sealed class Third : CustomHeader //// TODO better name
        {
            public Third(HeaderCharacter.O o, HeaderCharacter.D d, NotAHeaderCharacter thirdCharacter, IEnumerable<HeaderCharacter> remainingCharacters)
            {
                //// TODO this should be case insensitive
                O = o;
                D = d;
                ThirdCharacter = thirdCharacter;
                RemainingCharacters = remainingCharacters;
            }

            public HeaderCharacter.O O { get; }

            public HeaderCharacter.D D { get; }

            public NotAHeaderCharacter ThirdCharacter { get; } //// TODO you are here

            public IEnumerable<HeaderCharacter> RemainingCharacters { get; }
        }

        public sealed class Fourth : CustomHeader //// TODO better name
        {
            public Fourth(HeaderCharacter.O o, HeaderCharacter.D d, HeaderCharacter.A a, NotTHeaderCharacter fourthCharacter, IEnumerable<HeaderCharacter> remainingCharacters)
            {
                //// TODO this should be case insensitive
                O = o;
                D = d;
                A = a;
                FourthCharacter = fourthCharacter;
                RemainingCharacters = remainingCharacters;
            }

            public HeaderCharacter.O O { get; }

            public HeaderCharacter.D D { get; }

            public HeaderCharacter.A A { get; }

            public NotTHeaderCharacter FourthCharacter { get; }

            public IEnumerable<HeaderCharacter> RemainingCharacters { get; }
        }

        public sealed class Fifth : CustomHeader //// TODO better name
        {
            public Fifth(HeaderCharacter.O o, HeaderCharacter.D d, HeaderCharacter.A a, HeaderCharacter.T t, NotAHeaderCharacter fifthCharacter, IEnumerable<HeaderCharacter> remainingCharacters)
            {
                //// TODO this should be case insensitive
                O = o;
                D = d;
                A = a;
                T = t;
                FifthCharacter = fifthCharacter;
                RemainingCharacters = remainingCharacters;
            }

            public HeaderCharacter.O O { get; }

            public HeaderCharacter.D D { get; }

            public HeaderCharacter.A A { get; }

            public HeaderCharacter.T T { get; }

            public NotAHeaderCharacter FifthCharacter { get; }

            public IEnumerable<HeaderCharacter> RemainingCharacters { get; }
        }
    }
}
