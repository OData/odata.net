namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._allExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._allExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._allExpr> Parse(IInput<char>? input)
            {
                var _ʺx61x6Cx6Cʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx61x6Cx6CʺParser.Instance.Parse(input);
if (!_ʺx61x6Cx6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx61x6Cx6Cʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _lambdaVariableExpr_1 = __GeneratedOdataV3.Parsers.Rules._lambdaVariableExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_lambdaVariableExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_lambdaVariableExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _COLON_1 = __GeneratedOdataV3.Parsers.Rules._COLONParser.Instance.Parse(_BWS_2.Remainder);
if (!_COLON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _BWS_3 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COLON_1.Remainder);
if (!_BWS_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _lambdaPredicateExpr_1 = __GeneratedOdataV3.Parsers.Rules._lambdaPredicateExprParser.Instance.Parse(_BWS_3.Remainder);
if (!_lambdaPredicateExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _BWS_4 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_lambdaPredicateExpr_1.Remainder);
if (!_BWS_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_4.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._allExpr(_ʺx61x6Cx6Cʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _lambdaVariableExpr_1.Parsed, _BWS_2.Parsed, _COLON_1.Parsed, _BWS_3.Parsed, _lambdaPredicateExpr_1.Parsed, _BWS_4.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
