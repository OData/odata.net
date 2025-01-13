namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityColFunctionImportParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityColFunctionImport> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityColFunctionImport(_odataIdentifier_1);
    }
    
}
