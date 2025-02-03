namespace __GeneratedOdataV3.CstNodes.Rules
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
            public _keyPredicate_꘡singleNavigation꘡(__GeneratedOdataV3.CstNodes.Rules._keyPredicate _keyPredicate_1, __GeneratedOdataV3.CstNodes.Rules._singleNavigation? _singleNavigation_1)
            {
                this._keyPredicate_1 = _keyPredicate_1;
                this._singleNavigation_1 = _singleNavigation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._keyPredicate _keyPredicate_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._singleNavigation? _singleNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _filterInPath_꘡collectionNavigation꘡ : _collectionNavPath
        {
            public _filterInPath_꘡collectionNavigation꘡(__GeneratedOdataV3.CstNodes.Rules._filterInPath _filterInPath_1, __GeneratedOdataV3.CstNodes.Rules._collectionNavigation? _collectionNavigation_1)
            {
                this._filterInPath_1 = _filterInPath_1;
                this._collectionNavigation_1 = _collectionNavigation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._filterInPath _filterInPath_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._collectionNavigation? _collectionNavigation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _each_꘡boundOperation꘡ : _collectionNavPath
        {
            public _each_꘡boundOperation꘡(__GeneratedOdataV3.CstNodes.Rules._each _each_1, __GeneratedOdataV3.CstNodes.Rules._boundOperation? _boundOperation_1)
            {
                this._each_1 = _each_1;
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._each _each_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._boundOperation? _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boundOperation : _collectionNavPath
        {
            public _boundOperation(__GeneratedOdataV3.CstNodes.Rules._boundOperation _boundOperation_1)
            {
                this._boundOperation_1 = _boundOperation_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._boundOperation _boundOperation_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _count : _collectionNavPath
        {
            public _count(__GeneratedOdataV3.CstNodes.Rules._count _count_1)
            {
                this._count_1 = _count_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._count _count_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ref : _collectionNavPath
        {
            public _ref(__GeneratedOdataV3.CstNodes.Rules._ref _ref_1)
            {
                this._ref_1 = _ref_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._ref _ref_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
