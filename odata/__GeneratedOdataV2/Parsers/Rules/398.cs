namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻabemptyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pathⲻabempty> Instance { get; } = from _Ⲥʺx2Fʺ_segmentↃ_1 in Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._pathⲻabempty(_Ⲥʺx2Fʺ_segmentↃ_1);
    }
    
}
