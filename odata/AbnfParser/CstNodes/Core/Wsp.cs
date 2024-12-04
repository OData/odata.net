namespace AbnfParser.CstNodes.Core
{
    public abstract class Wsp
    {
        private Wsp()
        {
        }

        public sealed class Space : Wsp
        {
            public Space(Sp sp)
            {
                Sp = sp;
            }

            public Sp Sp { get; }
        }

        public sealed class Tab : Wsp
        {
            public Tab(Htab htab)
            {
                Htab = htab;
            }

            public Htab Htab { get; }
        }
    }
}
