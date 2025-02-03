namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _singleEnumValue
    {
        private _singleEnumValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_singleEnumValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_singleEnumValue._enumerationMember node, TContext context);
            protected internal abstract TResult Accept(_singleEnumValue._enumMemberValue node, TContext context);
        }
        
        public sealed class _enumerationMember : _singleEnumValue
        {
            public _enumerationMember(__GeneratedOdataV2.CstNodes.Rules._enumerationMember _enumerationMember_1)
            {
                this._enumerationMember_1 = _enumerationMember_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._enumerationMember _enumerationMember_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _enumMemberValue : _singleEnumValue
        {
            public _enumMemberValue(__GeneratedOdataV2.CstNodes.Rules._enumMemberValue _enumMemberValue_1)
            {
                this._enumMemberValue_1 = _enumMemberValue_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._enumMemberValue _enumMemberValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
