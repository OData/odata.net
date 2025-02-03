namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻrootlessParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pathⲻrootless> Instance { get; } = from _segmentⲻnz_1 in __GeneratedOdataV2.Parsers.Rules._segmentⲻnzParser.Instance
from _Ⲥʺx2Fʺ_segmentↃ_1 in Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._pathⲻrootless(_segmentⲻnz_1, _Ⲥʺx2Fʺ_segmentↃ_1);
    }
    
}
