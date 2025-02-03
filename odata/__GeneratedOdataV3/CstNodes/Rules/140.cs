namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _boolMethodCallExpr
    {
        private _boolMethodCallExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_boolMethodCallExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_boolMethodCallExpr._endsWithMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_boolMethodCallExpr._startsWithMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_boolMethodCallExpr._containsMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_boolMethodCallExpr._intersectsMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_boolMethodCallExpr._hasSubsetMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_boolMethodCallExpr._hasSubsequenceMethodCallExpr node, TContext context);
        }
        
        public sealed class _endsWithMethodCallExpr : _boolMethodCallExpr
        {
            public _endsWithMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._endsWithMethodCallExpr _endsWithMethodCallExpr_1)
            {
                this._endsWithMethodCallExpr_1 = _endsWithMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._endsWithMethodCallExpr _endsWithMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _startsWithMethodCallExpr : _boolMethodCallExpr
        {
            public _startsWithMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr _startsWithMethodCallExpr_1)
            {
                this._startsWithMethodCallExpr_1 = _startsWithMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr _startsWithMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _containsMethodCallExpr : _boolMethodCallExpr
        {
            public _containsMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._containsMethodCallExpr _containsMethodCallExpr_1)
            {
                this._containsMethodCallExpr_1 = _containsMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._containsMethodCallExpr _containsMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _intersectsMethodCallExpr : _boolMethodCallExpr
        {
            public _intersectsMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._intersectsMethodCallExpr _intersectsMethodCallExpr_1)
            {
                this._intersectsMethodCallExpr_1 = _intersectsMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._intersectsMethodCallExpr _intersectsMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _hasSubsetMethodCallExpr : _boolMethodCallExpr
        {
            public _hasSubsetMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._hasSubsetMethodCallExpr _hasSubsetMethodCallExpr_1)
            {
                this._hasSubsetMethodCallExpr_1 = _hasSubsetMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._hasSubsetMethodCallExpr _hasSubsetMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _hasSubsequenceMethodCallExpr : _boolMethodCallExpr
        {
            public _hasSubsequenceMethodCallExpr(__GeneratedOdataV3.CstNodes.Rules._hasSubsequenceMethodCallExpr _hasSubsequenceMethodCallExpr_1)
            {
                this._hasSubsequenceMethodCallExpr_1 = _hasSubsequenceMethodCallExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._hasSubsequenceMethodCallExpr _hasSubsequenceMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
