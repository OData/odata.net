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
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _WSP<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _WSP<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
