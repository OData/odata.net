namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _batchOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._batchOption>
    {
        private _batchOptionTranscriber()
        {
        }
        
        public static _batchOptionTranscriber Instance { get; } = new _batchOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._batchOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._batchOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._batchOption._format node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._formatTranscriber.Instance.Transcribe(node._format_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._batchOption._customQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._customQueryOptionTranscriber.Instance.Transcribe(node._customQueryOption_1, context);

return default;
            }
        }
    }
    
}
