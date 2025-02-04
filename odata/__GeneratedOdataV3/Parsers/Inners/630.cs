namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _IPv6addressⳆIPvFutureParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture> Instance { get; } = (_IPv6addressParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture>(_IPvFutureParser.Instance);
        
        public static class _IPv6addressParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address> Parse(IInput<char>? input)
                {
                    var _IPv6address_1 = __GeneratedOdataV3.Parsers.Rules._IPv6addressParser.Instance.Parse(input);
if (!_IPv6address_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPv6address(_IPv6address_1.Parsed), _IPv6address_1.Remainder);
                }
            }
        }
        
        public static class _IPvFutureParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture> Parse(IInput<char>? input)
                {
                    var _IPvFuture_1 = __GeneratedOdataV3.Parsers.Rules._IPvFutureParser.Instance.Parse(input);
if (!_IPvFuture_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._IPv6addressⳆIPvFuture._IPvFuture(_IPvFuture_1.Parsed), _IPvFuture_1.Remainder);
                }
            }
        }
    }
    
}
