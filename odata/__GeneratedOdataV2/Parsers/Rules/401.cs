namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._segment> Instance { get; } = from _pchar_1 in __GeneratedOdataV2.Parsers.Rules._pcharParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._segment(_pchar_1);
    }
    
}
