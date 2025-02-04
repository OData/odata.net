namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _durationValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._durationValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._durationValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._durationValue> Parse(IInput<char>? input)
            {
                var _SIGN_1 = __GeneratedOdataV3.Parsers.Rules._SIGNParser.Instance.Optional().Parse(input);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._durationValue)!, input);
}

var _ʺx50ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx50ʺParser.Instance.Parse(_SIGN_1.Remainder);
if (!_ʺx50ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._durationValue)!, input);
}

var _1ЖDIGIT_ʺx44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._1ЖDIGIT_ʺx44ʺParser.Instance.Optional().Parse(_ʺx50ʺ_1.Remainder);
if (!_1ЖDIGIT_ʺx44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._durationValue)!, input);
}

var _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1 = __GeneratedOdataV3.Parsers.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡Parser.Instance.Optional().Parse(_1ЖDIGIT_ʺx44ʺ_1.Remainder);
if (!_ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._durationValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._durationValue(_SIGN_1.Parsed.GetOrElse(null), _ʺx50ʺ_1.Parsed, _1ЖDIGIT_ʺx44ʺ_1.Parsed.GetOrElse(null), _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1.Parsed.GetOrElse(null)), _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1.Remainder);
            }
        }
    }
    
}
