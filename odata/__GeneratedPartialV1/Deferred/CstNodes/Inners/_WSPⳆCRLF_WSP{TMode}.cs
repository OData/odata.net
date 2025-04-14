namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPⳆCRLF_WSP<TMode> : IAstNode<char, _WSPⳆCRLF_WSP<ParseMode.Realized>>, IFromRealizedable<_WSPⳆCRLF_WSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _WSPⳆCRLF_WSP()
        {
        }
        
        internal static _WSPⳆCRLF_WSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _WSPⳆCRLF_WSP<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _WSPⳆCRLF_WSP<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_WSPⳆCRLF_WSP<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_WSPⳆCRLF_WSP<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_WSPⳆCRLF_WSP<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _WSPⳆCRLF_WSP<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_WSPⳆCRLF_WSP<TMode>.Realized._WSP node, TContext context);
                protected internal abstract TResult Accept(_WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP node, TContext context);
            }
            
            public sealed class _WSP : _WSPⳆCRLF_WSP<TMode>.Realized
            {
                private _WSP(__GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._WSP_1 = _WSP_1;
                    this.realizationResult = new RealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._WSP>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._WSP> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1 { get; }
                
                internal static IRealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._WSP> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _WSP_1 = __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP.Create(previousNodeRealizationResult).Realize();
if (!_WSP_1.Success)
{
return new RealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._WSP>(false, default, _WSP_1.RemainingTokens);
}


var node = new _WSP(_WSP_1.RealizedValue, _WSP_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
                {
                    return new _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CRLF_WSP : _WSPⳆCRLF_WSP<TMode>.Realized
            {
                private _CRLF_WSP(__GeneratedPartialV1.Deferred.CstNodes.Rules._CRLF<ParseMode.Realized> _CRLF_1, __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._CRLF_1 = _CRLF_1;
                    this._WSP_1 = _WSP_1;
                    this.realizationResult = new RealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._CRLF<ParseMode.Realized> _CRLF_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1 { get; }
                
                internal static IRealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _CRLF_1 = __GeneratedPartialV1.Deferred.CstNodes.Rules._CRLF.Create(previousNodeRealizationResult).Realize();
if (!_CRLF_1.Success)
{
return new RealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP>(false, default, _CRLF_1.RemainingTokens);
}


var _WSP_1 = __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP.Create(Future.Create(() => _CRLF_1)).Realize();
if (!_WSP_1.Success)
{
return new RealizationResult<char, _WSPⳆCRLF_WSP<TMode>.Realized._CRLF_WSP>(false, default, _WSP_1.RemainingTokens);
}


var node = new _CRLF_WSP(_CRLF_1.RealizedValue, _WSP_1.RealizedValue, _WSP_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
                {
                    return new _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}
