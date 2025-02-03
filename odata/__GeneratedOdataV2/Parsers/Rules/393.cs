namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _h16Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._h16> Instance { get; } = from _HEXDIG_1 in __GeneratedOdataV2.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, 4)
select new __GeneratedOdataV2.CstNodes.Rules._h16(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedFrom1To4<__GeneratedOdataV2.CstNodes.Rules._HEXDIG>(_HEXDIG_1));
    }
    
}
