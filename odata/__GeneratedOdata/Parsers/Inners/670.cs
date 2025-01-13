namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ> Instance { get; } = from _segmentⲻnz_1 in __GeneratedOdata.Parsers.Rules._segmentⲻnzParser.Instance
from _Ⲥʺx2Fʺ_segmentↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ(_segmentⲻnz_1, _Ⲥʺx2Fʺ_segmentↃ_1);
    }
    
}
