namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _ʺx24x69x64ʺⳆʺx69x64ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ>
    {
        private _ʺx24x69x64ʺⳆʺx69x64ʺTranscriber()
        {
        }
        
        public static _ʺx24x69x64ʺⳆʺx69x64ʺTranscriber Instance { get; } = new _ʺx24x69x64ʺⳆʺx69x64ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx24x69x64ʺTranscriber.Instance.Transcribe(node._ʺx24x69x64ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx69x64ʺTranscriber.Instance.Transcribe(node._ʺx69x64ʺ_1, context);

return default;
            }
        }
    }
    
}
