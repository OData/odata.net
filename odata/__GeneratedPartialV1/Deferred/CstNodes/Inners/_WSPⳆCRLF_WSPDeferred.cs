namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _WSPⳆCRLF_WSPDeferred : IAstNode<char, _WSPⳆCRLF_WSPRealized>
    {
        public _WSPⳆCRLF_WSPDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _WSPⳆCRLF_WSPDeferred(IFuture<IRealizationResult<char, _WSPⳆCRLF_WSPRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _WSPⳆCRLF_WSPRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _WSPⳆCRLF_WSPRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _WSPⳆCRLF_WSPRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _WSP = _WSPⳆCRLF_WSPRealized._WSP.Create(this.previousNodeRealizationResult);
if (_WSP.Success)
{
    return _WSP;
}

var _CRLF_WSP = _WSPⳆCRLF_WSPRealized._CRLF_WSP.Create(this.previousNodeRealizationResult);
if (_CRLF_WSP.Success)
{
    return _CRLF_WSP;
}
return new RealizationResult<char, _WSPⳆCRLF_WSPRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}
