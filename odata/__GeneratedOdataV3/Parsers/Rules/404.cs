namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._query> Instance { get; } = from _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1 in Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃParser.Instance.Many()
select new __GeneratedOdataV3.CstNodes.Rules._query(_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1);
    }
    
}
