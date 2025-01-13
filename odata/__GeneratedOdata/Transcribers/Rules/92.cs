namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectOption>
    {
        private _selectOptionTranscriber()
        {
        }
        
        public static _selectOptionTranscriber Instance { get; } = new _selectOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._selectOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOption._selectOptionPC node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._selectOptionPCTranscriber.Instance.Transcribe(node._selectOptionPC_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOption._compute node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._computeTranscriber.Instance.Transcribe(node._compute_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOption._select node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._selectTranscriber.Instance.Transcribe(node._select_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOption._expand node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._expandTranscriber.Instance.Transcribe(node._expand_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOption._aliasAndValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._aliasAndValueTranscriber.Instance.Transcribe(node._aliasAndValue_1, context);

return default;
            }
        }
    }
    
}
