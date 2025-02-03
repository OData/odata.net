namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnzParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._segmentⲻnz> Instance { get; } = from _pchar_1 in __GeneratedOdataV2.Parsers.Rules._pcharParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._segmentⲻnz(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._pchar>(_pchar_1));
    }
    
}
