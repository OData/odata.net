namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    public sealed class Concatenation
    {
        public Concatenation(Repetition repetition, IEnumerable<Inner> inners)
        {
            Repetition = repetition;
            Inners = inners;
        }

        public Repetition Repetition { get; }
        public IEnumerable<Inner> Inners { get; }

        public sealed class Inner
        {
            public Inner(IEnumerable<Cwsp> cwsps, Repetition repetition)
            {
                //// TODO assert one or more
                Cwsps = cwsps;
                Repetition = repetition;
            }

            public IEnumerable<Cwsp> Cwsps { get; }
            public Repetition Repetition { get; }
        }
    }
}
