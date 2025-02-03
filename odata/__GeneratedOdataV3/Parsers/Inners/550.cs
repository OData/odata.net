namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_polygonDataParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData)!, input);
}

var _polygonData_1 = __GeneratedOdataV3.Parsers.Rules._polygonDataParser.Instance.Parse(_COMMA_1.Remainder);
if (!_polygonData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_polygonData(_COMMA_1.Parsed,  _polygonData_1.Parsed), _polygonData_1.Remainder);
            }
        }
    }
    
}
