namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _pcharⳆʺx2FʺⳆʺx3FʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>
    {
        private _pcharⳆʺx2FʺⳆʺx3FʺTranscriber()
        {
        }
        
        public static _pcharⳆʺx2FʺⳆʺx3FʺTranscriber Instance { get; } = new _pcharⳆʺx2FʺⳆʺx3FʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(node._pchar_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx3FʺTranscriber.Instance.Transcribe(node._ʺx3Fʺ_1, context);

return default;
            }
        }
    }
    
}
