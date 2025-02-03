namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IRIⲻinⲻqueryParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻquery> Instance { get; } = from _qcharⲻnoⲻAMP_1 in __GeneratedOdataV3.Parsers.Rules._qcharⲻnoⲻAMPParser.Instance.Repeat(1, null)
select new __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻquery(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMP>(_qcharⲻnoⲻAMP_1));
    }
    
}
