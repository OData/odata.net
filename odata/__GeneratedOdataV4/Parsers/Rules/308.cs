namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumMemberValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._enumMemberValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._enumMemberValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._enumMemberValue> Parse(IInput<char>? input)
            {
                var _int64Value_1 = __GeneratedOdataV4.Parsers.Rules._int64ValueParser.Instance.Parse(input);
if (!_int64Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enumMemberValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._enumMemberValue(_int64Value_1.Parsed), _int64Value_1.Remainder);
            }
        }
    }
    
}
