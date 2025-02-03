namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexColFunctionImportCall> Instance { get; } = from _complexColFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._complexColFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._complexColFunctionImportCall(_complexColFunctionImport_1, _functionParameters_1);
    }
    
}
