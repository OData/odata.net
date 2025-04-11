namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _alphaNumeric<TMode> : IAstNode<char, _alphaNumeric<ParseMode.Realized>>, IFromRealizedable<_alphaNumeric<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _alphaNumeric<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _alphaNumeric<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _alphaNumeric<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _alphaNumeric<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _alphaNumeric<ParseMode.Deferred>
        {
            internal static _alphaNumeric<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _alphaNumeric<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
