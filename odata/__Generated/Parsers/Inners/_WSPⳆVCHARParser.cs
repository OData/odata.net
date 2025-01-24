namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _WSPⳆVCHARParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._WSPⳆVCHAR> Instance { get; } = (_WSPParser.Instance).Or<char, __Generated.CstNodes.Inners._WSPⳆVCHAR>(_VCHARParser.Instance);
        
        public static class _WSPParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._WSPⳆVCHAR._WSP> Instance { get; } = from _WSP_1 in __Generated.Parsers.Rules._WSPParser.Instance
select new __Generated.CstNodes.Inners._WSPⳆVCHAR._WSP(_WSP_1);
        }
        
        public static class _VCHARParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR> Instance { get; } = from _VCHAR_1 in __Generated.Parsers.Rules._VCHARParser.Instance
select new __Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR(_VCHAR_1);
        }
    }
    
}
