namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundPrimitiveColFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boundPrimitiveColFunctionCall> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _primitiveColFunction_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColFunctionParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boundPrimitiveColFunctionCall(_namespace_1, _ʺx2Eʺ_1, _primitiveColFunction_1, _functionParameters_1);
    }
    
}
