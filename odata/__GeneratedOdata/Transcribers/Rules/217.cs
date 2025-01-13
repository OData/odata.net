namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qcharⲻJSONⲻspecialTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>
    {
        private _qcharⲻJSONⲻspecialTranscriber()
        {
        }
        
        public static _qcharⲻJSONⲻspecialTranscriber Instance { get; } = new _qcharⲻJSONⲻspecialTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._SP node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._SPTranscriber.Instance.Transcribe(node._SP_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx7BʺTranscriber.Instance.Transcribe(node._ʺx7Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx7DʺTranscriber.Instance.Transcribe(node._ʺx7Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx5BʺTranscriber.Instance.Transcribe(node._ʺx5Bʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx5DʺTranscriber.Instance.Transcribe(node._ʺx5Dʺ_1, context);

return default;
            }
        }
    }
    
}
