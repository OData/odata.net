namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ> Instance { get; } = from _segmentⲻnz_1 in __GeneratedOdataV2.Parsers.Rules._segmentⲻnzParser.Instance
from _Ⲥʺx2Fʺ_segmentↃ_1 in Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ(_segmentⲻnz_1, _Ⲥʺx2Fʺ_segmentↃ_1);
    }
    
}
