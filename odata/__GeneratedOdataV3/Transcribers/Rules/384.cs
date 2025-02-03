namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _hierⲻpartTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._hierⲻpart>
    {
        private _hierⲻpartTranscriber()
        {
        }
        
        public static _hierⲻpartTranscriber Instance { get; } = new _hierⲻpartTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._hierⲻpart.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2Fx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fx2Fʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._authorityTranscriber.Instance.Transcribe(node._authority_1, context);
__GeneratedOdataV3.Trancsribers.Rules._pathⲻabemptyTranscriber.Instance.Transcribe(node._pathⲻabempty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pathⲻabsoluteTranscriber.Instance.Transcribe(node._pathⲻabsolute_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pathⲻrootlessTranscriber.Instance.Transcribe(node._pathⲻrootless_1, context);

return default;
            }
        }
    }
    
}
