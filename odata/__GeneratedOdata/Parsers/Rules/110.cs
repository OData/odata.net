namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._selectList> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _selectListItem_1 in __GeneratedOdata.Parsers.Rules._selectListItemParser.Instance
from _ⲤCOMMA_selectListItemↃ_1 in Inners._ⲤCOMMA_selectListItemↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectList(_OPEN_1, _selectListItem_1, _ⲤCOMMA_selectListItemↃ_1, _CLOSE_1);
    }
    
}
