namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ> Instance { get; } = from _propertyPathExprⳆboundFunctionExprⳆannotationExpr_1 in __GeneratedOdata.Parsers.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ(_propertyPathExprⳆboundFunctionExprⳆannotationExpr_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ>
        {
            public IOutput<char, _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ> Parse(IInput<char>? input)
            {
                var _propertyPathExprⳆboundFunctionExprⳆannotationExpr_1 = __GeneratedOdata.Parsers.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExprParser.Instance.Parse(input);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ(_propertyPathExprⳆboundFunctionExprⳆannotationExpr_1.Parsed),
                    _propertyPathExprⳆboundFunctionExprⳆannotationExpr_1.Remainder);
            }
        }
    }
    
}
