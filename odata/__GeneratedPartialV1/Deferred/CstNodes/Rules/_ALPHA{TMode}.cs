namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _ALPHA<TMode> : IAstNode<char, _ALPHA<ParseMode.Realized>>, IFromRealizedable<_ALPHA<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _ALPHA<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _ALPHA<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _ALPHA<ParseMode.Deferred>
        {
            internal static _ALPHA<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
