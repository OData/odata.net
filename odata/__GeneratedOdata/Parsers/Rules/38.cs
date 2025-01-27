namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._entityColFunctionImportCall> Instance { get; } = from _entityColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._entityColFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityColFunctionImportCall(_entityColFunctionImport_1, _functionParameters_1);
    }
    
}
