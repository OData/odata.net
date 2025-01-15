namespace __GeneratedV2.CstNodes.Inners
{
    public abstract class _Ⰳx20ⲻ21
    {
        private _Ⰳx20ⲻ21()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx20ⲻ21 node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx20ⲻ21._20 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx20ⲻ21._21 node, TContext context);
        }
        
        public sealed class _20 : _Ⰳx20ⲻ21
        {
            public _20(__GeneratedV2.CstNodes.Inners._2 _2_1, __GeneratedV2.CstNodes.Inners._0 _0_1)
            {
                this._2_1 = _2_1;
                this._0_1 = _0_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._2 _2_1 { get; }
            public __GeneratedV2.CstNodes.Inners._0 _0_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _21 : _Ⰳx20ⲻ21
        {
            public _21(__GeneratedV2.CstNodes.Inners._2 _2_1, __GeneratedV2.CstNodes.Inners._1 _1_1)
            {
                this._2_1 = _2_1;
                this._1_1 = _1_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._2 _2_1 { get; }
            public __GeneratedV2.CstNodes.Inners._1 _1_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
