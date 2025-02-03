namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖDIGIT_ʺx48ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ)!, input);
}

var _ʺx48ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx48ʺParser.Instance.Parse(_DIGIT_1.Remainder);
if (!_ʺx48ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed),  _ʺx48ʺ_1.Parsed), _ʺx48ʺ_1.Remainder);
            }
        }
    }
    
}
