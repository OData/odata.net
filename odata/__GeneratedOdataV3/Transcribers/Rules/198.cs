namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _collectionPropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri>
    {
        private _collectionPropertyInUriTranscriber()
        {
        }
        
        public static _collectionPropertyInUriTranscriber Instance { get; } = new _collectionPropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃTranscriber.Instance.Transcribe(node._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃTranscriber.Instance.Transcribe(node._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1, context);

return default;
            }
        }
    }
    
}
