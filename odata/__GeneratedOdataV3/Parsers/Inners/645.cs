namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥh16_ʺx3Aʺ_h16ↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ> Parse(IInput<char>? input)
            {
                var _h16_ʺx3Aʺ_h16_1 = __GeneratedOdataV3.Parsers.Inners._h16_ʺx3Aʺ_h16Parser.Instance.Parse(input);
if (!_h16_ʺx3Aʺ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3Aʺ_h16Ↄ(_h16_ʺx3Aʺ_h16_1.Parsed), _h16_ʺx3Aʺ_h16_1.Remainder);
            }
        }
    }
    
}
