namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ceilingMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx63x65x69x6Cx69x6Ex67ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx63x65x69x6Cx69x6Ex67ʺParser.Instance.Parse(input);
if (!_ʺx63x65x69x6Cx69x6Ex67ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx63x65x69x6Cx69x6Ex67ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._ceilingMethodCallExpr(_ʺx63x65x69x6Cx69x6Ex67ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
