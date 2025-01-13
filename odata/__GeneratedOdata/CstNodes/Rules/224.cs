namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _qualifiedTypeName
    {
        private _qualifiedTypeName()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qualifiedTypeName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qualifiedTypeName._singleQualifiedTypeName node, TContext context);
            protected internal abstract TResult Accept(_qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE node, TContext context);
        }
        
        public sealed class _singleQualifiedTypeName : _qualifiedTypeName
        {
            public _singleQualifiedTypeName(__GeneratedOdata.CstNodes.Rules._singleQualifiedTypeName _singleQualifiedTypeName_1)
            {
                this._singleQualifiedTypeName_1 = _singleQualifiedTypeName_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._singleQualifiedTypeName _singleQualifiedTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE : _qualifiedTypeName
        {
            public _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE(__GeneratedOdata.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1, __GeneratedOdata.CstNodes.Rules._OPEN _OPEN_1, __GeneratedOdata.CstNodes.Rules._singleQualifiedTypeName _singleQualifiedTypeName_1, __GeneratedOdata.CstNodes.Rules._CLOSE _CLOSE_1)
            {
                this._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1 = _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1;
                this._OPEN_1 = _OPEN_1;
                this._singleQualifiedTypeName_1 = _singleQualifiedTypeName_1;
                this._CLOSE_1 = _CLOSE_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._OPEN _OPEN_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._singleQualifiedTypeName _singleQualifiedTypeName_1 { get; }
            public __GeneratedOdata.CstNodes.Rules._CLOSE _CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
