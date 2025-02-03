namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ж1Ⲥh16_ʺx3AʺↃ_h16Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16> Parse(IInput<char>? input)
            {
                var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(0, 1).Parse(input);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16)!, input);
}

var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtMost1<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed),  _h16_1.Parsed), _h16_1.Remainder);
            }
        }
    }
    
}
