namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
    {
        private _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Fx24x65x6Ex74x69x74x79ʺ : _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
        {
            private _ʺx2Fx24x65x6Ex74x69x74x79ʺ()
            {
                this._ʺx2Fx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺ _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1 { get; }
            public static _ʺx2Fx24x65x6Ex74x69x74x79ʺ Instance { get; } = new _ʺx2Fx24x65x6Ex74x69x74x79ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fx24x64x65x6Cx74x61ʺ : _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ
        {
            private _ʺx2Fx24x64x65x6Cx74x61ʺ()
            {
                this._ʺx2Fx24x64x65x6Cx74x61ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x64x65x6Cx74x61ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x64x65x6Cx74x61ʺ _ʺx2Fx24x64x65x6Cx74x61ʺ_1 { get; }
            public static _ʺx2Fx24x64x65x6Cx74x61ʺ Instance { get; } = new _ʺx2Fx24x64x65x6Cx74x61ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
