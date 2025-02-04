namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr)!, input);
}

var _memberExpr_1 = __GeneratedOdataV3.Parsers.Rules._memberExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_memberExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleNavigationExpr(_ʺx2Fʺ_1.Parsed, _memberExpr_1.Parsed), _memberExpr_1.Remainder);
            }
        }
    }
    
}
