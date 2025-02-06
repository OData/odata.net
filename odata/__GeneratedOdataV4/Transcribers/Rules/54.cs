namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _metadataOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._metadataOption>
    {
        private _metadataOptionTranscriber()
        {
        }
        
        public static _metadataOptionTranscriber Instance { get; } = new _metadataOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._metadataOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._metadataOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._metadataOption._format node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._formatTranscriber.Instance.Transcribe(node._format_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._metadataOption._customQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._customQueryOptionTranscriber.Instance.Transcribe(node._customQueryOption_1, context);

return default;
            }
        }
    }
    
}
