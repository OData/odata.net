namespace AbnfParser.CstNodes
{
    public abstract class Element
    {
        private Element()
        {
        }

        public sealed class RuleName : Element
        {
            public RuleName(CstNodes.RuleName value)
            {
                Value = value;
            }

            public CstNodes.RuleName Value { get; }
        }

        public sealed class Group : Element
        {
            public Group(CstNodes.Group value)
            {
                Value = value;
            }

            public CstNodes.Group Value { get; }
        }

        public sealed class Option : Element
        {
            public Option(CstNodes.Option value)
            {
                Value = value;
            }

            public CstNodes.Option Value { get; }
        }

        public sealed class CharVal : Element
        {
            public CharVal(CstNodes.CharVal value)
            {
                Value = value;
            }

            public CstNodes.CharVal Value { get; }
        }

        public sealed class NumVal : Element
        {
            public NumVal(CstNodes.NumVal value)
            {
                Value = value;
            }

            public CstNodes.NumVal Value { get; }
        }

        public sealed class ProseVal : Element
        {
            public ProseVal(CstNodes.ProseVal value)
            {
                Value = value;
            }

            public CstNodes.ProseVal Value { get; }
        }
    }
}
