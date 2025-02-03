namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _IPv6addressⳆIPvFutureParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture> Instance { get; } = (_IPv6addressParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture>(_IPvFutureParser.Instance);
        
        public static class _IPv6addressParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address> Instance { get; } = from _IPv6address_1 in __GeneratedOdataV3.Parsers.Rules._IPv6addressParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address(_IPv6address_1);
        }
        
        public static class _IPvFutureParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture> Instance { get; } = from _IPvFuture_1 in __GeneratedOdataV3.Parsers.Rules._IPvFutureParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture(_IPvFuture_1);
        }
    }
    
}
