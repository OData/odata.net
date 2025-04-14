namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx21ⲻ7E<TMode> : IAstNode<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>, IFromRealizedable<_Ⰳx21ⲻ7E<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx21ⲻ7E()
        {
        }
        
        internal static _Ⰳx21ⲻ7E<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx21ⲻ7E<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
