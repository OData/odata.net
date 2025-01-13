namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _computedPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._computedProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._computedProperty(_odataIdentifier_1);
    }
    
}
