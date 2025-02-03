namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _batchOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._batchOptions> Instance { get; } = from _batchOption_1 in __GeneratedOdataV2.Parsers.Rules._batchOptionParser.Instance
from _Ⲥʺx26ʺ_batchOptionↃ_1 in Inners._Ⲥʺx26ʺ_batchOptionↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._batchOptions(_batchOption_1, _Ⲥʺx26ʺ_batchOptionↃ_1);
    }
    
}
