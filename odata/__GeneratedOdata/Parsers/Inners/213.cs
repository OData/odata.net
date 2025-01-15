namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _selectOptionPC_1 in __GeneratedOdata.Parsers.Rules._selectOptionPCParser.Instance
from _ⲤSEMI_selectOptionPCↃ_1 in Inners._ⲤSEMI_selectOptionPCↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE(_OPEN_1, _selectOptionPC_1, _ⲤSEMI_selectOptionPCↃ_1, _CLOSE_1);
    }
    
}
