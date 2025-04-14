namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _ALPHA<TMode> : IAstNode<char, _ALPHA<ParseMode.Realized>>, IFromRealizedable<_ALPHA<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ALPHA()
        {
        }
        
        internal static _ALPHA<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _ALPHA<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _ALPHA<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _ALPHA<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _ALPHA<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _ALPHA<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _ALPHA<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _ALPHA<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _ALPHA<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _ALPHA<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_ALPHA<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_ALPHA<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_ALPHA<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _ALPHA<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_ALPHA<TMode>.Realized._Ⰳx41ⲻ5A node, TContext context);
                protected internal abstract TResult Accept(_ALPHA<TMode>.Realized._Ⰳx61ⲻ7A node, TContext context);
            }
            
            public sealed class _Ⰳx41ⲻ5A : _ALPHA<TMode>.Realized
            {
                private _Ⰳx41ⲻ5A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx41ⲻ5A<TMode>> _Ⰳx41ⲻ5A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.realizationResult = new RealizationResult<char, _ALPHA<TMode>.Realized._Ⰳx41ⲻ5A>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx41ⲻ5A<TMode>> __Ⰳx41ⲻ5A_1 { get; }
                private IRealizationResult<char, _ALPHA<TMode>.Realized._Ⰳx41ⲻ5A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx41ⲻ5A<TMode> _Ⰳx41ⲻ5A_1 { get; }
                
                public override _ALPHA<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _Ⰳx61ⲻ7A : _ALPHA<TMode>.Realized
            {
                private _Ⰳx61ⲻ7A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx61ⲻ7A<TMode>> _Ⰳx61ⲻ7A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.realizationResult = new RealizationResult<char, _ALPHA<TMode>.Realized._Ⰳx61ⲻ7A>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx61ⲻ7A<TMode>> __Ⰳx61ⲻ7A_1 { get; }
                private IRealizationResult<char, _ALPHA<TMode>.Realized._Ⰳx61ⲻ7A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._Ⰳx61ⲻ7A<TMode> _Ⰳx61ⲻ7A_1 { get; }
                
                public override _ALPHA<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
        }
    }
    
}
