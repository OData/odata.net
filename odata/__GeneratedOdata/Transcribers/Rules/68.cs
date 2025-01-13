namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _expandRefOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._expandRefOption>
    {
        private _expandRefOptionTranscriber()
        {
        }
        
        public static _expandRefOptionTranscriber Instance { get; } = new _expandRefOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._expandRefOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._expandRefOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandRefOption._expandCountOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(node._expandCountOption_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandRefOption._orderby node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._orderbyTranscriber.Instance.Transcribe(node._orderby_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandRefOption._skip node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._skipTranscriber.Instance.Transcribe(node._skip_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandRefOption._top node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._topTranscriber.Instance.Transcribe(node._top_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandRefOption._inlinecount node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._inlinecountTranscriber.Instance.Transcribe(node._inlinecount_1, context);

return default;
            }
        }
    }
    
}
