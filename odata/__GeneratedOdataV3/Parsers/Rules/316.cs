namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lineStringDataParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._lineStringData> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._lineStringData>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._lineStringData> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringData)!, input);
}

var _positionLiteral_1 = __GeneratedOdataV3.Parsers.Rules._positionLiteralParser.Instance.Parse(_OPEN_1.Remainder);
if (!_positionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringData)!, input);
}

var _ⲤCOMMA_positionLiteralↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤCOMMA_positionLiteralↃParser.Instance.Repeat(1, null).Parse(_positionLiteral_1.Remainder);
if (!_ⲤCOMMA_positionLiteralↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringData)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_positionLiteralↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringData)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._lineStringData(_OPEN_1.Parsed, _positionLiteral_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ>(_ⲤCOMMA_positionLiteralↃ_1.Parsed),  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
