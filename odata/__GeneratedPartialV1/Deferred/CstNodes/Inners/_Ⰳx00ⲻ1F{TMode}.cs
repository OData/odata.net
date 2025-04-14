namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻ1F<TMode> : IAstNode<char, _Ⰳx00ⲻ1F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx00ⲻ1F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx00ⲻ1F()
        {
        }
        
        internal static _Ⰳx00ⲻ1F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx00ⲻ1F<ParseMode.Deferred>
        {
            internal static _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
