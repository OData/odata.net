namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleEnumValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue> Instance { get; } = (_enumerationMemberParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue>(_enumMemberValueParser.Instance);
        
        public static class _enumerationMemberParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumerationMember> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumerationMember>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumerationMember> Parse(IInput<char>? input)
                {
                    var _enumerationMember_1 = __GeneratedOdataV4.Parsers.Rules._enumerationMemberParser.Instance.Parse(input);
if (!_enumerationMember_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumerationMember)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumerationMember(_enumerationMember_1.Parsed), _enumerationMember_1.Remainder);
                }
            }
        }
        
        public static class _enumMemberValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumMemberValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumMemberValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumMemberValue> Parse(IInput<char>? input)
                {
                    var _enumMemberValue_1 = __GeneratedOdataV4.Parsers.Rules._enumMemberValueParser.Instance.Parse(input);
if (!_enumMemberValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumMemberValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._singleEnumValue._enumMemberValue(_enumMemberValue_1.Parsed), _enumMemberValue_1.Remainder);
                }
            }
        }
    }
    
}
