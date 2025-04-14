namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _HEXDIG<TMode> : IAstNode<char, _HEXDIG<ParseMode.Realized>>, IFromRealizedable<_HEXDIG<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _HEXDIG()
        {
        }
        
        internal static _HEXDIG<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _HEXDIG<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _HEXDIG<ParseMode.Deferred>
        {
            internal static _HEXDIG<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _HEXDIG<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _HEXDIG<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _HEXDIG<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
