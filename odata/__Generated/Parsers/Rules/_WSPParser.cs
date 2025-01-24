namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _WSPParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._WSP> Instance { get; } = (_SPParser.Instance).Or<char, __Generated.CstNodes.Rules._WSP>(_HTABParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._WSP._SP> Instance { get; } = from _SP_1 in __Generated.Parsers.Rules._SPParser.Instance
select new __Generated.CstNodes.Rules._WSP._SP(_SP_1);
        }
        
        public static class _HTABParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._WSP._HTAB> Instance { get; } = from _HTAB_1 in __Generated.Parsers.Rules._HTABParser.Instance
select new __Generated.CstNodes.Rules._WSP._HTAB(_HTAB_1);
        }
    }
    
}
