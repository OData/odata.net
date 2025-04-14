namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSP<TMode> : IAstNode<char, _WSP<ParseMode.Realized>>, IFromRealizedable<_WSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _WSP()
        {
        }
        
        internal static _WSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _WSP<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _WSP<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _WSP<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _WSP<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _WSP<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _WSP<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _WSP<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _WSP<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _WSP<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _WSP<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _WSP<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_WSP<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_WSP<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_WSP<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _WSP<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_WSP<TMode>.Realized._SP node, TContext context);
                protected internal abstract TResult Accept(_WSP<TMode>.Realized._HTAB node, TContext context);
            }
            
            public sealed class _SP : _WSP<TMode>.Realized
            {
                private _SP(__GeneratedPartialV1.Deferred.CstNodes.Rules._SP<TMode> _SP_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._SP_1 = _SP_1;
                    this.realizationResult = new RealizationResult<char, _WSP<TMode>.Realized._SP>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _WSP<TMode>.Realized._SP> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._SP<TMode> _SP_1 { get; }
                
                public override _WSP<ParseMode.Deferred> Convert()
                {
                    return new _WSP<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _WSP<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _HTAB : _WSP<TMode>.Realized
            {
                private _HTAB(__GeneratedPartialV1.Deferred.CstNodes.Rules._HTAB<TMode> _HTAB_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._HTAB_1 = _HTAB_1;
                    this.realizationResult = new RealizationResult<char, _WSP<TMode>.Realized._HTAB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _WSP<TMode>.Realized._HTAB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._HTAB<TMode> _HTAB_1 { get; }
                
                public override _WSP<ParseMode.Deferred> Convert()
                {
                    return new _WSP<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _WSP<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
        }
    }
    
}
