namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _BIT<TMode> : IAstNode<char, _BIT<ParseMode.Realized>>, IFromRealizedable<_BIT<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _BIT<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _BIT<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _BIT<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _BIT<ParseMode.Deferred>
        {
            internal static _BIT<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _BIT<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
