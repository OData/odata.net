namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._customValue> Instance { get; } = from _ⲤqcharⲻnoⲻAMPↃ_1 in Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._customValue(_ⲤqcharⲻnoⲻAMPↃ_1);
    }
    
}
