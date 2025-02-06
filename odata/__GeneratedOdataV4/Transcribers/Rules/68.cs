namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _expandRefOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._expandRefOption>
    {
        private _expandRefOptionTranscriber()
        {
        }
        
        public static _expandRefOptionTranscriber Instance { get; } = new _expandRefOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._expandRefOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._expandRefOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._expandRefOption._expandCountOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(node._expandCountOption_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._expandRefOption._orderby node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._orderbyTranscriber.Instance.Transcribe(node._orderby_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._expandRefOption._skip node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._skipTranscriber.Instance.Transcribe(node._skip_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._expandRefOption._top node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._topTranscriber.Instance.Transcribe(node._top_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._expandRefOption._inlinecount node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._inlinecountTranscriber.Instance.Transcribe(node._inlinecount_1, context);

return default;
            }
        }
    }
    
}
