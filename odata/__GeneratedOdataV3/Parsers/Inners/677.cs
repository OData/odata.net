namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _VCHARⳆobsⲻtextParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext> Instance { get; } = (_VCHARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext>(_obsⲻtextParser.Instance);
        
        public static class _VCHARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR> Instance { get; } = from _VCHAR_1 in __GeneratedOdataV3.Parsers.Rules._VCHARParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR(_VCHAR_1);
        }
        
        public static class _obsⲻtextParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext> Instance { get; } = from _obsⲻtext_1 in __GeneratedOdataV3.Parsers.Rules._obsⲻtextParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext(_obsⲻtext_1);
        }
    }
    
}
