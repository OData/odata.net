namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _elementTranscriber : ITranscriber<_element>
    {
        private _elementTranscriber()
        {
        }

        public static _elementTranscriber Instance { get; } = new _elementTranscriber();

        public void Transcribe(_element value, StringBuilder builder)
        {

        }

        private sealed class Visitor : _element.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_element._rulename node, StringBuilder context)
            {
                RuleNameTranscriber.Instance.Transcribe(node._rulename_1, context);

                return default;
            }

            protected internal override Void Accept(_element._group node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(_element._option node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(_element._charⲻval node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(_element._numⲻval node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(_element._proseⲻval node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
