namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT> Parse(IInput<char>? input)
            {
                var _ʺx65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx65ʺParser.Instance.Parse(input);
if (!_ʺx65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT)!, input);
}

var _SIGN_1 = __GeneratedOdataV4.Parsers.Rules._SIGNParser.Instance.Optional().Parse(_ʺx65ʺ_1.Remainder);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_SIGN_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT(_ʺx65ʺ_1.Parsed, _SIGN_1.Parsed.GetOrElse(null), new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
