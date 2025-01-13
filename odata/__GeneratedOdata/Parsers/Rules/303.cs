namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _secondParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._second> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdata.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdata.CstNodes.Rules._second(_zeroToFiftyNine_1);
    }
    
}
