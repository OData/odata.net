namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedFunctionNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedFunctionName> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _function_1 in __GeneratedOdataV2.Parsers.Rules._functionParser.Instance
from _OPEN_parameterNames_CLOSE_1 in __GeneratedOdataV2.Parsers.Inners._OPEN_parameterNames_CLOSEParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedFunctionName(_namespace_1, _ʺx2Eʺ_1, _function_1, _OPEN_parameterNames_CLOSE_1.GetOrElse(null));
    }
    
}
