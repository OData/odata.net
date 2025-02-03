namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundEntityFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boundEntityFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _entityFunction_1 in __GeneratedOdataV2.Parsers.Rules._entityFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boundEntityFunctionCall(_namespace_1, _ʺx2Eʺ_1, _entityFunction_1, _functionParameters_1);
    }
    
}
