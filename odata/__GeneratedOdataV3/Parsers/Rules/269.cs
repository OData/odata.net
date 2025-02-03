namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColFunctionImportParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveColFunctionImport> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveColFunctionImport(_odataIdentifier_1);
    }
    
}
