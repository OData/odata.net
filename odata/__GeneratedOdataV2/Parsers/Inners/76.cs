namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_batchOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_batchOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx26ʺParser.Instance
from _batchOption_1 in __GeneratedOdataV2.Parsers.Rules._batchOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_batchOption(_ʺx26ʺ_1, _batchOption_1);
    }
    
}
