namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_selectOptionPCParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SEMI_selectOptionPC> Instance { get; } = from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
from _selectOptionPC_1 in __GeneratedOdataV2.Parsers.Rules._selectOptionPCParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._SEMI_selectOptionPC(_SEMI_1, _selectOptionPC_1);
    }
    
}
