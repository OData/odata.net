namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ>
    {
        private _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺTranscriber()
        {
        }
        
        public static _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺTranscriber Instance { get; } = new _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺTranscriber.Instance.Transcribe(node._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ._ʺx6Dx69x6Ex69x6Dx61x6Cʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx6Dx69x6Ex69x6Dx61x6CʺTranscriber.Instance.Transcribe(node._ʺx6Dx69x6Ex69x6Dx61x6Cʺ_1, context);

return default;
            }
        }
    }
    
}
