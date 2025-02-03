namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundComplexFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boundComplexFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _complexFunction_1 in __GeneratedOdataV2.Parsers.Rules._complexFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boundComplexFunctionCall(_namespace_1, _ʺx2Eʺ_1, _complexFunction_1, _functionParameters_1);
    }
    
}
