namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundEntityColFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boundEntityColFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _entityColFunction_1 in __GeneratedOdataV2.Parsers.Rules._entityColFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boundEntityColFunctionCall(_namespace_1, _ʺx2Eʺ_1, _entityColFunction_1, _functionParameters_1);
    }
    
}
