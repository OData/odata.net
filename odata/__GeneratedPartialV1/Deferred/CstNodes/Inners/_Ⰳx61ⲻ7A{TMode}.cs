namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx61ⲻ7A<TMode> : IAstNode<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx61ⲻ7A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx61ⲻ7A()
        {
        }
        
        internal static _Ⰳx61ⲻ7A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx61ⲻ7A<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
