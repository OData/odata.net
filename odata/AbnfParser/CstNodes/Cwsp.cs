namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;

    public abstract class Cwsp
    {
        private Cwsp()
        {
        }

        public sealed class WspOnly : Cwsp
        {
            public WspOnly(Wsp wsp)
            {
                Wsp = wsp;
            }

            public Wsp Wsp { get; }
        }

        public sealed class CnlAndWsp : Cwsp
        {
            public CnlAndWsp(Cnl cnl, Wsp wsp)
            {
                Cnl = cnl;
                Wsp = wsp;
            }

            public Cnl Cnl { get; }
            public Wsp Wsp { get; }
        }
    }
}
