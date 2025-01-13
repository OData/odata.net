namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveLiteralInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON>
    {
        private _primitiveLiteralInJSONTranscriber()
        {
        }
        
        public static _primitiveLiteralInJSONTranscriber Instance { get; } = new _primitiveLiteralInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._stringInJSONTranscriber.Instance.Transcribe(node._stringInJSON_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._numberInJSONTranscriber.Instance.Transcribe(node._numberInJSON_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx74x72x75x65ʺTranscriber.Instance.Transcribe(node._ʺx74x72x75x65ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx66x61x6Cx73x65ʺTranscriber.Instance.Transcribe(node._ʺx66x61x6Cx73x65ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx6Ex75x6Cx6CʺTranscriber.Instance.Transcribe(node._ʺx6Ex75x6Cx6Cʺ_1, context);

return default;
            }
        }
    }
    
}
