namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _systemQueryOption
    {
        private _systemQueryOption()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_systemQueryOption node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_systemQueryOption._compute node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._deltatoken node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._expand node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._filter node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._format node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._id node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._inlinecount node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._orderby node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._schemaversion node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._search node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._select node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._skip node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._skiptoken node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._top node, TContext context);
            protected internal abstract TResult Accept(_systemQueryOption._index node, TContext context);
        }
        
        public sealed class _compute : _systemQueryOption
        {
            public _compute(__GeneratedOdataV4.CstNodes.Rules._compute _compute_1)
            {
                this._compute_1 = _compute_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._compute _compute_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _deltatoken : _systemQueryOption
        {
            public _deltatoken(__GeneratedOdataV4.CstNodes.Rules._deltatoken _deltatoken_1)
            {
                this._deltatoken_1 = _deltatoken_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._deltatoken _deltatoken_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _expand : _systemQueryOption
        {
            public _expand(__GeneratedOdataV4.CstNodes.Rules._expand _expand_1)
            {
                this._expand_1 = _expand_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._expand _expand_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _filter : _systemQueryOption
        {
            public _filter(__GeneratedOdataV4.CstNodes.Rules._filter _filter_1)
            {
                this._filter_1 = _filter_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._filter _filter_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _format : _systemQueryOption
        {
            public _format(__GeneratedOdataV4.CstNodes.Rules._format _format_1)
            {
                this._format_1 = _format_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._format _format_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _id : _systemQueryOption
        {
            public _id(__GeneratedOdataV4.CstNodes.Rules._id _id_1)
            {
                this._id_1 = _id_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._id _id_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _inlinecount : _systemQueryOption
        {
            public _inlinecount(__GeneratedOdataV4.CstNodes.Rules._inlinecount _inlinecount_1)
            {
                this._inlinecount_1 = _inlinecount_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._inlinecount _inlinecount_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _orderby : _systemQueryOption
        {
            public _orderby(__GeneratedOdataV4.CstNodes.Rules._orderby _orderby_1)
            {
                this._orderby_1 = _orderby_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._orderby _orderby_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _schemaversion : _systemQueryOption
        {
            public _schemaversion(__GeneratedOdataV4.CstNodes.Rules._schemaversion _schemaversion_1)
            {
                this._schemaversion_1 = _schemaversion_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._schemaversion _schemaversion_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _search : _systemQueryOption
        {
            public _search(__GeneratedOdataV4.CstNodes.Rules._search _search_1)
            {
                this._search_1 = _search_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._search _search_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _select : _systemQueryOption
        {
            public _select(__GeneratedOdataV4.CstNodes.Rules._select _select_1)
            {
                this._select_1 = _select_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._select _select_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _skip : _systemQueryOption
        {
            public _skip(__GeneratedOdataV4.CstNodes.Rules._skip _skip_1)
            {
                this._skip_1 = _skip_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._skip _skip_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _skiptoken : _systemQueryOption
        {
            public _skiptoken(__GeneratedOdataV4.CstNodes.Rules._skiptoken _skiptoken_1)
            {
                this._skiptoken_1 = _skiptoken_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._skiptoken _skiptoken_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _top : _systemQueryOption
        {
            public _top(__GeneratedOdataV4.CstNodes.Rules._top _top_1)
            {
                this._top_1 = _top_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._top _top_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _index : _systemQueryOption
        {
            public _index(__GeneratedOdataV4.CstNodes.Rules._index _index_1)
            {
                this._index_1 = _index_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._index _index_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
