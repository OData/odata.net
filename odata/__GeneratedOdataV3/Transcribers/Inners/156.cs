namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ>
    {
        private _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺTranscriber()
        {
        }
        
        public static _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺTranscriber Instance { get; } = new _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x74x6Fx70ʺTranscriber.Instance.Transcribe(node._ʺx24x74x6Fx70ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx74x6Fx70ʺTranscriber.Instance.Transcribe(node._ʺx74x6Fx70ʺ_1, context);

return default;
            }
        }
    }
    
}
