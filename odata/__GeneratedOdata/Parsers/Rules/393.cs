namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _h16Parser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._h16> Instance { get; } = from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, 4)
select new __GeneratedOdata.CstNodes.Rules._h16(new __GeneratedOdata.CstNodes.Inners.HelperRangedFrom1To4<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_1));
    }
    
}
