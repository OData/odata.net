namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _collectionPropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri>
    {
        private _collectionPropertyInUriTranscriber()
        {
        }
        
        public static _collectionPropertyInUriTranscriber Instance { get; } = new _collectionPropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._collectionPropertyInUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃTranscriber.Instance.Transcribe(node._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃTranscriber.Instance.Transcribe(node._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1, context);

return default;
            }
        }
    }
    
}