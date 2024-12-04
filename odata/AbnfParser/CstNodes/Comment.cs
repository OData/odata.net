namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public sealed class Comment
    {
        public Comment(x3B semicolon, IEnumerable<Inner> inners, Crlf crlf)
        {
            Semicolon = semicolon;
            Inners = inners;
            Crlf = crlf;
        }

        public x3B Semicolon { get; }
        public IEnumerable<Inner> Inners { get; }
        public Crlf Crlf { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }

            public sealed class InnerWsp : Inner
            {
                public InnerWsp(Wsp wsp)
                {
                    Wsp = wsp;
                }

                public Wsp Wsp { get; }
            }

            public sealed class InnerVchar : Inner
            {
                public InnerVchar(Vchar vchar)
                {
                    Vchar = vchar;
                }

                public Vchar Vchar { get; }
            }
        }
    }
}
