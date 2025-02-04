namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_pointDataParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_pointData> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_pointData>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_pointData> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_pointData)!, input);
}

var _pointData_1 = __GeneratedOdataV3.Parsers.Rules._pointDataParser.Instance.Parse(_COMMA_1.Remainder);
if (!_pointData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_pointData)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_pointData(_COMMA_1.Parsed, _pointData_1.Parsed), _pointData_1.Remainder);
            }
        }
    }
    
}
