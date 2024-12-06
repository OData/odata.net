namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    public sealed class Elements
    {
        public Elements(Alternation alternation, IEnumerable<Cwsp> cwsps)
        {
            Alternation = alternation;
            Cwsps = cwsps;
        }

        public Alternation Alternation { get; }
        public IEnumerable<Cwsp> Cwsps { get; }
    }
}
