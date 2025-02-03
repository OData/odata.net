namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _systemQueryOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._systemQueryOption>
    {
        private _systemQueryOptionTranscriber()
        {
        }
        
        public static _systemQueryOptionTranscriber Instance { get; } = new _systemQueryOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._systemQueryOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._compute node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._computeTranscriber.Instance.Transcribe(node._compute_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._deltatoken node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._deltatokenTranscriber.Instance.Transcribe(node._deltatoken_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._expand node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._expandTranscriber.Instance.Transcribe(node._expand_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._filter node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._filterTranscriber.Instance.Transcribe(node._filter_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._format node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._formatTranscriber.Instance.Transcribe(node._format_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._id node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._idTranscriber.Instance.Transcribe(node._id_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._inlinecount node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._inlinecountTranscriber.Instance.Transcribe(node._inlinecount_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._orderby node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._orderbyTranscriber.Instance.Transcribe(node._orderby_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._schemaversion node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._schemaversionTranscriber.Instance.Transcribe(node._schemaversion_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._search node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._searchTranscriber.Instance.Transcribe(node._search_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._select node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._selectTranscriber.Instance.Transcribe(node._select_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._skip node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._skipTranscriber.Instance.Transcribe(node._skip_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._skiptoken node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._skiptokenTranscriber.Instance.Transcribe(node._skiptoken_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._top node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._topTranscriber.Instance.Transcribe(node._top_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._systemQueryOption._index node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._indexTranscriber.Instance.Transcribe(node._index_1, context);

return default;
            }
        }
    }
    
}
