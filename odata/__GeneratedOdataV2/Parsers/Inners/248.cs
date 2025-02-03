namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_selectListItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_selectListItem> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _selectListItem_1 in __GeneratedOdataV2.Parsers.Rules._selectListItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_selectListItem(_COMMA_1, _selectListItem_1);
    }
    
}
