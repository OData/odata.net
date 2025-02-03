namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _EQ_customValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._EQ_customValue> Instance { get; } = from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _customValue_1 in __GeneratedOdataV2.Parsers.Rules._customValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._EQ_customValue(_EQ_1, _customValue_1);
    }
    
}
