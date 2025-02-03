namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityFunctionImportCall> Instance { get; } = from _entityFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._entityFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityFunctionImportCall(_entityFunctionImport_1, _functionParameters_1);
    }
    
}
