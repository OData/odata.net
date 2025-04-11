namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx21ⲻ7E<TMode> : IAstNode<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>, IFromRealizedable<_Ⰳx21ⲻ7E<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _Ⰳx21ⲻ7E<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _Ⰳx21ⲻ7E<ParseMode.Deferred>
        {
            internal static _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
