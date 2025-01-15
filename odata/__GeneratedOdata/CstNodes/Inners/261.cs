namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
    {
        private _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr node, TContext context);
            protected internal abstract TResult Accept(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr node, TContext context);
        }
        
        public sealed class _eqExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _eqExpr(__GeneratedOdata.CstNodes.Rules._eqExpr _eqExpr_1)
            {
                this._eqExpr_1 = _eqExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._eqExpr _eqExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _neExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _neExpr(__GeneratedOdata.CstNodes.Rules._neExpr _neExpr_1)
            {
                this._neExpr_1 = _neExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._neExpr _neExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ltExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _ltExpr(__GeneratedOdata.CstNodes.Rules._ltExpr _ltExpr_1)
            {
                this._ltExpr_1 = _ltExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._ltExpr _ltExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _leExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _leExpr(__GeneratedOdata.CstNodes.Rules._leExpr _leExpr_1)
            {
                this._leExpr_1 = _leExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._leExpr _leExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _gtExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _gtExpr(__GeneratedOdata.CstNodes.Rules._gtExpr _gtExpr_1)
            {
                this._gtExpr_1 = _gtExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._gtExpr _gtExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _geExpr(__GeneratedOdata.CstNodes.Rules._geExpr _geExpr_1)
            {
                this._geExpr_1 = _geExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._geExpr _geExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _hasExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _hasExpr(__GeneratedOdata.CstNodes.Rules._hasExpr _hasExpr_1)
            {
                this._hasExpr_1 = _hasExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._hasExpr _hasExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _inExpr : _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr
        {
            public _inExpr(__GeneratedOdata.CstNodes.Rules._inExpr _inExpr_1)
            {
                this._inExpr_1 = _inExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._inExpr _inExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
