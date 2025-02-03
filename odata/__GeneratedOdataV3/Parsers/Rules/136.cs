namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _anyExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._anyExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._anyExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._anyExpr> Parse(IInput<char>? input)
            {
                var _ʺx61x6Ex79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx61x6Ex79ʺParser.Instance.Parse(input);
if (!_ʺx61x6Ex79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx61x6Ex79ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

var _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1 = __GeneratedOdataV3.Parsers.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprParser.Instance.Optional().Parse(_BWS_1.Remainder);
if (!_lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._anyExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._anyExpr(_ʺx61x6Ex79ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1.Parsed.GetOrElse(null), _BWS_2.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
