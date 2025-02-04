namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _h16_ʺx3Aʺ_h16Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16> Parse(IInput<char>? input)
            {
                var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(input);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_h16_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16)!, input);
}

var _h16_2 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_h16_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ_h16(_h16_1.Parsed, _ʺx3Aʺ_1.Parsed, _h16_2.Parsed), _h16_2.Remainder);
            }
        }
    }
    
}
