namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _pathⲻrootlessParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._pathⲻrootless> Instance { get; } = from _segmentⲻnz_1 in __GeneratedOdata.Parsers.Rules._segmentⲻnzParser.Instance
from _Ⲥʺx2Fʺ_segmentↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._pathⲻrootless(_segmentⲻnz_1, _Ⲥʺx2Fʺ_segmentↃ_1);
    }
    
}