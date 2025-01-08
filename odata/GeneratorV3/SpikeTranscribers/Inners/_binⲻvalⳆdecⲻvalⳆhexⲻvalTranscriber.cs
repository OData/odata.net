namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;
    using Root;

    public sealed class _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber : ITranscriber<Inners._binⲻvalⳆdecⲻvalⳆhexⲻval>
    {
        private _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber()
        {
        }

        public static _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber Instance { get; } = new _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber();

        public void Transcribe(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._binⲻvalⳆdecⲻvalⳆhexⲻval.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval node, StringBuilder context)
            {
                throw new System.NotImplementedException();
            }

            protected internal override Void Accept(Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval node, StringBuilder context)
            {
                _hexⲻvalTranscriber.Instance.Transcribe(node._hexⲻval_1, context);

                return default;
            }
        }
    }
}
