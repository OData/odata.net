namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nameⲻseparatorParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._nameⲻseparator> Instance { get; } = from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _COLON_1 in __GeneratedOdataV2.Parsers.Rules._COLONParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._nameⲻseparator(_BWS_1, _COLON_1, _BWS_2);
    }
    
}
