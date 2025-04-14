namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPⳆCRLF_WSP<TMode> : IAstNode<char, _WSPⳆCRLF_WSP<ParseMode.Realized>>, IFromRealizedable<_WSPⳆCRLF_WSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _WSPⳆCRLF_WSP()
        {
        }
        
        internal static _WSPⳆCRLF_WSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _WSPⳆCRLF_WSP<ParseMode.Deferred>
        {
            internal static _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
            
            public override _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
            {
                throw new Exception("TODO");
            }
            
            public override IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
            {
                throw new Exception("TODO");
            }
        }
    }
    
}
