namespace AbnfParser.CstNodes
{
    public abstract class Repetition
    {
        private Repetition()
        {
        }

        public sealed class ElementOnly : Repetition
        {
            public ElementOnly(Element element)
            {
                Element = element;
            }

            public Element Element { get; }
        }

        public sealed class RepeatAndElement : Repetition
        {
            public RepeatAndElement(Repeat repeat, Element element)
            {
                Repeat = repeat;
                Element = element;
            }

            public Repeat Repeat { get; }
            public Element Element { get; }
        }
    }
}
