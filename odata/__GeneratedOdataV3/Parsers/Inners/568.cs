namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_preferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_preference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_preference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_preference> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_preference)!, input);
}

var _preference_1 = __GeneratedOdataV3.Parsers.Rules._preferenceParser.Instance.Parse(_COMMA_1.Remainder);
if (!_preference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_preference)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_preference(_COMMA_1.Parsed,  _preference_1.Parsed), _preference_1.Remainder);
            }
        }
    }
    
}
