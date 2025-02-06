namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _memberExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._memberExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._memberExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._memberExpr> Parse(IInput<char>? input)
            {

var _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._memberExpr(null, _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Parsed), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Remainder);
            }
        }
    }
    
}
