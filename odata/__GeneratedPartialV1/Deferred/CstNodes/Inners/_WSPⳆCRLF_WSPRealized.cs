namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _WSPⳆCRLF_WSPRealized : IFromRealizedable<_WSPⳆCRLF_WSPDeferred>
    {
        private _WSPⳆCRLF_WSPRealized()
        {
        }
        
        public abstract _WSPⳆCRLF_WSPDeferred Convert();
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSPⳆCRLF_WSPRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSP node, TContext context);
            protected internal abstract TResult Accept(_CRLF_WSP node, TContext context);
        }
        
        public sealed class _WSP : _WSPⳆCRLF_WSPRealized
        {
            private _WSP(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1 { get; }
            
            public static IRealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _WSPⳆCRLF_WSPRealized._WSP(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._WSP>(false, default, input);
}
            }
            
            public override _WSPⳆCRLF_WSPDeferred Convert()
            {
                return new _WSPⳆCRLF_WSPDeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _CRLF_WSP : _WSPⳆCRLF_WSPRealized
        {
            private _CRLF_WSP(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Rules._CRLF<ParseMode.Realized> _CRLF_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Rules._WSP<ParseMode.Realized> _WSP_1 { get; }
            
            public static IRealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _WSPⳆCRLF_WSPRealized._CRLF_WSP(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _WSPⳆCRLF_WSPRealized._CRLF_WSP>(false, default, input);
}
            }
            
            public override _WSPⳆCRLF_WSPDeferred Convert()
            {
                return new _WSPⳆCRLF_WSPDeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
