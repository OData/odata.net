namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ>
    {
        private _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃTranscriber()
        {
        }
        
        public static _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃTranscriber Instance { get; } = new _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(node._namespace_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1, context);
__GeneratedOdataV4.Trancsribers.Inners._ⲤtermNameⳆSTARↃTranscriber.Instance.Transcribe(node._ⲤtermNameⳆSTARↃ_1, context);

return default;
            }
        }
    }
    
}
