namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _polygonDataParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._polygonData> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._polygonData>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._polygonData> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonData)!, input);
}

var _ringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._ringLiteralParser.Instance.Parse(_OPEN_1.Remainder);
if (!_ringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonData)!, input);
}

var _ⲤCOMMA_ringLiteralↃ_1 = Inners._ⲤCOMMA_ringLiteralↃParser.Instance.Many().Parse(_ringLiteral_1.Remainder);
if (!_ⲤCOMMA_ringLiteralↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonData)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_ringLiteralↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonData)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._polygonData(_OPEN_1.Parsed, _ringLiteral_1.Parsed, _ⲤCOMMA_ringLiteralↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
