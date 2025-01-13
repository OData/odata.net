namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _qualifiedFunctionNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._qualifiedFunctionName> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _function_1 in __GeneratedOdata.Parsers.Rules._functionParser.Instance
from _OPEN_parameterNames_CLOSE_1 in __GeneratedOdata.Parsers.Inners._OPEN_parameterNames_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._qualifiedFunctionName(_namespace_1, _ʺx2Eʺ_1, _function_1, _OPEN_parameterNames_CLOSE_1.GetOrElse(null));
    }
    
}
