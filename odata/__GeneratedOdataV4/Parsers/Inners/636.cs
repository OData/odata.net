namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥh16_ʺx3AʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ> Parse(IInput<char>? input)
            {
                var _h16_ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._h16_ʺx3AʺParser.Instance.Parse(input);
if (!_h16_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ(_h16_ʺx3Aʺ_1.Parsed), _h16_ʺx3Aʺ_1.Remainder);
            }
        }
    }
    
}
