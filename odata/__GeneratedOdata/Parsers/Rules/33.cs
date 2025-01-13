namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _boundComplexColFunctionCallParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._boundComplexColFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _complexColFunction_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundComplexColFunctionCall(_namespace_1, _ʺx2Eʺ_1, _complexColFunction_1, _functionParameters_1);
    }
    
}
