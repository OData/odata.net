namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _collectionNavPath
    {
        private _collectionNavPath()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_collectionNavPath node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_collectionNavPath._keyPredicate_꘡singleNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_collectionNavPath._filterInPath_꘡collectionNavigation꘡ node, TContext context);
            protected internal abstract TResult Accept(_collectionNavPath._each_꘡boundOperation꘡ node, TContext context);
            protected internal abstract TResult Accept(_collectionNavPath._boundOperation node, TContext context);
            protected internal abstract TResult Accept(_collectionNavPath._count node, TContext context);
            protected internal abstract TResult Accept(_collectionNavPath._ref node, TContext context);
        }
        
        public sealed class _keyPredicate_꘡singleNavigation꘡ : _collectionNavPath
        {
            public _keyPredicate_꘡singleNavigation꘡(__GeneratedOdataV4.CstNodes.Rules._keyPredicate _keyPredicate_1, __GeneratedOdataV4.CstNodes.Rules._singleNavigation? _singleNavigation_1)
            {
                this._keyPredicate_1 = _keyPredicate_1;
                this._singleNavigation_1 = _singleNavigation_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._keyPredicate _keyPredicate_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._singleNavigation? _singleNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _filterInPath_꘡collectionNavigation꘡ : _collectionNavPath
        {
            public _filterInPath_꘡collectionNavigation꘡(__GeneratedOdataV4.CstNodes.Rules._filterInPath _filterInPath_1, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation? _collectionNavigation_1)
            {
                this._filterInPath_1 = _filterInPath_1;
                this._collectionNavigation_1 = _collectionNavigation_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._filterInPath _filterInPath_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._collectionNavigation? _collectionNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _each_꘡boundOperation꘡ : _collectionNavPath
        {
            public _each_꘡boundOperation꘡(__GeneratedOdataV4.CstNodes.Rules._each _each_1, __GeneratedOdataV4.CstNodes.Rules._boundOperation? _boundOperation_1)
            {
                this._each_1 = _each_1;
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._each _each_1 { get; }
            public __GeneratedOdataV4.CstNodes.Rules._boundOperation? _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _collectionNavPath
        {
            public _boundOperation(__GeneratedOdataV4.CstNodes.Rules._boundOperation _boundOperation_1)
            {
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._boundOperation _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _count : _collectionNavPath
        {
            private _count()
            {
                this._count_1 = __GeneratedOdataV4.CstNodes.Rules._count.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._count _count_1 { get; }
            public static _count Instance { get; } = new _count();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ref : _collectionNavPath
        {
            private _ref()
            {
                this._ref_1 = __GeneratedOdataV4.CstNodes.Rules._ref.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._ref _ref_1 { get; }
            public static _ref Instance { get; } = new _ref();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
