namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _intTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._int>
    {
        private _intTranscriber()
        {
        }
        
        public static _intTranscriber Instance { get; } = new _intTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._int value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._int.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ⲤoneToNine_ЖDIGITↃTranscriber.Instance.Transcribe(node._ⲤoneToNine_ЖDIGITↃ_1, context);

return default;
            }
        }
    }
    
}
