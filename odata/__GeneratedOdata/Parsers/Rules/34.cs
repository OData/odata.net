namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundPrimitiveFunctionCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._boundPrimitiveFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _primitiveFunction_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundPrimitiveFunctionCall(_namespace_1, _ʺx2Eʺ_1, _primitiveFunction_1, _functionParameters_1);
    }
    
}
