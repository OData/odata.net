namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _OPENTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._OPEN>
    {
        private _OPENTranscriber()
        {
        }
        
        public static _OPENTranscriber Instance { get; } = new _OPENTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._OPEN value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._OPEN.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._OPEN._ʺx28ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx28ʺTranscriber.Instance.Transcribe(node._ʺx28ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._OPEN._ʺx25x32x38ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx25x32x38ʺTranscriber.Instance.Transcribe(node._ʺx25x32x38ʺ_1, context);

return default;
            }
        }
    }
    
}