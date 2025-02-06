namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Aʺ_portParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port> Parse(IInput<char>? input)
            {
                var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port)!, input);
}

var _port_1 = __GeneratedOdataV4.Parsers.Rules._portParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_port_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ_port(_ʺx3Aʺ_1.Parsed, _port_1.Parsed), _port_1.Remainder);
            }
        }
    }
    
}
