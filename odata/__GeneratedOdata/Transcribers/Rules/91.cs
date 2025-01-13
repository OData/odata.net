namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectOptionPCTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectOptionPC>
    {
        private _selectOptionPCTranscriber()
        {
        }
        
        public static _selectOptionPCTranscriber Instance { get; } = new _selectOptionPCTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectOptionPC value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._selectOptionPC.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._filter node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._filterTranscriber.Instance.Transcribe(node._filter_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._search node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._searchTranscriber.Instance.Transcribe(node._search_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._inlinecount node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._inlinecountTranscriber.Instance.Transcribe(node._inlinecount_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._orderby node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._orderbyTranscriber.Instance.Transcribe(node._orderby_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._skip node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._skipTranscriber.Instance.Transcribe(node._skip_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectOptionPC._top node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._topTranscriber.Instance.Transcribe(node._top_1, context);

return default;
            }
        }
    }
    
}
