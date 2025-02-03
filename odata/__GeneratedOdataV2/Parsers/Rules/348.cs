namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _requestⲻidParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._requestⲻid> Instance { get; } = from _unreserved_1 in __GeneratedOdataV2.Parsers.Rules._unreservedParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._requestⲻid(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._unreserved>(_unreserved_1));
    }
    
}
