namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _CTL<TMode> : IAstNode<char, _CTL<ParseMode.Realized>>, IFromRealizedable<_CTL<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _CTL<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _CTL<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _CTL<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _CTL<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _CTL<ParseMode.Deferred>
        {
            internal static _CTL<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _CTL<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
