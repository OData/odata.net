namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ)!, input);
}

var _ʺx2Eʺ_1ЖDIGIT_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Eʺ_1ЖDIGITParser.Instance.Optional().Parse(_DIGIT_1.Remainder);
if (!_ʺx2Eʺ_1ЖDIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ)!, input);
}

var _ʺx53ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx53ʺParser.Instance.Parse(_ʺx2Eʺ_1ЖDIGIT_1.Remainder);
if (!_ʺx53ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed), _ʺx2Eʺ_1ЖDIGIT_1.Parsed.GetOrElse(null), _ʺx53ʺ_1.Parsed), _ʺx53ʺ_1.Remainder);
            }
        }
    }
    
}
