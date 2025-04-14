namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _BIT<TMode> : IAstNode<char, _BIT<ParseMode.Realized>>, IFromRealizedable<_BIT<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _BIT()
        {
        }
        
        internal static _BIT<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _BIT<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _BIT<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _BIT<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _BIT<ParseMode.Deferred>
        {
            internal static _BIT<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _BIT<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _BIT<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _BIT<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
