namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _HEXDIG<TMode> : IAstNode<char, _HEXDIG<ParseMode.Realized>>, IFromRealizedable<_HEXDIG<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _HEXDIG()
        {
        }
        
        internal static _HEXDIG<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _HEXDIG<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _HEXDIG<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _HEXDIG<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _HEXDIG<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _HEXDIG<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _HEXDIG<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _HEXDIG<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _HEXDIG<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _HEXDIG<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_HEXDIG<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_HEXDIG<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_HEXDIG<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _HEXDIG<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._DIGIT node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx41ʺ node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx42ʺ node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx43ʺ node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx44ʺ node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx45ʺ node, TContext context);
                protected internal abstract TResult Accept(_HEXDIG<TMode>.Realized._ʺx46ʺ node, TContext context);
            }
            
            public sealed class _DIGIT : _HEXDIG<TMode>.Realized
            {
                private _DIGIT(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._DIGIT<TMode>> _DIGIT_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__DIGIT_1 = _DIGIT_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._DIGIT>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Rules._DIGIT<TMode>> __DIGIT_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._DIGIT> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._DIGIT<TMode> _DIGIT_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx41ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx41ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ<TMode>> _ʺx41ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx41ʺ_1 = _ʺx41ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx41ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ<TMode>> __ʺx41ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx41ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx41ʺ<TMode> _ʺx41ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx42ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx42ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx42ʺ<TMode>> _ʺx42ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx42ʺ_1 = _ʺx42ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx42ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx42ʺ<TMode>> __ʺx42ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx42ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx42ʺ<TMode> _ʺx42ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx43ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx43ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ<TMode>> _ʺx43ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx43ʺ_1 = _ʺx43ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx43ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ<TMode>> __ʺx43ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx43ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx43ʺ<TMode> _ʺx43ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx44ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx44ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx44ʺ<TMode>> _ʺx44ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx44ʺ_1 = _ʺx44ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx44ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx44ʺ<TMode>> __ʺx44ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx44ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx44ʺ<TMode> _ʺx44ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx45ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx45ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx45ʺ<TMode>> _ʺx45ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx45ʺ_1 = _ʺx45ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx45ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx45ʺ<TMode>> __ʺx45ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx45ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx45ʺ<TMode> _ʺx45ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ʺx46ʺ : _HEXDIG<TMode>.Realized
            {
                private _ʺx46ʺ(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx46ʺ<TMode>> _ʺx46ʺ_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this.__ʺx46ʺ_1 = _ʺx46ʺ_1;
                    this.realizationResult = new RealizationResult<char, _HEXDIG<TMode>.Realized._ʺx46ʺ>(true, this, nextTokens);
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx46ʺ<TMode>> __ʺx46ʺ_1 { get; }
                private IRealizationResult<char, _HEXDIG<TMode>.Realized._ʺx46ʺ> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._ʺx46ʺ<TMode> _ʺx46ʺ_1 { get; }
                
                public override _HEXDIG<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
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
