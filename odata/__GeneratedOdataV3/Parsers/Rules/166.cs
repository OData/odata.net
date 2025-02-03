namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _floorMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx66x6Cx6Fx6Fx72ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx66x6Cx6Fx6Fx72ʺParser.Instance.Parse(input);
if (!_ʺx66x6Cx6Fx6Fx72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx66x6Cx6Fx6Fx72ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._floorMethodCallExpr(_ʺx66x6Cx6Fx6Fx72ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
