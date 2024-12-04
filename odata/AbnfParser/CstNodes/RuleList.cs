namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    public sealed class RuleList
    {
        public RuleList(IEnumerable<Inner> inners)
        {
            //// TODO assert one or more elements
            Inners = inners;
        }

        public IEnumerable<Inner> Inners { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }
        }
    }
}
