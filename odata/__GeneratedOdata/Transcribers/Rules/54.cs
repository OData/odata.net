namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _metadataOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._metadataOption>
    {
        private _metadataOptionTranscriber()
        {
        }
        
        public static _metadataOptionTranscriber Instance { get; } = new _metadataOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._metadataOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._metadataOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._metadataOption._format node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._formatTranscriber.Instance.Transcribe(node._format_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._metadataOption._customQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._customQueryOptionTranscriber.Instance.Transcribe(node._customQueryOption_1, context);

return default;
            }
        }
    }
    
}
