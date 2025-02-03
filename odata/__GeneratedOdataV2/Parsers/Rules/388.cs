namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hostParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._host> Instance { get; } = (_IPⲻliteralParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._host>(_IPv4addressParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._host>(_regⲻnameParser.Instance);
        
        public static class _IPⲻliteralParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._host._IPⲻliteral> Instance { get; } = from _IPⲻliteral_1 in __GeneratedOdataV2.Parsers.Rules._IPⲻliteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._host._IPⲻliteral(_IPⲻliteral_1);
        }
        
        public static class _IPv4addressParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._host._IPv4address> Instance { get; } = from _IPv4address_1 in __GeneratedOdataV2.Parsers.Rules._IPv4addressParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._host._IPv4address(_IPv4address_1);
        }
        
        public static class _regⲻnameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._host._regⲻname> Instance { get; } = from _regⲻname_1 in __GeneratedOdataV2.Parsers.Rules._regⲻnameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._host._regⲻname(_regⲻname_1);
        }
    }
    
}
