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
            
            public sealed class _WSP : _WSPⳆCRLF_WSP<TMode>.Realized
            {
                public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CRLF_WSP : _WSPⳆCRLF_WSP<TMode>.Realized
            {
                public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_WSPⳆCRLF_WSP<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _WSPⳆCRLF_WSP<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_WSPⳆCRLF_WSP<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}
