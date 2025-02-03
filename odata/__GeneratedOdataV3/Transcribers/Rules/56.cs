namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _entityIdOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._entityIdOption>
    {
        private _entityIdOptionTranscriber()
        {
        }
        
        public static _entityIdOptionTranscriber Instance { get; } = new _entityIdOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._entityIdOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._entityIdOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._entityIdOption._format node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._formatTranscriber.Instance.Transcribe(node._format_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._entityIdOption._customQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._customQueryOptionTranscriber.Instance.Transcribe(node._customQueryOption_1, context);

return default;
            }
        }
    }
    
}
