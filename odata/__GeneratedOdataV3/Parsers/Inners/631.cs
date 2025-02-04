namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤIPv6addressⳆIPvFutureↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤIPv6addressⳆIPvFutureↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤIPv6addressⳆIPvFutureↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤIPv6addressⳆIPvFutureↃ> Parse(IInput<char>? input)
            {
                var _IPv6addressⳆIPvFuture_1 = __GeneratedOdataV3.Parsers.Inners._IPv6addressⳆIPvFutureParser.Instance.Parse(input);
if (!_IPv6addressⳆIPvFuture_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤIPv6addressⳆIPvFutureↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤIPv6addressⳆIPvFutureↃ(_IPv6addressⳆIPvFuture_1.Parsed), _IPv6addressⳆIPvFuture_1.Remainder);
            }
        }
    }
    
}
