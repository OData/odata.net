namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _decimalValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue> Instance { get; } = (_꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue>(_nanInfinityParser.Instance);
        
        public static class _꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡> Parse(IInput<char>? input)
                {
                    var _SIGN_1 = __GeneratedOdataV3.Parsers.Rules._SIGNParser.Instance.Optional().Parse(input);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_SIGN_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡)!, input);
}

var _ʺx2Eʺ_1ЖDIGIT_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Eʺ_1ЖDIGITParser.Instance.Optional().Parse(_DIGIT_1.Remainder);
if (!_ʺx2Eʺ_1ЖDIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡)!, input);
}

var _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1 = __GeneratedOdataV3.Parsers.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGITParser.Instance.Optional().Parse(_ʺx2Eʺ_1ЖDIGIT_1.Remainder);
if (!_ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡(_SIGN_1.Parsed.GetOrElse(null), new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed), _ʺx2Eʺ_1ЖDIGIT_1.Parsed.GetOrElse(null), _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1.Parsed.GetOrElse(null)), _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1.Remainder);
                }
            }
        }
        
        public static class _nanInfinityParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity> Parse(IInput<char>? input)
                {
                    var _nanInfinity_1 = __GeneratedOdataV3.Parsers.Rules._nanInfinityParser.Instance.Parse(input);
if (!_nanInfinity_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity(_nanInfinity_1.Parsed), _nanInfinity_1.Remainder);
                }
            }
        }
    }
    
}
