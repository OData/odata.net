namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _guidValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._guidValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._guidValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._guidValue> Parse(IInput<char>? input)
            {
                var _HEXDIG_1 = __GeneratedOdataV4.Parsers.Rules._HEXDIGParser.Instance.Repeat(8, 8).Parse(input);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _ʺx2Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_HEXDIG_1.Remainder);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _HEXDIG_2 = __GeneratedOdataV4.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4).Parse(_ʺx2Dʺ_1.Remainder);
if (!_HEXDIG_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _ʺx2Dʺ_2 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_HEXDIG_2.Remainder);
if (!_ʺx2Dʺ_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _HEXDIG_3 = __GeneratedOdataV4.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4).Parse(_ʺx2Dʺ_2.Remainder);
if (!_HEXDIG_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _ʺx2Dʺ_3 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_HEXDIG_3.Remainder);
if (!_ʺx2Dʺ_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _HEXDIG_4 = __GeneratedOdataV4.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4).Parse(_ʺx2Dʺ_3.Remainder);
if (!_HEXDIG_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _ʺx2Dʺ_4 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_HEXDIG_4.Remainder);
if (!_ʺx2Dʺ_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

var _HEXDIG_5 = __GeneratedOdataV4.Parsers.Rules._HEXDIGParser.Instance.Repeat(12, 12).Parse(_ʺx2Dʺ_4.Remainder);
if (!_HEXDIG_5.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._guidValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._guidValue(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly8<__GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_HEXDIG_1.Parsed), _ʺx2Dʺ_1.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_HEXDIG_2.Parsed), _ʺx2Dʺ_2.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_HEXDIG_3.Parsed), _ʺx2Dʺ_3.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_HEXDIG_4.Parsed), _ʺx2Dʺ_4.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly12<__GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_HEXDIG_5.Parsed)), _HEXDIG_5.Remainder);
            }
        }
    }
    
}
