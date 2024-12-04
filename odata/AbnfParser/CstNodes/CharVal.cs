namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public class CharVal
    {
        public CharVal(Dquote openDquote, IEnumerable<Inner> inners, Dquote closeDquote)
        {
            OpenDquote = openDquote;
            Inners = inners;
            CloseDquote = closeDquote;
        }

        public Dquote OpenDquote { get; }
        public IEnumerable<Inner> Inners { get; }
        public Dquote CloseDquote { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }

            public sealed class x20 : Inner
            {
                public x20(Core.x20 value)
                {
                    Value = value;
                }

                public Core.x20 Value { get; }
            }
            
            //// TODO do the rest
        }
    }
}
