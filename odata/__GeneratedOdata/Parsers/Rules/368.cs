namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _OWSParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._OWS> Instance { get; } = from _ⲤSPⳆHTABↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSPⳆHTABↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._OWS(_ⲤSPⳆHTABↃ_1);
    }
    
}
