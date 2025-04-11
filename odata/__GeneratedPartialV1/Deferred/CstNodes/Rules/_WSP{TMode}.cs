namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSP<TMode> : IAstNode<char, _WSP<ParseMode.Realized>>, IFromRealizedable<_WSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _WSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _WSP<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _WSP<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _WSP<ParseMode.Deferred>
        {
            internal static _WSP<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
