namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _expandOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._expandOption>
    {
        private _expandOptionTranscriber()
        {
        }
        
        public static _expandOptionTranscriber Instance { get; } = new _expandOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._expandOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._expandOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._expandRefOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._expandRefOptionTranscriber.Instance.Transcribe(node._expandRefOption_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._select node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._selectTranscriber.Instance.Transcribe(node._select_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._expand node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._expandTranscriber.Instance.Transcribe(node._expand_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._compute node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._computeTranscriber.Instance.Transcribe(node._compute_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._levels node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._levelsTranscriber.Instance.Transcribe(node._levels_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandOption._aliasAndValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._aliasAndValueTranscriber.Instance.Transcribe(node._aliasAndValue_1, context);

return default;
            }
        }
    }
    
}
