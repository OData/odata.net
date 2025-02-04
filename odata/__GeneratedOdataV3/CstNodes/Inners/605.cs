namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ
    {
        private _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ._ʺx6Dx69x6Ex69x6Dx61x6Cʺ node, TContext context);
        }
        
        public sealed class _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ : _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ
        {
            private _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ()
            {
                this._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ_1 { get; }
            public static _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ Instance { get; } = new _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx6Dx69x6Ex69x6Dx61x6Cʺ : _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ
        {
            private _ʺx6Dx69x6Ex69x6Dx61x6Cʺ()
            {
                this._ʺx6Dx69x6Ex69x6Dx61x6Cʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ _ʺx6Dx69x6Ex69x6Dx61x6Cʺ_1 { get; }
            public static _ʺx6Dx69x6Ex69x6Dx61x6Cʺ Instance { get; } = new _ʺx6Dx69x6Ex69x6Dx61x6Cʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
