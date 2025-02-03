namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumMemberValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._enumMemberValue> Instance { get; } = from _int64Value_1 in __GeneratedOdataV2.Parsers.Rules._int64ValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._enumMemberValue(_int64Value_1);
    }
    
}
