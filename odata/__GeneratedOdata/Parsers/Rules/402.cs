namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnzParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._segmentⲻnz> Instance { get; } = from _pchar_1 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._segmentⲻnz(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._pchar>(_pchar_1));
    }
    
}
