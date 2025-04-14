namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻFF<TMode> : IAstNode<char, _Ⰳx00ⲻFF<ParseMode.Realized>>, IFromRealizedable<_Ⰳx00ⲻFF<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx00ⲻFF()
        {
        }
        
        internal static _Ⰳx00ⲻFF<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx00ⲻFF<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx00ⲻFF<ParseMode.Deferred>
        {
            internal static _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
