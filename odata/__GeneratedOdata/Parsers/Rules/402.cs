namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _segmentⲻnzParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._segmentⲻnz> Instance { get; } = from _pchar_1 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._segmentⲻnz(_pchar_1);
    }
    
}
