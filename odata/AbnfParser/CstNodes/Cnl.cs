namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;

    public abstract class Cnl
    {
        private Cnl()
        {
        }

        public sealed class Comment : Cnl
        {
            public Comment(CstNodes.Comment value)
            {
                Value = value;
            }

            public CstNodes.Comment Value { get; }
        }

        public sealed class Newline : Cnl
        {
            public Newline(Crlf crlf)
            {
                Crlf = crlf;
            }

            public Crlf Crlf { get; }
        }
    }
}
