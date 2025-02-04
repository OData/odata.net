namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _valueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._value> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._value>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._value> Parse(IInput<char>? input)
            {
                var _ʺx2Fx24x76x61x6Cx75x65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x76x61x6Cx75x65ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x76x61x6Cx75x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._value)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._value.Instance, _ʺx2Fx24x76x61x6Cx75x65ʺ_1.Remainder);
            }
        }
    }
    
}
