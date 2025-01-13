namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _VCHARⳆobsⲻtextParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext> Instance { get; } = (_VCHARParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext>(_obsⲻtextParser.Instance);
        
        public static class _VCHARParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR> Instance { get; } = from _VCHAR_1 in __GeneratedOdata.Parsers.Rules._VCHARParser.Instance
select new __GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR(_VCHAR_1);
        }
        
        public static class _obsⲻtextParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext> Instance { get; } = from _obsⲻtext_1 in __GeneratedOdata.Parsers.Rules._obsⲻtextParser.Instance
select new __GeneratedOdata.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext(_obsⲻtext_1);
        }
    }
    
}
