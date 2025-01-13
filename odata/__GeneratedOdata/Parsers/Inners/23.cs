namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx3Fʺ_batchOptionsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_batchOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _batchOptions_1 in __GeneratedOdata.Parsers.Rules._batchOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_batchOptions(_ʺx3Fʺ_1, _batchOptions_1);
    }
    
}
