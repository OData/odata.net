using AbnfParser.CstNodes.Core;
using System.Collections.Generic;

namespace AbnfParser.CstNodes
{
    public sealed class RuleName
    {
        public RuleName(Alpha alpha, IEnumerable<Inner> inners)
        {
            Alpha = alpha;
            Inners = inners;
        }

        public Alpha Alpha { get; }
        public IEnumerable<Inner> Inners { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }

            public sealed class AlphaInner : Inner
            {
                public AlphaInner(Alpha alpha)
                {
                    Alpha = alpha;
                }

                public Alpha Alpha { get; }
            }

            public sealed class DigitInner : Inner
            {
                public DigitInner(Digit digit)
                {
                    Digit = digit;
                }

                public Digit Digit { get; }
            }

            public sealed class DashInner : Inner
            {
                public DashInner(x2D dash)
                {
                    Dash = dash;
                }

                public x2D Dash { get; }
            }
        }
    }
}
