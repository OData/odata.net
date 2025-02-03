namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnzParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._segmentⲻnz> Instance { get; } = from _pchar_1 in __GeneratedOdataV3.Parsers.Rules._pcharParser.Instance.Repeat(1, null)
select new __GeneratedOdataV3.CstNodes.Rules._segmentⲻnz(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._pchar>(_pchar_1));
    }
    
}
