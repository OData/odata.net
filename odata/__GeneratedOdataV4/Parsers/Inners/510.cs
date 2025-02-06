namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖDIGIT_ʺx4DʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ)!, input);
}

var _ʺx4Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4DʺParser.Instance.Parse(_DIGIT_1.Remainder);
if (!_ʺx4Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed), _ʺx4Dʺ_1.Parsed), _ʺx4Dʺ_1.Remainder);
            }
        }
    }
    
}
