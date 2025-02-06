namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_lineStringDataParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData)!, input);
}

var _lineStringData_1 = __GeneratedOdataV4.Parsers.Rules._lineStringDataParser.Instance.Parse(_COMMA_1.Remainder);
if (!_lineStringData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData(_COMMA_1.Parsed, _lineStringData_1.Parsed), _lineStringData_1.Remainder);
            }
        }
    }
    
}
