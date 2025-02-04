namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ> Parse(IInput<char>? input)
            {
                var _propertyPathExprⳆboundFunctionExprⳆannotationExpr_1 = __GeneratedOdataV3.Parsers.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExprParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ(_propertyPathExprⳆboundFunctionExprⳆannotationExpr_1.Parsed), _propertyPathExprⳆboundFunctionExprⳆannotationExpr_1.Remainder);
            }
        }
    }
    
}
