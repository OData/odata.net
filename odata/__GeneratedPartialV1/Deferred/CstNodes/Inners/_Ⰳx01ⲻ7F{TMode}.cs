namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx01ⲻ7F<TMode> : IAstNode<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx01ⲻ7F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx01ⲻ7F()
        {
        }
        
        internal static _Ⰳx01ⲻ7F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx01ⲻ7F<ParseMode.Deferred>
        {
            internal static _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
