namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImportCall> Instance { get; } = from _primitiveColFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveColFunctionImportCall(_primitiveColFunctionImport_1, _functionParameters_1);
    }
    
}
