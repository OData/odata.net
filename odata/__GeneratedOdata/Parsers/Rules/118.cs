namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _memberExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._memberExpr> Instance { get; } = from _qualifiedEntityTypeName_ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional()
from _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._memberExpr(_qualifiedEntityTypeName_ʺx2Fʺ_1.GetOrElse(null), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._memberExpr> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._memberExpr>
        {
            public IOutput<char, _memberExpr> Parse(IInput<char>? input)
            {
                ////var _qualifiedEntityTypeName_ʺx2Fʺ_1 = __GeneratedOdata.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional().Parse(input);
                var _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 = __GeneratedOdata.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance.Parse(input);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._memberExpr(null, _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Parsed),
                    _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1.Remainder);
            }
        }
    }
    
}
