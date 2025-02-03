namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _refⳆOPEN_levels_CLOSE
    {
        private _refⳆOPEN_levels_CLOSE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_refⳆOPEN_levels_CLOSE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_refⳆOPEN_levels_CLOSE._ref node, TContext context);
            protected internal abstract TResult Accept(_refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE node, TContext context);
        }
        
        public sealed class _ref : _refⳆOPEN_levels_CLOSE
        {
            public _ref(__GeneratedOdataV3.CstNodes.Rules._ref _ref_1)
            {
                this._ref_1 = _ref_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._ref _ref_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _OPEN_levels_CLOSE : _refⳆOPEN_levels_CLOSE
        {
            public _OPEN_levels_CLOSE(__GeneratedOdataV3.CstNodes.Rules._OPEN _OPEN_1, __GeneratedOdataV3.CstNodes.Rules._levels _levels_1, __GeneratedOdataV3.CstNodes.Rules._CLOSE _CLOSE_1)
            {
                this._OPEN_1 = _OPEN_1;
                this._levels_1 = _levels_1;
                this._CLOSE_1 = _CLOSE_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._OPEN _OPEN_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._levels _levels_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._CLOSE _CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
