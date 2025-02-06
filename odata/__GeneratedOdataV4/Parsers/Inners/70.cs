namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_entitySetNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName)!, input);
}

var _entitySetName_1 = __GeneratedOdataV4.Parsers.Rules._entitySetNameParser.Instance.Parse(_COMMA_1.Remainder);
if (!_entitySetName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_entitySetName(_COMMA_1.Parsed, _entitySetName_1.Parsed), _entitySetName_1.Remainder);
            }
        }
    }
    
}
