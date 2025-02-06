namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ
    {
        private _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x6Cx65x76x65x6Cx73ʺ : _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ
        {
            private _ʺx24x6Cx65x76x65x6Cx73ʺ()
            {
                this._ʺx24x6Cx65x76x65x6Cx73ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺ _ʺx24x6Cx65x76x65x6Cx73ʺ_1 { get; }
            public static _ʺx24x6Cx65x76x65x6Cx73ʺ Instance { get; } = new _ʺx24x6Cx65x76x65x6Cx73ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Cx65x76x65x6Cx73ʺ : _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ
        {
            private _ʺx6Cx65x76x65x6Cx73ʺ()
            {
                this._ʺx6Cx65x76x65x6Cx73ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ _ʺx6Cx65x76x65x6Cx73ʺ_1 { get; }
            public static _ʺx6Cx65x76x65x6Cx73ʺ Instance { get; } = new _ʺx6Cx65x76x65x6Cx73ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
