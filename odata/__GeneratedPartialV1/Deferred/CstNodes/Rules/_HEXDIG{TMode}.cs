namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _HEXDIG<TMode> : IAstNode<char, _HEXDIG<ParseMode.Realized>>, IFromRealizedable<_HEXDIG<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _HEXDIG<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _HEXDIG<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _HEXDIG<ParseMode.Deferred>
        {
            internal static _HEXDIG<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
