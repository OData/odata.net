namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_batchOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_batchOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3FʺParser.Instance
from _batchOptions_1 in __GeneratedOdataV2.Parsers.Rules._batchOptionsParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_batchOptions(_ʺx3Fʺ_1, _batchOptions_1);
    }
    
}
