namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._segment> Instance { get; } = from _pchar_1 in __GeneratedOdataV3.Parsers.Rules._pcharParser.Instance.Many()
select new __GeneratedOdataV3.CstNodes.Rules._segment(_pchar_1);
    }
    
}
