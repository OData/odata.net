namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _h16_ʺx3AʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ> Parse(IInput<char>? input)
            {
                var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(input);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_h16_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._h16_ʺx3Aʺ(_h16_1.Parsed, _ʺx3Aʺ_1.Parsed), _ʺx3Aʺ_1.Remainder);
            }
        }
    }
    
}
