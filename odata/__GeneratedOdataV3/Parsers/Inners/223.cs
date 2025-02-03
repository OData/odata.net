namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _EQ_customValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._EQ_customValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._EQ_customValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._EQ_customValue> Parse(IInput<char>? input)
            {
                var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(input);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._EQ_customValue)!, input);
}

var _customValue_1 = __GeneratedOdataV3.Parsers.Rules._customValueParser.Instance.Parse(_EQ_1.Remainder);
if (!_customValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._EQ_customValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._EQ_customValue(_EQ_1.Parsed,  _customValue_1.Parsed), _customValue_1.Remainder);
            }
        }
    }
    
}
