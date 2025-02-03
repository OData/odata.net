namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_segmentParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_segment> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _segment_1 in __GeneratedOdataV2.Parsers.Rules._segmentParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_segment(_ʺx2Fʺ_1, _segment_1);
    }
    
}
