namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public abstract class Repeat
    {
        private Repeat()
        {
        }

        public sealed class Count : Repeat
        {
            public Count(IEnumerable<Digit> digits)
            {
                //// TODO assert one or more
                Digits = digits;
            }

            public IEnumerable<Digit> Digits { get; }
        }

        public sealed class Range : Repeat
        {
            public Range(IEnumerable<Digit> prefixDigits, x2A asterisk, IEnumerable<Digit> suffixDigits)
            {
                PrefixDigits = prefixDigits;
                Asterisk = asterisk;
                SuffixDigits = suffixDigits;
            }

            public IEnumerable<Digit> PrefixDigits { get; }
            public x2A Asterisk { get; }
            public IEnumerable<Digit> SuffixDigits { get; }
        }
    }
}
