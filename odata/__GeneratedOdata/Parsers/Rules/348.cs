namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _requestⲻidParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._requestⲻid> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._requestⲻid(_unreserved_1);
    }
    
}
