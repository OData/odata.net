namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumMemberValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._enumMemberValue> Instance { get; } = from _int64Value_1 in __GeneratedOdata.Parsers.Rules._int64ValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._enumMemberValue(_int64Value_1);
    }
    
}
