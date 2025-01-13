namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _intTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._int>
    {
        private _intTranscriber()
        {
        }
        
        public static _intTranscriber Instance { get; } = new _intTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._int value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._int.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._int._ʺx30ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ⲤoneToNine_ЖDIGITↃTranscriber.Instance.Transcribe(node._ⲤoneToNine_ЖDIGITↃ_1, context);

return default;
            }
        }
    }
    
}
