namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡> Parse(IInput<char>? input)
            {
                var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡)!, input);
}

var _second_1 = __GeneratedOdataV3.Parsers.Rules._secondParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_second_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡)!, input);
}

var _ʺx2Eʺ_fractionalSeconds_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Eʺ_fractionalSecondsParser.Instance.Optional().Parse(_second_1.Remainder);
if (!_ʺx2Eʺ_fractionalSeconds_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡(_ʺx3Aʺ_1.Parsed, _second_1.Parsed, _ʺx2Eʺ_fractionalSeconds_1.Parsed.GetOrElse(null)), _ʺx2Eʺ_fractionalSeconds_1.Remainder);
            }
        }
    }
    
}
