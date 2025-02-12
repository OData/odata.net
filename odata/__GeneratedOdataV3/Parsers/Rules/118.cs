namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _memberExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._memberExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._memberExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._memberExpr> Parse(IInput<char>? input)
            {
                var _qualifiedEntityTypeName_ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional().Parse(input);
if (!_qualifiedEntityTypeName_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._memberExpr)!, input);
}

var _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance.Parse(_qualifiedEntityTypeName_ʺx2Fʺ_1.Remainder);
if (!_ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._memberExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._memberExpr(_qualifiedEntityTypeName_ʺx2Fʺ_1.Parsed.GetOrElse(null), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Parsed), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Remainder);
            }
        }
    }
    
}
