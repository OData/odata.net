namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _nanInfinityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._nanInfinity>
    {
        private _nanInfinityTranscriber()
        {
        }
        
        public static _nanInfinityTranscriber Instance { get; } = new _nanInfinityTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._nanInfinity value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._nanInfinity.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx4Ex61x4EʺTranscriber.Instance.Transcribe(node._ʺx4Ex61x4Eʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx2Dx49x4Ex46ʺTranscriber.Instance.Transcribe(node._ʺx2Dx49x4Ex46ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx49x4Ex46ʺTranscriber.Instance.Transcribe(node._ʺx49x4Ex46ʺ_1, context);

return default;
            }
        }
    }
    
}
