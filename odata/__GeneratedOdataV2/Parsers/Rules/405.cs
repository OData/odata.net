namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fragmentParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fragment> Instance { get; } = from _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1 in Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._fragment(_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1);
    }
    
}
