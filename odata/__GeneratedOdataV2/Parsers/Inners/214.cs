namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_selectOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SEMI_selectOption> Instance { get; } = from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
from _selectOption_1 in __GeneratedOdataV2.Parsers.Rules._selectOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._SEMI_selectOption(_SEMI_1, _selectOption_1);
    }
    
}
