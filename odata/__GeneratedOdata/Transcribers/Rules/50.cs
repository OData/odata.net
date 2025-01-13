namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _queryOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._queryOption>
    {
        private _queryOptionTranscriber()
        {
        }
        
        public static _queryOptionTranscriber Instance { get; } = new _queryOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._queryOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._queryOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._queryOption._systemQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._systemQueryOptionTranscriber.Instance.Transcribe(node._systemQueryOption_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._queryOption._aliasAndValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._aliasAndValueTranscriber.Instance.Transcribe(node._aliasAndValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._queryOption._nameAndValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._nameAndValueTranscriber.Instance.Transcribe(node._nameAndValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._queryOption._customQueryOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._customQueryOptionTranscriber.Instance.Transcribe(node._customQueryOption_1, context);

return default;
            }
        }
    }
    
}
