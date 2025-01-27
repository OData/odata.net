namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColFunctionImportParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitiveColFunctionImport> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColFunctionImport(_odataIdentifier_1);
    }
    
}
