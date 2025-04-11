namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx41ⲻ5A<TMode> : IAstNode<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx41ⲻ5A<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _Ⰳx41ⲻ5A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _Ⰳx41ⲻ5A<ParseMode.Deferred>
        {
            internal static _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
