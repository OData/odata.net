namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._complexFunctionImportCall> Instance { get; } = from _complexFunctionImport_1 in __GeneratedOdata.Parsers.Rules._complexFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexFunctionImportCall(_complexFunctionImport_1, _functionParameters_1);
    }
    
}
