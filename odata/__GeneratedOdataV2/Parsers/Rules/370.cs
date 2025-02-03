namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _EQⲻhParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._EQⲻh> Instance { get; } = from _BWSⲻh_1 in __GeneratedOdataV2.Parsers.Rules._BWSⲻhParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _BWSⲻh_2 in __GeneratedOdataV2.Parsers.Rules._BWSⲻhParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._EQⲻh(_BWSⲻh_1, _EQ_1, _BWSⲻh_2);
    }
    
}
