namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻabsoluteParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pathⲻabsolute> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1 in __GeneratedOdataV2.Parsers.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._pathⲻabsolute(_ʺx2Fʺ_1, _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1.GetOrElse(null));
    }
    
}
