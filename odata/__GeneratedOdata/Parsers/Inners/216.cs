namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty> Instance { get; } = (_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty>(_ʺx2Fʺ_selectPropertyParser.Instance);
        
        public static class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _selectOption_1 in __GeneratedOdata.Parsers.Rules._selectOptionParser.Instance
from _ⲤSEMI_selectOptionↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSEMI_selectOptionↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE(_OPEN_1, _selectOption_1, _ⲤSEMI_selectOptionↃ_1, _CLOSE_1);
        }
        
        public static class _ʺx2Fʺ_selectPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _selectProperty_1 in __GeneratedOdata.Parsers.Rules._selectPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty(_ʺx2Fʺ_1, _selectProperty_1);
        }
    }
    
}
