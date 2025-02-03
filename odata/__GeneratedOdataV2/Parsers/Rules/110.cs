namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectList> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _selectListItem_1 in __GeneratedOdataV2.Parsers.Rules._selectListItemParser.Instance
from _ⲤCOMMA_selectListItemↃ_1 in Inners._ⲤCOMMA_selectListItemↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectList(_OPEN_1, _selectListItem_1, _ⲤCOMMA_selectListItemↃ_1, _CLOSE_1);
    }
    
}
