namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _HEXDIGParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._HEXDIG> Instance { get; } = (_DIGITParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._HEXDIG>(_AⲻtoⲻFParser.Instance);
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._HEXDIG._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._HEXDIG._DIGIT(_DIGIT_1);
        }
        
        public static class _AⲻtoⲻFParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._HEXDIG._AⲻtoⲻF> Instance { get; } = from _AⲻtoⲻF_1 in __GeneratedOdata.Parsers.Rules._AⲻtoⲻFParser.Instance
select new __GeneratedOdata.CstNodes.Rules._HEXDIG._AⲻtoⲻF(_AⲻtoⲻF_1);
        }
    }
    
}
