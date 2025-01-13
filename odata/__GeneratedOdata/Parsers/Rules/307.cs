namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _singleEnumValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._singleEnumValue> Instance { get; } = (_enumerationMemberParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._singleEnumValue>(_enumMemberValueParser.Instance);
        
        public static class _enumerationMemberParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._singleEnumValue._enumerationMember> Instance { get; } = from _enumerationMember_1 in __GeneratedOdata.Parsers.Rules._enumerationMemberParser.Instance
select new __GeneratedOdata.CstNodes.Rules._singleEnumValue._enumerationMember(_enumerationMember_1);
        }
        
        public static class _enumMemberValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._singleEnumValue._enumMemberValue> Instance { get; } = from _enumMemberValue_1 in __GeneratedOdata.Parsers.Rules._enumMemberValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._singleEnumValue._enumMemberValue(_enumMemberValue_1);
        }
    }
    
}
