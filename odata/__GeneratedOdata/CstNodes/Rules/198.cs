namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _collectionPropertyInUri
    {
        private _collectionPropertyInUri()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_collectionPropertyInUri node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ node, TContext context);
            protected internal abstract TResult Accept(_collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ node, TContext context);
        }
        
        public sealed class _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ : _collectionPropertyInUri
        {
            public _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ(__GeneratedOdata.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1)
            {
                this._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1 = _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ : _collectionPropertyInUri
        {
            public _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ(__GeneratedOdata.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1)
            {
                this._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1 = _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
