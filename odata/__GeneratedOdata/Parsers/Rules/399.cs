namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _pathⲻabsoluteParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._pathⲻabsolute> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1 in __GeneratedOdata.Parsers.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._pathⲻabsolute(_ʺx2Fʺ_1, _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1.GetOrElse(null));
    }
    
}
