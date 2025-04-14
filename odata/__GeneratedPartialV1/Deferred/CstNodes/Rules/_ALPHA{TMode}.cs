namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _ALPHA<TMode> : IAstNode<char, _ALPHA<ParseMode.Realized>>, IFromRealizedable<_ALPHA<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _ALPHA()
        {
        }
        
        internal static _ALPHA<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _ALPHA<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _ALPHA<ParseMode.Deferred>
        {
            internal static _ALPHA<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _ALPHA<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _ALPHA<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _ALPHA<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
