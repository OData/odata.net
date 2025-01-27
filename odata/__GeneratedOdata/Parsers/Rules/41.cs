namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitiveFunctionImportCall> Instance { get; } = from _primitiveFunctionImport_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveFunctionImportCall(_primitiveFunctionImport_1, _functionParameters_1);
    }
    
}
