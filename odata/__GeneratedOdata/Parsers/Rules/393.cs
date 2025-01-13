namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _h16Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._h16> Instance { get; } = from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._h16(_HEXDIG_1);
    }
    
}
