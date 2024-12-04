namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc5234
    /// </summary>
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

            public sealed class RuleInner : Inner
            {
                public RuleInner(Rule rule)
                {
                    Rule = rule;
                }

                public Rule Rule { get; }
            }

            public sealed class CommentInner : Inner
            {
                public CommentInner(IEnumerable<Cwsp> cwsps, Cnl cnl)
                {
                    Cwsps = cwsps;
                    Cnl = cnl;
                }

                public IEnumerable<Cwsp> Cwsps { get; }
                public Cnl Cnl { get; }
            }
        }
    }
}
