namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
    {
        private _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr node, TContext context);
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr node, TContext context);
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr node, TContext context);
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr node, TContext context);
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr node, TContext context);
            protected internal abstract TResult Accept(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr node, TContext context);
        }
        
        public sealed class _addExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _addExpr(__GeneratedOdataV2.CstNodes.Rules._addExpr _addExpr_1)
            {
                this._addExpr_1 = _addExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._addExpr _addExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _subExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _subExpr(__GeneratedOdataV2.CstNodes.Rules._subExpr _subExpr_1)
            {
                this._subExpr_1 = _subExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._subExpr _subExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _mulExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _mulExpr(__GeneratedOdataV2.CstNodes.Rules._mulExpr _mulExpr_1)
            {
                this._mulExpr_1 = _mulExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._mulExpr _mulExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _divExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _divExpr(__GeneratedOdataV2.CstNodes.Rules._divExpr _divExpr_1)
            {
                this._divExpr_1 = _divExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._divExpr _divExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _divbyExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _divbyExpr(__GeneratedOdataV2.CstNodes.Rules._divbyExpr _divbyExpr_1)
            {
                this._divbyExpr_1 = _divbyExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._divbyExpr _divbyExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _modExpr : _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr
        {
            public _modExpr(__GeneratedOdataV2.CstNodes.Rules._modExpr _modExpr_1)
            {
                this._modExpr_1 = _modExpr_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._modExpr _modExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
