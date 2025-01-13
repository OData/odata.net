namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _complexColFunctionImportCallParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._complexColFunctionImportCall> Instance { get; } = from _complexColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexColFunctionImportCall(_complexColFunctionImport_1, _functionParameters_1);
    }
    
}
