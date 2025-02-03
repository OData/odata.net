namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _searchPhraseⳆsearchWord
    {
        private _searchPhraseⳆsearchWord()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_searchPhraseⳆsearchWord node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_searchPhraseⳆsearchWord._searchPhrase node, TContext context);
            protected internal abstract TResult Accept(_searchPhraseⳆsearchWord._searchWord node, TContext context);
        }
        
        public sealed class _searchPhrase : _searchPhraseⳆsearchWord
        {
            public _searchPhrase(__GeneratedOdataV3.CstNodes.Rules._searchPhrase _searchPhrase_1)
            {
                this._searchPhrase_1 = _searchPhrase_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._searchPhrase _searchPhrase_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _searchWord : _searchPhraseⳆsearchWord
        {
            public _searchWord(__GeneratedOdataV3.CstNodes.Rules._searchWord _searchWord_1)
            {
                this._searchWord_1 = _searchWord_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._searchWord _searchWord_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
