namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPⲻliteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._IPⲻliteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._IPⲻliteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._IPⲻliteral> Parse(IInput<char>? input)
            {
                var _ʺx5Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5BʺParser.Instance.Parse(input);
if (!_ʺx5Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._IPⲻliteral)!, input);
}

var _ⲤIPv6addressⳆIPvFutureↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤIPv6addressⳆIPvFutureↃParser.Instance.Parse(_ʺx5Bʺ_1.Remainder);
if (!_ⲤIPv6addressⳆIPvFutureↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._IPⲻliteral)!, input);
}

var _ʺx5Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5DʺParser.Instance.Parse(_ⲤIPv6addressⳆIPvFutureↃ_1.Remainder);
if (!_ʺx5Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._IPⲻliteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._IPⲻliteral(_ʺx5Bʺ_1.Parsed, _ⲤIPv6addressⳆIPvFutureↃ_1.Parsed, _ʺx5Dʺ_1.Parsed), _ʺx5Dʺ_1.Remainder);
            }
        }
    }
    
}
