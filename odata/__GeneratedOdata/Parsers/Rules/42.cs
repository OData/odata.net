namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitiveColFunctionImportCall> Instance { get; } = from _primitiveColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._primitiveColFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColFunctionImportCall(_primitiveColFunctionImport_1, _functionParameters_1);
    }
    
}
