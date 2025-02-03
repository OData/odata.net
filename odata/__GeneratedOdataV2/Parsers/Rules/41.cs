namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImportCall> Instance { get; } = from _primitiveFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._primitiveFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdataV2.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveFunctionImportCall(_primitiveFunctionImport_1, _functionParameters_1);
    }
    
}
