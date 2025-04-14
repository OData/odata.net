namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _CTL<TMode> : IAstNode<char, _CTL<ParseMode.Realized>>, IFromRealizedable<_CTL<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _CTL()
        {
        }
        
        internal static _CTL<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _CTL<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _CTL<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _CTL<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _CTL<ParseMode.Deferred>
        {
            internal static _CTL<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _CTL<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _CTL<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _CTL<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
