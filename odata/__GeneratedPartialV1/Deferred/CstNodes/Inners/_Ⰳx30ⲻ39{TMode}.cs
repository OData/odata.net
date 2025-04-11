namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx30ⲻ39<TMode> : IAstNode<char, _Ⰳx30ⲻ39<ParseMode.Realized>>, IFromRealizedable<_Ⰳx30ⲻ39<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _Ⰳx30ⲻ39<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _Ⰳx30ⲻ39<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _Ⰳx30ⲻ39<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _Ⰳx30ⲻ39<ParseMode.Deferred>
        {
            internal static _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx30ⲻ39<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
