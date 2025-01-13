namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _complexPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._complexProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexProperty(_odataIdentifier_1);
    }
    
}
