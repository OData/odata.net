namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx01ⲻ7F<TMode> : IAstNode<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx01ⲻ7F<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _Ⰳx01ⲻ7F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _Ⰳx01ⲻ7F<ParseMode.Deferred>
        {
            internal static _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
