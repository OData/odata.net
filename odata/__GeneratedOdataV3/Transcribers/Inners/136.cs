namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ>
    {
        private _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺTranscriber()
        {
        }
        
        public static _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺTranscriber Instance { get; } = new _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx24x66x69x6Cx74x65x72ʺTranscriber.Instance.Transcribe(node._ʺx24x66x69x6Cx74x65x72ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx66x69x6Cx74x65x72ʺTranscriber.Instance.Transcribe(node._ʺx66x69x6Cx74x65x72ʺ_1, context);

return default;
            }
        }
    }
    
}
