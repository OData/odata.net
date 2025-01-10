namespace __Generated.Trancsribers.Inners
{
    public sealed class _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval>
    {
        private _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber()
        {
        }
        
        public static _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber Instance { get; } = new _binⲻvalⳆdecⲻvalⳆhexⲻvalTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._binⲻvalTranscriber.Instance.Transcribe(node._binⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._decⲻvalTranscriber.Instance.Transcribe(node._decⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._hexⲻvalTranscriber.Instance.Transcribe(node._hexⲻval_1, context);

return default;
            }
        }
    }
    
}
