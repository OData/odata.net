namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleEnumValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleEnumValue> Instance { get; } = (_enumerationMemberParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._singleEnumValue>(_enumMemberValueParser.Instance);
        
        public static class _enumerationMemberParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumerationMember> Instance { get; } = from _enumerationMember_1 in __GeneratedOdataV2.Parsers.Rules._enumerationMemberParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumerationMember(_enumerationMember_1);
        }
        
        public static class _enumMemberValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumMemberValue> Instance { get; } = from _enumMemberValue_1 in __GeneratedOdataV2.Parsers.Rules._enumMemberValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleEnumValue._enumMemberValue(_enumMemberValue_1);
        }
    }
    
}
