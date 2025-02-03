namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _expandCountOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._expandCountOption>
    {
        private _expandCountOptionTranscriber()
        {
        }
        
        public static _expandCountOptionTranscriber Instance { get; } = new _expandCountOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._expandCountOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._expandCountOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._expandCountOption._filter node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._filterTranscriber.Instance.Transcribe(node._filter_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._expandCountOption._search node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._searchTranscriber.Instance.Transcribe(node._search_1, context);

return default;
            }
        }
    }
    
}
