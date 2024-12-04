namespace AbnfParser.CstNodes.Core
{
    public sealed class Crlf
    {
        public Crlf(Cr cr, Lf lf)
        {
            Cr = cr;
            Lf = lf;
        }

        public Cr Cr { get; }
        public Lf Lf { get; }
    }
}
