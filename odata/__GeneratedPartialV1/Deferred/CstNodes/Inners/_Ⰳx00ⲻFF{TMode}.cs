namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻFF<TMode> : IAstNode<char, _Ⰳx00ⲻFF<ParseMode.Realized>>, IFromRealizedable<_Ⰳx00ⲻFF<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _Ⰳx00ⲻFF<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _Ⰳx00ⲻFF<ParseMode.Deferred>
        {
            internal static _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
