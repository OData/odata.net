namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _expandCountOption_1 in __GeneratedOdata.Parsers.Rules._expandCountOptionParser.Instance
from _ⲤSEMI_expandCountOptionↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSEMI_expandCountOptionↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE(_OPEN_1, _expandCountOption_1, _ⲤSEMI_expandCountOptionↃ_1, _CLOSE_1);
    }
    
}
