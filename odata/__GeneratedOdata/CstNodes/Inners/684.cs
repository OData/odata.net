namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _Ⰳx30ⲻ34
    {
        private _Ⰳx30ⲻ34()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx30ⲻ34 node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx30ⲻ34._30 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx30ⲻ34._31 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx30ⲻ34._32 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx30ⲻ34._33 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx30ⲻ34._34 node, TContext context);
        }
        
        public sealed class _30 : _Ⰳx30ⲻ34
        {
            public _30(__GeneratedOdata.CstNodes.Inners._3 _3_1, __GeneratedOdata.CstNodes.Inners._0 _0_1)
            {
                this._3_1 = _3_1;
                this._0_1 = _0_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._3 _3_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._0 _0_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _31 : _Ⰳx30ⲻ34
        {
            public _31(__GeneratedOdata.CstNodes.Inners._3 _3_1, __GeneratedOdata.CstNodes.Inners._1 _1_1)
            {
                this._3_1 = _3_1;
                this._1_1 = _1_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._3 _3_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._1 _1_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _32 : _Ⰳx30ⲻ34
        {
            public _32(__GeneratedOdata.CstNodes.Inners._3 _3_1, __GeneratedOdata.CstNodes.Inners._2 _2_1)
            {
                this._3_1 = _3_1;
                this._2_1 = _2_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._3 _3_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._2 _2_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _33 : _Ⰳx30ⲻ34
        {
            public _33(__GeneratedOdata.CstNodes.Inners._3 _3_1, __GeneratedOdata.CstNodes.Inners._3 _3_2)
            {
                this._3_1 = _3_1;
                this._3_2 = _3_2;
            }
            
            public __GeneratedOdata.CstNodes.Inners._3 _3_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._3 _3_2 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _34 : _Ⰳx30ⲻ34
        {
            public _34(__GeneratedOdata.CstNodes.Inners._3 _3_1, __GeneratedOdata.CstNodes.Inners._4 _4_1)
            {
                this._3_1 = _3_1;
                this._4_1 = _4_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._3 _3_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._4 _4_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
