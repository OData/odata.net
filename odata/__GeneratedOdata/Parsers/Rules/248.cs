namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitiveColPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColProperty(_odataIdentifier_1);
    }
    
}
