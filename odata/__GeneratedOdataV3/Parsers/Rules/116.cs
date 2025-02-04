namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rootExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._rootExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._rootExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._rootExpr> Parse(IInput<char>? input)
            {
                var _ʺx24x72x6Fx6Fx74x2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x72x6Fx6Fx74x2FʺParser.Instance.Parse(input);
if (!_ʺx24x72x6Fx6Fx74x2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExpr)!, input);
}

var _ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃParser.Instance.Parse(_ʺx24x72x6Fx6Fx74x2Fʺ_1.Remainder);
if (!_ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExpr)!, input);
}

var _singleNavigationExpr_1 = __GeneratedOdataV3.Parsers.Rules._singleNavigationExprParser.Instance.Optional().Parse(_ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1.Remainder);
if (!_singleNavigationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._rootExpr(_ʺx24x72x6Fx6Fx74x2Fʺ_1.Parsed, _ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1.Parsed, _singleNavigationExpr_1.Parsed.GetOrElse(null)), _singleNavigationExpr_1.Remainder);
            }
        }
    }
    
}
