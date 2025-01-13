namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ>
    {
        private _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺTranscriber()
        {
        }
        
        public static _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺTranscriber Instance { get; } = new _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺTranscriber.Instance.Transcribe(node._ʺx2Fx24x65x6Ex74x69x74x79ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2Fx24x64x65x6Cx74x61ʺTranscriber.Instance.Transcribe(node._ʺx2Fx24x64x65x6Cx74x61ʺ_1, context);

return default;
            }
        }
    }
    
}
