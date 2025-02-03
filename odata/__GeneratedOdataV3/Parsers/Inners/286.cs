namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr> Parse(IInput<char>? input)
            {
                var _lambdaVariableExpr_1 = __GeneratedOdataV3.Parsers.Rules._lambdaVariableExprParser.Instance.Parse(input);
if (!_lambdaVariableExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_lambdaVariableExpr_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr)!, input);
}

var _COLON_1 = __GeneratedOdataV3.Parsers.Rules._COLONParser.Instance.Parse(_BWS_1.Remainder);
if (!_COLON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COLON_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr)!, input);
}

var _lambdaPredicateExpr_1 = __GeneratedOdataV3.Parsers.Rules._lambdaPredicateExprParser.Instance.Parse(_BWS_2.Remainder);
if (!_lambdaPredicateExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr(_lambdaVariableExpr_1.Parsed, _BWS_1.Parsed, _COLON_1.Parsed, _BWS_2.Parsed,  _lambdaPredicateExpr_1.Parsed), _lambdaPredicateExpr_1.Remainder);
            }
        }
    }
    
}
