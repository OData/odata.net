namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_memberExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr)!, input);
}

var _memberExpr_1 = __GeneratedOdataV4.Parsers.Rules._memberExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_memberExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_memberExpr(_ʺx2Fʺ_1.Parsed, _memberExpr_1.Parsed), _memberExpr_1.Remainder);
            }
        }
    }
    
}
