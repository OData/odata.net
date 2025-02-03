namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _hierⲻpart
    {
        private _hierⲻpart()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_hierⲻpart node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty node, TContext context);
            protected internal abstract TResult Accept(_hierⲻpart._pathⲻabsolute node, TContext context);
            protected internal abstract TResult Accept(_hierⲻpart._pathⲻrootless node, TContext context);
        }
        
        public sealed class _ʺx2Fx2Fʺ_authority_pathⲻabempty : _hierⲻpart
        {
            public _ʺx2Fx2Fʺ_authority_pathⲻabempty(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx2Fʺ _ʺx2Fx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._authority _authority_1, __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty _pathⲻabempty_1)
            {
                this._ʺx2Fx2Fʺ_1 = _ʺx2Fx2Fʺ_1;
                this._authority_1 = _authority_1;
                this._pathⲻabempty_1 = _pathⲻabempty_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx2Fʺ _ʺx2Fx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._authority _authority_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty _pathⲻabempty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pathⲻabsolute : _hierⲻpart
        {
            public _pathⲻabsolute(__GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute _pathⲻabsolute_1)
            {
                this._pathⲻabsolute_1 = _pathⲻabsolute_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute _pathⲻabsolute_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pathⲻrootless : _hierⲻpart
        {
            public _pathⲻrootless(__GeneratedOdataV3.CstNodes.Rules._pathⲻrootless _pathⲻrootless_1)
            {
                this._pathⲻrootless_1 = _pathⲻrootless_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._pathⲻrootless _pathⲻrootless_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
