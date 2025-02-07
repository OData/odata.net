namespace __GeneratedOdataV4.Parsers.Inners
{
    using __GeneratedOdataV4.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx3FʺParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ>>
        {
            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _ʺx3Fʺ> Parse(in CombinatorParsingV3.StringInput input)
            {
                var remainder = input.Next(out var more);

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _ʺx3Fʺ>(
                    true,
                    __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ.Instance,
                    more,
                    remainder);
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ> Parse(IInput<char>? input)
            {
                var _x3F_1 = __GeneratedOdataV4.Parsers.Inners._x3FParser.Instance.Parse(input);
if (!_x3F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ.Instance, _x3F_1.Remainder);
            }
        }
    }
    
}
