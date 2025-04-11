namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPⳆCRLF_WSP<TMode> : IAstNode<char, _WSPⳆCRLF_WSP<ParseMode.Realized>>, IFromRealizedable<_WSPⳆCRLF_WSP<ParseMode.Deferred>> where TMode : ParseMode
    {
        internal static _WSPⳆCRLF_WSP<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public _WSPⳆCRLF_WSP<ParseMode.Deferred> Convert()
        {
            throw new System.Exception("TODO");
        }
        
        public IRealizationResult<char, _WSPⳆCRLF_WSP<ParseMode.Realized>> Realize()
        {
            throw new System.Exception("TODO");
        }
        
        public sealed class Deferred : _WSPⳆCRLF_WSP<ParseMode.Deferred>
        {
            internal static _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return _WSPⳆCRLF_WSP<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
            }
        }
    }
    
}
