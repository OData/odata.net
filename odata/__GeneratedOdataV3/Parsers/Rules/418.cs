namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IRIⲻinⲻheaderParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader> Instance { get; } = from _ⲤVCHARⳆobsⲻtextↃ_1 in __GeneratedOdataV3.Parsers.Inners._ⲤVCHARⳆobsⲻtextↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ>(_ⲤVCHARⳆobsⲻtextↃ_1));
    }
    
}
