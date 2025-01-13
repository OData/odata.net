namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityFunctionImportCallParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityFunctionImportCall> Instance { get; } = from _entityFunctionImport_1 in __GeneratedOdata.Parsers.Rules._entityFunctionImportParser.Instance
from _functionParameters_1 in __GeneratedOdata.Parsers.Rules._functionParametersParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityFunctionImportCall(_entityFunctionImport_1, _functionParameters_1);
    }
    
}
