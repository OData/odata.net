namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ>
    {
        private _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺTranscriber()
        {
        }
        
        public static _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺTranscriber Instance { get; } = new _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24x6Cx65x76x65x6Cx73ʺTranscriber.Instance.Transcribe(node._ʺx24x6Cx65x76x65x6Cx73ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx6Cx65x76x65x6Cx73ʺTranscriber.Instance.Transcribe(node._ʺx6Cx65x76x65x6Cx73ʺ_1, context);

return default;
            }
        }
    }
    
}
